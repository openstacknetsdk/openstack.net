namespace net.openstack.Providers.Rackspace
{
    internal interface IProviderFactory<T, T2>
    {
        T Get(T2 key);
    }
}