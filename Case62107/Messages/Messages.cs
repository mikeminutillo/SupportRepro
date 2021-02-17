namespace Messages
{
    using NServiceBus;

    public interface IClientGewijzigdEvent : IRegelGewijzigdEvent<ClientRegelData>
    {

    }

    public class ClientRegelData
    {

    }

    public interface IRegelGewijzigdEvent<TData> : IEvent
    {

    }
}
