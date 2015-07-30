using System.Linq;
using System.Net;
using OpenStack.Synchronous;
using OpenStack.Testing;
using Xunit;

namespace OpenStack.Networking.v2
{
    public class SubnetTests
    {
        private readonly NetworkingService _networkingService;

        public SubnetTests()
        {
            _networkingService = new NetworkingService(Stubs.AuthenticationProvider, "region");
        }

        [Fact]
        public void ListSubnets()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new SubnetCollection(new[] {new Subnet {Id = "subnet-id"}}));

                var subnets = _networkingService.ListSubnets();

                httpTest.ShouldHaveCalled("*/subnets");
                Assert.NotNull(subnets);
                Assert.Equal(1, subnets.Count());
                Assert.Equal("subnet-id", subnets.First().Id);
            }
        }

        [Fact]
        public void CreateSubnet()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Subnet { Id = "subnet-id" });

                var definition = new SubnetCreateDefinition("network-id", IPVersion.IP, "10.0.0.0/24");
                var subnet = _networkingService.CreateSubnet(definition);
                
                httpTest.ShouldHaveCalled("*/subnets");
                Assert.NotNull(subnet);
                Assert.Equal("subnet-id", subnet.Id);
            }
        }

        [Fact]
        public void CreateSubnets()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new SubnetCollection(new[] { new Subnet { Id = "subnet-id1" }, new Subnet { Id = "subnet-id2" } }));

                var definitions = new[] { new SubnetCreateDefinition("network-id", IPVersion.IP, "{cidr-1}"), new SubnetCreateDefinition("network-id", IPVersion.IPv6, "{cidr-2}") };
                var subnets = _networkingService.CreateSubnets(definitions);

                httpTest.ShouldHaveCalled("*/subnets");
                Assert.NotNull(subnets);
                Assert.Equal(2, subnets.Count());
                Assert.Equal("subnet-id1", subnets.First().Id);
                Assert.Equal("subnet-id2", subnets.Last().Id);
            }
        }

        [Fact]
        public void GetSubnets()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Subnet { Id = "subnet-id" });

                var subnet = _networkingService.GetSubnet("subnet-id");

                httpTest.ShouldHaveCalled("*/subnets/subnet-id");
                Assert.NotNull(subnet);
                Assert.Equal("subnet-id", subnet.Id);
            }
        }

        [Fact]
        public void DeleteSubnet()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith((int)HttpStatusCode.NoContent, "All gone!");

                _networkingService.DeleteSubnet("subnet-id");

                httpTest.ShouldHaveCalled("*/subnets/subnet-id");
            }
        }

        [Fact]
        public void WhenDeleteSubnet_Returns404NotFound_ShouldConsiderRequestSuccessful()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith((int)HttpStatusCode.NotFound, "Not here, boss...");

                _networkingService.DeleteSubnet("subnet-id");

                httpTest.ShouldHaveCalled("*/subnets/subnet-id");
            }
        }

        [Fact]
        public void UpdateSubnet()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Subnet { Id = "subnet-id" });

                var definition = new SubnetUpdateDefinition { Name = "new subnet name" };
                var subnet = _networkingService.UpdateSubnet("subnet-id", definition);

                httpTest.ShouldHaveCalled("*/subnets/subnet-id");
                Assert.NotNull(subnet);
                Assert.Equal("subnet-id", subnet.Id);
            }
        }
    }
}
