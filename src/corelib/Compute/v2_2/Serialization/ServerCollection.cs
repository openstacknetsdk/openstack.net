namespace OpenStack.Compute.v2_2.Serialization
{
    /// <summary />
    public class ServerCollection<T> : v2_1.Serialization.ServerCollection<T>
    { }

    /// <summary />
    public class ServerCollection : ServerCollection<ServerReference>
    { }
}