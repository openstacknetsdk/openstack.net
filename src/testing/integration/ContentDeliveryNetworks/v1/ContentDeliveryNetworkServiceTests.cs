using OpenStack.Synchronous;
using Xunit;
using Xunit.Abstractions;

namespace OpenStack.ContentDeliveryNetworks.v1
{
    [Trait("ci","false")]
    public class ContentDeliveryNetworkServiceTests
    {
        private readonly ContentDeliveryNetworkService _cdnService;

        public ContentDeliveryNetworkServiceTests(ITestOutputHelper testLog)
        {
            OpenStackNet.Tracing.Http.Listeners.Add(new XunitTraceListener(testLog));

            var authenticationProvider = TestIdentityProvider.GetIdentityProvider();
            _cdnService = new ContentDeliveryNetworkService(authenticationProvider, "DFW");
        }

        [Fact]
        public void Ping()
        {
            _cdnService.Ping();
        }
    }
}
