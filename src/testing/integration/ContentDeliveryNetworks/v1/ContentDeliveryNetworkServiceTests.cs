using net.openstack.Providers.Rackspace;
using OpenStack.Synchronous;
using Xunit;
using Xunit.Abstractions;

namespace OpenStack.ContentDeliveryNetworks.v1
{
    public class ContentDeliveryNetworkServiceTests
    {
        private readonly ContentDeliveryNetworkService _cdnService;

        public ContentDeliveryNetworkServiceTests(ITestOutputHelper testLog)
        {
            OpenStackNet.Tracing.Http.Listeners.Add(new XunitTraceListener(testLog));

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
