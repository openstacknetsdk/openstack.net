namespace OpenStackNetTests.Live
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenStack.Collections;
    using OpenStack.ObjectModel.JsonHome;
    using OpenStack.Security.Authentication;
    using OpenStack.Services.ContentDelivery.V1;
    using Path = System.IO.Path;

    partial class ContentDeliveryTests
    {
        /// <summary>
        /// This prefix is used for service names created by unit tests, to avoid overwriting services created by other
        /// applications.
        /// </summary>
        private const string TestServicePrefix = "UnitTestService-";

        /// <summary>
        /// This prefix is used for service domains created by unit tests, to avoid overwriting domains created by other
        /// applications.
        /// </summary>
        private const string TestDomainPrefix = "UnitTestDomain-";

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ContentDelivery)]
        public async Task TestGetHome()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IContentDeliveryService service = CreateService();
                using (GetHomeApiCall apiCall = await service.PrepareGetHomeAsync(cancellationToken))
                {
                    Tuple<HttpResponseMessage, HomeDocument> response = await apiCall.SendAsync(cancellationToken);
                    Assert.IsNotNull(response);

                    var responseMessage = response.Item1;
                    Assert.IsNotNull(responseMessage);
                    Assert.AreEqual(HttpStatusCode.OK, responseMessage.StatusCode);

                    HomeDocument homeDocument = response.Item2;
                    CheckHomeDocument(homeDocument);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ContentDelivery)]
        public async Task TestGetHomeExtension()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IContentDeliveryService service = CreateService();
                HomeDocument homeDocument = await service.GetHomeAsync(cancellationToken);
                CheckHomeDocument(homeDocument);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ContentDelivery)]
        public async Task TestPing()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IContentDeliveryService service = CreateService();
                using (PingApiCall apiCall = await service.PreparePingAsync(cancellationToken))
                {
                    Tuple<HttpResponseMessage, string> response = await apiCall.SendAsync(cancellationToken);
                    Assert.IsNotNull(response);

                    var responseMessage = response.Item1;
                    Assert.IsNotNull(responseMessage);
                    Assert.AreEqual(HttpStatusCode.NoContent, responseMessage.StatusCode);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ContentDelivery)]
        public async Task TestPingExtension()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IContentDeliveryService service = CreateService();
                await service.PingAsync(cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ContentDelivery)]
        public async Task TestListFlavors()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IContentDeliveryService service = CreateService();
                using (ListFlavorsApiCall apiCall = await service.PrepareListFlavorsAsync(cancellationToken))
                {
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Flavor>> response = await apiCall.SendAsync(cancellationToken);
                    Assert.IsNotNull(response);

                    var responseMessage = response.Item1;
                    Assert.IsNotNull(responseMessage);
                    Assert.AreEqual(HttpStatusCode.OK, responseMessage.StatusCode);

                    await CheckFlavorsAsync(response.Item2, cancellationToken);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ContentDelivery)]
        public async Task TestListFlavorsExtension()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IContentDeliveryService service = CreateService();
                ReadOnlyCollectionPage<Flavor> flavors = await service.ListFlavorsAsync(cancellationToken);
                Assert.IsNotNull(flavors);

                await CheckFlavorsAsync(flavors, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ContentDelivery)]
        public async Task TestListServices()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IContentDeliveryService service = CreateService();
                using (ListServicesApiCall apiCall = await service.PrepareListServicesAsync(cancellationToken))
                {
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Service>> response = await apiCall.SendAsync(cancellationToken);
                    Assert.IsNotNull(response);

                    var responseMessage = response.Item1;
                    Assert.IsNotNull(responseMessage);
                    Assert.AreEqual(HttpStatusCode.OK, responseMessage.StatusCode);

                    await CheckServicesAsync(response.Item2, cancellationToken);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ContentDelivery)]
        public async Task TestListServicesExtension()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IContentDeliveryService service = CreateService();
                ReadOnlyCollectionPage<Service> services = await service.ListServicesAsync(cancellationToken);
                Assert.IsNotNull(services);

                await CheckServicesAsync(services, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ContentDelivery)]
        public async Task TestAddRemoveService()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(30)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IContentDeliveryService service = CreateService();

                string name = TestServicePrefix + Path.GetRandomFileName();
                FlavorId flavorId = (await service.ListFlavorsAsync(cancellationToken)).First().Id;
                ImmutableArray<ServiceDomain> domains = ImmutableArray.Create(new ServiceDomain(TestDomainPrefix + "cdn.example.com", ServiceProtocol.Http));
                ImmutableArray<ServiceOrigin> origins = ImmutableArray.Create(new ServiceOrigin("rackspace.com", 80, false, default(ImmutableArray<ServiceOriginRule>)));
                ImmutableArray<ServiceCache> caching = default(ImmutableArray<ServiceCache>);
                ImmutableArray<ServiceRestriction> restrictions = default(ImmutableArray<ServiceRestriction>);
                ServiceData serviceData = new ServiceData(name, flavorId, domains, origins, caching, restrictions);

                ServiceId serviceId;
                using (AddServiceApiCall apiCall = await service.PrepareAddServiceAsync(serviceData, cancellationToken))
                {
                    Tuple<HttpResponseMessage, ServiceId> response = await apiCall.SendAsync(cancellationToken);
                    Assert.IsNotNull(response);

                    var responseMessage = response.Item1;
                    Assert.IsNotNull(responseMessage);
                    Assert.AreEqual(HttpStatusCode.Accepted, responseMessage.StatusCode);

                    serviceId = response.Item2;
                    Assert.IsNotNull(serviceId);
                }

                using (RemoveServiceApiCall apiCall = await service.PrepareRemoveServiceAsync(serviceId, cancellationToken))
                {
                    Tuple<HttpResponseMessage, string> response = await apiCall.SendAsync(cancellationToken);
                    Assert.IsNotNull(response);

                    var responseMessage = response.Item1;
                    Assert.IsNotNull(responseMessage);
                    Assert.AreEqual(HttpStatusCode.Accepted, responseMessage.StatusCode);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ContentDelivery)]
        public async Task TestAddRemoveServiceExtension()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IContentDeliveryService service = CreateService();

                string name = TestServicePrefix + Path.GetRandomFileName();
                FlavorId flavorId = (await service.ListFlavorsAsync(cancellationToken)).First().Id;
                ImmutableArray<ServiceDomain> domains = ImmutableArray.Create(new ServiceDomain(TestDomainPrefix + "cdn.example.com", ServiceProtocol.Http));
                ImmutableArray<ServiceOrigin> origins = ImmutableArray.Create(new ServiceOrigin("rackspace.com", 80, false, default(ImmutableArray<ServiceOriginRule>)));
                ImmutableArray<ServiceCache> caching = default(ImmutableArray<ServiceCache>);
                ImmutableArray<ServiceRestriction> restrictions = default(ImmutableArray<ServiceRestriction>);
                ServiceData serviceData = new ServiceData(name, flavorId, domains, origins, caching, restrictions);

                ServiceId serviceId = await service.AddServiceAsync(serviceData, cancellationToken);
                Assert.IsNotNull(serviceId);

                await service.RemoveServiceAsync(serviceId, cancellationToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.ContentDelivery)]
        public async Task TestGetService()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(30)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                IContentDeliveryService service = CreateService();

                string name = TestServicePrefix + Path.GetRandomFileName();
                FlavorId flavorId = (await service.ListFlavorsAsync(cancellationToken)).First().Id;
                ImmutableArray<ServiceDomain> domains = ImmutableArray.Create(new ServiceDomain(TestDomainPrefix + "cdn.example.com", ServiceProtocol.Http));
                ImmutableArray<ServiceOrigin> origins = ImmutableArray.Create(new ServiceOrigin("rackspace.com", 80, false, default(ImmutableArray<ServiceOriginRule>)));
                ImmutableArray<ServiceCache> caching = default(ImmutableArray<ServiceCache>);
                ImmutableArray<ServiceRestriction> restrictions = default(ImmutableArray<ServiceRestriction>);
                ServiceData serviceData = new ServiceData(name, flavorId, domains, origins, caching, restrictions);

                ServiceId serviceId = await service.AddServiceAsync(serviceData, cancellationToken);
                Assert.IsNotNull(serviceId);

                using (GetServiceApiCall apiCall = await service.PrepareGetServiceAsync(serviceId, cancellationToken))
                {
                    Tuple<HttpResponseMessage, Service> response = await apiCall.SendAsync(cancellationToken);
                    Assert.IsNotNull(response);

                    var responseMessage = response.Item1;
                    Assert.IsNotNull(responseMessage);
                    Assert.AreEqual(HttpStatusCode.OK, responseMessage.StatusCode);

                    Service serviceResult = response.Item2;
                    Assert.IsNotNull(serviceResult);
                }

                await service.RemoveServiceAsync(serviceId, cancellationToken);
            }
        }

        private void CheckHomeDocument(HomeDocument homeDocument)
        {
            Assert.IsNotNull(homeDocument);
        }

        private async Task CheckFlavorsAsync(ReadOnlyCollectionPage<Flavor> firstPage, CancellationToken cancellationToken)
        {
            ReadOnlyCollection<Flavor> allFlavors = await firstPage.GetAllPagesAsync(cancellationToken, null);
            Assert.IsNotNull(allFlavors);
            if (allFlavors.Count == 0)
                Assert.Inconclusive("No flavors were provided by the service.");

            foreach (Flavor flavor in allFlavors)
            {
                Assert.IsNotNull(flavor);
                Assert.IsNotNull(flavor.Id);

                //Assert.IsNotNull(flavor.Limits);
                Assert.IsFalse(flavor.Providers.IsDefault);
                Assert.IsTrue(flavor.Providers.Length > 0);
                foreach (FlavorProvider provider in flavor.Providers)
                {
                    Assert.IsNotNull(provider.Provider);
                    Assert.IsFalse(provider.Links.IsDefault);
                    Assert.IsTrue(provider.Links.Length > 0);
                    Assert.IsTrue(provider.Links.Any(i => "provider_url".Equals(i.Relation, StringComparison.OrdinalIgnoreCase)));
                }

                Assert.IsFalse(flavor.Links.IsDefault);
                Assert.IsTrue(flavor.Links.Length > 0);
                Assert.IsTrue(flavor.Links.Any(i => "self".Equals(i.Relation, StringComparison.OrdinalIgnoreCase)));
            }
        }

        private async Task CheckServicesAsync(ReadOnlyCollectionPage<Service> firstPage, CancellationToken cancellationToken)
        {
            ReadOnlyCollection<Service> allServices = await firstPage.GetAllPagesAsync(cancellationToken, null);
            Assert.IsNotNull(allServices);
            if (allServices.Count == 0)
                Assert.Inconclusive("No services were provided by the service.");

            foreach (Service service in allServices)
            {
                Assert.IsNotNull(service);
                Assert.IsNotNull(service.Id);

                Assert.IsFalse(service.Links.IsDefault);
                Assert.IsTrue(service.Links.Length > 0);
                Assert.IsTrue(service.Links.Any(i => "self".Equals(i.Relation, StringComparison.OrdinalIgnoreCase)));
            }
        }

        protected TimeSpan TestTimeout(TimeSpan timeSpan)
        {
            if (Debugger.IsAttached)
                return TimeSpan.FromDays(6);

            return timeSpan;
        }

        private IContentDeliveryService CreateService()
        {
            IAuthenticationService authenticationService = IdentityV2Tests.CreateAuthenticationService(Credentials, logRequests: false);
            return CreateService(authenticationService, Credentials);
        }

        internal static IContentDeliveryService CreateService(IAuthenticationService authenticationService, TestCredentials credentials)
        {
            ContentDeliveryClient client;
            switch (credentials.Vendor)
            {
            case "HP":
                // currently HP does not have a vendor-specific IContentDeliveryService
                goto default;

            case "Rackspace":
                // currently Rackspace does not have a vendor-specific IContentDeliveryService
                goto default;

            case "OpenStack":
            default:
                client = new ContentDeliveryClient(authenticationService, credentials.DefaultRegion, false);
                break;
            }

            TestProxy.ConfigureService(client, credentials.Proxy);
            client.BeforeAsyncWebRequest += TestHelpers.HandleBeforeAsyncWebRequest;
            client.AfterAsyncWebResponse += TestHelpers.HandleAfterAsyncWebResponse;

            return client;
        }
    }
}
