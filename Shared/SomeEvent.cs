using NServiceBus;

public class SomeEvent : IEvent
{
    public string Property { get; set; }
    public DataBusProperty<string> DataBusProperty { get; set; }
}