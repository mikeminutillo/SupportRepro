using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Settings;
using Shared;

namespace CaseHandlers
{
    public class ApplicationSubmittedHandler : IHandleMessages<ApplicationSubmitted>
    {
        public ReadOnlySettings Settings { get; set; }

        public Task Handle(ApplicationSubmitted message, IMessageHandlerContext context)
        {
            Console.WriteLine($"{message.ApplicationNumber} Case Handler: {Settings.EndpointName()}");
            return Task.CompletedTask;
        }
    }
}
