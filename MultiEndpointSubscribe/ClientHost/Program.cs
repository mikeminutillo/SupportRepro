using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace ClientHost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Console Host";

            var caseEndpointConfig = new EndpointConfiguration("Case");
            caseEndpointConfig.SendFailedMessagesTo("error");
            var caseTransport = caseEndpointConfig.UseTransport<MsmqTransport>();
            caseTransport.Routing().RegisterPublisher(typeof(ApplicationSubmitted), "Publisher");
            caseEndpointConfig.UsePersistence<InMemoryPersistence>();
            caseEndpointConfig.AssemblyScanner().ExcludeAssemblies("CustomerHandlers.dll");

            var caseEndpoint = await Endpoint.Start(caseEndpointConfig)
                .ConfigureAwait(false);

            Console.WriteLine("Case Endpoint Started");

            var customerEndpointConfig = new EndpointConfiguration("Customer");
            customerEndpointConfig.SendFailedMessagesTo("error");
            var customerTransport = customerEndpointConfig.UseTransport<MsmqTransport>();
            customerTransport.Routing().RegisterPublisher(typeof(ApplicationSubmitted), "Publisher");
            customerEndpointConfig.UsePersistence<InMemoryPersistence>();
            customerEndpointConfig.AssemblyScanner().ExcludeAssemblies("CaseHandlers.dll");

            var customerEndpoint = await Endpoint.Start(customerEndpointConfig)
                .ConfigureAwait(false);

            Console.WriteLine("Customer Endpoint Started");

            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                
            }

            await caseEndpoint.Stop()
                .ConfigureAwait(false);

            await customerEndpoint.Stop()
                .ConfigureAwait(false);
        }
    }
}
