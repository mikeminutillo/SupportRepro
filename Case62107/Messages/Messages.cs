namespace Messages
{
    using NServiceBus;

    public interface IClientGewijzigdEvent : IEvent
    {

    }

    public class ClientRegelData
    {

    }

    public interface IRegelGewijzigdEvent<TData>
    {

    }

    public class ClientGewijzigdChangedEvent : IRegelGewijzigdEvent<ClientRegelData>, IClientGewijzigdEvent
    {

    }
}
