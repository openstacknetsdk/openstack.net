using System;
using System.Linq;
using System.Net;
using OpenStack.Networking.v2.Serialization;
using OpenStack.Synchronous;
using OpenStack.Testing;
using Xunit;

namespace OpenStack.Networking.v2
{
    public class PortTests
    {
        private readonly NetworkingService _networkingService;

        public PortTests()
        {
            _networkingService = new NetworkingService(Stubs.AuthenticationProvider, "region");
        }

        [Fact]
        public void ListPorts()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new PortCollection(new[] {new Port {Id = Guid.Empty}}));

                var ports = _networkingService.ListPorts();

                httpTest.ShouldHaveCalled("*/ports");
                Assert.NotNull(ports);
                Assert.Equal(1, ports.Count());
                Assert.Equal(Guid.Empty, ports.First().Id);
            }
        }

        [Fact]
        public void CreatePort()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Port { Id = Guid.Empty });

                var definition = new PortCreateDefinition(Guid.NewGuid());
                var port = _networkingService.CreatePort(definition);

                httpTest.ShouldHaveCalled("*/ports");
                Assert.NotNull(port);
                Assert.Equal(Guid.Empty, port.Id);
            }
        }

        [Fact]
        public void CreatePorts()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new PortCollection(new[] {new Port {Name = "port-1"}, new Port {Name = "port-2"}}));

                Identifier networkId = Guid.NewGuid();
                var definitions = new[] { new PortCreateDefinition(networkId), new PortCreateDefinition(networkId),  };
                var ports = _networkingService.CreatePorts(definitions);

                httpTest.ShouldHaveCalled("*/ports");
                Assert.NotNull(ports);
                Assert.Equal(2, ports.Count());
                Assert.Equal("port-1", ports.First().Name);
                Assert.Equal("port-2", ports.Last().Name);
            }
        }

        [Fact]
        public void GetPorts()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier portId = Guid.Empty;
                httpTest.RespondWithJson(new Port { Id = portId });

                var port = _networkingService.GetPort(portId);

                httpTest.ShouldHaveCalled("*/ports/00000000-0000-0000-0000-000000000000");
                Assert.NotNull(port);
                Assert.Equal(portId, port.Id);
            }
        }

        [Fact]
        public void DeletePort()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith((int)HttpStatusCode.NoContent, "All gone!");

                _networkingService.DeletePort(Guid.Empty);

                httpTest.ShouldHaveCalled("*/ports/00000000-0000-0000-0000-000000000000");
            }
        }

        [Fact]
        public void WhenDeletePort_Returns404NotFound_ShouldConsiderRequestSuccessful()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith((int)HttpStatusCode.NotFound, "Not here, boss...");

                _networkingService.DeletePort(Guid.Empty);

                httpTest.ShouldHaveCalled("*/ports/00000000-0000-0000-0000-000000000000");
            }
        }

        [Fact]
        public void UpdatePort()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier portId = Guid.Empty;
                httpTest.RespondWithJson(new Port { Id = portId });

                var definition = new PortUpdateDefinition { Name = "new subnet name" };
                var port = _networkingService.UpdatePort(portId, definition);

                httpTest.ShouldHaveCalled("*/ports/00000000-0000-0000-0000-000000000000");
                Assert.NotNull(port);
                Assert.Equal(portId, port.Id);
            }
        }
    }
}
