﻿namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
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
    using HttpStatusCode = System.Net.HttpStatusCode;
    using Path = System.IO.Path;

    /// <summary>
    /// This class contains integration tests for the Rackspace Identity Provider that can
    /// be run with user (non-admin) credentials.
    /// </summary>
    /// <seealso cref="IIdentityProvider"/>
    /// <seealso cref="CloudIdentityProvider"/>
    [TestClass]
    public class UserIdentityTests
    {
        /// <summary>
        /// Users created by these unit tests have a username with this prefix, allowing
        /// them to be identified and cleaned up following a failed test.
        /// </summary>
        private static readonly string TestUserPrefix = "UnitTestUser-";

        /// <summary>
        /// Gets a unique, randomly generated username which can be added by a unit test.
        /// </summary>
        /// <remarks>
        /// The generated username will include the <see cref="TestUserPrefix"/> prefix to
        /// allow for automatic identification of orphaned users which were created by the
        /// integration tests. The <see cref="CleanupUsers"/> test can be executed to
        /// delete orphaned users.
        /// </remarks>
        /// <returns>The username.</returns>
        private static string GenerateUsername()
        {
            return TestUserPrefix + Path.GetRandomFileName();
        }

        /// <summary>
        /// This method tests the basic functionality of the <see cref="IIdentityProvider.Authenticate"/>
        /// method for correct credentials.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestAuthenticate()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();
            UserAccess userAccess = provider.Authenticate();

            Assert.IsNotNull(userAccess);
            Assert.IsNotNull(userAccess.Token);
            Assert.IsNotNull(userAccess.Token.Id);
            Assert.IsNotNull(userAccess.Token.Expires);
            Assert.IsFalse(userAccess.Token.IsExpired);

            Assert.IsNotNull(userAccess.User);
            Assert.IsNotNull(userAccess.User.Id);
            Assert.IsNotNull(userAccess.User.Name);

            Assert.IsNotNull(userAccess.ServiceCatalog);

            Console.Error.WriteLine(JsonConvert.SerializeObject(userAccess, Formatting.Indented));

            Console.WriteLine("Available Services:");
            foreach (ServiceCatalog serviceCatalog in userAccess.ServiceCatalog)
            {
                Console.WriteLine("    {0} ({1})", serviceCatalog.Name, serviceCatalog.Type);
                var regions = serviceCatalog.Endpoints.Select(i => i.Region ?? "global").OrderBy(i => i, StringComparer.OrdinalIgnoreCase);
                Console.WriteLine("        {0}", string.Join(", ", regions));
            }
        }

        /// <summary>
        /// This method tests the basic functionality of the <see cref="IIdentityProvider.ValidateToken"/>
        /// method for a validated token.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestValidateToken()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();
            UserAccess userAccess = provider.Authenticate();

            Assert.IsNotNull(userAccess);
            Assert.IsNotNull(userAccess.Token);
            Assert.IsNotNull(userAccess.Token.Id);

            try
            {
                UserAccess validated = provider.ValidateToken(userAccess.Token.Id);
                Assert.IsNotNull(validated);
                Assert.IsNotNull(validated.Token);
                Assert.AreEqual(userAccess.Token.Id, validated.Token.Id);

                Assert.IsNotNull(validated.User);
                Assert.AreEqual(userAccess.User.Id, validated.User.Id);
                Assert.AreEqual(userAccess.User.Name, validated.User.Name);
                Assert.AreEqual(userAccess.User.DefaultRegion, validated.User.DefaultRegion);
            }
            catch (UserNotAuthorizedException ex)
            {
                if (ex.Response.StatusCode != HttpStatusCode.Forbidden)
                    throw;

                Assert.Inconclusive("The service does not allow this user to access the Validate Token API.");
            }
        }

        /// <summary>
        /// This method tests the basic functionality of the <see cref="IIdentityProvider.ListEndpoints"/>
        /// method for an authenticated user.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestListEndpoints()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();
            UserAccess userAccess = provider.Authenticate();
            Assert.IsNotNull(userAccess);
            Assert.IsNotNull(userAccess.Token);
            Assert.IsNotNull(userAccess.Token.Id);

            try
            {
                IEnumerable<ExtendedEndpoint> endpoints = provider.ListEndpoints(userAccess.Token.Id);
                Console.WriteLine(JsonConvert.SerializeObject(userAccess, Formatting.Indented));
            }
            catch (UserNotAuthorizedException ex)
            {
                if (ex.Response.StatusCode != HttpStatusCode.Forbidden)
                    throw;

                Assert.Inconclusive("The service does not allow this user to access the List Endpoints API.");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestGetToken()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();

            IdentityToken token = provider.GetToken();
            Assert.IsNotNull(token);

            // ensure the provider is caching the access token
            IdentityToken cachedToken = provider.GetToken();
            Assert.AreSame(token, cachedToken);

            // ensure that the provider refreshes the token upon request
            IdentityToken newToken = provider.GetToken(forceCacheRefresh: true);
            Assert.AreNotSame(token, newToken);

            // ensure the the refresh was applied to the cache
            IdentityToken newCachedToken = provider.GetToken();
            Assert.AreSame(newToken, newCachedToken);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestListRoles()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();
            IExtendedCloudIdentityProvider extendedProvider = provider as IExtendedCloudIdentityProvider;
            if (extendedProvider == null)
                Assert.Inconclusive("The current identity provider does not implement {0}", typeof(IExtendedCloudIdentityProvider).Name);

            IEnumerable<Role> roles = extendedProvider.ListRoles(limit: 500);
            Console.WriteLine("Roles:");
            foreach (Role role in roles)
            {
                Console.WriteLine("  Role \"{0}\" (id: {1})", role.Name, role.Id);
                if (!string.IsNullOrEmpty(role.Description))
                    Console.WriteLine("    Description: {0}", role.Description);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestGetRolesByUser()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();

            UserAccess userAccess = provider.GetUserAccess();
            Assert.IsNotNull(userAccess);
            Assert.IsNotNull(userAccess.User);

            IEnumerable<Role> roles = provider.GetRolesByUser(userAccess.User.Id);
            Assert.IsNotNull(roles);
            Assert.IsTrue(roles.Any());

            foreach (Role role in roles)
            {
                Console.WriteLine("Role \"{0}\" (id: {1})", role.Name, role.Id);
                if (!string.IsNullOrEmpty(role.Description))
                    Console.WriteLine("    Description: {0}", role.Description);
            }
        }

        /// <summary>
        /// This method can be used to clean up users created during integration testing.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public void CleanupUsers()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();
            IEnumerable<User> users = provider.ListUsers();
            foreach (User user in users)
            {
                if (string.IsNullOrEmpty(user.Username))
                    continue;

                if (!user.Username.StartsWith(TestUserPrefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                bool deleted = provider.DeleteUser(user.Id);
                Assert.IsTrue(deleted);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestListUsers()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();
            IEnumerable<User> users = provider.ListUsers();
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Any());

            foreach (User user in users)
            {
                Console.WriteLine("User \"{0}\" (id: {1})", user.Username, user.Id);
                if (!user.Enabled)
                    Console.WriteLine("    Disabled");
                if (!string.IsNullOrEmpty(user.Email))
                    Console.WriteLine("    Email: {0}", user.Email);
                if (!string.IsNullOrEmpty(user.DefaultRegion))
                    Console.WriteLine("    Default region: {0}", user.DefaultRegion);
                if (user.DomainId != null)
                    Console.WriteLine("    Domain ID: {0}", user.DomainId);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestGetUserByName()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();
            User user = provider.GetUserByName(Bootstrapper.Settings.TestIdentity.Username);
            Assert.IsNotNull(user);
            Assert.AreEqual(Bootstrapper.Settings.TestIdentity.Username, user.Username);
            Assert.IsNotNull(user.Id);

            Console.WriteLine("User \"{0}\" (id: {1})", user.Username, user.Id);
            if (!user.Enabled)
                Console.WriteLine("    Disabled");
            if (!string.IsNullOrEmpty(user.Email))
                Console.WriteLine("    Email: {0}", user.Email);
            if (!string.IsNullOrEmpty(user.DefaultRegion))
                Console.WriteLine("    Default region: {0}", user.DefaultRegion);
            if (user.DomainId != null)
                Console.WriteLine("    Domain ID: {0}", user.DomainId);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestGetUser()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();
            User userByName = provider.GetUserByName(Bootstrapper.Settings.TestIdentity.Username);
            Assert.IsNotNull(userByName);
            Assert.AreEqual(Bootstrapper.Settings.TestIdentity.Username, userByName.Username);
            Assert.IsNotNull(userByName.Id);

            User user = provider.GetUser(userByName.Id);
            Assert.IsNotNull(user);
            Assert.AreEqual(Bootstrapper.Settings.TestIdentity.Username, user.Username);
            Assert.AreEqual(userByName.Id, user.Id);

            Console.WriteLine("User \"{0}\" (id: {1})", user.Username, user.Id);
            if (!user.Enabled)
                Console.WriteLine("    Disabled");
            if (!string.IsNullOrEmpty(user.Email))
                Console.WriteLine("    Email: {0}", user.Email);
            if (!string.IsNullOrEmpty(user.DefaultRegion))
                Console.WriteLine("    Default region: {0}", user.DefaultRegion);
            if (user.DomainId != null)
                Console.WriteLine("    Domain ID: {0}", user.DomainId);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestResetApiKey()
        {
            string username = GenerateUsername();
            NewUser newUser = new NewUser(username, username + "@example.com");

            CloudIdentityProvider provider = (CloudIdentityProvider)Bootstrapper.CreateIdentityProvider();
            NewUser user = provider.AddUser(newUser, null);

            Assert.IsNotNull(user);
            Assert.AreEqual(username, user.Username);
            Assert.IsFalse(string.IsNullOrEmpty(user.Id));
            Assert.IsFalse(string.IsNullOrEmpty(user.Password));

            UserCredential credentials = provider.ResetApiKey(user.Id);
            Assert.AreEqual(user.Username, credentials.Username);
            Assert.IsNotNull(credentials.APIKey);
            Assert.AreNotEqual(string.Empty, credentials.APIKey);
            Console.WriteLine("API Key: {0}", credentials.APIKey);

            UserCredential credentials2 = provider.ResetApiKey(user.Id);
            Assert.AreEqual(user.Username, credentials2.Username);
            Assert.IsNotNull(credentials2.APIKey);
            Assert.AreNotEqual(string.Empty, credentials2.APIKey);
            Assert.AreNotEqual(credentials.APIKey, credentials2.APIKey);
            Console.WriteLine("API Key: {0}", credentials2.APIKey);

            bool deleted = provider.DeleteUser(user.Id, null);
            Assert.IsTrue(deleted);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestAddUserUpdateUserDeleteUser()
        {
            string username = GenerateUsername();
            NewUser newUser = new NewUser(username, username + "@example.com");

            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();
            NewUser user = provider.AddUser(newUser);

            Assert.IsNotNull(user);
            Assert.AreEqual(username, user.Username);
            Assert.IsFalse(string.IsNullOrEmpty(user.Id));
            Assert.IsFalse(string.IsNullOrEmpty(user.Password));

            IEnumerable<Role> roles = provider.GetRolesByUser(user.Id);
            Console.WriteLine("Roles for the created user:");
            foreach (Role role in roles)
                Console.WriteLine("    {0} ({1}) # {2}", role.Name, role.Id, role.Description);

            Assert.IsTrue(roles.Any(i => string.Equals(i.Name, "identity:default", StringComparison.OrdinalIgnoreCase)));

            try
            {
                // make sure we can't add the same user twice
                provider.AddUser(newUser);
                Assert.Fail("Expected a conflict");
            }
            catch (ServiceConflictException)
            {
                // this makes the most sense
            }
            catch (ResponseException)
            {
                // this is allowed by the interface
            }

            User current = provider.GetUser(user.Id);

            Console.WriteLine();
            Console.WriteLine("Updating email address...");
            Console.WriteLine();

            current.Email = username + "2@example.com";
            User updated = provider.UpdateUser(current);
            Console.WriteLine(JsonConvert.SerializeObject(updated, Formatting.Indented));
            Assert.AreNotSame(current, updated);
            Assert.AreEqual(current.Email, updated.Email);

            Console.WriteLine();
            Console.WriteLine("Updating username...");
            Console.WriteLine();

            current.Username = username + "2";
            updated = provider.UpdateUser(current);
            Console.WriteLine(JsonConvert.SerializeObject(updated, Formatting.Indented));
            Assert.AreNotSame(current, updated);
            Assert.AreEqual(current.Username, updated.Username);

            Console.WriteLine();
            Console.WriteLine("Updating default region...");
            Console.WriteLine();

            current.DefaultRegion = current.DefaultRegion == "SYD" ? "DFW" : "SYD";
            updated = provider.UpdateUser(current);
            Console.WriteLine(JsonConvert.SerializeObject(updated, Formatting.Indented));
            Assert.AreNotSame(current, updated);
            Assert.AreEqual(current.DefaultRegion, updated.DefaultRegion);

            Console.WriteLine();
            Console.WriteLine("Updating enabled (toggling twice)...");
            Console.WriteLine();

            current.Enabled = !current.Enabled;
            updated = provider.UpdateUser(current);
            Console.WriteLine(JsonConvert.SerializeObject(updated, Formatting.Indented));
            Assert.AreNotSame(current, updated);
            Assert.AreEqual(current.Enabled, updated.Enabled);

            current.Enabled = !current.Enabled;
            updated = provider.UpdateUser(current);
            Console.WriteLine(JsonConvert.SerializeObject(updated, Formatting.Indented));
            Assert.AreNotSame(current, updated);
            Assert.AreEqual(current.Enabled, updated.Enabled);

            Assert.IsNotNull(provider.GetUser(user.Id));

            bool deleted = provider.DeleteUser(user.Id);
            Assert.IsTrue(deleted);
            try
            {
                provider.GetUser(user.Id);
                Assert.Fail("Expected an exception");
            }
            catch (ItemNotFoundException)
            {
                // this makes the most sense
            }
            catch (UserNotAuthorizedException)
            {
                // this is allowed by the interface, and some providers report it for security reasons
            }
            catch (ResponseException)
            {
                // this is allowed by the interface
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestListUserCredentials()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();
            User user = provider.GetUserByName(Bootstrapper.Settings.TestIdentity.Username);
            IEnumerable<UserCredential> credentials = provider.ListUserCredentials(user.Id);
            Assert.IsNotNull(credentials);
            Assert.IsTrue(credentials.Any());

            foreach (IGrouping<string, UserCredential> grouping in credentials.GroupBy(i => i.Name))
            {
                Console.WriteLine(grouping.Key);
                foreach (UserCredential credential in grouping)
                    Console.WriteLine("    {0}: {1}", credential.Username, credential.APIKey);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestListTenants()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();
            IEnumerable<Tenant> tenants = provider.ListTenants();
            Assert.IsNotNull(tenants);
            Assert.IsTrue(tenants.Any());

            foreach (Tenant tenant in tenants)
            {
                Console.WriteLine("{0}: {1}", tenant.Name, tenant.Id);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestGetUserAccess()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();

            UserAccess userAccess = provider.GetUserAccess();
            Assert.IsNotNull(userAccess);
            Assert.IsNotNull(userAccess.Token);

            // ensure the provider is caching the access token
            UserAccess cachedUserAccess = provider.GetUserAccess();
            Assert.AreSame(userAccess, cachedUserAccess);

            // ensure that the provider refreshes the userAccess upon request
            UserAccess newUserAccess = provider.GetUserAccess(forceCacheRefresh: true);
            Assert.AreNotSame(userAccess, newUserAccess);

            // ensure the the refresh was applied to the cache
            UserAccess newCachedUserAccess = provider.GetUserAccess();
            Assert.AreSame(newUserAccess, newCachedUserAccess);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestGetUserCredential()
        {
            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider();
            User user = provider.GetUserByName(Bootstrapper.Settings.TestIdentity.Username);
            IEnumerable<UserCredential> credentials = provider.ListUserCredentials(user.Id);
            Assert.IsNotNull(credentials);
            Assert.IsTrue(credentials.Any());

            foreach (UserCredential credential in credentials)
            {
                UserCredential value = provider.GetUserCredential(user.Id, credential.Name);
                Assert.AreEqual(credential.Username, value.Username);
                Assert.AreEqual(credential.Name, value.Name);
                Assert.AreEqual(credential.APIKey, value.APIKey);
            }

            Console.WriteLine(JsonConvert.SerializeObject(credentials, Formatting.Indented));
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public void TestDefaultIdentity()
        {
            CloudIdentity identity = Bootstrapper.Settings.TestIdentity;

            IIdentityProvider provider = Bootstrapper.CreateIdentityProvider(identity);
            Assert.AreEqual(identity, provider.DefaultIdentity);

            IIdentityProvider defaultProvider = Bootstrapper.CreateIdentityProvider();
            Assert.IsNotNull(defaultProvider.DefaultIdentity);
        }
    }
}
