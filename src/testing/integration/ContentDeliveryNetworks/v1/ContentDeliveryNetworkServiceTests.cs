using net.openstack.Providers.Rackspace;
using OpenStack.ContentDeliveryNetworks.v1.Synchronous;
using Xunit;

namespace OpenStack.ContentDeliveryNetworks.v1
{
    public class ContentDeliveryNetworkServiceTests
    {
        private readonly ContentDeliveryNetworkService _cdnService;

        public ContentDeliveryNetworkServiceTests()
        {
            var identity = TestIdentityProvider.GetIdentityFromEnvironment();
            var authenticationProvider = new CloudIdentityProvider(identity)
            {
                ApplicationUserAgent = "CI-BOT"
            };
            _cdnService = new ContentDeliveryNetworkService(authenticationProvider, "DFW");
        }

        [Fact]
        public void Ping()
        {
            _cdnService.Ping();
        }
    }
}
