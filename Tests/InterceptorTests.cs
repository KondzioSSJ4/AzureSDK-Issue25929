using Azure.Messaging.ServiceBus;
using AzureWebJobs_Issue25929.DynamicPrefetch;
using Castle.DynamicProxy;
using FluentAssertions;

namespace Tests
{
    public class InterceptorTests
    {
        private const string ConnectionString = "Endpoint=sb://sb-invalid-namespace-that-not-exists.servicebus.windows.net/;SharedAccessKeyName=Issue25929Recaiver;SharedAccessKey=1234567891234567891234567891234567891234567=";
        private const string QueueName = "test1";

        [Fact]
        public void ChangeValueOfPrefetch()
        {
            const int NewPrefetchValue = 454;

            var prefetchContainer = new RequestedPrefetchContainer();
            var generator = new ProxyGenerator();
            var interceptor = new ServiceBusInterceptor(generator, prefetchContainer);

            var client = new ServiceBusClient(ConnectionString);
            var newClient = generator.CreateClassProxyWithTarget<ServiceBusClient>(client, interceptor);

            var processor = newClient.CreateProcessor(QueueName);

            var previousPrefetch = processor.PrefetchCount;

            prefetchContainer.Prefetches.TryAdd(QueueName, NewPrefetchValue);

            processor.PrefetchCount.Should().Be(NewPrefetchValue, "value should be provided from prefech container");
            previousPrefetch.Should().NotBe(NewPrefetchValue, "previous value should be provided from default configuration");
        }
    }
}