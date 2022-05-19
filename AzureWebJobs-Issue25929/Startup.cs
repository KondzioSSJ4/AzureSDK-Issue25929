using AzureWebJobs_Issue25929.DynamicPrefetch;
using Castle.DynamicProxy;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

[assembly: WebJobsStartup(typeof(AzureWebJobs_Issue25929.Startup))]

// The order of execution is like that, I can't manipulate when WebJobsStartup is executed from external lib
[assembly: WebJobsStartup(typeof(AzureWebJobs_Issue25929.OtherStartupLike_ServiceBusWebJobsStartup))]

namespace AzureWebJobs_Issue25929
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            TryRemovePreviousImplementation(builder);

            InjectCustomVersionOfMessageProvider(builder);
        }

        private void TryRemovePreviousImplementation(IWebJobsBuilder builder)
        {
            var previousImplementations = builder.Services.Where(x => x.ServiceType == typeof(MessagingProvider)).ToList();

            if (previousImplementations.Count == 0)
            {
                Console.WriteLine($@"Can't remove previous implementation of '{nameof(MessagingProvider)}',
so my will be first on the list.
Any other decoration methods will also fail (like Scrutor),
because in current time I don't have reference to any '{nameof(MessagingProvider)}' implementations");

                return;
            }

            Console.WriteLine($"I can decorate '{nameof(MessagingProvider)}'!");
        }

        private static void InjectCustomVersionOfMessageProvider(IWebJobsBuilder builder)
        {
            builder.Services.AddSingleton<MessagingProvider, CustomMessagingProvider>();

            builder.Services.AddSingleton<IProxyGenerator>(x => new ProxyGenerator())
                .AddSingleton<ServiceBusInterceptor>();

            builder.Services.TryAddSingleton<InjectedClass>(
                x => new InjectedClass("from main startup class"));
        }
    }

    public class OtherStartupLike_ServiceBusWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.TryAddSingleton<InjectedClass>(
                x => new InjectedClass("from additional extension"));
        }
    }

    public class InjectedClass
    {
        public InjectedClass(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}