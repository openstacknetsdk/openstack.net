using System.Linq;
using System.Net;
using OpenStack.Synchronous;
using OpenStack.Testing;
using Xunit;

namespace OpenStack.Networking.v2
{
    public class NetworkTests
    {
        private readonly NetworkingService _networkingService;

        public NetworkTests()
        {
            _networkingService = new NetworkingService(Stubs.AuthenticationProvider, "region");
        }

        [Fact]
        public void ListNetworks()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new NetworkCollection(new[] {new Network {Id = "network-id"}}));

                var networks = _networkingService.ListNetworks();

                httpTest.ShouldHaveCalled("*/networks");
                Assert.NotNull(networks);
                Assert.Equal(1, networks.Count());
                Assert.Equal("network-id", networks.First().Id);
            }
        }

        [Fact]
        public void CreateNetwork()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Network{Id = "network-id"});

                var definition = new NetworkDefinition();
                var network = _networkingService.CreateNetwork(definition);

                httpTest.ShouldHaveCalled("*/networks");
                Assert.NotNull(network);
                Assert.Equal("network-id", network.Id);
            }
        }

        [Fact]
        public void CreateNetworks()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new NetworkCollection(new[] { new Network { Id = "network-id1" }, new Network{Id = "network-id2"} }));

                var definitions = new[] {new NetworkDefinition(), new NetworkDefinition()};
                var networks = _networkingService.CreateNetworks(definitions);

                httpTest.ShouldHaveCalled("*/networks");
                Assert.NotNull(networks);
                Assert.Equal(2, networks.Count());
                Assert.Equal("network-id1", networks.First().Id);
                Assert.Equal("network-id2", networks.Last().Id);
            }
        }

        [Fact]
        public void GetNetwork()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Network { Id = "network-id" });

                var network = _networkingService.GetNetwork("network-id");

                httpTest.ShouldHaveCalled("*/networks/network-id");
                Assert.NotNull(network);
                Assert.Equal("network-id", network.Id);
            }
        }

        [Fact]
        public void DeleteNetwork()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith((int)HttpStatusCode.NoContent, "All gone!");

                _networkingService.DeleteNetwork("network-id");

                httpTest.ShouldHaveCalled("*/networks/network-id");
            }
        }

        [Fact]
        public void WhenDeleteNetwork_Returns404NotFound_ShouldConsiderRequestSuccessful()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith((int)HttpStatusCode.NotFound, "Not here, boss...");

                _networkingService.DeleteNetwork("network-id");

                httpTest.ShouldHaveCalled("*/networks/network-id");
            }
        }
    }
}
