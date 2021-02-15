namespace Subscriber
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Logging;
    using NServiceBus.Pipeline;

    class IncomingPhysicalMessageSpy : Behavior<IIncomingPhysicalMessageContext>
    {
        public override Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
        {
            Log.Info($"Message Types Header: [{context.MessageId}] {context.Message.Headers[Headers.EnclosedMessageTypes]}");
            return next();
        }

        private static ILog Log = LogManager.GetLogger<IncomingPhysicalMessageSpy>();
    }
}