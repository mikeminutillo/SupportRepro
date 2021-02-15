namespace Subscriber
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using NServiceBus;

    public class ClientGewijzigdEventHandler : IHandleMessages<ClientGewijzigdChangedEvent>
    {
        public Task Handle(ClientGewijzigdChangedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine("Event Handled");
            return Task.CompletedTask;
        }
    }
}