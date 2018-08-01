using System;
using System.Threading.Tasks;
using NServiceBus.UnitOfWork;

namespace SampleEndpoint
{
    internal class UnitOfWorkProviderAdaptor : IManageUnitsOfWork
    {
        public Task Begin()
        {
            return Task.CompletedTask;
        }

        public Task End(Exception ex = null)
        {
            return Task.CompletedTask;
        }
    }
}