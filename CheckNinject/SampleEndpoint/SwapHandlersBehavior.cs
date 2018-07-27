using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

namespace SampleEndpoint
{
    class SwapHandlersBehavior : Behavior<IInvokeHandlerContext>
    {
        public bool Swapping { get; set; }

        public override Task Invoke(IInvokeHandlerContext context, Func<Task> next)
        {
            if (Swapping)
            {
                return context.MessageHandler.HandlerType == typeof(SomeMessageHandler) 
                    ? Task.CompletedTask 
                    : next();
            }

            return context.MessageHandler.HandlerType == typeof(ReplacementHandler) 
                ? Task.CompletedTask 
                : next();
        }
    }
}