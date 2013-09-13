namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Providers;
    using net.openstack.Providers.Rackspace;
    using Newtonsoft.Json;
    using Path = System.IO.Path;

    /// <summary>
    /// This class contains integration tests for the Rackspace Compute Provider
    /// (Cloud Servers) which are executed against an active server instance and
    /// can be run with user (non-admin) credentials.
    /// </summary>
    /// <seealso cref="IComputeProvider"/>
    /// <seealso cref="CloudServersProvider"/>
    [TestClass]
    public class UserServerTests
    {
        private static Server _server;
        private static string _password;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);
            string serverName = UserComputeTests.UnitTestServerPrefix + Path.GetRandomFileName();

            Flavor flavor = UserComputeTests.ListAllFlavorsWithDetails(provider).OrderBy(i => i.RAMInMB).ThenBy(i => i.DiskSizeInGB).FirstOrDefault();
            if (flavor == null)
                Assert.Inconclusive("Couldn't find a flavor to use for the test server.");

            SimpleServerImage[] images = UserComputeTests.ListAllImages(provider).ToArray();
            SimpleServerImage image = images.FirstOrDefault(i => i.Name.IndexOf("CentOS 6.0", StringComparison.OrdinalIgnoreCase) >= 0);
            if (image == null)
                Assert.Inconclusive("Couldn't find the CentOS 6.0 image to use for the test server.");

            Stopwatch timer = Stopwatch.StartNew();
            Console.Write("Creating server for image {0}...", image.Name);
            NewServer server = provider.CreateServer(serverName, image.Id, flavor.Id);
            Assert.IsNotNull(server);
            Assert.IsFalse(string.IsNullOrEmpty(server.Id));

            _password = server.AdminPassword;

            _server = provider.WaitForServerActive(server.Id);
            Assert.IsNotNull(_server);
            Assert.AreEqual(server.Id, _server.Id);
            Assert.AreEqual(ServerState.Active, _server.Status);

            Console.WriteLine("done. {0} seconds", timer.Elapsed.TotalSeconds);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);

            Stopwatch timer = Stopwatch.StartNew();
            Console.Write("  Deleting...");
            bool deleted = provider.DeleteServer(_server.Id);
            Assert.IsTrue(deleted);

            provider.WaitForServerDeleted(_server.Id);
            Console.Write("done. {0} seconds", timer.Elapsed.TotalSeconds);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);
            Server server = provider.GetDetails(_server.Id);
            if (server.Status != ServerState.Active)
                Assert.Inconclusive("Could not run test because the server is in the '{0}' state (expected '{1}').", server.Status, ServerState.Active);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestListServers()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<SimpleServer> servers = UserComputeTests.ListAllServers(provider);
            Assert.IsNotNull(servers);
            if (!servers.Any())
                Assert.Inconclusive("The test could not proceed because the specified account and/or region does not appear to contain any configured servers.");

            Console.WriteLine("Servers");
            foreach (SimpleServer server in servers)
            {
                Assert.IsNotNull(server);

                Console.WriteLine("    {0}: {1}", server.Id, server.Name);

                Assert.IsFalse(string.IsNullOrEmpty(server.Id));
                Assert.IsFalse(string.IsNullOrEmpty(server.Name));
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestListServersWithDetails()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Server> servers = UserComputeTests.ListAllServersWithDetails(provider);
            Assert.IsNotNull(servers);
            if (!servers.Any())
                Assert.Inconclusive("The test could not proceed because the specified account and/or region does not appear to contain any configured servers.");

            Console.WriteLine("Servers (with details)");
            foreach (Server server in servers)
            {
                Assert.IsNotNull(server);

                Console.WriteLine("    {0}: {1}", server.Id, server.Name);
                Console.WriteLine(JsonConvert.SerializeObject(server, Formatting.Indented));

                Assert.IsFalse(string.IsNullOrEmpty(server.Id));
                Assert.IsFalse(string.IsNullOrEmpty(server.Name));
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestGetDetails()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Server> servers = UserComputeTests.ListAllServersWithDetails(provider);
            Assert.IsNotNull(servers);
            if (!servers.Any())
                Assert.Inconclusive("The test could not proceed because the specified account and/or region does not appear to contain any configured servers.");

            foreach (Server server in servers)
            {
                Assert.IsNotNull(server);
                Server details = provider.GetDetails(server.Id);
                Assert.AreEqual(server.AccessIPv4, details.AccessIPv4);
                Assert.AreEqual(server.AccessIPv6, details.AccessIPv6);
                //Assert.AreEqual(server.Addresses, details.Addresses);
                Assert.AreEqual(server.Created, details.Created);
                Assert.AreEqual(server.DiskConfig, details.DiskConfig);
                Assert.AreEqual(server.Flavor.Id, details.Flavor.Id);
                Assert.AreEqual(server.HostId, details.HostId);
                Assert.AreEqual(server.Id, details.Id);
                Assert.AreEqual(server.Image.Id, details.Image.Id);
                //Assert.AreEqual(server.Links, details.Links);
                Assert.AreEqual(server.Name, details.Name);
                Assert.AreEqual(server.PowerState, details.PowerState);
                //Assert.AreEqual(server.Progress, details.Progress);
                //Assert.AreEqual(server.Status, details.Status);
                //Assert.AreEqual(server.TaskState, details.TaskState);
                Assert.AreEqual(server.TenantId, details.TenantId);
                Assert.AreEqual(server.Updated, details.Updated);
                Assert.AreEqual(server.UserId, details.UserId);
                //Assert.AreEqual(server.VMState, details.VMState);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestChangeAdministratorPassword()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);

            string password = Path.GetTempPath();
            bool changePasswordResult = provider.ChangeAdministratorPassword(_server.Id, password);
            Assert.IsTrue(changePasswordResult);
            _password = password;
            Server changePasswordServer = provider.WaitForServerActive(_server.Id);
            Assert.AreEqual(ServerState.Active, changePasswordServer.Status);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestHardRebootServer()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);
            bool rebootResult = provider.RebootServer(_server.Id, RebootType.Hard);
            Assert.IsTrue(rebootResult);
            Server rebootServer = provider.WaitForServerActive(_server.Id);
            Assert.AreEqual(ServerState.Active, rebootServer.Status);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestSoftRebootServer()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);
            bool rebootResult = provider.RebootServer(_server.Id, RebootType.Soft);
            Assert.IsTrue(rebootResult);
            Server rebootServer = provider.WaitForServerActive(_server.Id);
            Assert.AreEqual(ServerState.Active, rebootServer.Status);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestRescueServer()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);

            string rescueResult = provider.RescueServer(_server.Id);
            Assert.IsFalse(string.IsNullOrEmpty(rescueResult));
            Server rescueServer = provider.WaitForServerState(_server.Id, ServerState.Rescue, new[] { ServerState.Error });
            Assert.AreEqual(ServerState.Rescue, rescueServer.Status);

            bool unrescueResult = provider.UnRescueServer(_server.Id);
            Assert.IsTrue(unrescueResult);
            Server unrescueServer = provider.WaitForServerActive(_server.Id);
            Assert.AreEqual(ServerState.Active, unrescueServer.Status);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestUpdateServer()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);

            string newName = UserComputeTests.UnitTestServerPrefix + Path.GetRandomFileName() + "²";
            bool updated = provider.UpdateServer(_server.Id, name: newName);
            Assert.IsTrue(updated);
            Server updatedServer = provider.GetDetails(_server.Id);
            Assert.AreEqual(_server.Id, updatedServer.Id);
            Assert.AreEqual(newName, updatedServer.Name);
            Assert.AreNotEqual(_server.Name, updatedServer.Name);
            _server = updatedServer;
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestListAddresses()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);
            ServerAddresses serverAddresses = provider.ListAddresses(_server.Id);
            if (serverAddresses.Count == 0)
                Assert.Inconclusive("Couldn't find any addresses listed for the server.");

            bool foundAddress = false;
            foreach (KeyValuePair<string, IPAddressList> addresses in serverAddresses)
            {
                Console.WriteLine("Network: {0}", addresses.Key);
                foreach (IPAddress address in addresses.Value)
                {
                    foundAddress = true;
                    Console.WriteLine("  {0}", address);
                }
            }

            if (!foundAddress)
                Assert.Inconclusive("Couldn't find addresses on any network for the server.");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestListAddressesByNetwork()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);
            INetworksProvider networksProvider = new CloudNetworksProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<CloudNetwork> networks = networksProvider.ListNetworks();

            bool foundAddress = false;
            foreach (CloudNetwork network in networks)
            {
                Console.WriteLine("Network: {0}", network.Label);
                IEnumerable<IPAddress> addresses = provider.ListAddressesByNetwork(_server.Id, network.Label);
                foreach (IPAddress address in addresses)
                {
                    foundAddress = true;
                    Console.WriteLine("  {0}", address);
                }
            }

            if (!foundAddress)
                Assert.Inconclusive("Couldn't find addresses on any network for the server.");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestRebuildServer()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);

            Flavor flavor = UserComputeTests.ListAllFlavorsWithDetails(provider).OrderBy(i => i.RAMInMB).ThenBy(i => i.DiskSizeInGB).FirstOrDefault();
            if (flavor == null)
                Assert.Inconclusive("Couldn't find a flavor to use for the test server.");

            SimpleServerImage[] images = UserComputeTests.ListAllImages(provider).ToArray();
            SimpleServerImage image = images.FirstOrDefault(i => i.Name.IndexOf("CentOS 6.0", StringComparison.OrdinalIgnoreCase) >= 0);
            if (image == null)
                Assert.Inconclusive("Couldn't find the CentOS 6.0 image to use for the test server.");

            Server rebuilt = provider.RebuildServer(_server.Id, null, image.Id, flavor.Id, _password);
            Assert.IsNotNull(rebuilt);
            Server rebuiltServer = provider.WaitForServerActive(rebuilt.Id);
            Assert.AreEqual(ServerState.Active, rebuiltServer.Status);
            _server = rebuiltServer;
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestConfirmServerResize()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);

            string serverName = UserComputeTests.UnitTestServerPrefix + Path.GetRandomFileName();

            Flavor flavor = UserComputeTests.ListAllFlavorsWithDetails(provider).OrderBy(i => i.RAMInMB).ThenBy(i => i.DiskSizeInGB).FirstOrDefault(i => !i.Id.Equals(_server.Flavor.Id, StringComparison.OrdinalIgnoreCase));
            if (flavor == null)
                Assert.Inconclusive("Couldn't find a flavor to use for the test server.");

            bool resized = provider.ResizeServer(_server.Id, serverName, flavor.Id);
            Assert.IsTrue(resized);
            Server resizedServer = provider.WaitForServerState(_server.Id, ServerState.VerifyResize, new[] { ServerState.Error, ServerState.Unknown, ServerState.Suspended });
            Assert.AreEqual(ServerState.Active, resizedServer.Status);
            _server = resizedServer;

            bool confirmed = provider.ConfirmServerResize(resizedServer.Id);
            Assert.IsTrue(confirmed);
            Server confirmedServer = provider.WaitForServerActive(_server.Id);
            Assert.AreEqual(ServerState.Active, confirmedServer.Status);
            _server = confirmedServer;
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestRevertServerResize()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);

            string serverName = UserComputeTests.UnitTestServerPrefix + Path.GetRandomFileName();

            Flavor flavor = UserComputeTests.ListAllFlavorsWithDetails(provider).OrderBy(i => i.RAMInMB).ThenBy(i => i.DiskSizeInGB).FirstOrDefault(i => !i.Id.Equals(_server.Flavor.Id, StringComparison.OrdinalIgnoreCase));
            if (flavor == null)
                Assert.Inconclusive("Couldn't find a flavor to use for the test server.");

            bool resized = provider.ResizeServer(_server.Id, serverName, flavor.Id);
            Assert.IsTrue(resized);
            Server resizedServer = provider.WaitForServerState(_server.Id, ServerState.VerifyResize, new[] { ServerState.Error, ServerState.Unknown, ServerState.Suspended });
            Assert.AreEqual(ServerState.Active, resizedServer.Status);
            _server = resizedServer;

            bool reverted = provider.RevertServerResize(resizedServer.Id);
            Assert.IsTrue(reverted);
            Server revertedServer = provider.WaitForServerActive(_server.Id);
            Assert.AreEqual(ServerState.Active, revertedServer.Status);
            _server = revertedServer;
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestCreateImage()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);

            /* Create the image
             */
            string imageName = UserComputeTests.UnitTestImagePrefix + Path.GetRandomFileName();
            bool imaged = provider.CreateImage(_server.Id, imageName);
            Assert.IsTrue(imaged);
            ServerImage[] images = provider.ListImagesWithDetails(server: _server.Id, imageName: imageName).ToArray();
            Assert.IsNotNull(images);
            Assert.AreEqual(1, images.Length);

            ServerImage image = images[0];
            Assert.AreEqual(imageName, image.Name);
            Assert.IsFalse(string.IsNullOrEmpty(image.Id));

            Assert.AreEqual(ImageState.Active, provider.WaitForImageActive(image.Id).Status);

            /* Test metadata operations on the image
             */
            Assert.IsTrue(provider.SetImageMetadataItem(image.Id, "Item 1", "Value"));
            Assert.AreEqual("Value", provider.GetImageMetadataItem(image.Id, "Item 1"));
            Assert.IsTrue(provider.SetImageMetadataItem(image.Id, "Item 2", "Value ²"));
            Assert.AreEqual("Value ²", provider.GetImageMetadataItem(image.Id, "Item 2"));

            // setting the same key overwrites the previous value
            Assert.IsTrue(provider.SetImageMetadataItem(image.Id, "Item 1", "Value 1"));
            Assert.AreEqual("Value 1", provider.GetImageMetadataItem(image.Id, "Item 1"));

            Assert.IsTrue(provider.DeleteImageMetadataItem(image.Id, "Item 1"));
            Assert.IsFalse(provider.ListImageMetadata(image.Id).ContainsKey("Item 1"));

            Metadata metadata = new Metadata()
            {
                { "Different", "Variables" },
            };

            Assert.IsTrue(provider.UpdateImageMetadata(image.Id, metadata));
            Metadata actual = provider.ListImageMetadata(image.Id);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("Value ²", actual["Item 2"]);
            Assert.AreEqual("Variables", actual["Different"]);

            // a slight tweak
            metadata["Different"] = "Values";
            Assert.IsTrue(provider.SetImageMetadata(image.Id, metadata));
            actual = provider.ListImageMetadata(image.Id);
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Values", actual["Different"]);

            Assert.IsTrue(provider.SetImageMetadata(image.Id, new Metadata()));
            Assert.AreEqual(0, provider.ListImageMetadata(image.Id).Count);

            /* Cleanup
             */
            bool deleted = provider.DeleteImage(images[0].Id);
            Assert.IsTrue(deleted);
            provider.WaitForImageDeleted(images[0].Id);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestAttachServerVolume()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);

            IBlockStorageProvider blockStorageProvider = new CloudBlockStorageProvider(Bootstrapper.Settings.TestIdentity);
            VolumeType volumeType = UserBlockStorageTests.GetSsdVolumeTypeOrDefault(blockStorageProvider);
            string volumeName = UserBlockStorageTests.UnitTestVolumePrefix + Path.GetRandomFileName();
            Volume volume = blockStorageProvider.CreateVolume(UserBlockStorageTests.MinimumVolumeSize, displayName: volumeName, volumeType: volumeType != null ? volumeType.Id : null);
            Assert.AreEqual(VolumeState.Available, blockStorageProvider.WaitForVolumeAvailable(volume.Id).Status);

            /* AttachServerVolume
             */
            ServerVolume serverVolume = provider.AttachServerVolume(_server.Id, volume.Id);
            Assert.IsNotNull(serverVolume);
            Assert.IsFalse(string.IsNullOrEmpty(serverVolume.Id));
            Assert.AreEqual(_server.Id, serverVolume.ServerId);
            Assert.AreEqual(volume.Id, serverVolume.VolumeId);

            Assert.AreEqual(VolumeState.InUse, blockStorageProvider.WaitForVolumeState(volume.Id, VolumeState.InUse, new[] { VolumeState.Available, VolumeState.Error }).Status);

            /* ListServerVolumes
             */
            ServerVolume[] serverVolumes = provider.ListServerVolumes(_server.Id).ToArray();
            Assert.IsNotNull(serverVolumes);
            Assert.AreEqual(1, serverVolumes.Length);
            Assert.AreEqual(serverVolume.Id, serverVolumes[0].Id);
            Assert.AreEqual(serverVolume.ServerId, serverVolumes[0].ServerId);
            Assert.AreEqual(serverVolume.VolumeId, serverVolumes[0].VolumeId);

            /* GetServerVolumeDetails
             */
            ServerVolume volumeDetails = provider.GetServerVolumeDetails(_server.Id, volume.Id);
            Assert.IsNotNull(volumeDetails);
            Assert.AreEqual(serverVolume.Id, volumeDetails.Id);
            Assert.AreEqual(serverVolume.ServerId, volumeDetails.ServerId);
            Assert.AreEqual(serverVolume.VolumeId, volumeDetails.VolumeId);

            bool detach = provider.DetachServerVolume(_server.Id, volume.Id);
            Assert.IsTrue(detach);
            ServerVolume[] remainingVolumes = provider.ListServerVolumes(_server.Id).ToArray();
            Assert.AreEqual(0, remainingVolumes.Length);

            Assert.AreEqual(VolumeState.Available, blockStorageProvider.WaitForVolumeAvailable(volume.Id).Status);
            bool deleted = blockStorageProvider.DeleteVolume(volume.Id);
            Assert.IsTrue(blockStorageProvider.WaitForVolumeDeleted(volume.Id));
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestVirtualInterfaces()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);
            INetworksProvider networksProvider = new CloudNetworksProvider(Bootstrapper.Settings.TestIdentity);
            CloudNetwork publicNetwork = networksProvider.ListNetworks().Single(i => i.Label.Equals("public", StringComparison.OrdinalIgnoreCase));
            CloudNetwork privateNetwork = networksProvider.ListNetworks().Single(i => i.Label.Equals("private", StringComparison.OrdinalIgnoreCase));

            VirtualInterface publicVirtualInterface = provider.CreateVirtualInterface(_server.Id, publicNetwork.Id);
            Assert.IsNotNull(publicVirtualInterface);
            Assert.IsFalse(string.IsNullOrEmpty(publicVirtualInterface.Id));
            Assert.IsNotNull(publicVirtualInterface.MACAddress);

            VirtualInterface privateVirtualInterface = provider.CreateVirtualInterface(_server.Id, privateNetwork.Id);
            Assert.IsNotNull(privateVirtualInterface);
            Assert.IsFalse(string.IsNullOrEmpty(privateVirtualInterface.Id));
            Assert.IsNotNull(privateVirtualInterface.MACAddress);

            IEnumerable<VirtualInterface> virtualInterfaces = provider.ListVirtualInterfaces(_server.Id);
            Assert.IsNotNull(virtualInterfaces);
            Assert.IsTrue(virtualInterfaces.Where(i => i.Id.Equals(publicVirtualInterface.Id, StringComparison.OrdinalIgnoreCase)).Any());
            Assert.IsTrue(virtualInterfaces.Where(i => i.Id.Equals(privateVirtualInterface.Id, StringComparison.OrdinalIgnoreCase)).Any());

            bool deleted;
            deleted = provider.DeleteVirtualInterface(_server.Id, publicVirtualInterface.Id);
            Assert.IsTrue(deleted);

            deleted = provider.DeleteVirtualInterface(_server.Id, privateVirtualInterface.Id);
            Assert.IsTrue(deleted);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Compute)]
        public void TestServerMetadata()
        {
            IComputeProvider provider = new CloudServersProvider(Bootstrapper.Settings.TestIdentity);

            Metadata initialMetadata = provider.ListServerMetadata(_server.Id);
            if (initialMetadata.Count > 0)
            {
                Console.WriteLine("Actual metadata");
                foreach (KeyValuePair<string, string> meta in initialMetadata)
                    Console.WriteLine("  {0}: {1}", meta.Key, meta.Value);

                Assert.Inconclusive("Expected the server to not have any initial metadata.");
            }

            Assert.IsTrue(provider.SetServerMetadataItem(_server.Id, "Item 1", "Value"));
            Assert.AreEqual("Value", provider.GetServerMetadataItem(_server.Id, "Item 1"));
            Assert.IsTrue(provider.SetServerMetadataItem(_server.Id, "Item 2", "Value ²"));
            Assert.AreEqual("Value ²", provider.GetServerMetadataItem(_server.Id, "Item 2"));

            // setting the same key overwrites the previous value
            Assert.IsTrue(provider.SetServerMetadataItem(_server.Id, "Item 1", "Value 1"));
            Assert.AreEqual("Value 1", provider.GetServerMetadataItem(_server.Id, "Item 1"));

            Assert.IsTrue(provider.DeleteServerMetadataItem(_server.Id, "Item 1"));
            Assert.IsFalse(provider.ListServerMetadata(_server.Id).ContainsKey("Item 1"));

            Metadata metadata = new Metadata()
            {
                { "Different", "Variables" },
            };

            Assert.IsTrue(provider.UpdateServerMetadata(_server.Id, metadata));
            Metadata actual = provider.ListServerMetadata(_server.Id);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("Value ²", actual["Item 2"]);
            Assert.AreEqual("Variables", actual["Different"]);

            // a slight tweak
            metadata["Different"] = "Values";
            Assert.IsTrue(provider.SetServerMetadata(_server.Id, metadata));
            actual = provider.ListServerMetadata(_server.Id);
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Values", actual["Different"]);

            Assert.IsTrue(provider.SetServerMetadata(_server.Id, new Metadata()));
            Assert.AreEqual(0, provider.ListServerMetadata(_server.Id).Count);
        }
    }
}
