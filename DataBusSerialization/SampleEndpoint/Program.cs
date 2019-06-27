namespace SampleEndpoint
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using NServiceBus;
    using NServiceBus.ObjectBuilder;

    class Program
    {
        static async Task Main(string[] args)
        {
            const string connectionString = "host=localhost;username=user;password=password";
            const string dataBusPath = @"C:\Temp\DataBus";

            var config = new EndpointConfiguration("SampleEndpoint");
            config.UseSerialization<NewtonsoftSerializer>();
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
                await endpoint.Publish(new EventWithProp
                {
                    PrimaryId = Guid.NewGuid().ToString(),
                    Data = new DataBusProperty<byte[]>(new byte[] {1, 2, 3, 4, 5})
                });
            }

            await endpoint.Stop();
        }
    }
}
