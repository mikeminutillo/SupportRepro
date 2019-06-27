namespace SampleEndpoint.Messages
{
    using NServiceBus;

    public class EventWithProp : IEvent
    {
        public string PrimaryId { get; set; }
        public DataBusProperty<byte[]> Data { get; set; }
    }
}
