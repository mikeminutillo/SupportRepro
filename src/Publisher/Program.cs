using Contracts;
using NSB.Extensions;
using NServiceBus;
using System;

var endpointName = "Publisher";
Console.Title = endpointName;

var endpointConfiguration = new EndpointConfiguration(endpointName);
endpointConfiguration.UseSerialization<SystemJsonSerializer>();
endpointConfiguration.UseTransport(new LearningTransport());
endpointConfiguration.Conventions().DefiningEventsAs(t =>
    t.Namespace?.Contains("Contracts", StringComparison.InvariantCultureIgnoreCase) == true);
endpointConfiguration.MapEnclosedMessageTypes().Add(typeof(SomethingMoreHappened));

var endpointInstance = await Endpoint.Start(endpointConfiguration);

Console.WriteLine("Press enter to publish a message");
Console.WriteLine("Press any key to exit");

int i = 0;

while (true)
{
    var key = Console.ReadKey();

    if (key.Key != ConsoleKey.Enter)
    {
        break;
    }

    /*if (i % 2 == 0)
    {
        await endpointInstance.Publish<ISomethingMoreHappened>(sh =>
        {
            sh.SomeData = 1;
            sh.MoreInfo = "It's a secret.";
            sh.Child = new ComplexChild();
        });
    }
    else
    {

    }*/

    await endpointInstance.Publish(new SomethingMoreHappened
    {
        SomeData = 1,
        MoreInfo = "It's a secret.",
        Child = new ComplexChild(),
    });

    Console.WriteLine("Published event.");
    i++;
}

await endpointInstance.Stop();