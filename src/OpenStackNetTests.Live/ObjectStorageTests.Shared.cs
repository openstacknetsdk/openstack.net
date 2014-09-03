namespace OpenStackNetTests.Live
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;
    using ICSharpCode.SharpZipLib.BZip2;
    using ICSharpCode.SharpZipLib.GZip;
    using ICSharpCode.SharpZipLib.Tar;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json.Linq;
    using OpenStack.Collections;
    using OpenStack.Net;
    using OpenStack.Security.Authentication;
    using OpenStack.Services;
    using OpenStack.Services.ObjectStorage.V1;
    using Encoding = System.Text.Encoding;
    using File = System.IO.File;
    using MemoryStream = System.IO.MemoryStream;
    using Path = System.IO.Path;
    using Stream = System.IO.Stream;
    using StreamReader = System.IO.StreamReader;

    partial class ObjectStorageTests
    {
        /// <summary>
        /// This prefix is used for metadata keys created by unit tests, to avoid overwriting metadata created by other
        /// applications.
        /// </summary>
        private const string TestKeyPrefix = "UnitTestMetadataKey-";

        /// <summary>
        /// This prefix is used for containers created by unit tests, to avoid overwriting containers created by other
        /// applications.
        /// </summary>
        private const string TestContainerPrefix = "UnitTestContainer-";

        /// <summary>
        /// The minimum character allowed in metadata keys. This is drawn from the HTTP/1.1 specification, which does
        /// not allow ASCII control characters in header keys.
        /// </summary>
        private const char MinHeaderKeyCharacter = (char)32;

        /// <summary>
        /// The maximum character allowed in metadata keys. This is drawn from the HTTP/1.1 specification, which
        /// restricts header keys to the 7-bit ASCII character set.
        /// </summary>
        private const char MaxHeaderKeyCharacter = (char)127;

        /// <summary>
        /// The HTTP/1.1 separator characters.
        /// </summary>
        private const string SeparatorCharacters = "()<>@,;:\\\"/[]?={} \t\x7F";

        /// <summary>
        /// Characters which are technically allowed in headers by HTTP/1.1, but cannot be used in metadata keys for
        /// the Object Storage Service.
        /// </summary>
        /// <remarks>
        /// The underscore is disallowed by the OpenStack Object Storage Service implementation, which silently converts
        /// it to a dash.
        /// </remarks>
        private const string NotSupportedCharacters = "_";

        #region Discoverability

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestGetObjectStorageInfoAsync()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                var call = await service.PrepareGetObjectStorageInfoAsync(cancellationToken);
                Tuple<HttpResponseMessage, ReadOnlyDictionary<string, JToken>> response = await call.SendAsync(cancellationToken);
                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Item2);
            }
        }

        #endregion

        #region Container

        /// <summary>
        /// This test can be used to clear all of the metadata associated with every container in the Object Storage
        /// Service.
        /// </summary>
        /// <remarks>
        /// <para>This test is normally disabled. To run the cleanup method, comment out or remove the
        /// <see cref="IgnoreAttribute"/>.</para>
        /// </remarks>
        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        [Ignore]
        public async Task CleanupAllContainerMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ReadOnlyCollection<Container> containers = await ListAllContainersAsync(service, cancellationToken);
                foreach (Container container in containers)
                {
                    ContainerMetadata existingMetadata = await service.GetContainerMetadataAsync(container.Name, cancellationToken);
                    Dictionary<string, string> headers = existingMetadata.Headers.ToDictionary(pair => pair.Key, pair => string.Empty);
                    Dictionary<string, string> metadata = existingMetadata.Headers.ToDictionary(pair => pair.Key, pair => string.Empty);
                    await service.UpdateContainerMetadataAsync(container.Name, new ContainerMetadata(headers, metadata), cancellationToken);
                }
            }
        }

        /// <summary>
        /// This unit test clears the metadata associated with every container which is created by the unit tests in
        /// this class.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupTestContainerMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ReadOnlyCollection<Container> containers = await ListAllContainersAsync(service, cancellationToken);
                foreach (Container container in containers)
                {
                    if (!container.Name.Value.StartsWith(TestContainerPrefix, StringComparison.Ordinal))
                        continue;

                    ContainerMetadata existingMetadata = await service.GetContainerMetadataAsync(container.Name, cancellationToken);
                    Dictionary<string, string> headers = existingMetadata.Headers.ToDictionary(pair => pair.Key, pair => string.Empty);
                    Dictionary<string, string> metadata = existingMetadata.Headers.ToDictionary(pair => pair.Key, pair => string.Empty);
                    await service.UpdateContainerMetadataAsync(container.Name, new ContainerMetadata(headers, metadata), cancellationToken);
                }
            }
        }

        /// <summary>
        /// This unit test deletes all containers created by the unit tests, including all objects within those
        /// containers.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupTestContainers()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(120)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                Stopwatch timer = Stopwatch.StartNew();
                RemovedObjectStats stats = new RemovedObjectStats();

                try
                {
                    TestHelpers.SuppressOutput = true;

                    IObjectStorageService service = CreateService();
                    ReadOnlyCollection<Container> containers = await ListAllContainersAsync(service, cancellationToken);
                    foreach (Container container in containers)
                    {
                        if (container.Name.Value.StartsWith(TestContainerPrefix))
                        {
                            Console.WriteLine("Removing test container: {0}", container.Name);
                            await RemoveContainerWithObjectsAsync(service, container.Name, stats, cancellationToken);
                        }
                        else if (container.Name.Value.Equals(".CDN_ACCESS_LOGS"))
                        {
                            foreach (ContainerObject containerObject in await ListAllObjectsAsync(service, container.Name, cancellationToken))
                            {
                                if (!containerObject.Name.Value.StartsWith(TestContainerPrefix))
                                    continue;

                                await TryRemoveObjectAsync(service, container.Name, containerObject.Name, stats, cancellationToken);
                            }
                        }
                    }
                }
                finally
                {
                    Console.WriteLine();
                    Console.WriteLine("Attempted: {0}", stats.AttemptedCount);
                    Console.WriteLine("Removed: {0}", stats.RemovedCount);
                    Console.WriteLine("Not found: {0}", stats.NotFoundCount);
                    Console.WriteLine("Failed: {0}", stats.FailedCount);
                    Console.WriteLine("Canceled: {0}", stats.CanceledCount);
                    Console.WriteLine();
                    Console.WriteLine("Elapsed time: {0} sec", timer.Elapsed.TotalSeconds);

                    TestHelpers.SuppressOutput = false;
                }
            }
        }

        private class RemovedObjectStats
        {
            public long AttemptedCount;
            public long RemovedCount;
            public long NotFoundCount;
            public long FailedCount;
            public long CanceledCount;
        }

        private Task RemoveContainerWithObjectsAsync(IObjectStorageService service, ContainerName container, CancellationToken cancellationToken)
        {
            RemovedObjectStats stats = new RemovedObjectStats();
            return RemoveContainerWithObjectsAsync(service, container, stats, cancellationToken);
        }

        private async Task RemoveContainerWithObjectsAsync(IObjectStorageService service, ContainerName container, RemovedObjectStats stats, CancellationToken cancellationToken)
        {
            try
            {
                await service.RemoveContainerAsync(container, cancellationToken);
                return;
            }
            catch (HttpWebException ex)
            {
                // Conflict means not empty. Anything else is unexpected.
                if (ex.ResponseMessage == null || ex.ResponseMessage.StatusCode != HttpStatusCode.Conflict)
                    throw;
            }

            // The container is not empty. Remove all objects from the container before trying again.
            await RemoveAllObjectsAsync(service, container, stats, cancellationToken);

            await service.RemoveContainerAsync(container, cancellationToken);
        }

        private async Task RemoveAllObjectsAsync(IObjectStorageService service, ContainerName container, RemovedObjectStats stats, CancellationToken cancellationToken)
        {
            while (true)
            {
                ServiceClient serviceClient = service as ServiceClient;
                if (serviceClient != null && (serviceClient.ConnectionLimit ?? 2) < 100)
                    serviceClient.ConnectionLimit = 100;

                Tuple<ContainerMetadata, ReadOnlyCollectionPage<ContainerObject>> page = await service.ListObjectsAsync(container, cancellationToken);
                if (page.Item2.Count == 0)
                    break;

                Console.WriteLine("  Removing a block of {0} objects", page.Item2.Count);

                long previousRemoved = Interlocked.Read(ref stats.RemovedCount);

                List<Task> tasks = new List<Task>();
                foreach (ContainerObject obj in page.Item2)
                {
                    tasks.Add(TryRemoveObjectAsync(service, container, obj.Name, stats, cancellationToken));
                }

                try
                {
                    await Task.WhenAll(tasks);
                }
                catch
                {
                    // continue as long as we are making progress
                    if (Interlocked.Read(ref stats.RemovedCount) == previousRemoved)
                        throw;
                }
            }
        }

        private async Task TryRemoveObjectAsync(IObjectStorageService service, ContainerName container, ObjectName @object, RemovedObjectStats stats, CancellationToken cancellationToken)
        {
            try
            {
                Interlocked.Increment(ref stats.AttemptedCount);

                await service.RemoveObjectAsync(container, @object, cancellationToken);

                Interlocked.Increment(ref stats.RemovedCount);
            }
            catch (HttpWebException ex)
            {

                if (ex.ResponseMessage != null)
                {
                    switch (ex.ResponseMessage.StatusCode)
                    {
                    case HttpStatusCode.NotFound:
                        Interlocked.Increment(ref stats.NotFoundCount);
                        return;

                    default:
                        break;
                    }
                }

                Interlocked.Increment(ref stats.FailedCount);
                throw;
            }
            catch (TaskCanceledException)
            {
                Interlocked.Increment(ref stats.CanceledCount);
                throw;
            }
            catch
            {
                Interlocked.Increment(ref stats.FailedCount);
                throw;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestListContainers()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ReadOnlyCollection<Container> containers = await ListAllContainersAsync(service, cancellationToken);
                if (!containers.Any())
                    Assert.Inconclusive("The account does not have any containers in the region.");

                Console.WriteLine("Containers");
                foreach (Container container in containers)
                {
                    Console.WriteLine("  {0}", container.Name);
                    Console.WriteLine("    Objects: {0}", container.ObjectCount);
                    Console.WriteLine("    Bytes: {0}", container.Size);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestContainerProperties()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ReadOnlyCollection<Container> containers = await ListAllContainersAsync(service, cancellationToken);
                if (!containers.Any())
                    Assert.Inconclusive("The account does not have any containers in the region.");

                int containersTested = 0;
                long? objectsTested = 0;
                long? totalSizeTested = 0;
                int nonEmptyContainersTested = 0;
                int nonEmptyBytesContainersTested = 0;
                foreach (Container container in containers)
                {
                    Assert.IsTrue(container.ObjectCount >= 0);
                    Assert.IsTrue(container.Size >= 0);

                    containersTested++;
                    if (container.ObjectCount > 0)
                        nonEmptyContainersTested++;
                    if (container.Size > 0)
                        nonEmptyBytesContainersTested++;

                    long objectCount = 0;
                    long? objectSize = 0;
                    foreach (var obj in await ListAllObjectsAsync(service, container.Name, cancellationToken))
                    {
                        objectCount++;
                        objectSize += obj.Size;
                    }

                    objectsTested += objectCount;
                    totalSizeTested += objectSize;

                    Assert.AreEqual(container.ObjectCount, objectCount);
                    Assert.AreEqual(container.Size, objectSize);

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
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestCreateContainer()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName container = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());

                CreateContainerApiCall apiCall;
                Tuple<HttpResponseMessage, string> result;

                apiCall = await service.PrepareCreateContainerAsync(container, cancellationToken);
                result = await apiCall.SendAsync(cancellationToken);
                Assert.AreEqual(HttpStatusCode.Created, result.Item1.StatusCode);

                apiCall = await service.PrepareCreateContainerAsync(container, cancellationToken);
                result = await apiCall.SendAsync(cancellationToken);
                Assert.AreEqual(HttpStatusCode.Accepted, result.Item1.StatusCode);

                await service.RemoveContainerAsync(container, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestCreateContainerSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName container = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());

                await service.CreateContainerAsync(container, cancellationToken);

                await service.CreateContainerAsync(container, cancellationToken);

                await service.RemoveContainerAsync(container, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestVersionedContainer()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                IObjectVersioningExtension objectVersioningExtension = service.GetServiceExtension(PredefinedObjectStorageExtensions.ObjectVersioning);

                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ContainerName versionsContainerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());

                await service.CreateContainerAsync(versionsContainerName, cancellationToken);

                CreateVersionedContainerApiCall createCall = await objectVersioningExtension.PrepareCreateVersionedContainerAsync(containerName, versionsContainerName, cancellationToken);
                await createCall.SendAsync(cancellationToken);

                ContainerName location = await service.GetVersionsLocationAsync(containerName, cancellationToken);
                Assert.AreEqual(versionsContainerName, location);

                ObjectName objectName = new ObjectName(Path.GetRandomFileName());
                string fileData1 = "first-content";
                string fileData2 = "second-content";

                /*
                 * Create the object
                 */
                using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData1)))
                {
                    await service.CreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, null);
                }

                string actualData = await ReadAllObjectTextAsync(service, containerName, objectName, Encoding.UTF8, cancellationToken);
                Assert.AreEqual(fileData1, actualData);

                /*
                 * Overwrite the object
                 */

                using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData2)))
                {
                    await service.CreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, null);
                }

                actualData = await ReadAllObjectTextAsync(service, containerName, objectName, Encoding.UTF8, cancellationToken);
                Assert.AreEqual(fileData2, actualData);

                /*
                 * Delete the object once
                 */

                await service.RemoveObjectAsync(containerName, objectName, cancellationToken);

                actualData = await ReadAllObjectTextAsync(service, containerName, objectName, Encoding.UTF8, cancellationToken);
                Assert.AreEqual(fileData1, actualData);

                /*
                 * Cleanup
                 */

                await RemoveContainerWithObjectsAsync(service, versionsContainerName, cancellationToken);
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestRemoveContainer()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName objectName = new ObjectName(Path.GetRandomFileName());
                string fileContents = "File contents!";

                await service.CreateContainerAsync(containerName, cancellationToken);

                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContents));
                await service.CreateObjectAsync(containerName, objectName, stream, cancellationToken, null);

                try
                {
                    await service.RemoveContainerAsync(containerName, cancellationToken);
                    Assert.Fail("Expected an exception");
                }
                catch (HttpWebException ex)
                {
                    Assert.IsNotNull(ex.ResponseMessage);
                    Assert.AreEqual(HttpStatusCode.Conflict, ex.ResponseMessage.StatusCode);
                }

                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestGetContainerHeader()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());

                await service.CreateContainerAsync(containerName, cancellationToken);

                ContainerMetadata metadata = await service.GetContainerMetadataAsync(containerName, cancellationToken);
                Assert.IsNotNull(metadata);

                IDictionary<string, string> headers = metadata.Headers;
                Console.WriteLine("Container Headers");
                foreach (KeyValuePair<string, string> pair in headers)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestGetContainerMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());

                await service.CreateContainerAsync(containerName, cancellationToken);

                Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Key1", "Value 1" },
                    { "Key2", "Value ²" },
                };


                await service.UpdateContainerMetadataAsync(containerName, new ContainerMetadata(ContainerMetadata.Empty.Headers, new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase)), cancellationToken);

                ContainerMetadata actualMetadata = await service.GetContainerMetadataAsync(containerName, cancellationToken);
                Console.WriteLine("Container Metadata");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestContainerHeaderKeyCharacters()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());

                await service.CreateContainerAsync(containerName, cancellationToken);

                List<char> keyCharList = new List<char>();
                for (char i = MinHeaderKeyCharacter; i <= MaxHeaderKeyCharacter; i++)
                {
                    if (!SeparatorCharacters.Contains(i) && !NotSupportedCharacters.Contains(i))
                        keyCharList.Add(i);
                }

                string key = TestKeyPrefix + new string(keyCharList.ToArray());
                Console.WriteLine("Expected key: {0}", key);

                ContainerMetadata updatedMetadata = new ContainerMetadata(
                    ContainerMetadata.Empty.Headers,
                    new Dictionary<string, string> { { key, "Value" } });
                await service.UpdateContainerMetadataAsync(containerName, updatedMetadata, cancellationToken);

                ContainerMetadata metadata = await service.GetContainerMetadataAsync(containerName, cancellationToken);
                Assert.IsNotNull(metadata);
                Assert.IsNotNull(metadata.Metadata);

                string value;
                Assert.IsTrue(metadata.Metadata.TryGetValue(key, out value));
                Assert.AreEqual("Value", value);

                updatedMetadata = new ContainerMetadata(
                    ContainerMetadata.Empty.Headers,
                    new Dictionary<string, string> { { key, null } });
                await service.UpdateContainerMetadataAsync(containerName, updatedMetadata, cancellationToken);

                metadata = await service.GetContainerMetadataAsync(containerName, cancellationToken);
                Assert.IsNotNull(metadata);
                Assert.IsNotNull(metadata.Metadata);
                Assert.IsFalse(metadata.Metadata.TryGetValue(key, out value));

                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestContainerInvalidHeaderKeyCharacters()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(60)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());

                await service.CreateContainerAsync(containerName, cancellationToken);

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
                        ContainerMetadata updatedMetadata = new ContainerMetadata(
                            ContainerMetadata.Empty.Headers,
                            new Dictionary<string, string> { { invalidKey, "Value" } });
                        await service.UpdateContainerMetadataAsync(containerName, updatedMetadata, cancellationToken);
                        Assert.Fail("Should throw an exception for invalid keys.");
                    }
                    catch (FormatException)
                    {
                        if (i >= MinHeaderKeyCharacter && i <= MaxHeaderKeyCharacter)
                            StringAssert.Contains(SeparatorCharacters, invalidKey);
                    }
                    catch (NotSupportedException)
                    {
                        StringAssert.Contains(NotSupportedCharacters, invalidKey);
                    }
                }

                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestUpdateContainerMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(60)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());

                await service.CreateContainerAsync(containerName, cancellationToken);

                Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Key1", "Value 1" },
                    { "Key2", "Value ²" },
                };

                await service.UpdateContainerMetadataAsync(containerName, new ContainerMetadata(ContainerMetadata.Empty.Headers, metadata), cancellationToken);

                ContainerMetadata actualMetadata = await service.GetContainerMetadataAsync(containerName, cancellationToken);
                Console.WriteLine("Container Metadata");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                metadata["Key2"] = "Value 2";
                Dictionary<string, string> updatedMetadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Key2", "Value 2" }
                };
                await service.UpdateContainerMetadataAsync(containerName, new ContainerMetadata(ContainerMetadata.Empty.Headers, updatedMetadata), cancellationToken);

                actualMetadata = await service.GetContainerMetadataAsync(containerName, cancellationToken);
                Console.WriteLine("Container Metadata");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestDeleteContainerMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(60)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());

                await service.CreateContainerAsync(containerName, cancellationToken);

                Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Key1", "Value 1" },
                    { "Key2", "Value ²" },
                    { "Key3", "Value 3" },
                    { "Key4", "Value 4" },
                };

                await service.UpdateContainerMetadataAsync(containerName, new ContainerMetadata(ContainerMetadata.Empty.Headers, metadata), cancellationToken);

                ContainerMetadata actualMetadata = await service.GetContainerMetadataAsync(containerName, cancellationToken);
                Console.WriteLine("Container Metadata");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                /* Check the overload which takes a single key
                 */
                // remove Key3 first to make sure we still have a ² character in a remaining value
                metadata.Remove("Key3");
                await service.RemoveContainerMetadataAsync(containerName, new[] { "Key3" }, cancellationToken);

                actualMetadata = await service.GetContainerMetadataAsync(containerName, cancellationToken);
                Console.WriteLine("Container Metadata after removing Key3");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                /* Check the overload which takes multiple keys
                 */
                metadata.Remove("Key2");
                metadata.Remove("Key4");
                await service.RemoveContainerMetadataAsync(containerName, new[] { "Key2", "Key4" }, cancellationToken);

                actualMetadata = await service.GetContainerMetadataAsync(containerName, cancellationToken);
                Console.WriteLine("Container Metadata after removing Key2, Key4");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                /* Check that duplicate removal is a NOP
                 */
                metadata.Remove("Key2");
                metadata.Remove("Key4");
                await service.RemoveContainerMetadataAsync(containerName, new[] { "Key2", "Key4" }, cancellationToken);

                actualMetadata = await service.GetContainerMetadataAsync(containerName, cancellationToken);
                Console.WriteLine("Container Metadata after removing Key2, Key4");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                /* Cleanup
                 */
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        private static async Task<ReadOnlyCollection<Container>> ListAllContainersAsync(IObjectStorageService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            Tuple<AccountMetadata, ReadOnlyCollectionPage<Container>> firstPage = await service.ListContainersAsync(cancellationToken);
            return await firstPage.Item2.GetAllPagesAsync(cancellationToken, null);
        }

        private static async Task<ReadOnlyCollection<ContainerObject>> ListAllObjectsAsync(IObjectStorageService service, ContainerName container, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            Tuple<ContainerMetadata, ReadOnlyCollectionPage<ContainerObject>> firstPage = await service.ListObjectsAsync(container, cancellationToken);
            return await firstPage.Item2.GetAllPagesAsync(cancellationToken, null);
        }

        private static async Task<string> GetObjectContentTypeAsync(IObjectStorageService service, ContainerName container, ObjectName @object, CancellationToken cancellationToken)
        {
            ObjectMetadata metadata = await service.GetObjectMetadataAsync(container, @object, cancellationToken);

            string contentType;
            if (!metadata.Headers.TryGetValue("Content-Type", out contentType))
                return null;

            return contentType.ToLowerInvariant();
        }

        #endregion

        #region Objects

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestGetObjectHeaders()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName objectName = new ObjectName(Path.GetRandomFileName());
                string objectData = "";

                await service.CreateContainerAsync(containerName, cancellationToken);

                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(objectData));
                await service.CreateObjectAsync(containerName, objectName, stream, cancellationToken, null);
                ObjectMetadata headers = await service.GetObjectMetadataAsync(containerName, objectName, cancellationToken);
                Assert.IsNotNull(headers);
                Assert.IsNotNull(headers.Headers);
                Console.WriteLine("Headers");
                foreach (var pair in headers.Headers)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(pair.Key));
                    Console.WriteLine("  {0}: {1}", pair.Key, pair.Value);
                }

                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestGetObjectMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName objectName = new ObjectName(Path.GetRandomFileName());
                string objectData = "";

                await service.CreateContainerAsync(containerName, cancellationToken);

                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(objectData));
                await service.CreateObjectAsync(containerName, objectName, stream, cancellationToken, null);
                Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Key1", "Value 1" },
                    { "Key2", "Value ²" },
                };

                await service.UpdateObjectMetadataAsync(containerName, objectName, new ObjectMetadata(ObjectMetadata.Empty.Headers, metadata), cancellationToken);

                ObjectMetadata actualMetadata = await service.GetObjectMetadataAsync(containerName, objectName, cancellationToken);
                Console.WriteLine("Object Metadata");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestUpdateObjectMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName objectName = new ObjectName(Path.GetRandomFileName());
                string objectData = "";
                string contentType = "text/plain-jane";

                await service.CreateContainerAsync(containerName, cancellationToken);

                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(objectData));
                CreateObjectApiCall createCall = await service.PrepareCreateObjectAsync(containerName, objectName, stream, cancellationToken, null);
                createCall.RequestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                await createCall.SendAsync(cancellationToken);

                Assert.AreEqual(contentType, await GetObjectContentTypeAsync(service, containerName, objectName, cancellationToken));

                Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Key1", "Value 1" },
                    { "Key2", "Value ²" },
                };

                await service.UpdateObjectMetadataAsync(containerName, objectName, new ObjectMetadata(ObjectMetadata.Empty.Headers, metadata), cancellationToken);
                Assert.AreEqual(contentType, await GetObjectContentTypeAsync(service, containerName, objectName, cancellationToken));

                ObjectMetadata actualMetadata = await service.GetObjectMetadataAsync(containerName, objectName, cancellationToken);
                Console.WriteLine("Object Metadata");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                metadata["Key2"] = "Value 2";
                Dictionary<string, string> updatedMetadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Key2", "Value 2" }
                };
                await service.UpdateObjectMetadataAsync(containerName, objectName, new ObjectMetadata(ObjectMetadata.Empty.Headers, updatedMetadata), cancellationToken);
                Assert.AreEqual(contentType, await GetObjectContentTypeAsync(service, containerName, objectName, cancellationToken));

                actualMetadata = await service.GetObjectMetadataAsync(containerName, objectName, cancellationToken);
                Console.WriteLine("Object Metadata");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestRemoveObjectMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName objectName = new ObjectName(Path.GetRandomFileName());
                string objectData = "";

                await service.CreateContainerAsync(containerName, cancellationToken);

                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(objectData));
                await service.CreateObjectAsync(containerName, objectName, stream, cancellationToken, null);
                Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Key1", "Value 1" },
                    { "Key2", "Value ²" },
                    { "Key3", "Value 3" },
                    { "Key4", "Value 4" },
                };

                await service.UpdateObjectMetadataAsync(containerName, objectName, new ObjectMetadata(ObjectMetadata.Empty.Headers, metadata), cancellationToken);

                ObjectMetadata actualMetadata = await service.GetObjectMetadataAsync(containerName, objectName, cancellationToken);
                Console.WriteLine("Object Metadata");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                /* Check the overload which takes a single key
                 */
                // remove Key3 first to make sure we still have a ² character in a remaining value
                metadata.Remove("Key3");
                await service.RemoveObjectMetadataAsync(containerName, objectName, new[] { "Key3" }, cancellationToken);

                actualMetadata = await service.GetObjectMetadataAsync(containerName, objectName, cancellationToken);
                Console.WriteLine("Object Metadata after removing Key3");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                /* Check the overload which takes multiple keys
                 */
                metadata.Remove("Key2");
                metadata.Remove("Key4");
                await service.RemoveObjectMetadataAsync(containerName, objectName, new[] { "Key2", "Key4" }, cancellationToken);

                actualMetadata = await service.GetObjectMetadataAsync(containerName, objectName, cancellationToken);
                Console.WriteLine("Object Metadata after removing Key2, Key4");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                /* Check that duplicate removal is a NOP
                 */
                metadata.Remove("Key2");
                metadata.Remove("Key4");
                await service.RemoveObjectMetadataAsync(containerName, objectName, new[] { "Key2", "Key4" }, cancellationToken);

                actualMetadata = await service.GetObjectMetadataAsync(containerName, objectName, cancellationToken);
                Console.WriteLine("Object Metadata after removing Key2, Key4");
                foreach (KeyValuePair<string, string> pair in actualMetadata.Metadata)
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                CheckMetadataCollections(metadata, actualMetadata.Metadata);

                /* Cleanup
                 */
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestListObjects()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName[] objectNames = { new ObjectName(Path.GetRandomFileName()), new ObjectName(Path.GetRandomFileName()) };
                // another random name counts as random content
                string fileData = Path.GetRandomFileName();

                await service.CreateContainerAsync(containerName, cancellationToken);

                foreach (ObjectName objectName in objectNames)
                {
                    using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
                    {
                        await service.CreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, null);
                    }
                }

                Console.WriteLine("Objects in container {0}", containerName);
                foreach (ContainerObject containerObject in await ListAllObjectsAsync(service, containerName, cancellationToken))
                {
                    Console.WriteLine("  {0}", containerObject.Name);
                }

                /* Cleanup
                 */
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestSpecialCharacters()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                string[] specialNames = { "#", " ", " lead", "trail ", "%", "x//x" };
                // another random name counts as random content
                string fileData = Path.GetRandomFileName();

                foreach (string containerSuffix in specialNames)
                {
                    if (containerSuffix.IndexOf('/') >= 0)
                        continue;

                    ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName() + containerSuffix);

                    await service.CreateContainerAsync(containerName, cancellationToken);

                    foreach (string specialName in specialNames)
                    {
                        ObjectName objectName = new ObjectName(specialName);
                        using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
                        {
                            await service.CreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, null);
                        }
                    }

                    Console.WriteLine("Objects in container {0}", containerName);
                    foreach (ContainerObject containerObject in await ListAllObjectsAsync(service, containerName, cancellationToken))
                    {
                        Console.WriteLine("  {0}", containerObject.Name);
                    }

                    /* Cleanup
                     */
                    await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestCreateObject()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName objectName = new ObjectName(Path.GetRandomFileName());
                // another random name counts as random content
                string fileData = Path.GetRandomFileName();

                await service.CreateContainerAsync(containerName, cancellationToken);

                using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
                {
                    await service.CreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, null);

                    // it's ok to create the same file twice
                    uploadStream.Position = 0;
                    ProgressMonitor progressMonitor = new ProgressMonitor(uploadStream.Length);
                    await service.CreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, progressMonitor);
                    Assert.IsTrue(progressMonitor.IsComplete, "Failed to notify progress monitor callback of status update.");
                }

                string actualData = await ReadAllObjectTextAsync(service, containerName, objectName, Encoding.UTF8, cancellationToken);
                Assert.AreEqual(fileData, actualData);

                /* Cleanup
                 */
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestCreateObjectIfNoneMatch()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName objectName = new ObjectName(Path.GetRandomFileName());
                // another random name counts as random content
                char[] fileDataChars = new char[5000];
                for (int i = 0; i < fileDataChars.Length; i++)
                    fileDataChars[i] = (char)i;

                string fileData = new string(fileDataChars);

                await service.CreateContainerAsync(containerName, cancellationToken);

                using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
                {
                    await service.CreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, null);

                    uploadStream.Position = 0;
                    ProgressMonitor progressMonitor = new ProgressMonitor(uploadStream.Length);
                    try
                    {
                        CreateObjectApiCall apiCall = await service.PrepareCreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, progressMonitor);
                        apiCall.RequestMessage.Headers.IfNoneMatch.Add(new EntityTagHeaderValue("*"));
                        await apiCall.SendAsync(cancellationToken);
                        Assert.Fail("Expected a 412 (Precondition Failed)");
                    }
                    catch (HttpWebException ex)
                    {
                        Assert.IsNotNull(ex.ResponseMessage);
                        Assert.AreEqual(HttpStatusCode.PreconditionFailed, ex.ResponseMessage.StatusCode);
                        Assert.AreEqual(0, uploadStream.Position);
                    }

                    try
                    {
                        CreateObjectApiCall apiCall = await service.PrepareCreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, progressMonitor);
                        apiCall.RequestMessage.Headers.IfNoneMatch.Add(new EntityTagHeaderValue("*"));
                        await apiCall.SendAsync(cancellationToken);
                        Assert.Fail("Expected a 412 (Precondition Failed)");
                    }
                    catch (HttpWebException ex)
                    {
                        Assert.IsNotNull(ex.ResponseMessage);
                        Assert.AreEqual(HttpStatusCode.PreconditionFailed, ex.ResponseMessage.StatusCode);
                        Assert.AreEqual(0, uploadStream.Position);
                    }
                }

                string actualData = await ReadAllObjectTextAsync(service, containerName, objectName, Encoding.UTF8, cancellationToken);
                Assert.AreEqual(fileData, actualData);

                /* Cleanup
                 */
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestCreateObjectWithMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName objectName = new ObjectName(Path.GetRandomFileName());
                // another random name counts as random content
                string fileData = Path.GetRandomFileName();

                await service.CreateContainerAsync(containerName, cancellationToken);

                using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
                {
                    CreateObjectApiCall apiCall = await service.PrepareCreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, null);
                    apiCall.RequestMessage.Headers.Add(ObjectMetadata.ObjectMetadataPrefix + "Project-code", "ProjectCode");
                    apiCall.RequestMessage.Headers.Add(ObjectMetadata.ObjectMetadataPrefix + "File-description", "FileDescription");
                    apiCall.RequestMessage.Headers.Add(ObjectMetadata.ObjectMetadataPrefix + "User-code", "User Code");
                    await apiCall.SendAsync(cancellationToken);
                }

                string actualData = await ReadAllObjectTextAsync(service, containerName, objectName, Encoding.UTF8, cancellationToken);
                Assert.AreEqual(fileData, actualData);

                ObjectMetadata metadata = await service.GetObjectMetadataAsync(containerName, objectName, cancellationToken);
                Assert.AreEqual("ProjectCode", metadata.Metadata["Project-code"]);
                Assert.AreEqual("FileDescription", metadata.Metadata["file-Description"]);
                Assert.AreEqual("User Code", metadata.Metadata["user-code"]);

                /* Cleanup
                 */
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestSupportsExtractArchive()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                bool supportsExtractArchive = await service.SupportsExtractArchiveAsync(cancellationToken);
                Assert.IsTrue(supportsExtractArchive);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        [DeploymentItem("DarkKnightRises.jpg")]
        public async Task TestExtractArchiveTar()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName sourceFileName = new ObjectName("DarkKnightRises.jpg");
                byte[] content = File.ReadAllBytes("DarkKnightRises.jpg");
                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (TarArchive archive = TarArchive.CreateOutputTarArchive(outputStream))
                    {
                        archive.IsStreamOwner = false;
                        archive.RootPath = Path.GetDirectoryName(Path.GetFullPath(sourceFileName.Value)).Replace('\\', '/');
                        TarEntry entry = TarEntry.CreateEntryFromFile(sourceFileName.Value);
                        archive.WriteEntry(entry, true);
                        archive.Close();
                    }

                    outputStream.Flush();
                    outputStream.Position = 0;
                    ExtractArchiveResponse response = await service.ExtractArchiveAsync(containerName, outputStream, ArchiveFormat.Tar, cancellationToken, null);
                    Assert.IsNotNull(response);
                    Assert.AreEqual(1, response.CreatedFiles);
                    Assert.IsNotNull(response.Errors);
                    Assert.AreEqual(0, response.Errors.Count);
                }

                using (MemoryStream downloadStream = new MemoryStream())
                {
                    var result = await service.GetObjectAsync(containerName, sourceFileName, cancellationToken);
                    await result.Item2.CopyToAsync(downloadStream);

                    Assert.AreEqual(content.Length, await GetContainerObjectSizeAsync(service, containerName, sourceFileName, cancellationToken));

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
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        [DeploymentItem("DarkKnightRises.jpg")]
        public async Task TestExtractArchiveTarGz()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName sourceFileName = new ObjectName("DarkKnightRises.jpg");
                byte[] content = File.ReadAllBytes("DarkKnightRises.jpg");
                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (GZipOutputStream gzoStream = new GZipOutputStream(outputStream))
                    {
                        gzoStream.IsStreamOwner = false;
                        gzoStream.SetLevel(9);
                        using (TarArchive archive = TarArchive.CreateOutputTarArchive(gzoStream))
                        {
                            archive.IsStreamOwner = false;
                            archive.RootPath = Path.GetDirectoryName(Path.GetFullPath(sourceFileName.Value)).Replace('\\', '/');
                            TarEntry entry = TarEntry.CreateEntryFromFile(sourceFileName.Value);
                            archive.WriteEntry(entry, true);
                            archive.Close();
                        }
                    }

                    outputStream.Flush();
                    outputStream.Position = 0;
                    ExtractArchiveResponse response = await service.ExtractArchiveAsync(containerName, outputStream, ArchiveFormat.TarGz, cancellationToken, null);
                    Assert.IsNotNull(response);
                    Assert.AreEqual(1, response.CreatedFiles);
                    Assert.IsNotNull(response.Errors);
                    Assert.AreEqual(0, response.Errors.Count);
                }

                using (MemoryStream downloadStream = new MemoryStream())
                {
                    var result = await service.GetObjectAsync(containerName, sourceFileName, cancellationToken);
                    await result.Item2.CopyToAsync(downloadStream);

                    Assert.AreEqual(content.Length, await GetContainerObjectSizeAsync(service, containerName, sourceFileName, cancellationToken));

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
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        [DeploymentItem("DarkKnightRises.jpg")]
        public async Task TestExtractArchiveTarBz2()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName sourceFileName = new ObjectName("DarkKnightRises.jpg");
                byte[] content = File.ReadAllBytes("DarkKnightRises.jpg");
                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (BZip2OutputStream bz2Stream = new BZip2OutputStream(outputStream))
                    {
                        bz2Stream.IsStreamOwner = false;
                        using (TarArchive archive = TarArchive.CreateOutputTarArchive(bz2Stream))
                        {
                            archive.IsStreamOwner = false;
                            archive.RootPath = Path.GetDirectoryName(Path.GetFullPath(sourceFileName.Value)).Replace('\\', '/');
                            TarEntry entry = TarEntry.CreateEntryFromFile(sourceFileName.Value);
                            archive.WriteEntry(entry, true);
                            archive.Close();
                        }
                    }

                    outputStream.Flush();
                    outputStream.Position = 0;
                    ExtractArchiveResponse response = await service.ExtractArchiveAsync(containerName, outputStream, ArchiveFormat.TarBz2, cancellationToken, null);
                    Assert.IsNotNull(response);
                    Assert.AreEqual(1, response.CreatedFiles);
                    Assert.IsNotNull(response.Errors);
                    Assert.AreEqual(0, response.Errors.Count);
                }

                using (MemoryStream downloadStream = new MemoryStream())
                {
                    var result = await service.GetObjectAsync(containerName, sourceFileName, cancellationToken);
                    await result.Item2.CopyToAsync(downloadStream);

                    Assert.AreEqual(content.Length, await GetContainerObjectSizeAsync(service, containerName, sourceFileName, cancellationToken));

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
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        [DeploymentItem("DarkKnightRises.jpg")]
        public async Task TestExtractArchiveTarGzCreateContainer()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName sourceFileName = new ObjectName("DarkKnightRises.jpg");
                byte[] content = File.ReadAllBytes("DarkKnightRises.jpg");
                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (GZipOutputStream gzoStream = new GZipOutputStream(outputStream))
                    {
                        gzoStream.IsStreamOwner = false;
                        gzoStream.SetLevel(9);
                        using (TarOutputStream tarOutputStream = new TarOutputStream(gzoStream))
                        {
                            tarOutputStream.IsStreamOwner = false;
                            TarEntry entry = TarEntry.CreateTarEntry(containerName.Value + '/' + sourceFileName);
                            entry.Size = content.Length;
                            tarOutputStream.PutNextEntry(entry);
                            tarOutputStream.Write(content, 0, content.Length);
                            tarOutputStream.CloseEntry();
                            tarOutputStream.Close();
                        }
                    }

                    outputStream.Flush();
                    outputStream.Position = 0;
                    ExtractArchiveResponse response = await service.ExtractArchiveAsync(outputStream, ArchiveFormat.TarGz, cancellationToken, null);
                    Assert.IsNotNull(response);
                    Assert.AreEqual(1, response.CreatedFiles);
                    Assert.IsNotNull(response.Errors);
                    Assert.AreEqual(0, response.Errors.Count);
                }

                using (MemoryStream downloadStream = new MemoryStream())
                {
                    var objectData = await service.GetObjectAsync(containerName, sourceFileName, cancellationToken);
                    await objectData.Item2.CopyToAsync(downloadStream);

                    Assert.AreEqual(content.Length, await GetContainerObjectSizeAsync(service, containerName, sourceFileName, cancellationToken));

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
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        private static async Task<long> GetContainerObjectSizeAsync(IObjectStorageService service, ContainerName container, ObjectName @object, CancellationToken cancellationToken)
        {
            ObjectMetadata metadata = await service.GetObjectMetadataAsync(container, @object, cancellationToken);
            return long.Parse(metadata.Headers["Content-Length"]);
        }

        private class ProgressMonitor : IProgress<long>
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

            public void Report(long value)
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
        public async Task TestCopyObject()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName objectName = new ObjectName(Path.GetRandomFileName());
                ObjectName copiedName = new ObjectName(Path.GetRandomFileName());
                // another random name counts as random content
                string fileData = Path.GetRandomFileName();
                string contentType = "text/plain-jane";

                await service.CreateContainerAsync(containerName, cancellationToken);

                using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
                {
                    CreateObjectApiCall createCall = await service.PrepareCreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, null);
                    createCall.RequestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                    await createCall.SendAsync(cancellationToken);
                }

                string actualData = await ReadAllObjectTextAsync(service, containerName, objectName, Encoding.UTF8, cancellationToken);
                Assert.AreEqual(fileData, actualData);

                await service.CopyObjectAsync(containerName, objectName, containerName, copiedName, cancellationToken);

                // make sure the item is available at the copied location
                actualData = await ReadAllObjectTextAsync(service, containerName, copiedName, Encoding.UTF8, cancellationToken);
                Assert.AreEqual(fileData, actualData);

                // make sure the original object still exists
                actualData = await ReadAllObjectTextAsync(service, containerName, objectName, Encoding.UTF8, cancellationToken);
                Assert.AreEqual(fileData, actualData);

                // make sure the content type was not changed by the copy operation
                Assert.AreEqual(contentType, await GetObjectContentTypeAsync(service, containerName, objectName, cancellationToken));
                Assert.AreEqual(contentType, await GetObjectContentTypeAsync(service, containerName, copiedName, cancellationToken));

                /* Cleanup
                 */
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestMoveObject()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName objectName = new ObjectName(Path.GetRandomFileName());
                ObjectName movedName = new ObjectName(Path.GetRandomFileName());
                // another random name counts as random content
                string fileData = Path.GetRandomFileName();
                string contentType = "text/plain-jane";

                await service.CreateContainerAsync(containerName, cancellationToken);

                using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
                {
                    CreateObjectApiCall createCall = await service.PrepareCreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, null);
                    createCall.RequestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                    await createCall.SendAsync(cancellationToken);
                }

                string actualData = await ReadAllObjectTextAsync(service, containerName, objectName, Encoding.UTF8, cancellationToken);
                Assert.AreEqual(fileData, actualData);

                await service.MoveObjectAsync(containerName, objectName, containerName, movedName, cancellationToken);

                try
                {
                    using (MemoryStream downloadStream = new MemoryStream())
                    {
                        await service.GetObjectAsync(containerName, objectName, cancellationToken);
                    }

                    Assert.Fail("Expected an exception (object should not exist)");
                }
                catch (HttpWebException ex)
                {
                    Assert.IsNotNull(ex.ResponseMessage);
                    Assert.AreEqual(HttpStatusCode.NotFound, ex.ResponseMessage.StatusCode);
                }

                // make sure the item is available at the new location
                actualData = await ReadAllObjectTextAsync(service, containerName, movedName, Encoding.UTF8, cancellationToken);
                Assert.AreEqual(fileData, actualData);

                // make sure the content type was preserved by the move
                Assert.AreEqual(contentType, await GetObjectContentTypeAsync(service, containerName, movedName, cancellationToken));

                /* Cleanup
                 */
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestRemoveObject()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                ContainerName containerName = new ContainerName(TestContainerPrefix + Path.GetRandomFileName());
                ObjectName objectName = new ObjectName(Path.GetRandomFileName());
                // another random name counts as random content
                string fileData = Path.GetRandomFileName();

                await service.CreateContainerAsync(containerName, cancellationToken);

                using (MemoryStream uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileData)))
                {
                    await service.CreateObjectAsync(containerName, objectName, uploadStream, cancellationToken, null);
                }

                string actualData = await ReadAllObjectTextAsync(service, containerName, objectName, Encoding.UTF8, cancellationToken);
                Assert.AreEqual(fileData, actualData);

                await service.RemoveObjectAsync(containerName, objectName, cancellationToken);

                try
                {
                    using (MemoryStream downloadStream = new MemoryStream())
                    {
                        await service.GetObjectAsync(containerName, objectName, cancellationToken);
                    }

                    Assert.Fail("Expected an exception (object should not exist)");
                }
                catch (HttpWebException ex)
                {
                    Assert.IsNotNull(ex.ResponseMessage);
                    Assert.AreEqual(HttpStatusCode.NotFound, ex.ResponseMessage.StatusCode);
                }

                /* Cleanup
                 */
                await RemoveContainerWithObjectsAsync(service, containerName, cancellationToken);
            }
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
        [TestCategory(TestCategories.Cleanup)]
        [Ignore]
        public async Task CleanupAllAccountMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                AccountMetadata metadata = await service.GetAccountMetadataAsync(cancellationToken);
                Dictionary<string, string> removedMetadata = metadata.Metadata.ToDictionary(i => i.Key, i => string.Empty);
                await service.UpdateAccountMetadataAsync(new AccountMetadata(AccountMetadata.Empty.Headers, removedMetadata), cancellationToken);
            }
        }

        /// <summary>
        /// This unit test clears the metadata associated with the account which is
        /// created by the unit tests in this class.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupTestAccountMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                IDictionary<string, string> metadata = await GetAccountMetadataWithPrefixAsync(service, TestKeyPrefix, cancellationToken);
                Dictionary<string, string> removedMetadata = metadata.ToDictionary(i => i.Key, i => string.Empty);
                await service.UpdateAccountMetadataAsync(new AccountMetadata(AccountMetadata.Empty.Headers, removedMetadata), cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestGetAccountHeaders()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                AccountMetadata headers = await service.GetAccountMetadataAsync(cancellationToken);
                Assert.IsNotNull(headers);
                Assert.IsNotNull(headers.Headers);
                if (headers.Headers.Count == 0)
                    Assert.Inconclusive("The account did not appear to have any non-metadata headers.");

                Console.WriteLine("Account Headers:");
                foreach (var pair in headers.Headers)
                {
                    Assert.IsNotNull(pair.Key);
                    Assert.IsNotNull(pair.Value);
                    Assert.IsFalse(string.IsNullOrEmpty(pair.Key));
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                    // case insensitivity check
                    Assert.IsTrue(headers.Headers.ContainsKey(pair.Key.ToUpperInvariant()));
                    Assert.IsTrue(headers.Headers.ContainsKey(pair.Key.ToLowerInvariant()));
                }

                long? containerCount = headers.GetContainerCount();
                Assert.IsTrue(containerCount >= 0);

                long? accountBytes = headers.GetBytesUsed();
                Assert.IsTrue(accountBytes >= 0);

                long? objectCount = headers.GetObjectCount();
                if (objectCount.HasValue)
                {
                    // the X-Account-Object-Count header is optional, but when included should be a non-negative integer
                    Assert.IsTrue(objectCount >= 0);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestGetAccountMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                AccountMetadata metadata = await service.GetAccountMetadataAsync(cancellationToken);
                Assert.IsNotNull(metadata);
                Assert.IsNotNull(metadata.Metadata);

                Console.WriteLine("Account Metadata:");
                foreach (var pair in metadata.Metadata)
                {
                    Assert.IsNotNull(pair.Key);
                    Assert.IsNotNull(pair.Value);
                    Assert.IsFalse(string.IsNullOrEmpty(pair.Key));
                    Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);

                    // case insensitivity check
                    Assert.IsTrue(metadata.Metadata.ContainsKey(pair.Key.ToUpperInvariant()));
                    Assert.IsTrue(metadata.Metadata.ContainsKey(pair.Key.ToLowerInvariant()));
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestAccountHeaderKeyCharacters()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();

                List<char> keyCharList = new List<char>();
                for (char i = MinHeaderKeyCharacter; i <= MaxHeaderKeyCharacter; i++)
                {
                    if (!SeparatorCharacters.Contains(i) && !NotSupportedCharacters.Contains(i))
                        keyCharList.Add(i);
                }

                string key = TestKeyPrefix + new string(keyCharList.ToArray());
                Console.WriteLine("Expected key: {0}", key);

                await service.UpdateAccountMetadataAsync(new AccountMetadata(AccountMetadata.Empty.Headers, new Dictionary<string, string> { { key, "Value" } }), cancellationToken);

                AccountMetadata metadata = await service.GetAccountMetadataAsync(cancellationToken);
                Assert.IsNotNull(metadata);
                Assert.IsNotNull(metadata.Metadata);

                string value;
                Assert.IsTrue(metadata.Metadata.TryGetValue(key, out value));
                Assert.AreEqual("Value", value);

                await service.UpdateAccountMetadataAsync(new AccountMetadata(AccountMetadata.Empty.Headers, new Dictionary<string, string> { { key, null } }), cancellationToken);

                metadata = await service.GetAccountMetadataAsync(cancellationToken);
                Assert.IsNotNull(metadata);
                Assert.IsNotNull(metadata.Metadata);
                Assert.IsFalse(metadata.Metadata.TryGetValue(key, out value));
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestAccountInvalidHeaderKeyCharacters()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();

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
                        await service.PrepareUpdateAccountMetadataAsync(new AccountMetadata(AccountMetadata.Empty.Headers, new Dictionary<string, string> { { invalidKey, "Value" } }), cancellationToken).ConfigureAwait(false);
                        Assert.Fail("Should throw an exception during preparation of the request for invalid keys.");
                    }
                    catch (FormatException)
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
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ObjectStorage)]
        public async Task TestUpdateAccountMetadata()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IObjectStorageService service = CreateService();
                AccountMetadata metadata = await service.GetAccountMetadataAsync(cancellationToken);
                if (metadata.Metadata.Any(i => i.Key.StartsWith(TestKeyPrefix, StringComparison.OrdinalIgnoreCase)))
                {
                    Assert.Inconclusive("The account contains metadata from a previous unit test run. Run CleanupTestAccountMetadata and try again.");
                    return;
                }

                // test add metadata
                await TestGetAccountMetadata();
                await service.UpdateAccountMetadataAsync(new AccountMetadata(AccountMetadata.Empty.Headers, new Dictionary<string, string> { { TestKeyPrefix + "1", "Value ĳ" }, { TestKeyPrefix + "2", "Value ²" }, }), cancellationToken);
                await TestGetAccountMetadata();

                Dictionary<string, string> expected =
                    new Dictionary<string, string>
                    {
                        { TestKeyPrefix + "1", "Value ĳ" },
                        { TestKeyPrefix + "2", "Value ²" },
                    };
                CheckMetadataCollections(expected, await GetAccountMetadataWithPrefixAsync(service, TestKeyPrefix, cancellationToken));

                // test update metadata
                await service.UpdateAccountMetadataAsync(new AccountMetadata(AccountMetadata.Empty.Headers, new Dictionary<string, string> { { TestKeyPrefix + "1", "Value 1" }, }), cancellationToken);

                expected = new Dictionary<string, string>
                {
                    { TestKeyPrefix + "1", "Value 1" },
                    { TestKeyPrefix + "2", "Value ²" },
                };
                CheckMetadataCollections(expected, await GetAccountMetadataWithPrefixAsync(service, TestKeyPrefix, cancellationToken));

                // test remove metadata
                await service.UpdateAccountMetadataAsync(new AccountMetadata(AccountMetadata.Empty.Headers, new Dictionary<string, string> { { TestKeyPrefix + "1", null }, { TestKeyPrefix + "2", string.Empty }, }), cancellationToken);

                expected = new Dictionary<string, string>();
                CheckMetadataCollections(expected, await GetAccountMetadataWithPrefixAsync(service, TestKeyPrefix, cancellationToken));
            }
        }

        private static async Task<IDictionary<string, string>> GetAccountMetadataWithPrefixAsync(IObjectStorageService service, string prefix, CancellationToken cancellationToken)
        {
            AccountMetadata accountMetadata = await service.GetAccountMetadataAsync(cancellationToken);
            return FilterMetadataPrefix(accountMetadata.Metadata, prefix);
        }

        #endregion

        private static IDictionary<string, string> FilterMetadataPrefix(IDictionary<string, string> metadata, string prefix)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, string> pair in metadata)
            {
                if (pair.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    result.Add(pair.Key, pair.Value);
            }

            return result;
        }

        private static void CheckMetadataCollections(IDictionary<string, string> expected, IDictionary<string, string> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            CheckMetadataSubset(expected, actual);
        }

        private static void CheckMetadataSubset(IDictionary<string, string> expected, IDictionary<string, string> actual)
        {
            foreach (var pair in expected)
                Assert.IsTrue(actual.Contains(pair), "Expected metadata item {{ {0} : {1} }} not found.", pair.Key, pair.Value);
        }

        private static async Task<string> ReadAllObjectTextAsync(IObjectStorageService service, ContainerName container, ObjectName @object, Encoding encoding/*, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null*/, CancellationToken cancellationToken, int chunkSize = 65536)
        {
            using (MemoryStream downloadStream = new MemoryStream())
            {
                Tuple<ObjectMetadata, Stream> objectData = await service.GetObjectAsync(container, @object, cancellationToken);
                await objectData.Item2.CopyToAsync(downloadStream, chunkSize, cancellationToken);

                downloadStream.Position = 0;
                StreamReader reader = new StreamReader(downloadStream, encoding);
                return await reader.ReadToEndAsync();
            }
        }

        protected TimeSpan TestTimeout(TimeSpan timeSpan)
        {
            if (Debugger.IsAttached)
                return TimeSpan.FromDays(6);

            return timeSpan;
        }

        private IObjectStorageService CreateService()
        {
            IAuthenticationService authenticationService = IdentityV2Tests.CreateAuthenticationService(Credentials);
            return CreateService(authenticationService, Credentials);
        }

        internal static IObjectStorageService CreateService(IAuthenticationService authenticationService, TestCredentials credentials)
        {
            ObjectStorageClient client;
            switch (credentials.Vendor)
            {
            case "HP":
                // currently HP does not have a vendor-specific IObjectStorageService
                goto default;

            case "Rackspace":
                // currently Rackspace does not have a vendor-specific IObjectStorageService
                goto default;

            case "OpenStack":
            default:
                client = new ObjectStorageClient(authenticationService, credentials.DefaultRegion, false);
                break;
            }

            TestProxy.ConfigureService(client, credentials.Proxy);
            client.BeforeAsyncWebRequest += TestHelpers.HandleBeforeAsyncWebRequest;
            client.AfterAsyncWebResponse += TestHelpers.HandleAfterAsyncWebResponse;

            return client;
        }
    }
}
