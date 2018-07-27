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

            var swapper = new SwapHandlersBehavior();
            endpointConfiguration.Pipeline.Register(swapper, "Blocks one handler or the other");

            var endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Started");

            var done = false;
            while (!done)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Escape:
                        done = true;
                        break;
                    case ConsoleKey.S:
                        swapper.Swapping = !swapper.Swapping;
                        Console.WriteLine($"Swapping {swapper.Swapping}");
                        break;
                    default:
                        await endpoint.SendLocal(new SomeMessage()).ConfigureAwait(false);
                        break;
                }
            }

            Console.WriteLine("Stopping");

            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}
