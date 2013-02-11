using System;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace.Exceptions;

namespace net.openstack.Providers.Rackspace
{
    internal class IdentityProviderFactory : IProviderFactory<IExtendedIdentityProvider, CloudInstance>
    {
        private readonly string USIdentityUrlBase;
        private readonly string LONIdentityUrlBase;

        private readonly ICache<UserAccess> _tokenCache;
        private readonly IRestService _restService;

        public IdentityProviderFactory(IRestService restService = null, ICache<UserAccess> tokenCache = null, string usInstanceUrlBase = null, string ukInstanceUrlBase = null)
        {
            if (restService == null)
                restService = new JsonRestServices();

            if (tokenCache == null)
                tokenCache = UserAccessCache.Instance;

            USIdentityUrlBase = string.IsNullOrWhiteSpace(usInstanceUrlBase) ? "https://identity.api.rackspacecloud.com" : usInstanceUrlBase;
            LONIdentityUrlBase = string.IsNullOrWhiteSpace(ukInstanceUrlBase) ? "https://lon.identity.api.rackspacecloud.com" : ukInstanceUrlBase;

            _restService = restService;
            _tokenCache = tokenCache;
        }

        public IExtendedIdentityProvider Get(CloudInstance cloudInstance)
        {
            switch (cloudInstance)
            {
                case CloudInstance.Default:
                    return new GeographicalIdentityProvider(new Uri(USIdentityUrlBase), _restService, _tokenCache);
                case CloudInstance.UK:
                    return new GeographicalIdentityProvider(new Uri(LONIdentityUrlBase), _restService, _tokenCache);
                default:
                    throw new UnknownGeographyException(cloudInstance.ToString());
            }
        }
    }
}
