namespace AzureWebJobs_Issue25929.DynamicPrefetch
{
    public class HasPrefetch
    {
        public virtual int PrefetchCount { get; }
    }

    public interface IHasBackingField
    {
        HasPrefetch Retrieve();
    }

    /// <summary>
    /// Required to load property into proxy
    /// </summary>
    public class HasBackingField : IHasBackingField
    {
        public HasPrefetch Retrieve()
        {
            return new HasPrefetch();
        }
    }
}