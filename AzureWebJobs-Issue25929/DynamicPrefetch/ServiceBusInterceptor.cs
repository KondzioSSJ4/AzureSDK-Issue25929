using Azure.Messaging.ServiceBus;
using Castle.DynamicProxy;

namespace AzureWebJobs_Issue25929.DynamicPrefetch
{
    public class ServiceBusInterceptor : IInterceptor
    {
        private readonly IProxyGenerator proxyGenerator;
        private readonly RequestedPrefetchContainer requestedPrefetchContainer;

        public ServiceBusInterceptor(
            IProxyGenerator proxyGenerator,
            RequestedPrefetchContainer requestedPrefetchContainer)
        {
            this.proxyGenerator = proxyGenerator;
            this.requestedPrefetchContainer = requestedPrefetchContainer;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            if (invocation.ReturnValue is ServiceBusProcessor proc)
            {
                var proxyOptions = new ProxyGenerationOptions();
                proxyOptions.AddMixinInstance(new HasBackingField());

                var processorInterceptor = new ServiceBusProcessorInterceptor(requestedPrefetchContainer, proc.EntityPath);

                var newProcessor = proxyGenerator.CreateClassProxyWithTarget<ServiceBusProcessor>(proc, proxyOptions, processorInterceptor);
                invocation.ReturnValue = newProcessor;
            }
        }
    }
}