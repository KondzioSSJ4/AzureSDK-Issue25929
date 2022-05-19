using System.Collections.Concurrent;

namespace AzureWebJobs_Issue25929.DynamicPrefetch
{
    public class RequestedPrefetchContainer
    {
        public ConcurrentDictionary<string, int> Prefetches { get; } = new ConcurrentDictionary<string, int>();
    }
}