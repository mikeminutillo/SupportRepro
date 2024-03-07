using NServiceBus;
using System;

var endpointName = "V2.Subscriber";
Console.Title = endpointName;

var endpointConfiguration = new EndpointConfiguration(endpointName);
endpointConfiguration.UseSerialization<SystemJsonSerializer>();
endpointConfiguration.UseTransport(new LearningTransport());
endpointConfiguration.Conventions().DefiningMessagesAs(t =>
    t.Namespace?.Contains("Contracts", StringComparison.InvariantCultureIgnoreCase) == true);


var endpointInstance = await Endpoint.Start(endpointConfiguration);

Console.WriteLine("Press any key to exit");
Console.ReadKey();

await endpointInstance.Stop();