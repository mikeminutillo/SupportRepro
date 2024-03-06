using MessageContracts;

namespace Receiver;

class ComplexContractHandler : IHandleMessages<IComplexContract>
{
    public Task Handle(IComplexContract message, IMessageHandlerContext context)
    {
        Console.WriteLine($"Received message {message?.Child?.IsComplex}");
        return Task.CompletedTask;
    }
}
