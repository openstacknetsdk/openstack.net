namespace net.openstack.Providers.Rackspace
{
    internal interface IProviderFactory<T>
    {
        T Get(string geo);
    }
}