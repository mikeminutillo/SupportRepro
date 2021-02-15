namespace Subscriber
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Subscriber";

            var config = new EndpointConfiguration("Subscriber");

            var transport = config.UseTransport<RabbitMQTransport>();
            transport.ConnectionString("host=localhost;username=user;password=password");
            transport.UseConventionalRoutingTopology();

            config.UsePersistence<LearningPersistence>();
            config.UseSerialization<NewtonsoftSerializer>();

            config.EnableInstallers();

            var endpoint = await Endpoint.Start(config)
                .ConfigureAwait(false);

            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
            }

            await endpoint.Stop()
                .ConfigureAwait(false);
        }
    }
}
