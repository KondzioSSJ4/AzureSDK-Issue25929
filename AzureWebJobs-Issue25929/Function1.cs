using Castle.DynamicProxy;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace AzureWebJobs_Issue25929
{
    public class Function1
    {
        private readonly List<MessagingProvider> allProviders;
        private readonly IProxyGenerator proxyGenerator;
        private readonly List<InjectedClass> classes;

        public Function1(
            IEnumerable<MessagingProvider> allProviders,
            IProxyGenerator proxyGenerator,
            IEnumerable<InjectedClass> classes)
        {
            this.allProviders = allProviders.ToList();
            this.proxyGenerator = proxyGenerator;
            this.classes = classes.ToList();
        }

        [FunctionName("Function1")]
        public void Run([ServiceBusTrigger("issue-25929", Connection = "ServiceBusCon")] string myQueueItem, ILogger log)
        {
            log.LogWarning($@"Has {allProviders.Count} providers,
where last is used for service bus and it's with type: {allProviders.Last().GetType()}");

            log.LogWarning($@"Because {nameof(InjectedClass)} was injected in dependant project using `TryAddSingleton` instead of AddSingleton then only 1 instance was injected
That's why class can be override, and was created from: {classes.Single().Name} (Expected: `from main startup class`)");
        }
    }
}