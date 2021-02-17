namespace Subscriber
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using NServiceBus;

    public class ClientGewijzigdEventHandler : IHandleMessages<IClientGewijzigdEvent>
    {
        public Task Handle(IClientGewijzigdEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine("Event Handled");
            return Task.CompletedTask;
        }
    }
}