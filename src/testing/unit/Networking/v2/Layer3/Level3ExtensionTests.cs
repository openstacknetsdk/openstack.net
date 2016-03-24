using System;
using System.Linq;
using System.Net;
using OpenStack.Serialization;
using OpenStack.Networking.v2.Layer3.Synchronous;
using OpenStack.Networking.v2.Serialization;
using OpenStack.Testing;
using Xunit;

namespace OpenStack.Networking.v2.Layer3
{
    public class Level3ExtensionTests
    {
        private readonly NetworkingService _networking;

        public Level3ExtensionTests()
        {
            _networking = new NetworkingService(Stubs.AuthenticationProvider, "region");
        }

        [Fact]
        public void CreateFloatingIP()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                Identifier floatingIPId = Guid.NewGuid();
                httpTest.RespondWithJson(new FloatingIP { Id = floatingIPId });

                var definition = new FloatingIPCreateDefinition(networkId);
                var result = _networking.CreateFloatingIP(definition);

                httpTest.ShouldHaveCalled("*/floatingips");
                Assert.NotNull(result);
                Assert.Equal(floatingIPId, result.Id);
                Assert.IsType<NetworkingApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Fact]
        public void GetFloatingIP()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier floatingIPId = Guid.NewGuid();
                httpTest.RespondWithJson(new FloatingIP { Id = floatingIPId });

                var result = _networking.GetFloatingIP(floatingIPId);

                httpTest.ShouldHaveCalled($"*/floatingips/{floatingIPId}");
                Assert.NotNull(result);
                Assert.Equal(floatingIPId, result.Id);
                Assert.IsType<NetworkingApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Fact]
        public void ListFloatingIPs()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier floatingIPId = Guid.NewGuid();
                httpTest.RespondWithJson(new FloatingIPCollection
                {
                    new FloatingIP { Id = floatingIPId }
                });

                var results = _networking.ListFloatingIPs(new FloatingIPListOptions {Status = FloatingIPStatus.Active});

                httpTest.ShouldHaveCalled("*/floatingips?status=ACTIVE");
                Assert.Equal(1, results.Count());
                var result = results.First();
                Assert.Equal(floatingIPId, result.Id);
                Assert.IsType<NetworkingApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Fact]
        public void AssociateFloatingIP()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier portId = Guid.NewGuid();
                Identifier floatingIPId = Guid.NewGuid();
                httpTest.RespondWithJson(new FloatingIP { Id = floatingIPId });
                httpTest.RespondWithJson(new FloatingIP { Id = floatingIPId, PortId = portId});

                var floatingIP = _networking.GetFloatingIP(floatingIPId);
                floatingIP.Associate(portId);

                httpTest.ShouldHaveCalled($"*/floatingips/{floatingIPId}");
                Assert.Equal(floatingIP.PortId, portId);
            }
        }

        [Fact]
        public void DisassociateFloatingIP()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier portId = Guid.NewGuid();
                Identifier floatingIPId = Guid.NewGuid();
                httpTest.RespondWithJson(new FloatingIP { Id = floatingIPId, PortId =  portId});
                httpTest.RespondWithJson(new FloatingIP { Id = floatingIPId });

                var floatingIP = _networking.GetFloatingIP(floatingIPId);
                floatingIP.Disassociate();

                httpTest.ShouldHaveCalled($"*/floatingips/{floatingIPId}");
                Assert.Null(floatingIP.PortId);
            }
        }

        [Theory]
        [InlineData(HttpStatusCode.Accepted)]
        [InlineData(HttpStatusCode.NotFound)]
        public void DeleteFloatingIP(HttpStatusCode responseCode)
        {
            using (var httpTest = new HttpTest())
            {
                Identifier floatingIPId = Guid.NewGuid();
                httpTest.RespondWith((int)responseCode, "All gone!");

                _networking.DeleteFloatingIP(floatingIPId);

                httpTest.ShouldHaveCalled($"*/floatingips/{floatingIPId}");
            }
        }
    }
}
