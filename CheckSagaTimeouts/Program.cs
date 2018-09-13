using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace TestSagaTimeouts
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpointConfig = new EndpointConfiguration("TestHarness");
            endpointConfig.UseTransport<LearningTransport>();
            endpointConfig.UsePersistence<LearningPersistence>();
            endpointConfig.EnableCallbacks();
            endpointConfig.MakeInstanceUniquelyAddressable("TheOnlyInstance");

            var endpoint = await Endpoint.Start(endpointConfig).ConfigureAwait(false);
            await endpoint.Subscribe<Initiated>().ConfigureAwait(false);
            await endpoint.Subscribe<Submitted>().ConfigureAwait(false);

            Console.WriteLine("I - Initiate");
            Console.WriteLine("S - Submit");

            var key = Console.ReadKey(true).Key;
            var currentId = default(Guid);
            while (key != ConsoleKey.Escape)
            {
                switch (key)
                {
                    case ConsoleKey.I:
                        currentId = Guid.NewGuid();
                        await endpoint.Publish<Initiated>(m =>
                        {
                            m.DraftId = currentId;
                            m.Reference = currentId.ToString();
                        }).ConfigureAwait(false);
                        break;
                    case ConsoleKey.S:
                        await endpoint.Publish<Submitted>(m => m.DraftId = currentId).ConfigureAwait(false);
                        break;
                }

                key = Console.ReadKey(true).Key;
            }

            await endpoint.Stop().ConfigureAwait(false);
        }
    }


    public class DraftOfferManager : Saga<DraftOfferManagerSagaData>, IAmStartedByMessages<Initiated>, IHandleMessages<Submitted>, IHandleTimeouts<CancelDraftOfferManager>
    {
        private static readonly ILog Log = LogManager.GetLogger<DraftOfferManager>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<DraftOfferManagerSagaData> mapper)
        {
            mapper.ConfigureMapping<Initiated>(m => m.DraftId).ToSaga(s => s.DraftId);
            mapper.ConfigureMapping<Submitted>(m => m.DraftId).ToSaga(s => s.DraftId);
        }

        public async Task Handle(Initiated message, IMessageHandlerContext context)
        {
            Log.Info($">>> DRAFT OFFER STARTED: ${message.DraftId}");
            Data.SubmitterId = message.SubmitterId;
            Data.Reference = message.Reference;
            var timespan = TimeSpan.FromSeconds(15);
            await RequestTimeout<CancelDraftOfferManager>(context, timespan).ConfigureAwait(false);
        }

        public Task Handle(Submitted message, IMessageHandlerContext context)
        {
            Log.Info("!!! DRAFT OFFER has been submitted");
            MarkAsComplete();
            return Task.CompletedTask;
        }


        public Task Timeout(CancelDraftOfferManager state, IMessageHandlerContext context)
        {
            if (Data != null)
            {
                Log.Info($"!!! DRAFT OFFER ${Data.Reference} has been cancelled due to age!");
            }
            else
            {
                Log.Warn($"DRAFT OFFER Timeout occurred but State is missing");
            }

            return Task.CompletedTask;
        }
    }

    public interface Initiated : IMessage
    {
        Guid DraftId { get; set; }
        string Reference { get; set; }
        string SubmitterId { get; set; }
    }


    public interface Submitted : IMessage
    {
        Guid DraftId { get; set; }
    }

    public class CancelDraftOfferManager : IMessage { }

    public class DraftOfferManagerSagaData : IContainSagaData
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }
        public Guid DraftId { get; set; }
        public string SubmitterId { get; set; }
        public string Reference { get; set; }
    }
}
