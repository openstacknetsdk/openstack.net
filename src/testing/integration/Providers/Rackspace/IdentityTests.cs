using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;

namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    [TestClass]
    public class IdentityTests
    {
        private TestContext testContextInstance;
        private static RackspaceCloudIdentity _testIdentity;
        private static RackspaceCloudIdentity _testAdminIdentity;
        private static User _userDetails;
        private static User _adminUserDetails;
        private const string NewPassword = "my_new_password";

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
        }

        [TestMethod]
        public void Should_Authenticate_Test_Identity()
        {
            IIdentityProvider serviceProvider = new IdentityProvider();
            var userAccess = serviceProvider.Authenticate(_testIdentity);

            Assert.IsNotNull(userAccess);
        }

        [TestMethod]
        public void Should_Authenticate_Test_Admin_Identity()
        {
            IIdentityProvider serviceProvider = new IdentityProvider();
            var userAccess = serviceProvider.Authenticate(_testAdminIdentity);

            Assert.IsNotNull(userAccess);
        }

        [TestMethod]
        public void Should_Throw_Error_When_Authenticating_With_Invalid_Password()
        {
            var identity = new RackspaceCloudIdentity()
                               {
                                   Username = _testIdentity.Username,
                                   Password = "bad password"
                               };
            IIdentityProvider serviceProvider = new IdentityProvider();

            try
            {
                var userAccess = serviceProvider.Authenticate(identity);

                throw new Exception("This code path is invalid, exception was expected.");
            }
            catch (net.openstack.Core.Exceptions.Response.ResponseException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Should_Throw_Error_When_Authenticating_With_Invalid_API_Key()
        {
            var identity = new RackspaceCloudIdentity()
                               {
                                   Username = _testIdentity.Username,
                                   APIKey = "bad api key"
                               };
            IIdentityProvider serviceProvider = new IdentityProvider();

            try
            {
                var userAccess = serviceProvider.Authenticate(identity);

                throw new Exception("This code path is invalid, exception was expected.");
            }
            catch(net.openstack.Core.Exceptions.Response.ResponseException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Should_Throw_Error_When_Authenticating_With_Invalid_Username()
        {
            var identity = new RackspaceCloudIdentity()
            {
                Username = "I'm a bad bad user",
                APIKey = "bad api key"
            };
            IIdentityProvider serviceProvider = new IdentityProvider();

            try
            {
                var userAccess = serviceProvider.Authenticate(identity);

                throw new Exception("This code path is invalid, exception was expected.");
            }
            catch (net.openstack.Core.Exceptions.Response.ResponseException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Should_List_Only_Self_When_Retrieving_List_Of_Users_With_Non_Admin_Account()
        {
            IIdentityProvider serviceProvider = new IdentityProvider();

            var users = serviceProvider.ListUsers(_testIdentity);

            Assert.IsTrue(users.Count() == 1);
            Assert.AreEqual(_testIdentity.Username, users[0].Username);
        }

        [TestMethod]
        public void Should_List_Multiple_Users_When_Retrieving_List_Of_Users_With_Admin_Account()
        {
            IIdentityProvider serviceProvider = new IdentityProvider();

            var users = serviceProvider.ListUsers(_testAdminIdentity);

            Assert.IsTrue(users.Count() > 1);
        }

        [TestMethod]
        public void Should_List_Details_Of_Self_When_Retrieving_User_By_Name_With_Non_Admin_Account()
        {
            IIdentityProvider serviceProvider = new IdentityProvider();

            _userDetails = serviceProvider.GetUserByName(_testIdentity, _testIdentity.Username);

            Assert.IsNotNull(_userDetails);
            Assert.AreEqual(_testIdentity.Username, _userDetails.Username);
        }

        [TestMethod]
        public void Should_List_Details_Of_Self_When_Retrieving_User_By_Name_With_Admin_Account()
        {
            IIdentityProvider serviceProvider = new IdentityProvider();

            _adminUserDetails = serviceProvider.GetUserByName(_testAdminIdentity, _testAdminIdentity.Username);

            Assert.IsNotNull(_adminUserDetails);
            Assert.AreEqual(_testAdminIdentity.Username, _adminUserDetails.Username);
        }

        [TestMethod]
        public void Should_List_Details_Of_Other_User_When_Retrieving_User_By_Name_With_Admin_Account()
        {
            IIdentityProvider serviceProvider = new IdentityProvider();

            var details = serviceProvider.GetUserByName(_testAdminIdentity, _testIdentity.Username);

            Assert.IsNotNull(details);
            Assert.AreEqual(_testIdentity.Username, details.Username);
        }

        [TestMethod]
        public void Should_Throw_Exception_When_Trying_To_Get_Details_Of_A_Different_User_When_Retrieving_User_By_Name_With_Non_Admin_Account()
        {
            IIdentityProvider serviceProvider = new IdentityProvider();

            try
            {
                var details = serviceProvider.GetUserByName(_testIdentity, _testAdminIdentity.Username);

                throw new Exception("This code path is invalid, exception was expected.");
            }
            catch (net.openstack.Core.Exceptions.Response.ResponseException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Should_List_Details_Of_Self_When_Retrieving_User_By_Id_With_Non_Admin_Account()
        {
            IIdentityProvider serviceProvider = new IdentityProvider();

            _userDetails = serviceProvider.GetUser(_testIdentity, _userDetails.Id);

            Assert.IsNotNull(_userDetails);
            Assert.AreEqual(_testIdentity.Username, _userDetails.Username);
        }

        [TestMethod]
        public void Should_List_Details_Of_Self_When_Retrieving_User_By_Id_With_Admin_Account()
        {
            IIdentityProvider serviceProvider = new IdentityProvider();

            _adminUserDetails = serviceProvider.GetUser(_testAdminIdentity, _adminUserDetails.Id);

            Assert.IsNotNull(_adminUserDetails);
            Assert.AreEqual(_testAdminIdentity.Username, _adminUserDetails.Username);
        }

        [TestMethod]
        public void Should_List_Details_Of_Other_User_When_Retrieving_User_By_Id_With_Admin_Account()
        {
            IIdentityProvider serviceProvider = new IdentityProvider();

            var details = serviceProvider.GetUser(_testAdminIdentity, _userDetails.Id);

            Assert.IsNotNull(details);
            Assert.AreEqual(_testIdentity.Username, details.Username);
        }

        [TestMethod]
        public void Should_Throw_Exception_When_Trying_To_Get_Details_Of_A_Different_User_When_Retrieving_User_By_Id_With_Non_Admin_Account()
        {
            IIdentityProvider serviceProvider = new IdentityProvider();

            try
            {
                var details = serviceProvider.GetUser(_testIdentity, _adminUserDetails.Id);

                throw new Exception("This code path is invalid, exception was expected.");
            }
            catch (net.openstack.Core.Exceptions.Response.ResponseException)
            {
                Assert.IsTrue(true);
            }
        }
    }
}
