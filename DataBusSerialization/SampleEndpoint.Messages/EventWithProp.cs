namespace SampleEndpoint.Messages
{
    using NServiceBus;

    public class EventWithProp : IEvent
    {
        public EventWithProp(string primaryId, byte[] data)
        {
            PrimaryId = primaryId;
            Data = new DataBusProperty<byte[]>(data);
        }

        public string PrimaryId { get; set; }
        public DataBusProperty<byte[]> Data { get; set; }
    }
}
