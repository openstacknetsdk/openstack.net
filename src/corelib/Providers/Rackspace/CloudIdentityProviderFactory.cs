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
        private readonly string IdentityUrlBase;
        private readonly CloudIdentity _defaultIdentity;
        private readonly ICache<UserAccess> _tokenCache;
        private readonly IRestService _restService;

        public CloudIdentityProviderFactory(CloudIdentity defaultIdentity, IRestService restService, ICache<UserAccess> tokenCache, string urlBase)
        {
            if (restService == null)
                restService = new JsonRestServices();

            if (tokenCache == null)
                tokenCache = UserAccessCache.Instance;

            IdentityUrlBase = string.IsNullOrEmpty(urlBase) ? "https://identity.api.rackspacecloud.com" : urlBase;

            _restService = restService;
            _tokenCache = tokenCache;
            _defaultIdentity = defaultIdentity;
        }

        /// <inheritdoc/>
        public IExtendedCloudIdentityProvider Get(CloudIdentity identity)
        {
            if (identity == null)
                identity = _defaultIdentity;

            return new GeographicalCloudIdentityProvider(new Uri(IdentityUrlBase), _defaultIdentity, _restService, _tokenCache, HttpResponseCodeValidator.Default);
        }
    }
}
