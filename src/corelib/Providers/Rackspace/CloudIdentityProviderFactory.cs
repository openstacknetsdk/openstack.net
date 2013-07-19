using System;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core.Caching;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace.Exceptions;
using net.openstack.Providers.Rackspace.Objects;
using net.openstack.Providers.Rackspace.Validators;

namespace net.openstack.Providers.Rackspace
{
    internal class CloudIdentityProviderFactory : IProviderFactory<IExtendedCloudIdentityProvider, CloudIdentity>
    {
        private readonly string USIdentityUrlBase;
        private readonly string LONIdentityUrlBase;
        private readonly CloudIdentity _defaultIdentity;
        private readonly ICache<UserAccess> _tokenCache;
        private readonly IRestService _restService;

        public CloudIdentityProviderFactory(CloudIdentity defaultIdentity, IRestService restService, ICache<UserAccess> tokenCache, string usInstanceUrlBase, string ukInstanceUrlBase)
        {
            if (restService == null)
                restService = new JsonRestServices();

            if (tokenCache == null)
                tokenCache = UserAccessCache.Instance;

            USIdentityUrlBase = string.IsNullOrWhiteSpace(usInstanceUrlBase) ? "https://identity.api.rackspacecloud.com" : usInstanceUrlBase;
            LONIdentityUrlBase = string.IsNullOrWhiteSpace(ukInstanceUrlBase) ? "https://lon.identity.api.rackspacecloud.com" : ukInstanceUrlBase;

            _restService = restService;
            _tokenCache = tokenCache;
            _defaultIdentity = defaultIdentity;
        }

        public IExtendedCloudIdentityProvider Get(CloudIdentity identity)
        {
            if (identity == null)
                identity = _defaultIdentity;

            var rackspaceCloudIdentity = identity as RackspaceCloudIdentity;

            var cloudInstance = CloudInstance.Default;
            if (rackspaceCloudIdentity != null)
                cloudInstance = rackspaceCloudIdentity.CloudInstance;

            switch (cloudInstance)
            {
                case CloudInstance.Default:
                    return new GeographicalCloudIdentityProvider(new Uri(USIdentityUrlBase), _defaultIdentity, _restService, _tokenCache, HttpResponseCodeValidator.Default);
                case CloudInstance.UK:
                    return new GeographicalCloudIdentityProvider(new Uri(LONIdentityUrlBase), _defaultIdentity, _restService, _tokenCache, HttpResponseCodeValidator.Default);
                default:
                    throw new UnknownGeographyException(cloudInstance.ToString());
            }
        }
    }
}
