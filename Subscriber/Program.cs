using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.DataBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "Subscriber";

        #region Endpoint Configuration

        var endpointConfiguration = new EndpointConfiguration("Samples.ASBS.PubSub.Subscriber");
        endpointConfiguration.EnableInstallers();

        var transportConnectionString = Environment.GetEnvironmentVariable("AzureServiceBus_ConnectionString");
        if (string.IsNullOrWhiteSpace(transportConnectionString))
        {
            throw new Exception("Could not read the 'AzureServiceBus_ConnectionString' environment variable. Check the sample prerequisites.");
        }

        var storageConnectionString = Environment.GetEnvironmentVariable("AzureStorage_ConnectionString");
        if(string.IsNullOrWhiteSpace(storageConnectionString))
        {
            throw new Exception("Could not read the 'AzureStorage_ConnectionString' environment variable. Check the sample prerequisites.");
        }

        var transport = new AzureServiceBusTransport(transportConnectionString);
        endpointConfiguration.UseTransport(transport);
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        #endregion

#pragma warning disable CS0618 // Type or member is obsolete
        var databus = endpointConfiguration.UseDataBus<AzureDataBus, BinaryFormatterDataBusSerializer>();
#pragma warning restore CS0618 // Type or member is obsolete
        databus.AddDeserializer<SystemJsonDataBusSerializer>();
        databus.ConnectionString(storageConnectionString);

        var endpointInstance = await Endpoint.Start(endpointConfiguration);

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();

        await endpointInstance.Stop();
    }
}