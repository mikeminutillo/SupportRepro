namespace SampleEndpoint
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using NServiceBus.Logging;
    using NServiceBus.Pipeline;

    class CheckDataBusLoads : Behavior<IIncomingLogicalMessageContext>
    {
        private static ILog Log = LogManager.GetLogger<CheckDataBusLoads>();

        public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            if (context.Message.Instance is EventWithProp eventWithProp)
            {
                Log.Info($"Key: {eventWithProp.Data.Key} ({eventWithProp.Data.HasValue})");

            }
            return next();
        }

        public class Registration : RegisterStep
        {
            public Registration() : base(nameof(CheckDataBusLoads), typeof(CheckDataBusLoads), "Checks DataBus Loads")
            {
                InsertAfter("DataBusReceive");
            }
        }
    }
}