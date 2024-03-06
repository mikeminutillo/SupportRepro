using MessageContracts;
using Messages;

Console.Title = "Sender";

var endpointConfig = new EndpointConfiguration("Sender");

endpointConfig.UseTransport<LearningTransport>();
endpointConfig.UseSerialization<SystemJsonSerializer>();
endpointConfig.Conventions().DefiningMessagesAs(t => t.Assembly == typeof(IComplexContract).Assembly
                                                  || t == typeof(ConcreteContract));

var endpoint = await Endpoint.Start(endpointConfig);

Console.WriteLine("Started");

while (Console.ReadKey(true).Key != ConsoleKey.Escape)
{
    await endpoint.Send(
        "Receiver",
        new ConcreteContract
        {
            Child = new ConcreteChild()
        }
    );
    Console.WriteLine("Message sent");
}

await endpoint.Stop();

Console.WriteLine("Stopped");
