namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Core;
    using net.openstack.Core.Collections;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.Databases;
    using CancellationToken = System.Threading.CancellationToken;
    using CancellationTokenSource = System.Threading.CancellationTokenSource;
    using CloudIdentity = net.openstack.Core.Domain.CloudIdentity;
    using Debugger = System.Diagnostics.Debugger;
    using Encoding = System.Text.Encoding;
    using Formatting = Newtonsoft.Json.Formatting;
    using HttpWebRequest = System.Net.HttpWebRequest;
    using HttpWebResponse = System.Net.HttpWebResponse;
    using IIdentityProvider = net.openstack.Core.Providers.IIdentityProvider;
    using JsonConvert = Newtonsoft.Json.JsonConvert;
    using Path = System.IO.Path;
    using WebException = System.Net.WebException;
    using WebResponse = System.Net.WebResponse;

    [TestClass]
    public class UserDatabaseTests
    {
        /// <summary>
        /// Databases created by these unit tests have a name with this prefix, allowing
        /// them to be identified and cleaned up following a failed test.
        /// </summary>
        private static readonly string TestDatabaseInstancePrefix = "UnitTestDbInstance-";

        /// <summary>
        /// Database backups created by these unit tests have a name with this prefix, allowing
        /// them to be identified and cleaned up following a failed test.
        /// </summary>
        private static readonly string TestDatabaseBackupPrefix = "UnitTestDbBackup-";

        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupTestDatabases()
        {
            IDatabaseService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                ReadOnlyCollection<DatabaseInstance> instances = await ListAllDatabaseInstancesAsync(provider, null, cancellationTokenSource.Token);
                List<Task> tasks = new List<Task>();
                foreach (DatabaseInstance instance in instances)
                {
                    if (string.IsNullOrEmpty(instance.Name) || !instance.Name.StartsWith(TestDatabaseInstancePrefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    Console.WriteLine("Deleting database instance '{0}' ({1})...", instance.Name, instance.Id);
                    tasks.Add(provider.RemoveDatabaseInstanceAsync(instance.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null));
                }

                if (tasks.Count > 0)
                    await Task.Factory.ContinueWhenAll(tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupTestDatabaseBackups()
        {
            IDatabaseService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                ReadOnlyCollection<Backup> backups = await provider.ListBackupsAsync(cancellationTokenSource.Token);
                List<Task> tasks = new List<Task>();
                foreach (Backup backup in backups)
                {
                    if (string.IsNullOrEmpty(backup.Name) || !backup.Name.StartsWith(TestDatabaseBackupPrefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    Console.WriteLine("Deleting database backup '{0}' ({1})...", backup.Name, backup.Id);
                    tasks.Add(provider.RemoveBackupAsync(backup.Id, cancellationTokenSource.Token));
                }

                if (tasks.Count > 0)
                    await Task.Factory.ContinueWhenAll(tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestCreateInstance()
        {
            IDatabaseService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                ReadOnlyCollection<DatabaseFlavor> flavors = await provider.ListFlavorsAsync(cancellationTokenSource.Token);
                if (flavors.Count == 0)
                    Assert.Inconclusive("The service did not report any flavors.");

                DatabaseFlavor smallestFlavor = flavors.Where(i => i.Memory.HasValue).OrderBy(i => i.Memory).First();
                string instanceName = CreateRandomDatabaseInstanceName();
                DatabaseInstanceConfiguration configuration = new DatabaseInstanceConfiguration(smallestFlavor.Href, new DatabaseVolumeConfiguration(1), instanceName);
                DatabaseInstance instance = await provider.CreateDatabaseInstanceAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                /* Cleanup
                 */
                await provider.RemoveDatabaseInstanceAsync(instance.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestEnableRootUser()
        {
            IDatabaseService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                ReadOnlyCollection<DatabaseFlavor> flavors = await provider.ListFlavorsAsync(cancellationTokenSource.Token);
                if (flavors.Count == 0)
                    Assert.Inconclusive("The service did not report any flavors.");

                DatabaseFlavor smallestFlavor = flavors.Where(i => i.Memory.HasValue).OrderBy(i => i.Memory).First();
                string instanceName = CreateRandomDatabaseInstanceName();
                DatabaseInstanceConfiguration configuration = new DatabaseInstanceConfiguration(smallestFlavor.Href, new DatabaseVolumeConfiguration(1), instanceName);
                DatabaseInstance instance = await provider.CreateDatabaseInstanceAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                bool? enabled = await provider.CheckRootEnabledStatusAsync(instance.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(enabled);
                Assert.IsFalse(enabled.Value);

                RootUser rootUser = await provider.EnableRootUserAsync(instance.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(rootUser);
                Assert.IsFalse(string.IsNullOrEmpty(rootUser.Name));
                Assert.IsFalse(string.IsNullOrEmpty(rootUser.Password));

                enabled = await provider.CheckRootEnabledStatusAsync(instance.Id, cancellationTokenSource.Token);

                RootUser anotherRootUser = await provider.EnableRootUserAsync(instance.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(anotherRootUser);
                Assert.IsFalse(string.IsNullOrEmpty(rootUser.Name));
                Assert.IsFalse(string.IsNullOrEmpty(rootUser.Password));

                /* Cleanup
                 */
                await provider.RemoveDatabaseInstanceAsync(instance.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestRebootInstance()
        {
            IDatabaseService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                ReadOnlyCollection<DatabaseFlavor> flavors = await provider.ListFlavorsAsync(cancellationTokenSource.Token);
                if (flavors.Count == 0)
                    Assert.Inconclusive("The service did not report any flavors.");

                DatabaseFlavor smallestFlavor = flavors.Where(i => i.Memory.HasValue).OrderBy(i => i.Memory).First();
                string instanceName = CreateRandomDatabaseInstanceName();
                DatabaseInstanceConfiguration configuration = new DatabaseInstanceConfiguration(smallestFlavor.Href, new DatabaseVolumeConfiguration(1), instanceName);
                DatabaseInstance instance = await provider.CreateDatabaseInstanceAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                await provider.RestartDatabaseInstanceAsync(instance.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                /* Cleanup
                 */
                await provider.RemoveDatabaseInstanceAsync(instance.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestResizeInstance()
        {
            IDatabaseService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                ReadOnlyCollection<DatabaseFlavor> flavors = await provider.ListFlavorsAsync(cancellationTokenSource.Token);
                if (flavors.Count == 0)
                    Assert.Inconclusive("The service did not report any flavors.");

                DatabaseFlavor smallestFlavor = flavors.Where(i => i.Memory.HasValue).OrderBy(i => i.Memory).First();
                string instanceName = CreateRandomDatabaseInstanceName();
                DatabaseInstanceConfiguration configuration = new DatabaseInstanceConfiguration(smallestFlavor.Href, new DatabaseVolumeConfiguration(1), instanceName);
                DatabaseInstance instance = await provider.CreateDatabaseInstanceAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                DatabaseFlavor nextSmallestFlavor = flavors.Where(i => i.Memory.HasValue).OrderBy(i => i.Memory).Skip(1).First();
                await provider.ResizeDatabaseInstanceAsync(instance.Id, nextSmallestFlavor.Href, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                /* Cleanup
                 */
                await provider.RemoveDatabaseInstanceAsync(instance.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestResizeInstanceVolume()
        {
            IDatabaseService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                ReadOnlyCollection<DatabaseFlavor> flavors = await provider.ListFlavorsAsync(cancellationTokenSource.Token);
                if (flavors.Count == 0)
                    Assert.Inconclusive("The service did not report any flavors.");

                DatabaseFlavor smallestFlavor = flavors.Where(i => i.Memory.HasValue).OrderBy(i => i.Memory).First();
                string instanceName = CreateRandomDatabaseInstanceName();
                DatabaseInstanceConfiguration configuration = new DatabaseInstanceConfiguration(smallestFlavor.Href, new DatabaseVolumeConfiguration(1), instanceName);
                DatabaseInstance instance = await provider.CreateDatabaseInstanceAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                await provider.ResizeDatabaseInstanceVolumeAsync(instance.Id, 2, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                /* Cleanup
                 */
                await provider.RemoveDatabaseInstanceAsync(instance.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestCreateDatabase()
        {
            IDatabaseService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                ReadOnlyCollection<DatabaseFlavor> flavors = await provider.ListFlavorsAsync(cancellationTokenSource.Token);
                if (flavors.Count == 0)
                    Assert.Inconclusive("The service did not report any flavors.");

                DatabaseFlavor smallestFlavor = flavors.Where(i => i.Memory.HasValue).OrderBy(i => i.Memory).First();
                string instanceName = CreateRandomDatabaseInstanceName();
                DatabaseInstanceConfiguration configuration = new DatabaseInstanceConfiguration(smallestFlavor.Href, new DatabaseVolumeConfiguration(1), instanceName);
                DatabaseInstance instance = await provider.CreateDatabaseInstanceAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                DatabaseName databaseName = CreateRandomDatabaseName();
                DatabaseConfiguration databaseConfiguration = new DatabaseConfiguration(databaseName, null, null);
                await provider.CreateDatabaseAsync(instance.Id, databaseConfiguration, cancellationTokenSource.Token);

                Console.WriteLine("Databases in instance '{0}':", instance.Name);
                foreach (var database in await ListAllDatabasesAsync(provider, instance.Id, null, cancellationTokenSource.Token))
                    Console.WriteLine("    {0}", database.Name);

                await provider.RemoveDatabaseAsync(instance.Id, databaseName, cancellationTokenSource.Token);

                /* Cleanup
                 */
                await provider.RemoveDatabaseInstanceAsync(instance.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestListFlavors()
        {
            IDatabaseService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                ReadOnlyCollection<DatabaseFlavor> flavors = await provider.ListFlavorsAsync(cancellationTokenSource.Token);
                if (flavors.Count == 0)
                    Assert.Inconclusive("The service did not report any flavors.");

                foreach (var flavor in flavors)
                {
                    Console.WriteLine("Flavor: '{0}' ({1})", flavor.Name, flavor.Id);
                    Console.WriteLine("  URI: {0}", flavor.Href);
                    Console.WriteLine("  RAM: {0} MB", flavor.Memory);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestGetFlavor()
        {
            IDatabaseService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                ReadOnlyCollection<DatabaseFlavor> flavors = await provider.ListFlavorsAsync(cancellationTokenSource.Token);
                if (flavors.Count == 0)
                    Assert.Inconclusive("The service did not report any flavors.");

                Task<DatabaseFlavor>[] flavorTasks = Array.ConvertAll(flavors.ToArray(), flavor => provider.GetFlavorAsync(flavor.Id, cancellationTokenSource.Token));
                await Task.WhenAll(flavorTasks);

                for (int i = 0; i < flavors.Count; i++)
                {
                    DatabaseFlavor referenceFlavor = flavors[i];
                    Assert.IsNotNull(referenceFlavor);

                    DatabaseFlavor individualFlavor = flavorTasks[i].Result;
                    Assert.IsNotNull(individualFlavor);

                    Assert.AreEqual(referenceFlavor.Id, individualFlavor.Id);
                    Assert.AreEqual(referenceFlavor.Name, individualFlavor.Name);
                    Assert.AreEqual(referenceFlavor.Href, individualFlavor.Href);
                    Assert.AreEqual(referenceFlavor.Memory, individualFlavor.Memory);
                    Assert.AreEqual(referenceFlavor.Links.Count, individualFlavor.Links.Count);
                    for (int j = 0; j < referenceFlavor.Links.Count; j++)
                    {
                        Assert.AreEqual(referenceFlavor.Links[j].Href, individualFlavor.Links[j].Href);
                        Assert.AreEqual(referenceFlavor.Links[j].Rel, individualFlavor.Links[j].Rel);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestCreateBackup()
        {
            IDatabaseService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                ReadOnlyCollection<DatabaseFlavor> flavors = await provider.ListFlavorsAsync(cancellationTokenSource.Token);
                if (flavors.Count == 0)
                    Assert.Inconclusive("The service did not report any flavors.");

                DatabaseFlavor smallestFlavor = flavors.Where(i => i.Memory.HasValue).OrderBy(i => i.Memory).First();
                string instanceName = CreateRandomDatabaseInstanceName();
                DatabaseInstanceConfiguration configuration = new DatabaseInstanceConfiguration(smallestFlavor.Href, new DatabaseVolumeConfiguration(1), instanceName);
                DatabaseInstance instance = await provider.CreateDatabaseInstanceAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                ReadOnlyCollection<Backup> initialBackups = await provider.ListBackupsForInstanceAsync(instance.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(initialBackups);
                Assert.AreEqual(0, initialBackups.Count);

                string backupName = CreateRandomBackupName();
                string backupDescription = "My backup";
                BackupConfiguration backupConfiguration = new BackupConfiguration(instance.Id, backupName, backupDescription);
                Backup backup = await provider.CreateBackupAsync(backupConfiguration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Assert.AreEqual(BackupStatus.Completed, backup.Status);

                Backup backupCopy = await provider.GetBackupAsync(backup.Id, cancellationTokenSource.Token);
                Assert.AreEqual(backup.Id, backupCopy.Id);

                ReadOnlyCollection<Backup> allBackups = await provider.ListBackupsAsync(cancellationTokenSource.Token);
                ReadOnlyCollection<Backup> instanceBackups = await provider.ListBackupsForInstanceAsync(instance.Id, cancellationTokenSource.Token);
                Assert.IsTrue(allBackups.Count >= instanceBackups.Count);
                Assert.AreEqual(1, instanceBackups.Count);
                Assert.AreEqual(backupName, instanceBackups[0].Name);
                Assert.AreEqual(backupDescription, instanceBackups[0].Description);

                await provider.EnableRootUserAsync(instance.Id, cancellationTokenSource.Token);
                Assert.AreEqual(true, await provider.CheckRootEnabledStatusAsync(instance.Id, cancellationTokenSource.Token));

                await provider.RemoveDatabaseInstanceAsync(instance.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                ReadOnlyCollection<Backup> instanceBackupsAfterRemove = await provider.ListBackupsForInstanceAsync(instance.Id, cancellationTokenSource.Token);
                Assert.AreEqual(instanceBackups.Count, instanceBackupsAfterRemove.Count);

                configuration = new DatabaseInstanceConfiguration(smallestFlavor.Href, new DatabaseVolumeConfiguration(1), instanceName, new RestorePoint(backup.Id));
                instance = await provider.CreateDatabaseInstanceAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Assert.AreEqual(false, await provider.CheckRootEnabledStatusAsync(instance.Id, cancellationTokenSource.Token));

                await provider.RemoveBackupAsync(backup.Id, cancellationTokenSource.Token);

                /* Cleanup
                 */
                await provider.RemoveDatabaseInstanceAsync(instance.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        protected async Task<ReadOnlyCollection<DatabaseInstance>> ListAllDatabaseInstancesAsync(IDatabaseService provider, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<DatabaseInstance>> progress = null)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException("blockSize");

            return await (await provider.ListDatabaseInstancesAsync(null, blockSize, cancellationToken)).GetAllPagesAsync(cancellationToken, progress);
        }

        protected async Task<ReadOnlyCollection<Database>> ListAllDatabasesAsync(IDatabaseService provider, DatabaseInstanceId instanceId, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<Database>> progress = null)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (instanceId == null)
                throw new ArgumentNullException("instanceId");
            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException("blockSize");

            return await (await provider.ListDatabasesAsync(instanceId, null, blockSize, cancellationToken)).GetAllPagesAsync(cancellationToken, progress);
        }

        private TimeSpan TestTimeout(TimeSpan timeout)
        {
            if (Debugger.IsAttached)
                return TimeSpan.FromDays(1);

            return timeout;
        }

        internal static string CreateRandomDatabaseInstanceName()
        {
            return TestDatabaseInstancePrefix + Path.GetRandomFileName();
        }

        internal static DatabaseName CreateRandomDatabaseName()
        {
            return new DatabaseName(Path.GetRandomFileName().Replace('.', '_'));
        }

        internal static UserName CreateRandomUserName(string host)
        {
            if (host == null)
                return new UserName(Path.GetRandomFileName().Replace('.', '_'));
            else
                return new UserName(Path.GetRandomFileName().Replace('.', '_'), host);
        }

        internal static string CreateRandomPassword()
        {
            return Path.GetRandomFileName().Replace('.', '_');
        }

        internal static string CreateRandomBackupName()
        {
            return TestDatabaseBackupPrefix + Path.GetRandomFileName();
        }

        internal static IDatabaseService CreateProvider()
        {
            CloudDatabasesProvider provider = new TestCloudDatabasesProvider(Bootstrapper.Settings.TestIdentity, Bootstrapper.Settings.DefaultRegion, null);
            provider.BeforeAsyncWebRequest +=
                (sender, e) =>
                {
                    Console.Error.WriteLine("{0} (Request) {1} {2}", DateTime.Now, e.Request.Method, e.Request.RequestUri);
                };
            provider.AfterAsyncWebResponse +=
                (sender, e) =>
                {
                    Console.Error.WriteLine("{0} (Result {1}) {2}", DateTime.Now, e.Response.StatusCode, e.Response.ResponseUri);
                };

            return provider;
        }

        protected class TestCloudDatabasesProvider : CloudDatabasesProvider
        {
            public TestCloudDatabasesProvider(CloudIdentity defaultIdentity, string defaultRegion, IIdentityProvider identityProvider)
                : base(defaultIdentity, defaultRegion, identityProvider, null, null)
            {
            }

            protected override byte[] EncodeRequestBodyImpl<TBody>(HttpWebRequest request, TBody body)
            {
                return TestHelpers.EncodeRequestBody(request, body, base.EncodeRequestBodyImpl);
            }

            protected override Tuple<HttpWebResponse, string> ReadResultImpl(Task<WebResponse> task, CancellationToken cancellationToken)
            {
                return TestHelpers.ReadResult(task, cancellationToken, base.ReadResultImpl);
            }
        }
    }
}
