using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenStack.Networking.v2.Serialization;
using OpenStack.ObjectStorage.v1.Serialization;
using OpenStack.Synchronous;
using OpenStack.Testing;
using OpenStackNet.Testing.Unit;
using Xunit;
using Assert = Xunit.Assert;

namespace OpenStack.ObjectStorage.v1
{
    [TestClass]
    public class ObjectStorageTests
    {
        private readonly ObjectStorageService _objectStorageService;

        public ObjectStorageTests()
        {
            _objectStorageService = new ObjectStorageService(Stubs.AuthenticationProvider, "region");
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task ObjectStorageTests_ListNetworks()
        {
            using (var httpTest = new HttpTest())
            {
	            var sourceContainers = new ContainerCollection(new[]
	            {
		            new Container() {Name = "www", Bytes = 1003, Count = 3},
		            new Container() {Name = "media", Bytes = 1005, Count = 5}
	            });
                httpTest.RespondWithJson(sourceContainers);

                var containers = await _objectStorageService.ListContainersAsync();

                httpTest.ShouldHaveCalled("*");
                Assert.NotNull(containers);
                Assert.Equal(sourceContainers.Count, containers.Count());
                Assert.Equal(sourceContainers[0].Name, containers.First().Name);
                Assert.Equal(sourceContainers[0].Bytes, containers.First().Bytes);
                Assert.Equal(sourceContainers[0].Count, containers.First().Count);
            }
        }


		/*
		[TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void CreateNetwork()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                httpTest.RespondWithJson(new Network{Id = networkId});

                var definition = new NetworkDefinition();
                var network = _objectStorageService.CreateNetwork(definition);

                httpTest.ShouldHaveCalled("/networks");
                Assert.NotNull(network);
                Assert.Equal(networkId, network.Id);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void CreateNetworks()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new NetworkCollection(new[] { new Network { Name = "network-1"}, new Network{Name = "network-2"} }));

                var definitions = new[] {new NetworkDefinition(), new NetworkDefinition()};
                var networks = _objectStorageService.CreateNetworks(definitions);

                httpTest.ShouldHaveCalled("/networks");
                Assert.NotNull(networks);
                Assert.Equal(2, networks.Count());
                Assert.Equal("network-1", networks.First().Name);
                Assert.Equal("network-2", networks.Last().Name);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void GetNetwork()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                httpTest.RespondWithJson(new Network { Id = networkId });

                var network = _objectStorageService.GetNetwork(networkId);

                httpTest.ShouldHaveCalled("/networks/" + networkId);
                Assert.NotNull(network);
                Assert.Equal(networkId, network.Id);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void DeleteNetwork()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                httpTest.RespondWith((int)HttpStatusCode.NoContent, "All gone!");

                _objectStorageService.DeleteNetwork(networkId);

                httpTest.ShouldHaveCalled("/networks/" + networkId);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void WhenDeleteNetwork_Returns404NotFound_ShouldConsiderRequestSuccessful()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                httpTest.RespondWith((int)HttpStatusCode.NotFound, "Not here, boss...");

                _objectStorageService.DeleteNetwork(networkId);

                httpTest.ShouldHaveCalled("/networks/" + networkId);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void UpdateNetwork()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier networkId = Guid.NewGuid();
                httpTest.RespondWithJson(new Network {Id = networkId});

                var definition = new NetworkDefinition { Name = "new network name" };
                var network = _objectStorageService.UpdateNetwork(networkId, definition);

                httpTest.ShouldHaveCalled("/networks/" + networkId);
                Assert.NotNull(network);
                Assert.Equal(networkId, network.Id);
            }
        }
		*/
    }
}
