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
    using Newtonsoft.Json;
    using Path = System.IO.Path;

    /// <summary>
    /// This class contains integration tests for the Rackspace Block Storage Provider
    /// (Cloud Block Storage) that can be run with user (non-admin) credentials.
    /// </summary>
    /// <seealso cref="IBlockStorageProvider"/>
    /// <seealso cref="CloudBlockStorageProvider"/>
    [TestClass]
    public class UserBlockStorageTests
    {
        /// <summary>
        /// This prefix is used for volumes created by unit tests, to avoid
        /// overwriting volumes created by other applications.
        /// </summary>
        private const string UnitTestVolumePrefix = "UnitTestVolume-";

        /// <summary>
        /// This prefix is used for snapshots created by unit tests, to avoid
        /// overwriting snapshots created by other applications.
        /// </summary>
        private const string UnitTestSnapshotPrefix = "UnitTestSnapshot-";

        #region Volume

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.BlockStorage)]
        public void CleanupTestVolumes()
        {
            IBlockStorageProvider provider = new CloudBlockStorageProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Volume> volumes = provider.ListVolumes();
            foreach (Volume volume in volumes)
            {
                if (string.IsNullOrEmpty(volume.DisplayName))
                    continue;

                if (!volume.DisplayName.StartsWith(UnitTestVolumePrefix))
                    continue;

                Console.WriteLine("Deleting unit test volume... {0} ({1})", volume.DisplayName, volume.Id);
                provider.DeleteVolume(volume.Id);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.BlockStorage)]
        public void TestListVolumes()
        {
            IBlockStorageProvider provider = new CloudBlockStorageProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Volume> volumes = provider.ListVolumes();
            Assert.IsNotNull(volumes);
            if (!volumes.Any())
                Assert.Inconclusive("The test could not proceed because the specified account and/or region does not appear to contain any configured volumes.");

            Console.WriteLine("Volumes");
            foreach (Volume volume in volumes)
            {
                Assert.IsNotNull(volume);
                Assert.IsFalse(string.IsNullOrEmpty(volume.Id));
                Console.WriteLine(JsonConvert.SerializeObject(volume, Formatting.Indented));
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.BlockStorage)]
        public void TestBasicVolumeFeatures()
        {
            IBlockStorageProvider provider = new CloudBlockStorageProvider(Bootstrapper.Settings.TestIdentity);
            string displayName = UnitTestVolumePrefix + Path.GetRandomFileName();
            Volume result = provider.CreateVolume(100, displayName: displayName);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Id);

            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

            Volume updated = provider.WaitForVolumeAvailable(result.Id);
            Assert.IsNotNull(updated);
            Assert.AreEqual(result.Id, updated.Id);
            Assert.AreEqual(VolumeState.Available, updated.Status);

            bool deleted = provider.DeleteVolume(result.Id);
            Assert.IsTrue(deleted);

            deleted = provider.WaitForVolumeDeleted(result.Id);
            Assert.IsTrue(deleted);

            try
            {
                provider.ShowVolume(result.Id);
                Assert.Fail("Expected an exception after a volume is deleted.");
            }
            catch (ItemNotFoundException)
            {
                // this makes the most sense
            }
            catch (UserNotAuthorizedException)
            {
                // this is allowed by the interface, and some providers could report it for security reasons
            }
            catch (ResponseException)
            {
                // this is allowed by the interface
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.BlockStorage)]
        public void TestShowVolume()
        {
            IBlockStorageProvider provider = new CloudBlockStorageProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Volume> volumes = provider.ListVolumes();
            Assert.IsNotNull(volumes);
            if (!volumes.Any())
                Assert.Inconclusive("The test could not proceed because the specified account and/or region does not appear to contain any configured volumes.");

            foreach (Volume volume in volumes)
            {
                Assert.IsNotNull(volume);
                Assert.IsFalse(string.IsNullOrEmpty(volume.Id));
                Volume showVolume = provider.ShowVolume(volume.Id);
                Assert.IsNotNull(showVolume);
                Assert.AreEqual(volume.AvailabilityZone, showVolume.AvailabilityZone);
                Assert.AreEqual(volume.CreatedAt, showVolume.CreatedAt);
                Assert.AreEqual(volume.DisplayDescription, showVolume.DisplayDescription);
                Assert.AreEqual(volume.DisplayName, showVolume.DisplayName);
                Assert.AreEqual(volume.Id, showVolume.Id);
                Assert.AreEqual(volume.Size, showVolume.Size);
                Assert.AreEqual(volume.SnapshotId, showVolume.SnapshotId);
                Assert.IsNotNull(volume.Status);
                Assert.AreEqual(volume.VolumeType, showVolume.VolumeType);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.BlockStorage)]
        public void TestListVolumeTypes()
        {
            IBlockStorageProvider provider = new CloudBlockStorageProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<VolumeType> volumeTypes = provider.ListVolumeTypes();
            Assert.IsNotNull(volumeTypes);
            if (!volumeTypes.Any())
                Assert.Inconclusive("The test could not proceed because the specified account and/or region does not appear to contain any volume types.");

            Console.WriteLine("Volume Types");
            foreach (VolumeType volumeType in volumeTypes)
            {
                Assert.IsNotNull(volumeType);
                Console.WriteLine(JsonConvert.SerializeObject(volumeType, Formatting.Indented));
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.BlockStorage)]
        public void TestDescribeVolumeType()
        {
            IBlockStorageProvider provider = new CloudBlockStorageProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<VolumeType> volumeTypes = provider.ListVolumeTypes();
            Assert.IsNotNull(volumeTypes);
            if (!volumeTypes.Any())
                Assert.Inconclusive("The test could not proceed because the specified account and/or region does not appear to contain any volume types.");

            foreach (VolumeType volumeType in volumeTypes)
            {
                Assert.IsNotNull(volumeType);
                Assert.IsFalse(string.IsNullOrEmpty(volumeType.Id));
                VolumeType type = provider.DescribeVolumeType(volumeType.Id);
                Assert.IsNotNull(type);
                Assert.AreEqual(volumeType.Id, type.Id);
                Assert.AreEqual(volumeType.Name, type.Name);
            }
        }

        #endregion

        #region Snapshot

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.BlockStorage)]
        public void CleanupTestSnapshots()
        {
            IBlockStorageProvider provider = new CloudBlockStorageProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Snapshot> snapshots = provider.ListSnapshots();
            foreach (Snapshot snapshot in snapshots)
            {
                if (string.IsNullOrEmpty(snapshot.DisplayName))
                    continue;

                if (!snapshot.DisplayName.StartsWith(UnitTestSnapshotPrefix))
                    continue;

                Console.WriteLine("Deleting unit test snapshot... {0} ({1})", snapshot.DisplayName, snapshot.Id);
                provider.DeleteSnapshot(snapshot.Id);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.BlockStorage)]
        public void TestListSnapshots()
        {
            IBlockStorageProvider provider = new CloudBlockStorageProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Snapshot> snapshots = provider.ListSnapshots();
            Assert.IsNotNull(snapshots);
            if (!snapshots.Any())
                Assert.Inconclusive("The test could not proceed because the specified account and/or region does not appear to contain any snapshots.");

            Console.WriteLine("Snapshots");
            foreach (Snapshot snapshot in snapshots)
            {
                Assert.IsNotNull(snapshot);
                Assert.IsFalse(string.IsNullOrEmpty(snapshot.Id));
                Console.WriteLine(JsonConvert.SerializeObject(snapshot, Formatting.Indented));
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.BlockStorage)]
        public void TestBasicSnapshotFeatures()
        {
            IBlockStorageProvider provider = new CloudBlockStorageProvider(Bootstrapper.Settings.TestIdentity);

            //
            // Volume setup...
            //

            string volumeDisplayName = UnitTestVolumePrefix + Path.GetRandomFileName();

            VolumeType ssdType = GetSsdVolumeTypeOrDefault(provider);
            if (ssdType != null)
                Console.WriteLine("Using an SSD volume to improve snapshot test performance.");
            else
                Console.WriteLine("No SSD volume type is available for the snapshot test... falling back to the default.");

            Volume result = provider.CreateVolume(100, displayName: volumeDisplayName, volumeType: ssdType != null ? ssdType.Id : null);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Id);

            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

            Volume updated = provider.WaitForVolumeAvailable(result.Id);
            Assert.IsNotNull(updated);
            Assert.AreEqual(result.Id, updated.Id);
            Assert.AreEqual(VolumeState.Available, updated.Status);

            //
            // Snapshot testing
            //

            string snapshotDisplayName = UnitTestSnapshotPrefix + Path.GetRandomFileName();
            Snapshot snapshot = provider.CreateSnapshot(result.Id, displayName: snapshotDisplayName);
            Assert.IsNotNull(snapshot);
            Assert.IsNotNull(snapshot.Id);

            Console.WriteLine(JsonConvert.SerializeObject(snapshot, Formatting.Indented));

            Snapshot updatedSnapshot = provider.WaitForSnapshotAvailable(snapshot.Id);
            Assert.IsNotNull(updatedSnapshot);
            Assert.AreEqual(snapshot.Id, updatedSnapshot.Id);
            Assert.AreEqual(SnapshotState.Available, updatedSnapshot.Status);

            bool deleted = provider.DeleteSnapshot(snapshot.Id);
            Assert.IsTrue(deleted);

            deleted = provider.WaitForSnapshotDeleted(snapshot.Id);
            Assert.IsTrue(deleted);

            try
            {
                provider.ShowSnapshot(snapshot.Id);
                Assert.Fail("Expected an exception after a snapshot is deleted.");
            }
            catch (ItemNotFoundException)
            {
                // this makes the most sense
            }
            catch (UserNotAuthorizedException)
            {
                // this is allowed by the interface, and some providers could report it for security reasons
            }
            catch (ResponseException)
            {
                // this is allowed by the interface
            }

            //
            // Volume cleanup...
            //

            bool deletedVolume = provider.DeleteVolume(result.Id);
            Assert.IsTrue(deletedVolume);

            deletedVolume = provider.WaitForVolumeDeleted(result.Id);
            Assert.IsTrue(deletedVolume);
        }

        private static VolumeType GetSsdVolumeTypeOrDefault(IBlockStorageProvider provider)
        {
            foreach (var volumeType in provider.ListVolumeTypes())
            {
                if (string.Equals(volumeType.Name, "SSD", StringComparison.OrdinalIgnoreCase))
                    return volumeType;
            }

            return null;
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.BlockStorage)]
        public void TestShowSnapshot()
        {
            IBlockStorageProvider provider = new CloudBlockStorageProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Snapshot> snapshots = provider.ListSnapshots();
            Assert.IsNotNull(snapshots);
            if (!snapshots.Any())
                Assert.Inconclusive("The test could not proceed because the specified account and/or region does not appear to contain any snapshots.");

            Console.WriteLine("Snapshots");
            foreach (Snapshot snapshot in snapshots)
            {
                Assert.IsNotNull(snapshot);
                Assert.IsFalse(string.IsNullOrEmpty(snapshot.Id));
                Snapshot showSnapshot = provider.ShowSnapshot(snapshot.Id);
                Assert.IsNotNull(showSnapshot);
                Assert.AreEqual(snapshot.CreatedAt, showSnapshot.CreatedAt);
                Assert.AreEqual(snapshot.DisplayDescription, showSnapshot.DisplayDescription);
                Assert.AreEqual(snapshot.DisplayName, showSnapshot.DisplayName);
                Assert.AreEqual(snapshot.Id, showSnapshot.Id);
                Assert.AreEqual(snapshot.Size, showSnapshot.Size);
                //Assert.AreEqual(snapshot.Status, showSnapshot.Status);
                Assert.AreEqual(snapshot.VolumeId, showSnapshot.VolumeId);
            }
        }

        #endregion
    }
}
