namespace Subscriber
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus.Logging;
    using NServiceBus.Pipeline;

    class IncomingLogicalMessageSpy : Behavior<IIncomingLogicalMessageContext>
    {
        public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            Log.Info($"Message Type: [{context.MessageId}] {context.Message.MessageType}");
            return next();
        }

        private static ILog Log = LogManager.GetLogger<IncomingLogicalMessageSpy>();
    }
}