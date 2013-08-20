namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Exceptions.Response;
    using net.openstack.Core.Providers;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Validators;
    using Path = System.IO.Path;

    /// <summary>
    /// This class contains integration tests for the Rackspace Networks Provider
    /// (Cloud Networks) that can be run with user (non-admin) credentials.
    /// </summary>
    /// <seealso cref="INetworksProvider"/>
    /// <seealso cref="CloudNetworksProvider"/>
    [TestClass]
    public class UserNetworksTests
    {
        /// <summary>
        /// This prefix is used for networks created by unit tests, to avoid
        /// overwriting networks created by other applications.
        /// </summary>
        private const string UnitTestNetworkPrefix = "UnitTestNetwork-";

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networks)]
        public void TestListNetworks()
        {
            INetworksProvider provider = new CloudNetworksProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<CloudNetwork> networks = provider.ListNetworks();
            Assert.IsNotNull(networks);
            if (!networks.Any())
                Assert.Inconclusive("The test could not proceed because the specified account and/or region does not appear to contain any configured networks.");

            Console.WriteLine("Networks");
            foreach (CloudNetwork network in networks)
            {
                Assert.IsNotNull(network);

                Console.WriteLine("    {0}: {1} ({2})", network.Id, network.Label, network.Cidr);

                Assert.IsFalse(string.IsNullOrEmpty(network.Id));
                Assert.IsFalse(string.IsNullOrEmpty(network.Label));

                if (!string.IsNullOrEmpty(network.Cidr))
                    CloudNetworksValidator.Default.ValidateCidr(network.Cidr);
            }
        }

        /// <summary>
        /// This test covers the basic functionality of the <see cref="INetworksProvider.CreateNetwork"/>,
        /// <see cref="INetworksProvider.ShowNetwork"/>, and <see cref="INetworksProvider.DeleteNetwork"/>
        /// methods.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networks)]
        public void TestBasicFunctionality()
        {
            INetworksProvider provider = new CloudNetworksProvider(Bootstrapper.Settings.TestIdentity);
            string networkName = UnitTestNetworkPrefix + Path.GetRandomFileName();
            string cidr = "192.0.2.0/24";

            CloudNetwork network;
            try
            {
                network = provider.CreateNetwork(cidr, networkName);
            }
            catch (BadServiceRequestException ex)
            {
                if (ex.Message == "Quota exceeded, too many networks.")
                    Assert.Inconclusive("The required test network could not be created due to a quota.");

                throw;
            }

            Assert.IsNotNull(network);

            CloudNetwork showNetwork = provider.ShowNetwork(network.Id);
            Assert.IsNotNull(showNetwork);
            Assert.AreEqual(network.Id, showNetwork.Id);
            Assert.AreEqual(network.Label, showNetwork.Label);
            Assert.AreEqual(network.Cidr, showNetwork.Cidr);
        }

        /// <summary>
        /// This unit test deletes all the networks created by the unit tests in this class.
        /// These are identified by the prefix <see cref="UnitTestNetworkPrefix"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networks)]
        public void CleanupTestNetworks()
        {
            INetworksProvider provider = new CloudNetworksProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<CloudNetwork> networks = provider.ListNetworks();
            Assert.IsNotNull(networks);

            foreach (CloudNetwork network in networks)
            {
                Assert.IsNotNull(network);
                if (!network.Label.StartsWith(UnitTestNetworkPrefix))
                    continue;

                Console.WriteLine("Removing network... {0}: {1}", network.Id, network.Label);
                provider.DeleteNetwork(network.Id);
            }
        }
    }
}
