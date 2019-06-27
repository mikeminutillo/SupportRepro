namespace SampleEndpoint
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using NServiceBus;

    class EventHandler : IHandleMessages<EventWithProp>
    {
        public Task Handle(EventWithProp message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received {message.PrimaryId}");
            Console.WriteLine(string.Join(", ", message.Data.Value));
            return Task.FromResult(0);
        }
    }
}