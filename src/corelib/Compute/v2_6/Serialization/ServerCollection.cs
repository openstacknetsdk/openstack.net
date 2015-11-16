namespace OpenStack.Compute.v2_6.Serialization
{
    /// <summary />
    public class ServerCollection<T> : v2_2.Serialization.ServerCollection<T>
    { }

    /// <summary />
    public class ServerCollection : ServerCollection<ServerReference>
    { }
}