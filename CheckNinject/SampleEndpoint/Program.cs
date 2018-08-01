using System;
using System.Threading.Tasks;
using Ninject;
using NServiceBus;
using NServiceBus.UnitOfWork;

namespace SampleEndpoint
{
    class Program
    {
        static async Task Main()
        {
            Console.Title = "SampleEndpoint";

            var kernel = new StandardKernel();
            kernel.Bind<IManageUnitsOfWork, UnitOfWorkProviderAdaptor>().To<UnitOfWorkProviderAdaptor>();
            kernel.Bind<IBusSettingsConfiguration>().To<BusSettingsConfiguration>();
            var endpoint = EndpointBuilder.Build(kernel);

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
