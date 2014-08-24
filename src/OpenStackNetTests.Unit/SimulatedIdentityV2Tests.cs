namespace OpenStackNetTests.Unit
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using OpenStack.Collections;
    using OpenStack.Security.Authentication;
    using OpenStack.Services.Identity;
    using OpenStack.Services.Identity.V2;
    using OpenStackNetTests.Unit.Simulator.IdentityService.V2;
    using TestHelpers = OpenStackNetTests.Live.TestHelpers;

    [TestClass]
    public class SimulatedIdentityV2Tests
    {
        private SimulatedIdentityService _simulator;

        protected Uri BaseAddress
        {
            get
            {
                return new Uri("http://localhost:5000");
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _simulator = new SimulatedIdentityService(5000);
            _simulator.StartAsync(CancellationToken.None);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _simulator.Dispose();
            _simulator = null;
        }

        [TestMethod]
        public async Task TestListExtensions()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (IIdentityService service = CreateService())
                {
                    ListExtensionsApiCall apiCall = await service.PrepareListExtensionsAsync(cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Extension>> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    ReadOnlyCollectionPage<Extension> extensions = response.Item2;
                    Assert.IsNotNull(extensions);
                    Assert.AreNotEqual(0, extensions.Count);
                    Assert.IsFalse(extensions.CanHaveNextPage);

                    foreach (Extension extension in extensions)
                        CheckExtension(extension);
                }
            }
        }

        [TestMethod]
        public async Task TestListExtensionsSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (IIdentityService service = CreateService())
                {
                    ReadOnlyCollectionPage<Extension> extensions = await service.ListExtensionsAsync(cancellationTokenSource.Token);
                    Assert.IsNotNull(extensions);
                    Assert.AreNotEqual(0, extensions.Count);
                    Assert.IsFalse(extensions.CanHaveNextPage);

                    foreach (Extension extension in extensions)
                        CheckExtension(extension);
                }
            }
        }

        [TestMethod]
        public async Task TestGetExtension()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (IIdentityService service = CreateService())
                {
                    ListExtensionsApiCall apiCall = await service.PrepareListExtensionsAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Extension>> response = await apiCall.SendAsync(cancellationTokenSource.Token).ConfigureAwait(false);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    ReadOnlyCollectionPage<Extension> extensions = response.Item2;
                    Assert.IsNotNull(extensions);
                    Assert.AreNotEqual(0, extensions.Count);
                    Assert.IsFalse(extensions.CanHaveNextPage);

                    foreach (Extension listedExtension in extensions)
                    {
                        Assert.IsNotNull(listedExtension);
                        Assert.IsNotNull(listedExtension.Alias);

                        GetExtensionApiCall getApiCall = await service.PrepareGetExtensionAsync(listedExtension.Alias, cancellationTokenSource.Token).ConfigureAwait(false);
                        Tuple<HttpResponseMessage, ExtensionResponse> getResponse = await getApiCall.SendAsync(cancellationTokenSource.Token).ConfigureAwait(false);

                        Extension extension = getResponse.Item2.Extension;
                        CheckExtension(extension);
                        Assert.AreEqual(listedExtension.Alias, extension.Alias);
                    }
                }
            }
        }

        [TestMethod]
        public async Task TestGetExtensionSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (IIdentityService service = CreateService())
                {
                    ReadOnlyCollectionPage<Extension> extensions = await service.ListExtensionsAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(extensions);
                    Assert.AreNotEqual(0, extensions.Count);
                    Assert.IsFalse(extensions.CanHaveNextPage);

                    foreach (Extension listedExtension in extensions)
                    {
                        Assert.IsNotNull(listedExtension);
                        Assert.IsNotNull(listedExtension.Alias);

                        Extension extension = await service.GetExtensionAsync(listedExtension.Alias, cancellationTokenSource.Token);
                        CheckExtension(extension);
                        Assert.AreEqual(listedExtension.Alias, extension.Alias);
                    }
                }
            }
        }

        [TestMethod]
        public async Task TestAuthenticate()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (IIdentityService service = CreateService())
                {
                    string tenantName = "simulated_tenant";
                    string username = "simulated_user";
                    string password = "simulated_password";
                    PasswordCredentials passwordCredentials = new PasswordCredentials(username, password);
                    AuthenticationData auth = new AuthenticationData(tenantName, passwordCredentials);
                    AuthenticationRequest request = new AuthenticationRequest(auth);

                    AuthenticateApiCall apiCall = await service.PrepareAuthenticateAsync(request, cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, AccessResponse> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    Access access = response.Item2.Access;
                    Assert.IsNotNull(access);
                    Assert.IsNotNull(access.Token);
                    Assert.IsNotNull(access.ServiceCatalog);
                    Assert.IsNotNull(access.User);

                    // check the token
                    Token token = access.Token;
                    Assert.IsNotNull(token);
                    Assert.IsNotNull(token.Id);
                    Assert.IsNotNull(token.Tenant);
                    Assert.IsNotNull(token.IssuedAt);
                    Assert.IsNotNull(token.ExpiresAt);

                    // check the user
                    User user = access.User;
                    Assert.IsNotNull(user);
                    Assert.IsNotNull(user.Id);
                    Assert.IsNotNull(user.Name);
                    Assert.IsNotNull(user.Username);
                    Assert.IsNotNull(user.Roles);
                    Assert.IsNotNull(user.RolesLinks);

                    Assert.AreNotEqual(0, user.Roles.Count);
                    foreach (Role role in user.Roles)
                    {
                        Assert.IsNotNull(role);
                        Assert.IsNotNull(role.Name);
                        Assert.AreNotEqual(string.Empty, role.Name);
                    }

                    foreach (Link link in user.RolesLinks)
                    {
                        Assert.IsNotNull(link);
                        Assert.IsNotNull(link.Relation);
                        Assert.AreNotEqual(string.Empty, link.Relation);
                        Assert.IsNotNull(link.Target);
                        Assert.IsTrue(link.Target.IsAbsoluteUri);
                    }

                    // check the service catalog
                    ReadOnlyCollection<ServiceCatalogEntry> serviceCatalog = access.ServiceCatalog;
                    Assert.IsNotNull(serviceCatalog);
                    Assert.AreNotEqual(0, serviceCatalog.Count);
                    foreach (ServiceCatalogEntry entry in serviceCatalog)
                    {
                        Assert.IsNotNull(entry);
                        Assert.IsNotNull(entry.Name);
                        Assert.IsNotNull(entry.Type);
                        Assert.IsNotNull(entry.Endpoints);
                        Assert.IsNotNull(entry.EndpointsLinks);

                        Assert.AreNotEqual(0, entry.Endpoints.Count);
                        foreach (Endpoint endpoint in entry.Endpoints)
                        {
                            Assert.IsNotNull(endpoint);
                            Assert.IsNotNull(endpoint.Id);
                            Assert.IsNotNull(endpoint.Region);
                            Assert.IsFalse(endpoint.PublicUrl == null && endpoint.InternalUrl == null && endpoint.AdminUrl == null);
                        }

                        foreach (Link link in entry.EndpointsLinks)
                        {
                            Assert.IsNotNull(link);
                            Assert.IsNotNull(link.Relation);
                            Assert.AreNotEqual(string.Empty, link.Relation);
                            Assert.IsNotNull(link.Target);
                            Assert.IsTrue(link.Target.IsAbsoluteUri);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public async Task TestListTenants()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                string tenantName = "simulated_tenant";
                string username = "simulated_user";
                string password = "simulated_password";
                PasswordCredentials passwordCredentials = new PasswordCredentials(username, password);
                AuthenticationData auth = new AuthenticationData(tenantName, passwordCredentials);
                AuthenticationRequest request = new AuthenticationRequest(auth);

                IdentityV2AuthenticationService authenticationService = new IdentityV2AuthenticationService(CreateService(), request);

                using (IIdentityService service = CreateService(authenticationService))
                {
                    ListTenantsApiCall apiCall = await service.PrepareListTenantsAsync(cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Tenant>> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    ReadOnlyCollectionPage<Tenant> tenants = response.Item2;
                    Assert.IsNotNull(tenants);
                    Assert.AreNotEqual(0, tenants.Count);
                    Assert.IsFalse(tenants.CanHaveNextPage);

                    foreach (Tenant tenant in tenants)
                        CheckTenant(tenant);
                }
            }
        }

        protected void CheckExtension(Extension extension)
        {
            Assert.IsNotNull(extension);
            Assert.IsNotNull(extension.Alias);
            Assert.IsNotNull(extension.Description);
            Assert.IsNotNull(extension.Name);
            Assert.IsNotNull(extension.Namespace);
            try
            {
                Assert.IsNotNull(extension.LastModified);
            }
            catch (JsonException)
            {
                // OpenStack is known to return an unrecognized (and undefined) timestamp format for this
                // property...
            }
        }

        protected void CheckTenant(Tenant tenant)
        {
            Assert.IsNotNull(tenant);
            Assert.IsNotNull(tenant.Id);
            Assert.IsNotNull(tenant.Name);
            Assert.IsFalse(string.IsNullOrEmpty(tenant.Name));
            Assert.IsNotNull(tenant.Description);
            Assert.IsFalse(string.IsNullOrEmpty(tenant.Description));
            Assert.AreEqual(true, tenant.Enabled);
        }

        protected TimeSpan TestTimeout(TimeSpan timeSpan)
        {
            if (Debugger.IsAttached)
                return TimeSpan.FromDays(6);

            return timeSpan;
        }

        protected IIdentityService CreateService()
        {
            IdentityClient client = new IdentityClient(BaseAddress);
            client.BeforeAsyncWebRequest += TestHelpers.HandleBeforeAsyncWebRequest;
            client.AfterAsyncWebResponse += TestHelpers.HandleAfterAsyncWebResponse;

            return client;
        }

        protected IIdentityService CreateService(IAuthenticationService authenticationService)
        {
            IdentityClient client = new IdentityClient(authenticationService, BaseAddress);
            client.BeforeAsyncWebRequest += TestHelpers.HandleBeforeAsyncWebRequest;
            client.AfterAsyncWebResponse += TestHelpers.HandleAfterAsyncWebResponse;

            return client;
        }
    }
}
