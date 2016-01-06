namespace OpenStack.Serialization
{
    /// <summary />
    public interface IPageLink
    {
        /// <summary />
        string Url { get; }

        /// <summary />
        bool IsNextPage { get; }
    }
}