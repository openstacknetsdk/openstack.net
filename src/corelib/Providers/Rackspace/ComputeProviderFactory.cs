using System;
using SimpleRestServices.Client;
using SimpleRestServices.Client.Json;
using net.openstack.Core;
using net.openstack.Providers.Rackspace.Exceptions;

namespace net.openstack.Providers.Rackspace
{
    internal class ComputeProviderFactory : IProviderFactory<IComputeProvider>
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IRestService _restService;

        internal ComputeProviderFactory(IIdentityProvider identityProvider = null, IRestService restService = null)
        {
            if(identityProvider == null)
                identityProvider = new IdentityProvider();

            if(restService == null)
                restService = new JsonRestServices();

            _identityProvider = identityProvider;
            _restService = restService;
        }

        public IComputeProvider Get(string geo)
        {
            switch (geo.ToLower())
            {
                case "dfw":
                    return new RegionalComputeProvider(_identityProvider, _restService);
                case "ord":
                    return new RegionalComputeProvider(_identityProvider, _restService);
                case "lon":
                    return new RegionalComputeProvider(_identityProvider, _restService);
                default:
                    throw new UnknownGeographyException(geo);
            }
        }
    }
}
