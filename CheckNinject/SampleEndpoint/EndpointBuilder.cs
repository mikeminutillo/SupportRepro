using System;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Ninject;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;
using NServiceBus.UnitOfWork;

namespace SampleEndpoint
{
    public class EndpointBuilder
    {
        public static IEndpointInstance Build(IKernel kernel)
        {
            var unitOfWorkProviderAdapter = kernel.Get<IManageUnitsOfWork>();
            var busSettingsConfiguration = kernel.Get<IBusSettingsConfiguration>();

            var logger = LogManager.GetLogger<EndpointBuilder>();

            var endpointConfiguration = new EndpointConfiguration(busSettingsConfiguration.EndpointName);

            var jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var serialization = endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            serialization.Settings(jsonSettings);

            endpointConfiguration.SendFailedMessagesTo("error");

            SetMessageProcessingConcurrency(busSettingsConfiguration, logger, endpointConfiguration);

            endpointConfiguration.PurgeOnStartup(busSettingsConfiguration.PurgeMessagesOnStartup);

            var recoverability = endpointConfiguration.Recoverability();

            recoverability.Delayed(delay =>
            {
                delay.NumberOfRetries(5);
                delay.TimeIncrease(TimeSpan.FromSeconds(3));
            });

            endpointConfiguration.EnableInstallers();
            var connectionString = @"Server=.\SQLEXPRESS;Database=NServiceBus;Integrated Security=SSPI;";

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(connectionString);

            //endpointConfiguration.RegisterMessageMutator(new PrincipalMutator());
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            // Cache Subscriptions for one minute. We don't change them very often anyways.
            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));

            persistence.ConnectionBuilder(() =>
            {
                return new SqlConnection(connectionString);
            });

            SetupIoc(endpointConfiguration, kernel);
            SetupUnitOWorkProvider(endpointConfiguration, unitOfWorkProviderAdapter);

            return Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        }

        private static void SetMessageProcessingConcurrency(IBusSettingsConfiguration busSettingsConfiguration, ILog logger,
            EndpointConfiguration endpointConfiguration)
        {
            if (busSettingsConfiguration.LimitMessageProcessingConcurrencyToPercentage.HasValue)
            {
                var logicalProcessorPercentage = busSettingsConfiguration.LimitMessageProcessingConcurrencyToPercentage.Value;
                var limitMessageProcessingConcurrencyTo =
                    decimal.ToInt32(Environment.ProcessorCount * logicalProcessorPercentage);
                if (limitMessageProcessingConcurrencyTo < 2 && Environment.ProcessorCount >= 2)
                {
                    logger.Warn(
                        $"Using LimitMessageProcessingConcurrencyToPercentage: '{logicalProcessorPercentage}' {Environment.NewLine}would results in a NServiceBus Message Processing Concurrency of: {limitMessageProcessingConcurrencyTo}.{Environment.NewLine}Setting to a minimum value of 2");
                    limitMessageProcessingConcurrencyTo = 2;
                }
                else
                {
                    logger.Info(
                        $"Logical Processor Count: {Environment.ProcessorCount}, NServiceBus Message Processing Concurrency: {limitMessageProcessingConcurrencyTo}");
                }

                endpointConfiguration
                    .LimitMessageProcessingConcurrencyTo(limitMessageProcessingConcurrencyTo);
            }
            else
            {
                logger.Info($"Using NServiceBus default Concurrency Max({Environment.ProcessorCount}, 2)");
            }
        }

        private static void SetupUnitOWorkProvider(EndpointConfiguration endpointConfiguration, IManageUnitsOfWork unitOfWorkProviderAdaptor)
        {
            endpointConfiguration.RegisterComponents(
                registration: components =>
                {
                    components.ConfigureComponent<UnitOfWorkProviderAdaptor>(
                        DependencyLifecycle.InstancePerUnitOfWork);
                }
            );
        }

        private static void SetupIoc(EndpointConfiguration endpointConfiguration, IKernel kernel)
        {
            if (kernel != null)
            {
                endpointConfiguration.UseContainer<NinjectBuilder>(cust =>
                {
                    cust.ExistingKernel(kernel);
                });

            }
        }
    }
}
