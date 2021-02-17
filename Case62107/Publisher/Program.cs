namespace Publisher
{
    using System;
    using System.Threading.Tasks;
    using Api;
    using Messages;
    using NServiceBus;

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Publisher";

            var config = new EndpointConfiguration("Publisher");

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
                await endpoint.Publish<ClientGewijzigdEvent>()
                    .ConfigureAwait(false);
                Console.WriteLine("Event Published");
            }

            await endpoint.Stop()
                .ConfigureAwait(false);
        }
    }
}
