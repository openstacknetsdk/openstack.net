using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Objects;

namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    /// <summary>
    /// Summary description for CloudNetworksTests
    /// </summary>
    [TestClass]
    public class CloudNetworksTests
    {
        private static CloudIdentity _testIdentity;
        private static string _created_network_id;
        private static string _created_network_id2;
        private static string _created_server_id;

        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _testIdentity = new RackspaceCloudIdentity(Bootstrapper.Settings.TestIdentity);
        }

        [TestMethod]
        public void Should_Have_Networks()
        {
            var provider = new CloudNetworksProvider(_testIdentity);
            var networks = provider.ListNetworks();
            Assert.IsNotNull(networks);
        }

        [TestMethod]
        public void Should_Have_Private_Network()
        {
            var provider = new CloudNetworksProvider(_testIdentity);
            var networks = provider.ListNetworks();
            Assert.IsNotNull(networks.SingleOrDefault(n => n.Id == "11111111-1111-1111-1111-111111111111" && n.Label == "private"));
        }

        [TestMethod]
        public void Should_Have_Public_Network()
        {
            var provider = new CloudNetworksProvider(_testIdentity);
            var networks = provider.ListNetworks();
            Assert.IsNotNull(networks.SingleOrDefault(n => n.Id == "00000000-0000-0000-0000-000000000000" && n.Label == "public"));
        }

        [TestMethod]
        public void Should_Create_A_Network()
        {
            var provider = new CloudNetworksProvider(_testIdentity);
            var newNetwork = provider.CreateNetwork("192.0.2.0/24", "net-sdk-test-network");
            Assert.IsNotNull(newNetwork);
            _created_network_id = newNetwork.Id;
        }

        [TestMethod]
        public void Should_Show_Newly_Created_Network()
        {
            var provider = new CloudNetworksProvider(_testIdentity);
            var network = provider.ShowNetwork(_created_network_id);
            Assert.IsNotNull(network);
            Assert.AreEqual(network.Label, "net-sdk-test-network");
            Assert.AreEqual(network.Cidr, "192.0.2.0/24");
        }


        [TestMethod]
        public void Should_Delete_Newly_Created_Network()
        {
            var provider = new CloudNetworksProvider(_testIdentity);
            Assert.IsTrue(provider.DeleteNetwork(_created_network_id));
        }

        [TestMethod]
        public void Should_Not_Find_Just_Deleted_Network()
        {
            var provider = new CloudNetworksProvider(_testIdentity);

            try
            {
                var network = provider.ShowNetwork(_created_network_id);
                Assert.Fail("Deleted network was still found in ShowNetwork");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(net.openstack.Core.Exceptions.Response.ItemNotFoundException), "Expected ItemNotFoundException was not thrown");
            }
        }


        [TestMethod]
        public void Should_Create_A_Network_For_A_Later_Server()
        {
            var provider = new CloudNetworksProvider(_testIdentity);
            var newNetwork = provider.CreateNetwork("192.0.2.0/24", "net-sdk-test-network-for-server");
            Assert.IsNotNull(newNetwork);
            _created_network_id2 = newNetwork.Id;
        }


        [TestMethod]
        public void Should_Create_A_Server_With_New_Network_And_Wait_For_Active()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var networks = new List<string>() { _created_network_id2 };
            var testServer = provider.CreateServer("net-sdk-test-server", "d531a2dd-7ae9-4407-bb5a-e5ea03303d98", "2", networks: networks);

            Assert.IsNotNull(testServer.Id);
            _created_server_id = testServer.Id;

            var serverDetails = provider.WaitForServerActive(_created_server_id);
            Assert.AreEqual("ACTIVE", serverDetails.Status);
        }

        [TestMethod]
        public void Should_Verify_Server_Has_New_Network()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var testServer = provider.GetDetails(_created_server_id);
            Assert.IsNotNull(testServer.Addresses.SingleOrDefault(x => x.Key == "net-sdk-test-network-for-server"));
        }

        [TestMethod]
        public void Should_Fail_Deleting_Network_When_Server_Is_Attached()
        {
            var provider = new CloudNetworksProvider(_testIdentity);

            try
            {
                provider.DeleteNetwork(_created_network_id2);
                Assert.Fail("Network should not be deletable if a server exists in it");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(net.openstack.Core.Exceptions.UserAuthorizationException), "Expected UserAuthorizationException was not thrown when deleting network");
            }
        }

        [TestMethod]
        public void Should_Delete_Server_To_Free_Up_Network()
        {
            var provider = new CloudServersProvider(_testIdentity);
            provider.DeleteServer(_created_server_id);
            provider.WaitForServerDeleted(_created_server_id);
        }

        [TestMethod]
        public void Should_Verify_Server_Is_Deleted()
        {
            var provider = new CloudServersProvider(_testIdentity);

            try
            {
                var server = provider.GetDetails(_created_server_id);
                Assert.Fail("Deleted server was still found");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(net.openstack.Core.Exceptions.Response.ItemNotFoundException), "Expected ItemNotFoundException was not thrown");
            }
        }

        [TestMethod]
        public void Should_Delete_Network_After_Server_Deleted()
        {
            var provider = new CloudNetworksProvider(_testIdentity);
            Assert.IsTrue(provider.DeleteNetwork(_created_network_id2));
        }

        [TestMethod]
        public void Should_Verify_Network_Missing_After_Delete()
        {
            var provider = new CloudNetworksProvider(_testIdentity);

            try
            {
                var network = provider.ShowNetwork(_created_network_id2);
                Assert.Fail("Deleted network was still found in ShowNetwork");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), typeof(net.openstack.Core.Exceptions.Response.ItemNotFoundException), "Expected ItemNotFoundException was not thrown");
            }
        }



        [TestMethod]
        public void CleanupServers()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var srvs = provider.ListServers();
            foreach (var svr in srvs.Where(s => s.Name == "net-sdk-test-server"))
                provider.DeleteServer(svr.Id);


            var networkProvider = new CloudNetworksProvider(_testIdentity);
            var networks = networkProvider.ListNetworks();
            foreach (var network in networks.Where(n => n.Label == "net-sdk-test-network" || n.Label == "net-sdk-test-network-for-server"))
                networkProvider.DeleteNetwork(network.Id);
        }


    }
}
