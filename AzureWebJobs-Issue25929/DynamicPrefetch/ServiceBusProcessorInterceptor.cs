using Azure.Messaging.ServiceBus;
using Castle.DynamicProxy;

namespace AzureWebJobs_Issue25929.DynamicPrefetch
{
    public class ServiceBusProcessorInterceptor : IInterceptor
    {
        private RequestedPrefetchContainer prefetchContainer;
        private readonly string queueName;

        public ServiceBusProcessorInterceptor(RequestedPrefetchContainer prefetchContainer, string queueName)
        {
            this.prefetchContainer = prefetchContainer;
            this.queueName = queueName;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            if (invocation.Method.Name == $"get_{nameof(ServiceBusProcessor.PrefetchCount)}"
                && prefetchContainer.Prefetches.TryGetValue(queueName, out var customPrefetch))
            {
                invocation.ReturnValue = customPrefetch;
            }
        }
    }
}