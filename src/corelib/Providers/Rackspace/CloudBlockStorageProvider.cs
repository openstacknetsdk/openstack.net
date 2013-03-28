using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace.Validators;

namespace net.openstack.Providers.Rackspace
{
    public class CloudBlockStorageProvider : ProviderBase, ICloudBlockStorageProvider
    {

        private readonly ICloudBlockStorageValidator _cloudBlockStorageValidator;

        public CloudBlockStorageProvider()
            : this(null) { }

        public CloudBlockStorageProvider(CloudIdentity defaultIdentity)
            : this(defaultIdentity, new IdentityProvider(), new JsonRestServices(), new CloudBlockStorageValidator()) { }

        public CloudBlockStorageProvider(IIdentityProvider identityProvider, IRestService restService, ICloudBlockStorageValidator cloudBlockStorageValidator)
            : this(null, identityProvider, restService, cloudBlockStorageValidator) { }

        public CloudBlockStorageProvider(CloudIdentity defaultIdentity, IIdentityProvider identityProvider, IRestService restService, ICloudBlockStorageValidator cloudBlockStorageValidator)
            : base(defaultIdentity, identityProvider, restService)
        {
            _cloudBlockStorageValidator = cloudBlockStorageValidator;
        }




        #region Private methods

        protected string GetServiceEndpoint(CloudIdentity identity = null, string region = null)
        {
            return base.GetPublicServiceEndpoint(identity, "cloudBlockStorage", region);
        }

        #endregion
    }
}
