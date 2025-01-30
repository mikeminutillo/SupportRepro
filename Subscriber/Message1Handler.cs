using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class Message1Handler :
    IHandleMessages<SomeEvent>
{
    static ILog log = LogManager.GetLogger<Message1Handler>();

    public Task Handle(SomeEvent message, IMessageHandlerContext context)
    {
        log.Info($"Received SomeEvent: {message.Property}");

        return Task.CompletedTask;
    }
}