namespace SampleEndpoint
{
    using NServiceBus.Features;

    class CheckDataBus : Feature
    {
        public CheckDataBus()
        {
            EnableByDefault();
            DependsOn<DataBus>();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            context.Pipeline.Register<CheckDataBusSends.Registration>();
            context.Pipeline.Register<CheckDataBusReceives.Registration>();
            context.Pipeline.Register<CheckDataBusLoads.Registration>();
        }
    }
}