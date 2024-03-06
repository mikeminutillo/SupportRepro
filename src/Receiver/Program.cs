using MessageContracts;

Console.Title = "Receiver";

var endpointConfig = new EndpointConfiguration("Receiver");

endpointConfig.UseTransport<LearningTransport>();
endpointConfig.UseSerialization<SystemJsonSerializer>();

endpointConfig.Conventions().DefiningMessagesAs(t => t.Assembly == typeof(IComplexContract).Assembly
                                                  || t.Name == "ConcreteContract");

var endpoint = await Endpoint.Start(endpointConfig);

while(Console.ReadKey(true).Key != ConsoleKey.Escape)
{

}

await endpoint.Stop();

