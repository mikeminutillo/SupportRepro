using NServiceBus;

public class SomeEvent : IEvent
{
    public string Property { get; set; }
}