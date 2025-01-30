using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "Publisher";

        #region config

        var endpointConfiguration = new EndpointConfiguration("Samples.ASBS.PubSub.Publisher");
        endpointConfiguration.EnableInstallers();

        var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus_ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Could not read the 'AzureServiceBus_ConnectionString' environment variable. Check the sample prerequisites.");
        }

        var storageConnectionString = Environment.GetEnvironmentVariable("AzureStorage_ConnectionString");
        if (string.IsNullOrWhiteSpace(storageConnectionString))
        {
            throw new Exception("Could not read the 'AzureStorage_ConnectionString' environment variable. Check the sample prerequisites.");
        }

        var transport = new AzureServiceBusTransport(connectionString);
        endpointConfiguration.UseTransport(transport);
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        endpointConfiguration.UseDataBus<AzureDataBus, SystemJsonDataBusSerializer>();

        var databus = endpointConfiguration.UseDataBus<AzureDataBus, SystemJsonDataBusSerializer>();
        databus.ConnectionString(storageConnectionString);

        #endregion

        var endpointInstance = await Endpoint.Start(endpointConfiguration);
        Console.WriteLine("Press 'enter' to publish an event");
        Console.WriteLine("Press any other key to exit");

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key != ConsoleKey.Enter)
            {
                break;
            }

            var message = new SomeEvent
            {
                Property = "Hello from Publisher",
                DataBusProperty = new DataBusProperty<string>("Large message from publisher")
            };

            await endpointInstance.Publish(message);
            Console.WriteLine("Event published");
        }
        await endpointInstance.Stop();
    }
}
