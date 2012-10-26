using SimpleRestServices.Client;
using SimpleRestServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public class ComputeProvider : IComputeProvider 
    {
        private readonly ComputeProviderFactory _factory;

        public ComputeProvider() : this(new JsonRestServices(), new IdentityTokenCache())
        {
        }

        public ComputeProvider(IRestService restService, ICache<IdentityToken> tokenCache)
        {
            _factory = new ComputeProviderFactory();
        }

        public MetaData GetMetaData(string apiServerId, CloudIdentity identity)
        {
            var provider = _factory.Get(identity.Region);

            return provider.GetMetaData(apiServerId, identity);
        }

        public ServerDetails GetDetails(string apiServerId, CloudIdentity identity)
        {
            var provider = _factory.Get(identity.Region);

            return provider.GetDetails(apiServerId, identity);
        }

        public NewServer CreateServer(string cloudServerName, string friendlyName, string imageName, string flavor, CloudIdentity identity)
        {
            var provider = _factory.Get(identity.Region);

            return provider.CreateServer(cloudServerName, friendlyName, imageName, flavor, identity);
        }

        public bool DeleteServer(string cloudServerId, CloudIdentity identity)
        {
            var provider = _factory.Get(identity.Region);

            return provider.DeleteServer(cloudServerId, identity);
        }
    }
}
