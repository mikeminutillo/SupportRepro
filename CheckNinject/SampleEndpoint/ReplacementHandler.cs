using System;
using System.Threading.Tasks;
using NServiceBus;

namespace SampleEndpoint
{
    class ReplacementHandler : IHandleMessages<SomeMessage>
    {
        public Task Handle(SomeMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Replacement Handler {GetHashCode()}");
            return Task.CompletedTask;
        }
    }
}