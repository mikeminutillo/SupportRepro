using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Persistence;
using Shared;

namespace Publisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Publisher";

            var endpointConfig = new EndpointConfiguration("Publisher");
            endpointConfig.SendFailedMessagesTo("error");
            endpointConfig.UseTransport<MsmqTransport>();
            var persistence = endpointConfig.UsePersistence<NHibernatePersistence>();
            persistence.ConnectionString(@"Data Source=.\SqlExpress;Database=nservicebus;");

            var endpoint = await Endpoint.Start(endpointConfig)
                .ConfigureAwait(false);

            Console.WriteLine("Press ESC to close");
            Console.WriteLine("Press any other key to publish ApplicationSubmitted");

            var applicationNumber = 1;
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                await endpoint.Publish(new ApplicationSubmitted { ApplicationNumber = applicationNumber++ })
                    .ConfigureAwait(false);
            }

            await endpoint.Stop()
                .ConfigureAwait(false);
        }
    }
}
