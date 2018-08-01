namespace SampleEndpoint
{
    public interface IBusSettingsConfiguration
    {
        decimal? LimitMessageProcessingConcurrencyToPercentage { get; set; }
        string EndpointName { get; set; }
        bool PurgeMessagesOnStartup { get; set; }
    }
}