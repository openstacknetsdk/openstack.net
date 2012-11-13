using System;
using System.Collections.Generic;
using SimpleRestServices.Client;
using net.openstack.Core;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public class ComputeProvider : IComputeProvider 
    {
        private readonly ComputeProviderFactory _factory;

        public ComputeProvider(IIdentityProvider identityProvider = null, IRestService restService = null)
        {
            _factory = new ComputeProviderFactory(identityProvider, restService);
        }

        public IEnumerable<Server> ListServers(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null)
        {
            var provider = _factory.Get(identity.Region);

            return provider.ListServers(identity);
        }

        public IEnumerable<ServerDetails> ListServersWithDetails(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = new int?(), DateTime? changesSince = new DateTime?())
        {
            var provider = _factory.Get(identity.Region);

            return provider.ListServersWithDetails(identity);
        }

        public Metadata ListMetadata(string cloudServerId, CloudIdentity identity)
        {
            var provider = _factory.Get(identity.Region);

            return provider.ListMetadata(cloudServerId, identity);
        }

        public ServerDetails GetDetails(string cloudServerId, CloudIdentity identity)
        {
            var provider = _factory.Get(identity.Region);

            return provider.GetDetails(cloudServerId, identity);
        }

        public NewServer CreateServer(string cloudServerName, string friendlyName, string imageName, string flavor, CloudIdentity identity)
        {
            var provider = _factory.Get(identity.Region);

            return provider.CreateServer(cloudServerName, friendlyName, imageName, flavor, identity);
        }

        public bool UpdateServer(string cloudServerId, CloudIdentity identity, string name, string ipV4Address, string ipV6Address)
        {
            var provider = _factory.Get(identity.Region);

            return provider.UpdateServer(cloudServerId, identity, name, ipV4Address, ipV6Address);
        }

        public bool DeleteServer(string cloudServerId, CloudIdentity identity)
        {
            var provider = _factory.Get(identity.Region);

            return provider.DeleteServer(cloudServerId, identity);
        }
    }
}
