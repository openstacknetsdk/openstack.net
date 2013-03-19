﻿using System;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace.Exceptions;

namespace net.openstack.Providers.Rackspace
{
    internal class IdentityProviderFactory : IProviderFactory<IExtendedIdentityProvider, CloudIdentity>
    {
        private readonly string USIdentityUrlBase;
        private readonly string LONIdentityUrlBase;
        private readonly CloudIdentity _defaultIdentity;
        private readonly ICache<UserAccess> _tokenCache;
        private readonly IRestService _restService;

        public IdentityProviderFactory(CloudIdentity defaultIdentity, IRestService restService, ICache<UserAccess> tokenCache, string usInstanceUrlBase, string ukInstanceUrlBase)
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

        public IExtendedIdentityProvider Get(CloudIdentity identity)
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
                    return new GeographicalIdentityProvider(new Uri(USIdentityUrlBase), _defaultIdentity, _restService, _tokenCache);
                case CloudInstance.UK:
                    return new GeographicalIdentityProvider(new Uri(LONIdentityUrlBase), _defaultIdentity, _restService, _tokenCache);
                default:
                    throw new UnknownGeographyException(cloudInstance.ToString());
            }
        }
    }
}
