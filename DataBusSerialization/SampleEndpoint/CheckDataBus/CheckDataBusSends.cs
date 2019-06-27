namespace SampleEndpoint
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using NServiceBus.Logging;
    using NServiceBus.Pipeline;

    class CheckDataBusSends : Behavior<IOutgoingLogicalMessageContext>
    {
        private static ILog Log = LogManager.GetLogger<CheckDataBusSends>();

        public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
        {
            if (context.Message.Instance is EventWithProp eventWithProp)
            {
                Log.Info($"Key: {eventWithProp.Data.Key} ({eventWithProp.Data.HasValue})");
            }

            return next();
        }

        public class Registration: RegisterStep
        {
            public Registration() : base(nameof(CheckDataBusSends), typeof(CheckDataBusSends), "Checks the databus")
            {
                InsertAfter("DataBusSend");       
            }
        }
    }
}