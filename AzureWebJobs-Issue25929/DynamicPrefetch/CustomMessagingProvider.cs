using Azure.Core;
using Azure.Messaging.ServiceBus;
using Castle.DynamicProxy;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace AzureWebJobs_Issue25929.DynamicPrefetch
{
    public class CustomMessagingProvider : MessagingProvider
    {
        private readonly ILogger<CustomMessagingProvider> logger;
        private readonly IProxyGenerator proxy;
        private readonly ServiceBusInterceptor interceptor;

        public CustomMessagingProvider(
            ILogger<CustomMessagingProvider> logger,
            IProxyGenerator proxy,
            ServiceBusInterceptor interceptor,
            IOptions<ServiceBusOptions> options)
            : base(options)
        {
            this.logger = logger;
            this.proxy = proxy;
            this.interceptor = interceptor;
        }

        protected override ServiceBusReceiver CreateBatchMessageReceiver(ServiceBusClient client, string entityPath, ServiceBusReceiverOptions options)
        {
            LogCreation();
            return base.CreateBatchMessageReceiver(client, entityPath, options);
        }

        protected override ServiceBusClient CreateClient(string connectionString, ServiceBusClientOptions options)
        {
            LogCreation();
            return base.CreateClient(connectionString, options);
        }

        protected override ServiceBusClient CreateClient(string fullyQualifiedNamespace, TokenCredential credential, ServiceBusClientOptions options)
        {
            LogCreation();
            return base.CreateClient(fullyQualifiedNamespace, credential, options);
        }

        protected override MessageProcessor CreateMessageProcessor(ServiceBusClient client, string entityPath, ServiceBusProcessorOptions options)
        {
            LogCreation();
            var proxyClient = proxy.CreateInterfaceProxyWithTarget<ServiceBusClient>(client, interceptor);
            return base.CreateMessageProcessor(proxyClient, entityPath, options);
        }

        protected override ServiceBusSender CreateMessageSender(ServiceBusClient client, string entityPath)
        {
            LogCreation();
            return base.CreateMessageSender(client, entityPath);
        }

        protected override ServiceBusProcessor CreateProcessor(ServiceBusClient client, string entityPath, ServiceBusProcessorOptions options)
        {
            LogCreation();
            return base.CreateProcessor(client, entityPath, options);
        }

        protected override SessionMessageProcessor CreateSessionMessageProcessor(ServiceBusClient client, string entityPath, ServiceBusSessionProcessorOptions options)
        {
            LogCreation();
            return base.CreateSessionMessageProcessor(client, entityPath, options);
        }

        protected override ServiceBusSessionProcessor CreateSessionProcessor(ServiceBusClient client, string entityPath, ServiceBusSessionProcessorOptions options)
        {
            LogCreation();
            return base.CreateSessionProcessor(client, entityPath, options);
        }

        private void LogCreation([CallerMemberName] string caller = null)
        {
            logger.LogWarning($"Created instance in: {caller}");
        }
    }
}