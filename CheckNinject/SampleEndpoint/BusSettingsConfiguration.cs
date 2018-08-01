namespace SampleEndpoint
{
    internal class BusSettingsConfiguration : IBusSettingsConfiguration
    {
        public decimal? LimitMessageProcessingConcurrencyToPercentage { get; set; }
        public string EndpointName { get; set; } = "SomeEndpoint";
        public bool PurgeMessagesOnStartup { get; set; }
    }
}