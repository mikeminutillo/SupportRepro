namespace SampleEndpoint
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using Newtonsoft.Json;
    using NServiceBus;

    class Program
    {
        static async Task Main(string[] args)
        {
            const string connectionString = "host=localhost;username=user;password=password";
            const string dataBusPath = @"C:\Temp\DataBus";

            var config = new EndpointConfiguration("SampleEndpoint");
            var serialization = config.UseSerialization<NewtonsoftSerializer>();
            serialization.Settings(new JsonSerializerSettings
            {
                ContractResolver = new SkipConstructorContractResolver(),
                TypeNameHandling = TypeNameHandling.Auto
            });
            config.UseTransport<RabbitMQTransport>()
                .UseDirectRoutingTopology()
                .ConnectionString(connectionString);
            config.UsePersistence<InMemoryPersistence>();
            config.AuditProcessedMessagesTo("audit");
            config.EnableInstallers();

            config.UseDataBus<FileShareDataBus>().BasePath(dataBusPath);

            var endpoint = await Endpoint.Start(config);

            Console.WriteLine("Press ESC to quit. Any other key to publish an event");

            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                await endpoint.Publish(new EventWithProp( 
                    primaryId: Guid.NewGuid().ToString(),
                    data: new byte[] {1, 2, 3, 4, 5}
                ));
            }

            await endpoint.Stop();
        }
    }
}
