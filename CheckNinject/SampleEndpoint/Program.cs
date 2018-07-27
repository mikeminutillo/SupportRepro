using System;
using System.Threading.Tasks;
using NServiceBus;

namespace SampleEndpoint
{
    class Program
    {
        static async Task Main()
        {
            Console.Title = "SampleEndpoint";

            var endpointConfiguration = new EndpointConfiguration("SampleEndpoint");
            endpointConfiguration.UseTransport<LearningTransport>();
            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.UseContainer<NinjectBuilder>();

            var endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Started");

            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                await endpoint.SendLocal(new SomeMessage()).ConfigureAwait(false);
            }

            Console.WriteLine("Stopping");

            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}
