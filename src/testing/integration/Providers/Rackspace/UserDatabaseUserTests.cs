namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Core;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.Databases;
    using CancellationTokenSource = System.Threading.CancellationTokenSource;

    [TestClass]
    public class UserDatabaseUserTests
    {
        private static DatabaseInstance _instance;
        private static DatabaseName _databaseName;
        private static DatabaseName _databaseName2;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            IDatabaseService provider = UserDatabaseTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                DatabaseFlavor[] flavors = provider.ListFlavorsAsync(cancellationTokenSource.Token).Result;
                if (flavors.Length == 0)
                    Assert.Inconclusive("The service did not report any flavors.");

                DatabaseFlavor smallestFlavor = flavors.Where(i => i.Memory.HasValue).OrderBy(i => i.Memory).First();
                string instanceName = UserDatabaseTests.CreateRandomDatabaseInstanceName();
                DatabaseInstanceConfiguration configuration = new DatabaseInstanceConfiguration(smallestFlavor.Href, new DatabaseVolumeConfiguration(1), instanceName);
                _instance = provider.CreateDatabaseInstanceAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null).Result;

                _databaseName = UserDatabaseTests.CreateRandomDatabaseName();
                DatabaseConfiguration databaseConfiguration = new DatabaseConfiguration(_databaseName, null, null);
                provider.CreateDatabaseAsync(_instance.Id, databaseConfiguration, cancellationTokenSource.Token).Wait();

                _databaseName2 = UserDatabaseTests.CreateRandomDatabaseName();
                DatabaseConfiguration databaseConfiguration2 = new DatabaseConfiguration(_databaseName2, null, null);
                provider.CreateDatabaseAsync(_instance.Id, databaseConfiguration2, cancellationTokenSource.Token).Wait();
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            IDatabaseService provider = UserDatabaseTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                provider.RemoveDatabaseInstanceAsync(_instance.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null).Wait();
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            //IComputeProvider provider = Bootstrapper.CreateComputeProvider();
            //Server server = provider.GetDetails(_server.Id);
            //if (server.Status != ServerState.Active)
            //    Assert.Inconclusive("Could not run test because the server is in the '{0}' state (expected '{1}').", server.Status, ServerState.Active);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestCreateUser()
        {
            IDatabaseService provider = UserDatabaseTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                string host = null;
                UserName userName = UserDatabaseTests.CreateRandomUserName(host);
                string password = UserDatabaseTests.CreateRandomPassword();
                UserConfiguration userConfiguration = new UserConfiguration(userName, password);
                await provider.CreateUserAsync(_instance.Id, userConfiguration, cancellationTokenSource.Token);

                await provider.ListDatabaseUsersAsync(_instance.Id, null, null, cancellationTokenSource.Token);

                await provider.RemoveUserAsync(_instance.Id, userName, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestCreateUserWithGlobalHost()
        {
            IDatabaseService provider = UserDatabaseTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                string host = "%";
                UserName userName = UserDatabaseTests.CreateRandomUserName(host);
                string password = UserDatabaseTests.CreateRandomPassword();
                UserConfiguration userConfiguration = new UserConfiguration(userName, password);
                await provider.CreateUserAsync(_instance.Id, userConfiguration, cancellationTokenSource.Token);

                await provider.ListDatabaseUsersAsync(_instance.Id, null, null, cancellationTokenSource.Token);

                await provider.RemoveUserAsync(_instance.Id, userName, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestCreateUserWithSpecificHost()
        {
            IDatabaseService provider = UserDatabaseTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                string host = "10.0.0.1";
                UserName userName = UserDatabaseTests.CreateRandomUserName(host);
                string password = UserDatabaseTests.CreateRandomPassword();
                UserConfiguration userConfiguration = new UserConfiguration(userName, password);
                await provider.CreateUserAsync(_instance.Id, userConfiguration, cancellationTokenSource.Token);

                await provider.ListDatabaseUsersAsync(_instance.Id, null, null, cancellationTokenSource.Token);

                await provider.RemoveUserAsync(_instance.Id, userName, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestCreateUserWithSingleDatabaseAccess()
        {
            IDatabaseService provider = UserDatabaseTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                string host = null;
                UserName userName = UserDatabaseTests.CreateRandomUserName(host);
                string password = UserDatabaseTests.CreateRandomPassword();
                UserConfiguration userConfiguration = new UserConfiguration(userName, password, _databaseName);
                await provider.CreateUserAsync(_instance.Id, userConfiguration, cancellationTokenSource.Token);

                await provider.ListDatabaseUsersAsync(_instance.Id, null, null, cancellationTokenSource.Token);

                await provider.RemoveUserAsync(_instance.Id, userName, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestCreateUserWithMultipleDatabaseAccess()
        {
            IDatabaseService provider = UserDatabaseTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                string host = null;
                UserName userName = UserDatabaseTests.CreateRandomUserName(host);
                string password = UserDatabaseTests.CreateRandomPassword();
                UserConfiguration userConfiguration = new UserConfiguration(userName, password, _databaseName, _databaseName2);
                await provider.CreateUserAsync(_instance.Id, userConfiguration, cancellationTokenSource.Token);

                await provider.ListDatabaseUsersAsync(_instance.Id, null, null, cancellationTokenSource.Token);

                await provider.RemoveUserAsync(_instance.Id, userName, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestListUsers()
        {
            IDatabaseService provider = UserDatabaseTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                string host = null;
                UserName userName = UserDatabaseTests.CreateRandomUserName(host);
                string password = UserDatabaseTests.CreateRandomPassword();
                UserConfiguration userConfiguration = new UserConfiguration(userName, password);
                await provider.CreateUserAsync(_instance.Id, userConfiguration, cancellationTokenSource.Token);

                await provider.ListDatabaseUsersAsync(_instance.Id, null, null, cancellationTokenSource.Token);

                await provider.RemoveUserAsync(_instance.Id, userName, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestSetUserPassword()
        {
            IDatabaseService provider = UserDatabaseTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                string host = null;
                UserName userName = UserDatabaseTests.CreateRandomUserName(host);
                string password = UserDatabaseTests.CreateRandomPassword();
                UserConfiguration userConfiguration = new UserConfiguration(userName, password);
                await provider.CreateUserAsync(_instance.Id, userConfiguration, cancellationTokenSource.Token);

                await provider.SetUserPasswordAsync(_instance.Id, userName, UserDatabaseTests.CreateRandomPassword(), cancellationTokenSource.Token);

                await provider.RemoveUserAsync(_instance.Id, userName, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestUpdateUser()
        {
            IDatabaseService provider = UserDatabaseTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                string host = null;
                UserName userName = UserDatabaseTests.CreateRandomUserName(host);
                string password = UserDatabaseTests.CreateRandomPassword();
                UserConfiguration userConfiguration = new UserConfiguration(userName, password);
                await provider.CreateUserAsync(_instance.Id, userConfiguration, cancellationTokenSource.Token);

                UpdateUserConfiguration updateUserConfiguration = new UpdateUserConfiguration(null, UserDatabaseTests.CreateRandomPassword());
                await provider.UpdateUserAsync(_instance.Id, userName, updateUserConfiguration, cancellationTokenSource.Token);

                await provider.RemoveUserAsync(_instance.Id, userName, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestGetUser()
        {
            IDatabaseService provider = UserDatabaseTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                string host = null;
                UserName userName = UserDatabaseTests.CreateRandomUserName(host);
                string password = UserDatabaseTests.CreateRandomPassword();
                UserConfiguration userConfiguration = new UserConfiguration(userName, password);
                await provider.CreateUserAsync(_instance.Id, userConfiguration, cancellationTokenSource.Token);

                DatabaseUser user = await provider.GetUserAsync(_instance.Id, userName, cancellationTokenSource.Token);

                await provider.RemoveUserAsync(_instance.Id, userName, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Database)]
        public async Task TestUserDatabaseAccess()
        {
            IDatabaseService provider = UserDatabaseTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(600))))
            {
                string host = null;
                UserName userName = UserDatabaseTests.CreateRandomUserName(host);
                string password = UserDatabaseTests.CreateRandomPassword();
                UserConfiguration userConfiguration = new UserConfiguration(userName, password);
                await provider.CreateUserAsync(_instance.Id, userConfiguration, cancellationTokenSource.Token);

                DatabaseName[] accessible;
                accessible = await provider.ListUserAccessAsync(_instance.Id, userName, cancellationTokenSource.Token);

                // grant twice different
                await provider.GrantUserAccessAsync(_instance.Id, _databaseName, userName, cancellationTokenSource.Token);
                await provider.GrantUserAccessAsync(_instance.Id, _databaseName2, userName, cancellationTokenSource.Token);

                accessible = await provider.ListUserAccessAsync(_instance.Id, userName, cancellationTokenSource.Token);

                // revoke twice different
                await provider.RevokeUserAccessAsync(_instance.Id, _databaseName, userName, cancellationTokenSource.Token);
                await provider.RevokeUserAccessAsync(_instance.Id, _databaseName2, userName, cancellationTokenSource.Token);

                accessible = await provider.ListUserAccessAsync(_instance.Id, userName, cancellationTokenSource.Token);

                // grant twice same
                await provider.GrantUserAccessAsync(_instance.Id, _databaseName, userName, cancellationTokenSource.Token);
                await provider.GrantUserAccessAsync(_instance.Id, _databaseName, userName, cancellationTokenSource.Token);

                accessible = await provider.ListUserAccessAsync(_instance.Id, userName, cancellationTokenSource.Token);

                // revoke twice same
                await provider.RevokeUserAccessAsync(_instance.Id, _databaseName, userName, cancellationTokenSource.Token);

                try
                {
                    try
                    {
                        await provider.RevokeUserAccessAsync(_instance.Id, _databaseName, userName, cancellationTokenSource.Token);
                        Assert.Fail("Expected a 404 response.");
                    }
                    catch (WebException webException)
                    {
                        throw new AggregateException(webException);
                    }
                }
                catch (AggregateException ex)
                {
                    ReadOnlyCollection<Exception> innerExceptions = ex.Flatten().InnerExceptions;
                    if (innerExceptions.Count != 1)
                        throw;

                    WebException webException = innerExceptions[0] as WebException;
                    if (webException == null)
                        throw;

                    HttpWebResponse response = webException.Response as HttpWebResponse;
                    if (response == null)
                        throw;

                    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
                }

                accessible = await provider.ListUserAccessAsync(_instance.Id, userName, cancellationTokenSource.Token);

                await provider.RemoveUserAsync(_instance.Id, userName, cancellationTokenSource.Token);
            }
        }

        private static TimeSpan TestTimeout(TimeSpan timeout)
        {
            if (Debugger.IsAttached)
                return TimeSpan.FromDays(1);

            return timeout;
        }
    }
}
