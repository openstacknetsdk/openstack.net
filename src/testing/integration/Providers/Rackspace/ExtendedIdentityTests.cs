using System;
using JSIStudios.SimpleRESTServices.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Core.Caching;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Core.Providers;
using net.openstack.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Objects;

namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    [TestClass]
    public class ExtendedIdentityTests
    {
        private TestContext testContextInstance;
        private static RackspaceCloudIdentity _testIdentity;
        private static RackspaceCloudIdentity _testAdminIdentity;
        private static ExtendedRackspaceCloudIdentity _testDomainIdentity;
        private static User _userDetails;
        private static User _adminUserDetails;
        private const string NewPassword = "My_n3w_p@$$w0rd";
        private const string AdminNewPassword = "My_n3w_@dmin_p@$$w0rd";
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
            _testAdminIdentity = new RackspaceCloudIdentity(Bootstrapper.Settings.TestAdminIdentity);
            _testDomainIdentity = new ExtendedRackspaceCloudIdentity(Bootstrapper.Settings.TestDomainIdentity);

            var provider = BuildProvider();

            _userDetails = provider.GetUserByName(_testIdentity.Username, _testIdentity);
            _adminUserDetails = provider.GetUserByName(_testAdminIdentity.Username, _testAdminIdentity);
        }

        private static IExtendedCloudIdentityProvider BuildProvider(IRestService restService = null, ICache<UserAccess> cache = null)
        {
            Uri baseUri = null;
            if (Bootstrapper.Settings.RackspaceExtendedIdentityUrl != null)
                baseUri = new Uri(Bootstrapper.Settings.RackspaceExtendedIdentityUrl);
            return new CloudIdentityProvider(restService, cache, baseUri);
        }

        [TestMethod]
        public void Test001_Should_Throw_Exception_When_Trying_To_Change_Password_For_Self_When_Changeing_Password_As_Non_Admin_Identity()
        {
            var provider = BuildProvider();

            try
            {
                provider.SetUserPassword(_userDetails, NewPassword, _testIdentity);

                throw new Exception("This code path is invalid, exception was expected.");
            }
            catch(ResponseException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Test002_Should_Throw_Exception_When_Trying_To_Change_Password_For_Other_User_When_Changeing_Password_As_Non_Admin_Identity()
        {
            var provider = BuildProvider();

            try
            {
                provider.SetUserPassword(_adminUserDetails, AdminNewPassword, _testIdentity);

                throw new Exception("This code path is invalid, exception was expected.");
            }
            catch (ResponseException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Test003_Should_Change_Password_For_Self_When_Changeing_Password_As_Admin_Identity()
        {
            var provider = BuildProvider();

            var result = provider.SetUserPassword(_adminUserDetails, AdminNewPassword, _testAdminIdentity);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test004_Should_Successfully_Authenticate_With_New_Password_For_Admin_User()
        {
            var provider = BuildProvider();

            var userAcess =
                provider.Authenticate(new RackspaceCloudIdentity
                {
                    Username = _testAdminIdentity.Username,
                    Password = AdminNewPassword,
                });

            Assert.IsNotNull(userAcess);
        }

        [TestMethod]
        public void Test005_Should_Change_Password_Back_For_Self_When_Changeing_Password_As_Admin_Identity()
        {
            var provider = BuildProvider();

            var result = provider.SetUserPassword(_adminUserDetails, _testAdminIdentity.Password, _testAdminIdentity);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test006_Should_Successfully_Authenticate_With_Old_Password_For_Admin_User()
        {
            var provider = BuildProvider();

            var userAcess =
                provider.Authenticate(_testAdminIdentity);

            Assert.IsNotNull(userAcess);
        }

        [TestMethod]
        public void Test007_Should_Change_Password_For_Other_User_When_Changeing_Password_As_Admin_Identity()
        {
            var provider = BuildProvider();

            var result = provider.SetUserPassword(_adminUserDetails, AdminNewPassword, _testAdminIdentity);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test008_Should_Successfully_Authenticate_With_New_Password_For_Non_Admin_User()
        {
            var provider = BuildProvider();

            var userAcess =
                provider.Authenticate(new RackspaceCloudIdentity
                {
                    Username = _testAdminIdentity.Username,
                    Password = AdminNewPassword,
                });

            Assert.IsNotNull(userAcess);
        }

        [TestMethod]
        public void Test009_Should_Change_Password_Back_For_Other_User_When_Changeing_Password_As_Admin_Identity()
        {
            var provider = BuildProvider();

            var result = provider.SetUserPassword(_adminUserDetails, _testAdminIdentity.Password, _testAdminIdentity);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test010_Should_Successfully_Authenticate_With_Old_Password_For_Non_Admin_User()
        {
            var provider = BuildProvider();

            var userAcess =
                provider.Authenticate(_testAdminIdentity);

            Assert.IsNotNull(userAcess);
        }

        [TestMethod]
        public void Test011_Should_Successfully_Authenticate_Using_Domain_Credentials()
        {
            var provider = BuildProvider();

            var creds = provider.Authenticate(_testDomainIdentity);

            Assert.IsNotNull(creds);
            Assert.IsNotNull(creds.Token);
            Assert.IsNotNull(creds.User);
            Assert.AreEqual(_testDomainIdentity.Username, creds.User.Id);
        }
    }
}
