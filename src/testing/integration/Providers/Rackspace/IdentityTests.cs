using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Core.Domain;
using net.openstack.Core.Providers;
using net.openstack.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Objects;

namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    [TestClass]
    public class IdentityTests
    {
        private TestContext testContextInstance;
        private static ExtendedRackspaceCloudIdentity _testIdentity;
        private static ExtendedRackspaceCloudIdentity _testAdminIdentity;
        private static User _userDetails;
        private static User _adminUserDetails;
        private static User _testUser;
        private static string _newTestUserPassword;
        private const string NewUserPassword = "My_n3wuser2_p@$$ssw0rd";

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
            _testIdentity = new ExtendedRackspaceCloudIdentity(Bootstrapper.Settings.TestIdentity);
            _testAdminIdentity = new ExtendedRackspaceCloudIdentity(Bootstrapper.Settings.TestAdminIdentity);
           
        }

        [TestMethod]
        public void Should_Authenticate_Test_Identity()
        {
            IIdentityProvider serviceProvider = new CloudIdentityProvider(_testIdentity);
            var userAccess = serviceProvider.Authenticate();

            Assert.IsNotNull(userAccess);
        }

        [TestMethod]
        public void Should_Authenticate_Test_Admin_Identity()
        {
            IIdentityProvider serviceProvider = new CloudIdentityProvider(_testAdminIdentity);
            var userAccess = serviceProvider.Authenticate();

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
            IIdentityProvider serviceProvider = new CloudIdentityProvider(identity);

            try
            {
                var userAccess = serviceProvider.Authenticate();

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
            IIdentityProvider serviceProvider = new CloudIdentityProvider(identity);

            try
            {
                var userAccess = serviceProvider.Authenticate();

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
            IIdentityProvider serviceProvider = new CloudIdentityProvider(identity);

            try
            {
                var userAccess = serviceProvider.Authenticate();

                throw new Exception("This code path is invalid, exception was expected.");
            }
            catch (net.openstack.Core.Exceptions.Response.ResponseException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Should_List_Only_User_In_Account_When_Retrieving_List_Of_Users_With_User_Admin_Account()
        {
            IIdentityProvider serviceProvider = new CloudIdentityProvider(_testIdentity);

            var users = serviceProvider.ListUsers();

            Assert.IsTrue(users.Any());
            Assert.AreEqual(_testIdentity.Username, users.First().Username);
        }

        [TestMethod]
        public void Should_List_Multiple_Users_When_Retrieving_List_Of_Users_With_Admin_Account()
        {
            IIdentityProvider serviceProvider = new CloudIdentityProvider(_testAdminIdentity);

            var users = serviceProvider.ListUsers();

            Assert.IsTrue(users.Any());
        }

        [TestMethod]
        public void Should_List_Details_Of_Self_When_Retrieving_User_By_Name_With_Non_Admin_Account()
        {
            IIdentityProvider serviceProvider = new CloudIdentityProvider(_testIdentity);

            _userDetails = serviceProvider.GetUserByName(_testIdentity.Username);

            Assert.IsNotNull(_userDetails);
            Assert.AreEqual(_testIdentity.Username, _userDetails.Username);
        }

        [TestMethod]
        public void Should_List_Details_Of_Self_When_Retrieving_User_By_Name_With_Admin_Account()
        {
            IIdentityProvider serviceProvider = new CloudIdentityProvider(_testAdminIdentity);

            _adminUserDetails = serviceProvider.GetUserByName(_testAdminIdentity.Username);

            Assert.IsNotNull(_adminUserDetails);
            Assert.AreEqual(_testAdminIdentity.Username, _adminUserDetails.Username);
        }

        [TestMethod]
        public void Should_List_Details_Of_Other_User_When_Retrieving_User_By_Name_With_Admin_Account()
        {
            IIdentityProvider serviceProvider = new CloudIdentityProvider(_testAdminIdentity);

            var details = serviceProvider.GetUserByName(_testIdentity.Username);

            Assert.IsNotNull(details);
            Assert.AreEqual(_testIdentity.Username, details.Username);
        }

        [TestMethod]
        public void Should_Throw_Exception_When_Trying_To_Get_Details_Of_A_Different_User_When_Retrieving_User_By_Name_With_Non_Admin_Account()
        {
            IIdentityProvider serviceProvider = new CloudIdentityProvider(_testIdentity);

            try
            {
                var details = serviceProvider.GetUserByName(_testAdminIdentity.Username);

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
            IIdentityProvider serviceProvider = new CloudIdentityProvider(_testIdentity);

            _userDetails = serviceProvider.GetUser(_userDetails.Id);

            Assert.IsNotNull(_userDetails);
            Assert.AreEqual(_testIdentity.Username, _userDetails.Username);
        }

        [TestMethod]
        public void Should_List_Details_Of_Self_When_Retrieving_User_By_Id_With_Admin_Account()
        {
            IIdentityProvider serviceProvider = new CloudIdentityProvider(_testAdminIdentity);

            _adminUserDetails = serviceProvider.GetUser(_adminUserDetails.Id);

            Assert.IsNotNull(_adminUserDetails);
            Assert.AreEqual(_testAdminIdentity.Username, _adminUserDetails.Username);
        }

        [TestMethod]
        public void Should_List_Details_Of_Other_User_When_Retrieving_User_By_Id_With_Admin_Account()
        {
            IIdentityProvider serviceProvider = new CloudIdentityProvider(_testAdminIdentity);

            var details = serviceProvider.GetUser(_userDetails.Id);

            Assert.IsNotNull(details);
            Assert.AreEqual(_testIdentity.Username, details.Username);
        }

        [TestMethod]
        public void Should_Throw_Exception_When_Trying_To_Get_Details_Of_A_Different_User_When_Retrieving_User_By_Id_With_Non_Admin_Account()
        {
            IIdentityProvider serviceProvider = new CloudIdentityProvider(_testIdentity);

            try
            {
                var details = serviceProvider.GetUser(_adminUserDetails.Id);

                throw new Exception("This code path is invalid, exception was expected.");
            }
            catch (net.openstack.Core.Exceptions.Response.ResponseException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Should_Add_New_User_1_Without_Specifying_A_Password_Or_Default_Region_To_Account_When_Requesting_As_User_Admin()
        {
            IIdentityProvider provider = new CloudIdentityProvider(_testIdentity);

            var newTestUser = provider.AddUser(new NewUser { Username = "openstacknettestuser1", Email = "newuser@me.com", Enabled = true });

            _newTestUserPassword = newTestUser.Password;
            Assert.IsNotNull(newTestUser);
            Assert.AreEqual("openstacknettestuser1", newTestUser.Username);
            Assert.AreEqual("newuser@me.com", newTestUser.Email);
            Assert.AreEqual(true, newTestUser.Enabled);
            Assert.IsFalse(string.IsNullOrWhiteSpace(newTestUser.Password));
        }

        [TestMethod]
        public void Should_Retrieve_New_User_1_As_User_Admin()
        {
            IIdentityProvider provider = new CloudIdentityProvider(_testAdminIdentity);

            _testUser = provider.GetUserByName("openstacknettestuser1");

            Assert.IsNotNull(_testUser);
            Assert.AreEqual("openstacknettestuser1", _testUser.Username);
            Assert.AreEqual("newuser@me.com", _testUser.Email);
            Assert.AreEqual(true, _testUser.Enabled);
        }

        [TestMethod]
        public void Should_Authenticate_NewUser()
        {
            Assert.IsNotNull(_testUser);

            IIdentityProvider provider = new CloudIdentityProvider();

            var userAccess =
                provider.Authenticate(new RackspaceCloudIdentity
                                          {Username = _testUser.Username, Password = _newTestUserPassword});

            Assert.IsNotNull(userAccess);
        }

        [TestMethod]
        public void Should_Update_NewUser_Username_And_Email_When_Requesting_As_User_Admin()
        {
            IIdentityProvider provider = new CloudIdentityProvider(_testIdentity);

            var user = new User
                           {
                               Id = _testUser.Id,
                               Username = "openstacknettestuser12",
                               Email = "newuser2@me.com",
                               Enabled = true
                           };
            var updatedUser = provider.UpdateUser(user);

            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("openstacknettestuser12", updatedUser.Username);
            Assert.AreEqual("newuser2@me.com", updatedUser.Email);
            Assert.AreEqual(true, updatedUser.Enabled);
            Assert.IsTrue(string.IsNullOrWhiteSpace(updatedUser.DefaultRegion));
        }

        [TestMethod]
        public void Should_Delete_NewUser_When_Requesting_As_User_Admin()
        {
            IIdentityProvider provider = new CloudIdentityProvider(_testIdentity);

            var response = provider.DeleteUser(_testUser.Id);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Should_Throw_Exception_When_Requesting_The_NewUser_After_It_Has_Been_Deleted_When_Requesting_As_User_Admin()
        {
            IIdentityProvider provider = new CloudIdentityProvider(_testIdentity);

            try
            {
                provider.GetUser(_testUser.Id);

                throw new Exception("This code path is invalid, exception was expected.");
            }
            catch(Exception )
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Should_Add_New_User_2_With_Specifying_A_Password_But_Not_Default_Region_To_Account_When_Requesting_As_User_Admin()
        {
            IIdentityProvider provider = new CloudIdentityProvider(_testIdentity);

            var newUser = provider.AddUser(new NewUser { Username = "openstacknettestuser2", Email = "newuser2@me.com", Enabled = true, Password = NewUserPassword });
            _newTestUserPassword = newUser.Password;

            Assert.IsNotNull(newUser);
            Assert.AreEqual("openstacknettestuser2", newUser.Username);
            Assert.AreEqual("newuser2@me.com", newUser.Email);
            Assert.AreEqual(true, newUser.Enabled);
            Assert.AreEqual(NewUserPassword, newUser.Password);
            Assert.IsFalse(string.IsNullOrWhiteSpace(newUser.Password));
        }

        [TestMethod]
        public void Should_Retrieve_New_User_2_As_User_Admin()
        {
            IIdentityProvider provider = new CloudIdentityProvider(_testAdminIdentity);

            _testUser = provider.GetUserByName("openstacknettestuser2");

            Assert.IsNotNull(_testUser);
            Assert.AreEqual("openstacknettestuser2", _testUser.Username);
            Assert.AreEqual("newuser2@me.com", _testUser.Email);
            Assert.AreEqual(true, _testUser.Enabled);
        }

        [TestMethod]
        public void Should_Update_NewUser_Username_And_Email_And_Default_Region_When_Requesting_As_User_Admin()
        {
            IIdentityProvider provider = new CloudIdentityProvider(_testIdentity);

            var user = new User
            {
                Id = _testUser.Id,
                Username = "openstacknettestuser32",
                Email = "newuser32@me.com",
                Enabled = true,
                DefaultRegion = "DFW"
            };
            var updatedUser = provider.UpdateUser(user);

            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("openstacknettestuser32", updatedUser.Username);
            Assert.AreEqual("newuser32@me.com", updatedUser.Email);
            Assert.AreEqual(true, updatedUser.Enabled);
            Assert.AreEqual("DFW", updatedUser.DefaultRegion);
        }

        [TestMethod]
        public void Should_Get_NewUser_When_Requesting_As_Self()
        {
            IIdentityProvider provider = new CloudIdentityProvider(new RackspaceCloudIdentity { Username = _testUser.Username, Password = _newTestUserPassword });

            var user = provider.GetUser(_testUser.Id);

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void Should_Update_NewUser_Username_And_Email_When_Requesting_As_Self()
        {
            IIdentityProvider provider = new CloudIdentityProvider(new RackspaceCloudIdentity { Username = _testUser.Username, Password = _newTestUserPassword });

            var user = new User
            {
                Id = _testUser.Id,
                Username = "openstacknettestuser42",
                Email = "newuser42@me.com",
                Enabled = true,
            };
            var updatedUser = provider.UpdateUser(user);

            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("openstacknettestuser42", updatedUser.Username);
            Assert.AreEqual("newuser42@me.com", updatedUser.Email);
            Assert.AreEqual(true, updatedUser.Enabled);
        }

        [TestMethod]
        public void Should_List_Only_Self_When_Retrieving_List_Of_Users_With_Non_Admin_Account()
        {
            IIdentityProvider serviceProvider = new CloudIdentityProvider(new RackspaceCloudIdentity { Username = _testUser.Username, Password = _newTestUserPassword });

            var users = serviceProvider.ListUsers();

            Assert.IsTrue(users.Count() == 1);
            Assert.AreEqual(_testUser.Username, users.First().Username);
        }

        [TestMethod]
        public void Should_Return_The_Users_Tenant_When_Requesting_As_Non_Admin()
        {
            IIdentityProvider provider = new CloudIdentityProvider(_testIdentity);

            var tenants = provider.ListTenants();

            Assert.IsTrue(tenants.Any());

            if(!string.IsNullOrWhiteSpace(_testIdentity.TenantId))
            {
                Assert.IsTrue(tenants.Any(t => t.Id == _testIdentity.TenantId));
            }
        }

        [TestMethod]
        public void Should_Return_List_Of_Users_Credentials_When_Requesting_As_Non_Admin()
        {
            IIdentityProvider provider = new CloudIdentityProvider(_testIdentity);

            var creds = provider.ListUserCredentials(_userDetails.Id);

            Assert.IsNotNull(creds);
            Assert.IsTrue(creds.Any());

            foreach (var cred in creds)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(cred.Name));
                Assert.IsFalse(string.IsNullOrWhiteSpace(cred.APIKey));
                Assert.IsFalse(string.IsNullOrWhiteSpace(cred.Username));
            }
        }

        [TestMethod]
        public void Should_Return_User_API_Credential_When_Requesting_As_Non_Admin()
        {
            IIdentityProvider provider = new CloudIdentityProvider(_testIdentity);

            var cred = provider.GetUserCredential(_userDetails.Id, "RAX-KSKEY:apiKeyCredentials");

            Assert.IsNotNull(cred);
            Assert.AreEqual("RAX-KSKEY:apiKeyCredentials", cred.Name);
            Assert.IsFalse(string.IsNullOrWhiteSpace(cred.APIKey));
            Assert.IsFalse(string.IsNullOrWhiteSpace(cred.Username));
        }
    }
}
