namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Exceptions;
    using net.openstack.Core.Exceptions.Response;
    using net.openstack.Core.Providers;
    using net.openstack.Providers.Rackspace;
    using Newtonsoft.Json;
    using Container = net.openstack.Core.Domain.Container;
    using File = System.IO.File;
    using FileInfo = System.IO.FileInfo;
    using HttpWebRequest = System.Net.HttpWebRequest;
    using MD5 = System.Security.Cryptography.MD5;
    using MemoryStream = System.IO.MemoryStream;
    using Path = System.IO.Path;
    using Stream = System.IO.Stream;
    using StreamReader = System.IO.StreamReader;
    using WebRequest = System.Net.WebRequest;
    using WebResponse = System.Net.WebResponse;

    /// <summary>
    /// This class contains integration tests for the Rackspace Object Storage Provider
    /// (Cloud Files) that can be run with user (non-admin) credentials.
    /// </summary>
    /// <seealso cref="CloudFilesProvider"/>
    [TestClass]
    public class UserObjectStorageTests
    {
        /// <summary>
        /// This prefix is used for metadata keys created by unit tests, to avoid
        /// overwriting metadata created by other applications.
        /// </summary>
        private const string TestKeyPrefix = "UnitTestMetadataKey-";

        /// <summary>
        /// This prefix is used for containers created by unit tests, to avoid
        /// overwriting containers created by other applications.
        /// </summary>
        private const string TestContainerPrefix = "UnitTestContainer-";

        /// <summary>
        /// The minimum character allowed in metadata keys. This is drawn from
        /// the HTTP/1.1 specification, which does not allow ASCII control
        /// characters in header keys.
        /// </summary>
        private const char MinHeaderKeyCharacter = (char)32;

        /// <summary>
        /// The maximum character allowed in metadata keys. This is drawn from
        /// the HTTP/1.1 specification, which restricts header keys to the 7-bit
        /// ASCII character set.
        /// </summary>
        private const char MaxHeaderKeyCharacter = (char)127;

        /// <summary>
        /// The HTTP/1.1 separator characters.
        /// </summary>
        private const string SeparatorCharacters = "()<>@,;:\\\"/[]?={} \t\x7F";

        /// <summary>
        /// Characters which are technically allowed by HTTP/1.1, but cannot be used in
        /// metadata keys for <see cref="CloudFilesProvider"/>.
        /// </summary>
        /// <remarks>
        /// The underscore is disallowed by the Cloud Files implementation, which silently
        /// converts it to a dash. The apostrophe is disallowed by <see cref="WebHeaderCollection"/>
        /// which is used by the implementation.
        /// </remarks>
        private const string NotSupportedCharacters = "_'";

        #region Container

        /// <summary>
        /// This test can be used to clear all of the metadata associated with every container in the storage provider.
        /// </summary>
        /// <remarks>
        /// This test is normally disabled. To run the cleanup method, comment out or remove the
        /// <see cref="IgnoreAttribute"/>.
        /// </remarks>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        [TestCategory(TestCategories.Cleanup)]
        [Ignore]
        public void CleanupAllContainerMetadata()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Container> containers = ListAllContainers(provider);
            foreach (Container container in containers)
            {
                Dictionary<string, string> metadata = provider.GetContainerMetaData(container.Name);
                provider.DeleteContainerMetadata(container.Name, metadata.Keys);
            }
        }

        /// <summary>
        /// This unit test clears the metadata associated with every container which is
        /// created by the unit tests in this class.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        [TestCategory(TestCategories.Cleanup)]
        public void CleanupTestContainerMetadata()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Container> containers = ListAllContainers(provider);
            foreach (Container container in containers)
            {
                Dictionary<string, string> metadata = GetContainerMetadataWithPrefix(provider, container, TestKeyPrefix);
                provider.DeleteContainerMetadata(container.Name, metadata.Keys);
            }
        }

        /// <summary>
        /// This unit test deletes all containers created by the unit tests, including all
        /// objects within those containers.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        [TestCategory(TestCategories.Cleanup)]
        public void CleanupTestContainers()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Container> containers = ListAllContainers(provider);
            foreach (Container container in containers)
            {
                if (container.Name.StartsWith(TestContainerPrefix))
                {
                    try
                    {
                        provider.DeleteContainer(container.Name, deleteObjects: true);
                    }
                    catch (ContainerNotEmptyException)
                    {
                        // this works around a bug in bulk delete, where files with trailing whitespace
                        // in the name do not get deleted
                        foreach (ContainerObject containerObject in ListAllObjects(provider, container.Name))
                            provider.DeleteObject(container.Name, containerObject.Name);

                        provider.DeleteContainer(container.Name, deleteObjects: false);
                    }
                }
                else if (container.Name.Equals(".CDN_ACCESS_LOGS"))
                {
                    foreach (ContainerObject containerObject in ListAllObjects(provider, container.Name))
                    {
                        if (containerObject.Name.StartsWith(TestContainerPrefix))
                            provider.DeleteObject(container.Name, containerObject.Name);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestListContainers()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Container> containers = ListAllContainers(provider);
            if (!containers.Any())
                Assert.Inconclusive("The account does not have any containers in the region.");

            Console.WriteLine("Containers");
            foreach (Container container in containers)
            {
                Console.WriteLine("  {0}", container.Name);
                Console.WriteLine("    Objects: {0}", container.Count);
                Console.WriteLine("    Bytes: {0}", container.Bytes);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestContainerProperties()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<Container> containers = ListAllContainers(provider);
            if (!containers.Any())
                Assert.Inconclusive("The account does not have any containers in the region.");

            int containersTested = 0;
            long objectsTested = 0;
            long totalSizeTested = 0;
            int nonEmptyContainersTested = 0;
            int nonEmptyBytesContainersTested = 0;
            foreach (Container container in containers)
            {
                Assert.IsTrue(container.Count >= 0);
                Assert.IsTrue(container.Bytes >= 0);

                containersTested++;
                if (container.Count > 0)
                    nonEmptyContainersTested++;
                if (container.Bytes > 0)
                    nonEmptyBytesContainersTested++;

                long objectCount = 0;
                long objectSize = 0;
                foreach (var obj in ListAllObjects(provider, container.Name))
                {
                    objectCount++;
                    objectSize += obj.Bytes;
                }

                objectsTested += objectCount;
                totalSizeTested += objectSize;

                Assert.AreEqual(container.Count, objectCount);
                Assert.AreEqual(container.Bytes, objectSize);

                if (containersTested >= 5 && nonEmptyContainersTested >= 5 && nonEmptyBytesContainersTested >= 5)
                    break;
            }

            if (containersTested == 0 || nonEmptyContainersTested == 0 || nonEmptyBytesContainersTested == 0)
                Assert.Inconclusive("The account does not have any non-empty containers in the region.");

            Console.WriteLine("Verified container properties for:");
            Console.WriteLine("  {0} containers", containersTested);
            Console.WriteLine("  {0} objects", objectsTested);
            Console.WriteLine("  {0} bytes", totalSizeTested);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestCreateContainer()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();

            ObjectStore result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, result);

            result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerExists, result);

            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestDeleteContainer()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            string fileContents = "File contents!";

            ObjectStore result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, result);

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContents));
            provider.CreateObject(containerName, stream, objectName);

            try
            {
                provider.DeleteContainer(containerName, deleteObjects: false);
                Assert.Fail("Expected a ContainerNotEmptyException");
            }
            catch (ContainerNotEmptyException)
            {
            }

            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestGetContainerHeader()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();

            ObjectStore result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, result);

            Dictionary<string, string> headers = provider.GetContainerHeader(containerName);
            Console.WriteLine("Container Headers");
            foreach (KeyValuePair<string, string> pair in headers)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestGetContainerMetaData()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();

            ObjectStore result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, result);

            Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Key1", "Value 1" },
                { "Key2", "Value ²" },
            };

            provider.UpdateContainerMetadata(containerName, new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase));

            Dictionary<string, string> actualMetadata = provider.GetContainerMetaData(containerName);
            Console.WriteLine("Container Metadata");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestContainerHeaderKeyCharacters()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();

            ObjectStore result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, result);

            List<char> keyCharList = new List<char>();
            for (char i = MinHeaderKeyCharacter; i <= MaxHeaderKeyCharacter; i++)
            {
                if (!SeparatorCharacters.Contains(i) && !NotSupportedCharacters.Contains(i))
                    keyCharList.Add(i);
            }

            string key = TestKeyPrefix + new string(keyCharList.ToArray());
            Console.WriteLine("Expected key: {0}", key);

            provider.UpdateContainerMetadata(
                containerName,
                new Dictionary<string, string>
                {
                    { key, "Value" }
                });

            Dictionary<string, string> metadata = provider.GetContainerMetaData(containerName);
            Assert.IsNotNull(metadata);

            string value;
            Assert.IsTrue(metadata.TryGetValue(key, out value));
            Assert.AreEqual("Value", value);

            provider.UpdateContainerMetadata(
                containerName,
                new Dictionary<string, string>
                {
                    { key, null }
                });

            metadata = provider.GetContainerMetaData(containerName);
            Assert.IsNotNull(metadata);
            Assert.IsFalse(metadata.TryGetValue(key, out value));

            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestContainerInvalidHeaderKeyCharacters()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();

            ObjectStore result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, result);

            List<char> validKeyCharList = new List<char>();
            for (char i = MinHeaderKeyCharacter; i <= MaxHeaderKeyCharacter; i++)
            {
                if (!SeparatorCharacters.Contains(i) && !NotSupportedCharacters.Contains(i))
                    validKeyCharList.Add(i);
            }

            for (int i = char.MinValue; i <= char.MaxValue; i++)
            {
                if (validKeyCharList.BinarySearch((char)i) >= 0)
                    continue;

                string invalidKey = new string((char)i, 1);

                try
                {
                    provider.UpdateContainerMetadata(
                        containerName,
                        new Dictionary<string, string>
                        {
                            { invalidKey, "Value" }
                        });
                    Assert.Fail("Should throw an exception for invalid keys.");
                }
                catch (ArgumentException)
                {
                    if (i >= MinHeaderKeyCharacter && i <= MaxHeaderKeyCharacter)
                        StringAssert.Contains(SeparatorCharacters, invalidKey);
                }
                catch (NotSupportedException)
                {
                    StringAssert.Contains(NotSupportedCharacters, invalidKey);
                }
            }

            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestUpdateContainerMetadata()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();

            ObjectStore result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, result);

            Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Key1", "Value 1" },
                { "Key2", "Value ²" },
            };

            provider.UpdateContainerMetadata(containerName, new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase));

            Dictionary<string, string> actualMetadata = provider.GetContainerMetaData(containerName);
            Console.WriteLine("Container Metadata");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            metadata["Key2"] = "Value 2";
            Dictionary<string, string> updatedMetadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Key2", "Value 2" }
            };
            provider.UpdateContainerMetadata(containerName, new Dictionary<string, string>(updatedMetadata, StringComparer.OrdinalIgnoreCase));

            actualMetadata = provider.GetContainerMetaData(containerName);
            Console.WriteLine("Container Metadata");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestDeleteContainerMetadata()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();

            ObjectStore result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, result);

            Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Key1", "Value 1" },
                { "Key2", "Value ²" },
                { "Key3", "Value 3" },
                { "Key4", "Value 4" },
            };

            provider.UpdateContainerMetadata(containerName, new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase));

            Dictionary<string, string> actualMetadata = provider.GetContainerMetaData(containerName);
            Console.WriteLine("Container Metadata");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            /* Check the overload which takes a single key
             */
            // remove Key3 first to make sure we still have a ² character in a remaining value
            metadata.Remove("Key3");
            provider.DeleteContainerMetadata(containerName, "Key3");

            actualMetadata = provider.GetContainerMetaData(containerName);
            Console.WriteLine("Container Metadata after removing Key3");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            /* Check the overload which takes multiple keys
             */
            metadata.Remove("Key2");
            metadata.Remove("Key4");
            provider.DeleteContainerMetadata(containerName, new[] { "Key2", "Key4" });

            actualMetadata = provider.GetContainerMetaData(containerName);
            Console.WriteLine("Container Metadata after removing Key2, Key4");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            /* Check that duplicate removal is a NOP
             */
            metadata.Remove("Key2");
            metadata.Remove("Key4");
            provider.DeleteContainerMetadata(containerName, new[] { "Key2", "Key4" });

            actualMetadata = provider.GetContainerMetaData(containerName);
            Console.WriteLine("Container Metadata after removing Key2, Key4");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            /* Cleanup
             */
            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestListCDNContainers()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            IEnumerable<ContainerCDN> containers = ListAllCDNContainers(provider);

            Console.WriteLine("Containers");
            foreach (ContainerCDN container in containers)
            {
                Console.WriteLine("  {1}{0}", container.Name, container.CDNEnabled ? "*" : "");
            }
        }

        /// <summary>
        /// This test covers most of the CDN functionality exposed by <see cref="IObjectStorageProvider"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestCDNOnContainer()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            string fileContents = "File contents!";

            ObjectStore result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, result);

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContents));
            provider.CreateObject(containerName, stream, objectName);

            Dictionary<string, string> cdnHeaders = provider.EnableCDNOnContainer(containerName, false);
            Assert.IsNotNull(cdnHeaders);
            Console.WriteLine("CDN Headers from EnableCDNOnContainer");
            foreach (var pair in cdnHeaders)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            ContainerCDN containerHeader = provider.GetContainerCDNHeader(containerName);
            Assert.IsNotNull(containerHeader);
            Console.WriteLine(JsonConvert.SerializeObject(containerHeader, Formatting.Indented));
            Assert.IsTrue(containerHeader.CDNEnabled);
            Assert.IsFalse(containerHeader.LogRetention);
            Assert.IsTrue(
                containerHeader.CDNUri != null
                || containerHeader.CDNIosUri != null
                || containerHeader.CDNSslUri != null
                || containerHeader.CDNStreamingUri != null);

            // Call the other overloads of EnableCDNOnContainer
            cdnHeaders = provider.EnableCDNOnContainer(containerName, containerHeader.Ttl);
            ContainerCDN updatedHeader = provider.GetContainerCDNHeader(containerName);
            Console.WriteLine(JsonConvert.SerializeObject(updatedHeader, Formatting.Indented));
            Assert.IsNotNull(updatedHeader);
            Assert.IsTrue(updatedHeader.CDNEnabled);
            Assert.IsFalse(updatedHeader.LogRetention);
            Assert.IsTrue(
                updatedHeader.CDNUri != null
                || updatedHeader.CDNIosUri != null
                || updatedHeader.CDNSslUri != null
                || updatedHeader.CDNStreamingUri != null);
            Assert.AreEqual(containerHeader.Ttl, updatedHeader.Ttl);

            cdnHeaders = provider.EnableCDNOnContainer(containerName, containerHeader.Ttl, true);
            updatedHeader = provider.GetContainerCDNHeader(containerName);
            Console.WriteLine(JsonConvert.SerializeObject(updatedHeader, Formatting.Indented));
            Assert.IsNotNull(updatedHeader);
            Assert.IsTrue(updatedHeader.CDNEnabled);
            Assert.IsTrue(updatedHeader.LogRetention);
            Assert.IsTrue(
                updatedHeader.CDNUri != null
                || updatedHeader.CDNIosUri != null
                || updatedHeader.CDNSslUri != null
                || updatedHeader.CDNStreamingUri != null);
            Assert.AreEqual(containerHeader.Ttl, updatedHeader.Ttl);

            // update the container CDN properties
            Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { CloudFilesProvider.CdnTTL, (updatedHeader.Ttl + 1).ToString() },
                { CloudFilesProvider.CdnLogRetention, false.ToString() },
                //{ CloudFilesProvider.CdnEnabled, true.ToString() },
            };

            provider.UpdateContainerCdnHeaders(containerName, headers);
            updatedHeader = provider.GetContainerCDNHeader(containerName);
            Console.WriteLine(JsonConvert.SerializeObject(updatedHeader, Formatting.Indented));
            Assert.IsNotNull(updatedHeader);
            Assert.IsTrue(updatedHeader.CDNEnabled);
            Assert.IsFalse(updatedHeader.LogRetention);
            Assert.IsTrue(
                updatedHeader.CDNUri != null
                || updatedHeader.CDNIosUri != null
                || updatedHeader.CDNSslUri != null
                || updatedHeader.CDNStreamingUri != null);
            Assert.AreEqual(containerHeader.Ttl + 1, updatedHeader.Ttl);

            // attempt to access the container over the CDN
            if (containerHeader.CDNUri != null || containerHeader.CDNSslUri != null)
            {
                string baseUri = containerHeader.CDNUri ?? containerHeader.CDNSslUri;
                Uri uri = new Uri(containerHeader.CDNUri + '/' + objectName);
                WebRequest request = HttpWebRequest.Create(uri);
                using (WebResponse response = request.GetResponse())
                {
                    Stream cdnStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(cdnStream, Encoding.UTF8);
                    string text = reader.ReadToEnd();
                    Assert.AreEqual(fileContents, text);
                }
            }
            else
            {
                Assert.Inconclusive("This integration test relies on cdn_uri or cdn_ssl_uri.");
            }

            IEnumerable<ContainerCDN> containers = ListAllCDNContainers(provider);
            Console.WriteLine("Containers");
            foreach (ContainerCDN container in containers)
            {
                Console.WriteLine("    {1}{0}", container.Name, container.CDNEnabled ? "*" : "");
            }

            cdnHeaders = provider.DisableCDNOnContainer(containerName);
            Assert.IsNotNull(cdnHeaders);
            Console.WriteLine("CDN Headers from DisableCDNOnContainer");
            foreach (var pair in cdnHeaders)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            updatedHeader = provider.GetContainerCDNHeader(containerName);
            Console.WriteLine(JsonConvert.SerializeObject(updatedHeader, Formatting.Indented));
            Assert.IsNotNull(updatedHeader);
            Assert.IsFalse(updatedHeader.CDNEnabled);
            Assert.IsFalse(updatedHeader.LogRetention);
            Assert.IsTrue(
                updatedHeader.CDNUri != null
                || updatedHeader.CDNIosUri != null
                || updatedHeader.CDNSslUri != null
                || updatedHeader.CDNStreamingUri != null);
            Assert.AreEqual(containerHeader.Ttl + 1, updatedHeader.Ttl);

            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestStaticWebOnContainer()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            string fileContents = "File contents!";

            ObjectStore result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, result);

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContents));
            provider.CreateObject(containerName, stream, objectName);

            Dictionary<string, string> cdnHeaders = provider.EnableCDNOnContainer(containerName, false);
            Assert.IsNotNull(cdnHeaders);
            Console.WriteLine("CDN Headers");
            foreach (var pair in cdnHeaders)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            string index = objectName;
            string error = objectName;
            string css = objectName;
            provider.EnableStaticWebOnContainer(containerName, index: index, error: error, listing: false);

            provider.DisableStaticWebOnContainer(containerName);

            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        private static IEnumerable<Container> ListAllContainers(IObjectStorageProvider provider, int? blockSize = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException("blockSize");

            Container lastContainer = null;

            do
            {
                string marker = lastContainer != null ? lastContainer.Name : null;
                IEnumerable<Container> containers = provider.ListContainers(blockSize, marker, null, region, useInternalUrl, identity);
                lastContainer = null;
                foreach (Container container in containers)
                {
                    lastContainer = container;
                    yield return container;
                }
            } while (lastContainer != null);
        }

        private static IEnumerable<ContainerObject> ListAllObjects(IObjectStorageProvider provider, string containerName, int? blockSize = null, string prefix = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException("blockSize");

            ContainerObject lastContainerObject = null;

            do
            {
                string marker = lastContainerObject != null ? lastContainerObject.Name : null;
                IEnumerable<ContainerObject> containerObjects = provider.ListObjects(containerName, blockSize, marker, null, prefix, region, useInternalUrl, identity);
                lastContainerObject = null;
                foreach (ContainerObject containerObject in containerObjects)
                {
                    lastContainerObject = containerObject;
                    yield return containerObject;
                }
            } while (lastContainerObject != null);
        }

        private static IEnumerable<ContainerCDN> ListAllCDNContainers(IObjectStorageProvider provider, int? blockSize = null, bool cdnEnabled = false, string region = null, CloudIdentity identity = null)
        {
            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException("blockSize");

            ContainerCDN lastContainer = null;

            do
            {
                string marker = lastContainer != null ? lastContainer.Name : null;
                IEnumerable<ContainerCDN> containers = provider.ListCDNContainers(blockSize, marker, null, cdnEnabled, region, identity);
                lastContainer = null;
                foreach (ContainerCDN container in containers)
                {
                    lastContainer = container;
                    yield return container;
                }
            } while (lastContainer != null);
        }

        private static Dictionary<string, string> GetContainerMetadataWithPrefix(IObjectStorageProvider provider, Container container, string prefix)
        {
            Dictionary<string, string> metadata = provider.GetContainerMetaData(container.Name);
            return FilterMetadataPrefix(metadata, prefix);
        }

        private static string GetObjectContentType(IObjectStorageProvider provider, string containerName, string objectName)
        {
            Dictionary<string, string> headers = provider.GetObjectHeaders(containerName, objectName);

            string contentType;
            if (!headers.TryGetValue("Content-Type", out contentType))
                return null;

            return contentType.ToLowerInvariant();
        }

        #endregion

        #region Objects

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestGetObjectHeaders()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            string objectData = "";

            ObjectStore containerResult = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, containerResult);

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(objectData));
            provider.CreateObject(containerName, stream, objectName);
            Dictionary<string, string> headers = provider.GetObjectHeaders(containerName, objectName);
            Assert.IsNotNull(headers);
            Console.WriteLine("Headers");
            foreach (var pair in headers)
            {
                Assert.IsFalse(string.IsNullOrEmpty(pair.Key));
                Console.WriteLine("  {0}: {1}", pair.Key, pair.Value);
            }

            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestGetObjectMetaData()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            string objectData = "";

            ObjectStore containerResult = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, containerResult);

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(objectData));
            provider.CreateObject(containerName, stream, objectName);
            Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Key1", "Value 1" },
                { "Key2", "Value ²" },
            };

            provider.UpdateObjectMetadata(containerName, objectName, new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase));

            Dictionary<string, string> actualMetadata = provider.GetObjectMetaData(containerName, objectName);
            Console.WriteLine("Object Metadata");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestUpdateObjectMetaData()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            string objectData = "";

            ObjectStore result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, result);

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(objectData));
            provider.CreateObject(containerName, stream, objectName);
            Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Key1", "Value 1" },
                { "Key2", "Value ²" },
            };

            provider.UpdateObjectMetadata(containerName, objectName, new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase));

            Dictionary<string, string> actualMetadata = provider.GetObjectMetaData(containerName, objectName);
            Console.WriteLine("Object Metadata");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            metadata["Key2"] = "Value 2";
            Dictionary<string, string> updatedMetadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Key2", "Value 2" }
            };
            provider.UpdateObjectMetadata(containerName, objectName, new Dictionary<string, string>(updatedMetadata, StringComparer.OrdinalIgnoreCase));

            actualMetadata = provider.GetObjectMetaData(containerName, objectName);
            Console.WriteLine("Object Metadata");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(updatedMetadata, actualMetadata);

            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestDeleteObjectMetaData()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            string objectData = "";

            ObjectStore result = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, result);

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(objectData));
            provider.CreateObject(containerName, stream, objectName);
            Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Key1", "Value 1" },
                { "Key2", "Value ²" },
                { "Key3", "Value 3" },
                { "Key4", "Value 4" },
            };

            provider.UpdateObjectMetadata(containerName, objectName, new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase));

            Dictionary<string, string> actualMetadata = provider.GetObjectMetaData(containerName, objectName);
            Console.WriteLine("Object Metadata");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            /* Check the overload which takes a single key
             */
            // remove Key3 first to make sure we still have a ² character in a remaining value
            metadata.Remove("Key3");
            provider.DeleteObjectMetadata(containerName, objectName, "Key3");

            actualMetadata = provider.GetObjectMetaData(containerName, objectName);
            Console.WriteLine("Object Metadata after removing Key3");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            /* Check the overload which takes multiple keys
             */
            metadata.Remove("Key2");
            metadata.Remove("Key4");
            provider.DeleteObjectMetadata(containerName, objectName, new[] { "Key2", "Key4" });

            actualMetadata = provider.GetObjectMetaData(containerName, objectName);
            Console.WriteLine("Object Metadata after removing Key2, Key4");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            /* Check that duplicate removal is a NOP
             */
            metadata.Remove("Key2");
            metadata.Remove("Key4");
            provider.DeleteObjectMetadata(containerName, objectName, new[] { "Key2", "Key4" });

            actualMetadata = provider.GetObjectMetaData(containerName, objectName);
            Console.WriteLine("Object Metadata after removing Key2, Key4");
            foreach (KeyValuePair<string, string> pair in actualMetadata)
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

            CheckMetadataCollections(metadata, actualMetadata);

            /* Cleanup
             */
            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestListObjects()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string[] objectNames = { Path.GetRandomFileName(), Path.GetRandomFileName() };
            // another random name counts as random content
            string fileData = Path.GetRandomFileName();

            ObjectStore containerResult = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, containerResult);

            foreach (string objectName in objectNames)
            {
                using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
                {
                    provider.CreateObject(containerName, uploadStream, objectName);
                }
            }

            Console.WriteLine("Objects in container {0}", containerName);
            foreach (ContainerObject containerObject in ListAllObjects(provider, containerName))
            {
                Console.WriteLine("  {0}", containerObject.Name);
            }

            /* Cleanup
             */
            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestSpecialCharacters()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string[] specialNames = { "#", " ", " lead", "trail ", "%", "x//x" };
            // another random name counts as random content
            string fileData = Path.GetRandomFileName();

            foreach (string containerSuffix in specialNames)
            {
                string containerName = TestContainerPrefix + Path.GetRandomFileName() + containerSuffix;

                ObjectStore containerResult = provider.CreateContainer(containerName);
                Assert.AreEqual(ObjectStore.ContainerCreated, containerResult);

                foreach (string objectName in specialNames)
                {
                    using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
                    {
                        provider.CreateObject(containerName, uploadStream, objectName);
                    }
                }

                Console.WriteLine("Objects in container {0}", containerName);
                foreach (ContainerObject containerObject in ListAllObjects(provider, containerName))
                {
                    Console.WriteLine("  {0}", containerObject.Name);
                }

                /* Cleanup
                 */
                provider.DeleteContainer(containerName, deleteObjects: true);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestCreateObjectFromFile_UseFileName()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            // another random name counts as random content
            string fileData = Path.GetRandomFileName();
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            ObjectStore containerResult = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, containerResult);

            try
            {
                File.WriteAllText(tempFilePath, fileData, Encoding.UTF8);
                provider.CreateObjectFromFile(containerName, tempFilePath);

                // it's ok to create the same file twice
                ProgressMonitor progressMonitor = new ProgressMonitor(new FileInfo(tempFilePath).Length);
                provider.CreateObjectFromFile(containerName, tempFilePath, progressUpdated: progressMonitor.Updated);
                Assert.IsTrue(progressMonitor.IsComplete, "Failed to notify progress monitor callback of status update.");
            }
            finally
            {
                File.Delete(tempFilePath);
            }

            using (MemoryStream testStream = new MemoryStream())
            {
                provider.GetObject(containerName, Path.GetFileName(tempFilePath), testStream);

                testStream.Position = 0;
                StreamReader reader = new StreamReader(testStream, Encoding.UTF8);
                string actualData = reader.ReadToEnd();
                Assert.AreEqual(fileData, actualData);
            }

            /* Cleanup
             */
            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestCreateObjectFromFile_UseCustomObjectName()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            // another random name counts as random content
            string fileData = Path.GetRandomFileName();
            string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            ObjectStore containerResult = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, containerResult);

            try
            {
                File.WriteAllText(tempFilePath, fileData, Encoding.UTF8);
                provider.CreateObjectFromFile(containerName, tempFilePath, objectName);

                // it's ok to create the same file twice
                ProgressMonitor progressMonitor = new ProgressMonitor(new FileInfo(tempFilePath).Length);
                provider.CreateObjectFromFile(containerName, tempFilePath, objectName, progressUpdated: progressMonitor.Updated);
                Assert.IsTrue(progressMonitor.IsComplete, "Failed to notify progress monitor callback of status update.");
            }
            finally
            {
                File.Delete(tempFilePath);
            }

            using (MemoryStream testStream = new MemoryStream())
            {
                provider.GetObject(containerName, objectName, testStream);

                testStream.Position = 0;
                StreamReader reader = new StreamReader(testStream, Encoding.UTF8);
                string actualData = reader.ReadToEnd();
                Assert.AreEqual(fileData, actualData);
            }

            /* Cleanup
             */
            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestCreateObject()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            // another random name counts as random content
            string fileData = Path.GetRandomFileName();

            ObjectStore containerResult = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, containerResult);

            using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
            {
                provider.CreateObject(containerName, uploadStream, objectName);

                // it's ok to create the same file twice
                uploadStream.Position = 0;
                ProgressMonitor progressMonitor = new ProgressMonitor(uploadStream.Length);
                provider.CreateObject(containerName, uploadStream, objectName, progressUpdated: progressMonitor.Updated);
                Assert.IsTrue(progressMonitor.IsComplete, "Failed to notify progress monitor callback of status update.");
            }

            using (MemoryStream downloadStream = new MemoryStream())
            {
                provider.GetObject(containerName, objectName, downloadStream);

                downloadStream.Position = 0;
                StreamReader reader = new StreamReader(downloadStream, Encoding.UTF8);
                string actualData = reader.ReadToEnd();
                Assert.AreEqual(fileData, actualData);
            }

            /* Cleanup
             */
            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        [DeploymentItem("DarkKnightRises.jpg")]
        public void TestCreateLargeObject()
        {
            IObjectStorageProvider provider =
                new CloudFilesProvider(Bootstrapper.Settings.TestIdentity)
                {
                    LargeFileBatchThreshold = 81920
                };

            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string sourceFileName = "DarkKnightRises.jpg";
            byte[] content = File.ReadAllBytes("DarkKnightRises.jpg");

            ObjectStore containerResult = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, containerResult);

            ProgressMonitor progressMonitor = new ProgressMonitor(content.Length);
            provider.CreateObjectFromFile(containerName, sourceFileName, progressUpdated: progressMonitor.Updated);
            Assert.IsTrue(progressMonitor.IsComplete, "Failed to notify progress monitor callback of status update.");

            using (MemoryStream downloadStream = new MemoryStream())
            {
                provider.GetObject(containerName, sourceFileName, downloadStream);
                Assert.AreEqual(content.Length, GetContainerObjectSize(provider, containerName, sourceFileName));

                downloadStream.Position = 0;
                byte[] actualData = new byte[downloadStream.Length];
                downloadStream.Read(actualData, 0, actualData.Length);
                Assert.AreEqual(content.Length, actualData.Length);
                using (MD5 md5 = MD5.Create())
                {
                    byte[] contentMd5 = md5.ComputeHash(content);
                    byte[] actualMd5 = md5.ComputeHash(actualData);
                    Assert.AreEqual(BitConverter.ToString(contentMd5), BitConverter.ToString(actualMd5));
                }
            }

            /* Cleanup
             */
            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        private static long GetContainerObjectSize(IObjectStorageProvider provider, string containerName, string objectName)
        {
            Dictionary<string, string> headers = provider.GetObjectHeaders(containerName, objectName);
            return long.Parse(headers["Content-Length"]);
        }

        private class ProgressMonitor
        {
            private readonly long _maxValue;
            private long _currentValue;

            public ProgressMonitor(long totalSize)
            {
                _maxValue = totalSize;
            }

            public bool IsComplete
            {
                get
                {
                    return _currentValue == _maxValue;
                }
            }

            public void Updated(long value)
            {
                Assert.IsTrue(value >= 0);
                Assert.IsTrue(value <= _maxValue);
                Assert.IsTrue(value >= _currentValue);
                _currentValue = value;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestGetObjectSaveToFile()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            // another random name counts as random content
            string fileData = Path.GetRandomFileName();

            ObjectStore containerResult = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, containerResult);

            using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
            {
                provider.CreateObject(containerName, uploadStream, objectName);
            }

            try
            {
                provider.GetObjectSaveToFile(containerName, Path.GetTempPath(), objectName);
                Assert.AreEqual(fileData, File.ReadAllText(Path.Combine(Path.GetTempPath(), objectName), Encoding.UTF8));

                // it's ok to download the same file twice
                ProgressMonitor progressMonitor = new ProgressMonitor(GetContainerObjectSize(provider, containerName, objectName));
                provider.GetObjectSaveToFile(containerName, Path.GetTempPath(), objectName, progressUpdated: progressMonitor.Updated);
                Assert.IsTrue(progressMonitor.IsComplete, "Failed to notify progress monitor callback of status update.");
            }
            finally
            {
                File.Delete(Path.Combine(Path.GetTempPath(), objectName));
            }

            string tempFileName = Path.GetRandomFileName();
            try
            {
                provider.GetObjectSaveToFile(containerName, Path.GetTempPath(), objectName, tempFileName);
                Assert.AreEqual(fileData, File.ReadAllText(Path.Combine(Path.GetTempPath(), tempFileName), Encoding.UTF8));

                // it's ok to download the same file twice
                ProgressMonitor progressMonitor = new ProgressMonitor(GetContainerObjectSize(provider, containerName, objectName));
                provider.GetObjectSaveToFile(containerName, Path.GetTempPath(), objectName, progressUpdated: progressMonitor.Updated);
                Assert.IsTrue(progressMonitor.IsComplete, "Failed to notify progress monitor callback of status update.");
            }
            finally
            {
                File.Delete(Path.Combine(Path.GetTempPath(), tempFileName));
            }

            /* Cleanup
             */
            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestCopyObject()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            string copiedName = Path.GetRandomFileName();
            // another random name counts as random content
            string fileData = Path.GetRandomFileName();
            string contentType = "text/plain-jane";

            ObjectStore containerResult = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, containerResult);

            using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
            {
                provider.CreateObject(containerName, uploadStream, objectName, contentType);
            }

            using (MemoryStream downloadStream = new MemoryStream())
            {
                provider.GetObject(containerName, objectName, downloadStream);

                downloadStream.Position = 0;
                StreamReader reader = new StreamReader(downloadStream, Encoding.UTF8);
                string actualData = reader.ReadToEnd();
                Assert.AreEqual(fileData, actualData);
            }

            provider.CopyObject(containerName, objectName, containerName, copiedName);

            // make sure the item is available at the copied location
            using (MemoryStream downloadStream = new MemoryStream())
            {
                provider.GetObject(containerName, copiedName, downloadStream);

                downloadStream.Position = 0;
                StreamReader reader = new StreamReader(downloadStream, Encoding.UTF8);
                string actualData = reader.ReadToEnd();
                Assert.AreEqual(fileData, actualData);
            }

            // make sure the original object still exists
            using (MemoryStream downloadStream = new MemoryStream())
            {
                provider.GetObject(containerName, objectName, downloadStream);

                downloadStream.Position = 0;
                StreamReader reader = new StreamReader(downloadStream, Encoding.UTF8);
                string actualData = reader.ReadToEnd();
                Assert.AreEqual(fileData, actualData);
            }

            // make sure the content type was not changed by the copy operation
            Assert.AreEqual(contentType, GetObjectContentType(provider, containerName, objectName));
            Assert.AreEqual(contentType, GetObjectContentType(provider, containerName, copiedName));

            /* Cleanup
             */
            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestMoveObject()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            string movedName = Path.GetRandomFileName();
            // another random name counts as random content
            string fileData = Path.GetRandomFileName();

            ObjectStore containerResult = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, containerResult);

            using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
            {
                provider.CreateObject(containerName, uploadStream, objectName);
            }

            using (MemoryStream downloadStream = new MemoryStream())
            {
                provider.GetObject(containerName, objectName, downloadStream);

                downloadStream.Position = 0;
                StreamReader reader = new StreamReader(downloadStream, Encoding.UTF8);
                string actualData = reader.ReadToEnd();
                Assert.AreEqual(fileData, actualData);
            }

            provider.MoveObject(containerName, objectName, containerName, movedName);

            try
            {
                using (MemoryStream downloadStream = new MemoryStream())
                {
                    provider.GetObject(containerName, objectName, downloadStream);
                }

                Assert.Fail("Expected an exception (object should not exist)");
            }
            catch (ResponseException)
            {
            }

            using (MemoryStream downloadStream = new MemoryStream())
            {
                provider.GetObject(containerName, movedName, downloadStream);

                downloadStream.Position = 0;
                StreamReader reader = new StreamReader(downloadStream, Encoding.UTF8);
                string actualData = reader.ReadToEnd();
                Assert.AreEqual(fileData, actualData);
            }

            /* Cleanup
             */
            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestDeleteObject()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            string containerName = TestContainerPrefix + Path.GetRandomFileName();
            string objectName = Path.GetRandomFileName();
            // another random name counts as random content
            string fileData = Path.GetRandomFileName();

            ObjectStore containerResult = provider.CreateContainer(containerName);
            Assert.AreEqual(ObjectStore.ContainerCreated, containerResult);

            using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
            {
                provider.CreateObject(containerName, uploadStream, objectName);
            }

            using (MemoryStream downloadStream = new MemoryStream())
            {
                provider.GetObject(containerName, objectName, downloadStream);

                downloadStream.Position = 0;
                StreamReader reader = new StreamReader(downloadStream, Encoding.UTF8);
                string actualData = reader.ReadToEnd();
                Assert.AreEqual(fileData, actualData);
            }

            provider.DeleteObject(containerName, objectName);

            try
            {
                using (MemoryStream downloadStream = new MemoryStream())
                {
                    provider.GetObject(containerName, objectName, downloadStream);
                }

                Assert.Fail("Expected an exception (object should not exist)");
            }
            catch (ResponseException)
            {
            }

            /* Cleanup
             */
            provider.DeleteContainer(containerName, deleteObjects: true);
        }

        #endregion

        #region Accounts

        /// <summary>
        /// This test can be used to clear all of the metadata associated with an account.
        /// </summary>
        /// <remarks>
        /// This test is normally disabled. To run the cleanup method, comment out or remove the
        /// <see cref="IgnoreAttribute"/>.
        /// </remarks>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        [TestCategory(TestCategories.Cleanup)]
        [Ignore]
        public void CleanupAllAccountMetadata()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            Dictionary<string, string> metadata = provider.GetAccountMetaData();
            Dictionary<string, string> removedMetadata = metadata.ToDictionary(i => i.Key, i => string.Empty);
            provider.UpdateAccountMetadata(removedMetadata);
        }

        /// <summary>
        /// This unit test clears the metadata associated with the account which is
        /// created by the unit tests in this class.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        [TestCategory(TestCategories.Cleanup)]
        public void CleanupTestAccountMetadata()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            Dictionary<string, string> metadata = GetAccountMetadataWithPrefix(provider, TestKeyPrefix);
            Dictionary<string, string> removedMetadata = metadata.ToDictionary(i => i.Key, i => string.Empty);
            provider.UpdateAccountMetadata(removedMetadata);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestGetAccountHeaders()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            Dictionary<string, string> headers = provider.GetAccountHeaders();
            Assert.IsNotNull(headers);

            Console.WriteLine("Account Headers:");
            foreach (var pair in headers)
            {
                Assert.IsNotNull(pair.Key);
                Assert.IsNotNull(pair.Value);
                Assert.IsFalse(string.IsNullOrEmpty(pair.Key));
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);
            }

            Assert.AreEqual(StringComparer.OrdinalIgnoreCase, headers.Comparer);

            string containerCountText;
            Assert.IsTrue(headers.TryGetValue(CloudFilesProvider.AccountContainerCount, out containerCountText));
            long containerCount;
            Assert.IsTrue(long.TryParse(containerCountText, out containerCount));
            Assert.IsTrue(containerCount >= 0);

            string accountBytesText;
            Assert.IsTrue(headers.TryGetValue(CloudFilesProvider.AccountBytesUsed, out accountBytesText));
            long accountBytes;
            Assert.IsTrue(long.TryParse(accountBytesText, out accountBytes));
            Assert.IsTrue(accountBytes >= 0);

            string objectCountText;
            if (headers.TryGetValue(CloudFilesProvider.AccountObjectCount, out objectCountText))
            {
                // the X-Account-Object-Count header is optional, but when included should be a non-negative integer
                long objectCount;
                Assert.IsTrue(long.TryParse(objectCountText, out objectCount));
                Assert.IsTrue(objectCount >= 0);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestGetAccountMetaData()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            Dictionary<string, string> metadata = provider.GetAccountMetaData();
            Assert.IsNotNull(metadata);
            Assert.AreEqual(StringComparer.OrdinalIgnoreCase, metadata.Comparer);

            Console.WriteLine("Account MetaData:");
            foreach (var pair in metadata)
            {
                Assert.IsNotNull(pair.Key);
                Assert.IsNotNull(pair.Value);
                Assert.IsFalse(string.IsNullOrEmpty(pair.Key));
                Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestAccountHeaderKeyCharacters()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);

            List<char> keyCharList = new List<char>();
            for (char i = MinHeaderKeyCharacter; i <= MaxHeaderKeyCharacter; i++)
            {
                if (!SeparatorCharacters.Contains(i) && !NotSupportedCharacters.Contains(i))
                    keyCharList.Add(i);
            }

            string key = TestKeyPrefix + new string(keyCharList.ToArray());
            Console.WriteLine("Expected key: {0}", key);

            provider.UpdateAccountMetadata(
                new Dictionary<string, string>
                {
                    { key, "Value" }
                });

            Dictionary<string, string> metadata = provider.GetAccountMetaData();
            Assert.IsNotNull(metadata);

            string value;
            Assert.IsTrue(metadata.TryGetValue(key, out value));
            Assert.AreEqual("Value", value);

            provider.UpdateAccountMetadata(
                new Dictionary<string, string>
                {
                    { key, null }
                });

            metadata = provider.GetAccountMetaData();
            Assert.IsNotNull(metadata);
            Assert.IsFalse(metadata.TryGetValue(key, out value));
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestAccountInvalidHeaderKeyCharacters()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);

            List<char> validKeyCharList = new List<char>();
            for (char i = MinHeaderKeyCharacter; i <= MaxHeaderKeyCharacter; i++)
            {
                if (!SeparatorCharacters.Contains(i) && !NotSupportedCharacters.Contains(i))
                    validKeyCharList.Add(i);
            }

            for (int i = char.MinValue; i <= char.MaxValue; i++)
            {
                if (validKeyCharList.BinarySearch((char)i) >= 0)
                    continue;

                string invalidKey = new string((char)i, 1);

                try
                {
                    provider.UpdateAccountMetadata(
                        new Dictionary<string, string>
                        {
                            { invalidKey, "Value" }
                        });
                    Assert.Fail("Should throw an exception for invalid keys.");
                }
                catch (ArgumentException)
                {
                    if (i >= MinHeaderKeyCharacter && i <= MaxHeaderKeyCharacter)
                        StringAssert.Contains(SeparatorCharacters, invalidKey);
                }
                catch (NotSupportedException)
                {
                    StringAssert.Contains(NotSupportedCharacters, invalidKey);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public void TestUpdateAccountMetadata()
        {
            IObjectStorageProvider provider = new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
            Dictionary<string, string> metadata = provider.GetAccountMetaData();
            if (metadata.Any(i => i.Key.StartsWith(TestKeyPrefix, StringComparison.OrdinalIgnoreCase)))
            {
                Assert.Inconclusive("The account contains metadata from a previous unit test run. Run CleanupTestAccountMetadata and try again.");
                return;
            }

            // test add metadata
            TestGetAccountMetaData();
            provider.UpdateAccountMetadata(
                new Dictionary<string, string>
            {
                { TestKeyPrefix + "1", "Value ĳ" },
                { TestKeyPrefix + "2", "Value ²" },
            });
            TestGetAccountMetaData();

            Dictionary<string, string> expected =
                new Dictionary<string, string>
            {
                { TestKeyPrefix + "1", "Value ĳ" },
                { TestKeyPrefix + "2", "Value ²" },
            };
            CheckMetadataCollections(expected, GetAccountMetadataWithPrefix(provider, TestKeyPrefix));

            // test update metadata
            provider.UpdateAccountMetadata(
                new Dictionary<string, string>
            {
                { TestKeyPrefix + "1", "Value 1" },
            });

            expected = new Dictionary<string, string>
            {
                { TestKeyPrefix + "1", "Value 1" },
                { TestKeyPrefix + "2", "Value ²" },
            };
            CheckMetadataCollections(expected, GetAccountMetadataWithPrefix(provider, TestKeyPrefix));

            // test remove metadata
            provider.UpdateAccountMetadata(
                new Dictionary<string, string>
            {
                { TestKeyPrefix + "1", null },
                { TestKeyPrefix + "2", string.Empty },
            });

            expected = new Dictionary<string, string>();
            CheckMetadataCollections(expected, GetAccountMetadataWithPrefix(provider, TestKeyPrefix));
        }

        private static Dictionary<string, string> GetAccountMetadataWithPrefix(IObjectStorageProvider provider, string prefix)
        {
            Dictionary<string, string> metadata = provider.GetAccountMetaData();
            return FilterMetadataPrefix(metadata, prefix);
        }

        #endregion

        private static Dictionary<string, string> FilterMetadataPrefix(Dictionary<string, string> metadata, string prefix)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, string> pair in metadata)
            {
                if (pair.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    result.Add(pair.Key, pair.Value);
            }

            return result;
        }

        private static void CheckMetadataCollections(Dictionary<string, string> expected, Dictionary<string, string> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            CheckMetadataSubset(expected, actual);
        }

        private static void CheckMetadataSubset(Dictionary<string, string> expected, Dictionary<string, string> actual)
        {
            foreach (var pair in expected)
                Assert.IsTrue(actual.Contains(pair), "Expected metadata item {{ {0} : {1} }} not found.", pair.Key, pair.Value);
        }
    }
}
