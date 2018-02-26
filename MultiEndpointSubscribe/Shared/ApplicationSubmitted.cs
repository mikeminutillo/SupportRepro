using NServiceBus;

namespace Shared
{
    public class ApplicationSubmitted : IEvent
    {
        public int ApplicationNumber { get; set; }
    }
}
