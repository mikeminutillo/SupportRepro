using System;
using System.Threading.Tasks;
using NServiceBus;

namespace SampleEndpoint
{
    class SomeMessageHandler : IHandleMessages<SomeMessage>
    {
        public Task Handle(SomeMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Handled from {GetHashCode()}");
            return Task.CompletedTask;
        }
    }
}