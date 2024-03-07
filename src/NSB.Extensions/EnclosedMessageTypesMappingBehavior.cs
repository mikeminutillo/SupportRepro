using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Features;
using NServiceBus.Pipeline;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace NSB.Extensions
{
    public class EnclosedMessageTypesMapper
    {
        public static string Transform(string enclosedMessageTypes, ImmutableDictionary<string, string> typeReplacementMappings)
        {
            var messageTypes = enclosedMessageTypes.Split(';');
            for (var i = 0; i < messageTypes.Length; i++)
            {
                var messageType = ConvertAssemblyQualifiedNameToTypeFullName(messageTypes[i]);
                if (typeReplacementMappings.TryGetValue(messageType, out var fixedType))
                {
                    messageTypes[i] = fixedType;
                }
            }

            return string.Join(";", messageTypes);
        }

        private static string ConvertAssemblyQualifiedNameToTypeFullName(string typeAssemblyQualifiedName)
        {
            return typeAssemblyQualifiedName.Split(',')[0];
        }
    }

    /// <summary>
    /// Configure using <see cref="EnclosedMessageTypesMappingConfigurationExtensions.MapEnclosedMessageTypes"/> 
    /// </summary>
    public class EnclosedMessageTypesMappingBehavior : Behavior<IIncomingPhysicalMessageContext>
    {
        readonly ImmutableDictionary<string, string> _typeReplacementMappings;
        readonly ConcurrentDictionary<string, string> _transformedEnclosedMessageTypes = new();

        /// <summary>
        /// Allow specification of types from the NServiceBus.EnclosedMessageTypes header that should be replaced with
        /// another type or the same type, but with the Assembly version info ignored.
        /// </summary>
        /// <param name="typeReplacementMappings">Supplies a mapping of type FullName to AssemblyQualifiedName. The FullName is
        /// matched against the types specified in the NServiceBus.EnclosedMessageTypes message header.  If a match is found it
        /// is replaced with the corresponding value provided. </param>
        public EnclosedMessageTypesMappingBehavior(ImmutableDictionary<string, string> typeReplacementMappings)
        {
            this._typeReplacementMappings = typeReplacementMappings;
        }

        public override Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
        {
            if (!context.Message.Headers.TryGetValue(Headers.EnclosedMessageTypes, out var enclosedMessageTypes))
            {
                return next();
            }

            var transformedHeader = _transformedEnclosedMessageTypes.GetOrAdd(
                enclosedMessageTypes, EnclosedMessageTypesMapper.Transform,
                _typeReplacementMappings);

            context.Message.Headers[Headers.EnclosedMessageTypes] = transformedHeader;
            return next();
        }
    }

    /// <summary>
    /// Allow specification of types from the NServiceBus.EnclosedMessageTypes header that should be replaced with
    /// another type or the same type, but with the Assembly version info ignored.  Configured using <see cref="EnclosedMessageTypesMappingConfiguration"/>.
    /// Configure using <see cref="EnclosedMessageTypesMappingConfigurationExtensions.MapEnclosedMessageTypes"/> 
    /// </summary>
    public class EnclosedMessageTypesMappingFeature : Feature
    {
        protected override void Setup(FeatureConfigurationContext context)
        {
            var config = context.Settings.Get<EnclosedMessageTypesMappingConfiguration>();
            context.Pipeline.Register(
                new EnclosedMessageTypesMappingBehavior(config.Types.ToImmutableDictionary()),
                "Fixes enclosed message types header for types which have moved"
            );
        }
    }

    /// <summary>
    /// Configuration of the <see cref="EnclosedMessageTypesMappingFeature"/>
    /// using <see cref="EnclosedMessageTypesMappingConfigurationExtensions.MapEnclosedMessageTypes"/> 
    /// </summary>
    public class EnclosedMessageTypesMappingConfiguration
    {
        internal readonly Dictionary<string, string> Types = [];

        /// <summary>
        /// Ignore Assembly version info for the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="Exception"></exception>
        public void Add(Type type) => Add(type, type);

        /// <summary>
        /// Perform a complete type replacement.
        /// </summary>
        /// <param name="mapFrom">The source type</param>
        /// <param name="mapTo">The target type</param>
        /// <exception cref="Exception">For invalid types</exception>
        public void Add(Type mapFrom, Type mapTo) => Types.Add(
            mapFrom.FullName ?? throw new Exception($"{mapFrom.Name} does not have a valid FullName"),
            mapTo.AssemblyQualifiedName ??
            throw new Exception($"{mapTo.Name} does not have a valid AssemblyQualifiedName")
        );


        public void AddMany(IEnumerable<Type> assemblyTypes)
        {
            foreach (var type in assemblyTypes)
            {
                Add(type);
            }
        }
    }

    public static class EnclosedMessageTypesMappingConfigurationExtensions
    {
        /// <summary>
        /// Allow specification of types from the NServiceBus.EnclosedMessageTypes header that should be replaced with
        /// another type or the same type, but with the Assembly version info ignored. 
        /// </summary>
        /// <param name="endpointConfiguration"></param>
        /// <returns><see cref="EnclosedMessageTypesMappingConfiguration"/>: Add types that should be affected by this behavior.
        /// Either ignoring the Assembly version info of the type or performing a complete type substitution.</returns>
        public static EnclosedMessageTypesMappingConfiguration MapEnclosedMessageTypes(
            this EndpointConfiguration endpointConfiguration
        )
        {
            endpointConfiguration.EnableFeature<EnclosedMessageTypesMappingFeature>();
            return endpointConfiguration.GetSettings().GetOrCreate<EnclosedMessageTypesMappingConfiguration>();
        }
    }
}
