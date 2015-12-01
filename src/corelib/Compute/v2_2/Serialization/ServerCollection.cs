namespace OpenStack.Compute.v2_2.Serialization
{
    /// <summary />
    public class ServerCollection<TPage, TItem> : v2_1.Serialization.ServerCollection<TPage, TItem>
        where TPage : ServerCollection<TPage, TItem>
    { }

    /// <summary />
    public class ServerCollection : ServerCollection<ServerCollection, ServerReference>
    { }
}