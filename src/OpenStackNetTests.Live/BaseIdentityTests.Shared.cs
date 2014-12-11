namespace OpenStackNetTests.Live
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenStack.Collections;
    using OpenStack.Services.Identity;
    using Rackspace.Services.Identity.V2;

    partial class BaseIdentityTests
    {
        protected Uri BaseAddress
        {
            get
            {
                TestCredentials credentials = Credentials;
                if (credentials == null)
                    return null;

                return credentials.BaseAddress;
            }
        }

        protected TestProxy Proxy
        {
            get
            {
                TestCredentials credentials = Credentials;
                if (credentials == null)
                    return null;

                return credentials.Proxy;
            }
        }

        protected string Vendor
        {
            get
            {
                TestCredentials credentials = Credentials;
                if (credentials == null)
                    return "OpenStack";

                return credentials.Vendor;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        [TestCategory(TestCategories.TestKind)]
        public async Task TestListApiVersions()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (IBaseIdentityService service = CreateService())
                {
                    ListApiVersionsApiCall apiCall = await service.PrepareListApiVersionsAsync(cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<ApiVersion>> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item1);

                    ReadOnlyCollectionPage<ApiVersion> versions = response.Item2;
                    Assert.IsNotNull(versions);
                    Assert.AreNotEqual(0, versions.Count);
                    Assert.IsFalse(versions.CanHaveNextPage);
                    Assert.IsFalse(versions.Contains(null));

                    foreach (ApiVersion version in versions)
                    {
                        Assert.IsNotNull(version);
                        Assert.IsNotNull(version.Id);
                        Assert.IsNotNull(version.LastModified);
                        Assert.IsFalse(version.MediaTypes.IsDefault);
                        Assert.IsFalse(version.Links.IsDefault);
                        Assert.IsNotNull(version.Status);

                        Assert.AreNotEqual(0, version.MediaTypes.Length);
                        foreach (MediaType mediaType in version.MediaTypes)
                        {
                            Assert.IsNotNull(mediaType);
                            Assert.IsNotNull(mediaType.Base);
                            Assert.IsNotNull(mediaType.Type);
                        }

                        Assert.AreNotEqual(0, version.Links.Length);
                        foreach (Link link in version.Links)
                        {
                            Assert.IsNotNull(link);
                            Assert.IsNotNull(link.Target);
                            Assert.IsNotNull(link.Relation);
                            Assert.IsTrue(link.Target.IsAbsoluteUri);
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        [TestCategory(TestCategories.TestKind)]
        public async Task TestListApiVersionsSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (IBaseIdentityService service = CreateService())
                {
                    // test using the simple extension method
                    ReadOnlyCollectionPage<ApiVersion> versions = await service.ListApiVersionsAsync(cancellationTokenSource.Token);
                    Assert.IsNotNull(versions);
                    Assert.AreNotEqual(0, versions.Count);
                    Assert.IsFalse(versions.CanHaveNextPage);
                    Assert.IsFalse(versions.Contains(null));
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        [TestCategory(TestCategories.TestKind)]
        public async Task TestGetApiVersion2()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (IBaseIdentityService service = CreateService())
                {
                    ApiVersionId version2 = new ApiVersionId("v2.0");

                    // test using the simple extension method
                    ApiVersion version = await service.GetApiVersionAsync(version2, cancellationTokenSource.Token);
                    Assert.IsNotNull(version);
                    Assert.AreEqual(version2, version.Id);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
        public async Task TestGetApiVersion3()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (IBaseIdentityService service = CreateService())
                {
                    ApiVersionId version3 = new ApiVersionId("v3");

                    // test using the simple extension method
                    ApiVersion version = await service.GetApiVersionAsync(version3, cancellationTokenSource.Token);
                    Assert.IsNotNull(version);
                    Assert.AreEqual(version3, version.Id);
                }
            }
        }

        protected TimeSpan TestTimeout(TimeSpan timeSpan)
        {
            if (Debugger.IsAttached)
                return TimeSpan.FromDays(6);

            return timeSpan;
        }

        protected IBaseIdentityService CreateService()
        {
            BaseIdentityClient client;
            switch (Vendor)
            {
            case "HP":
                // currently HP does not have a vendor-specific IBaseIdentityService
                goto default;

            case "Rackspace":
                client = new RackspaceIdentityClient(BaseAddress);
                break;

            case "OpenStack":
            default:
                client = new BaseIdentityClient(BaseAddress);
                break;
            }

            TestProxy.ConfigureService(client, Proxy);
            client.BeforeAsyncWebRequest += TestHelpers.HandleBeforeAsyncWebRequest;
            client.AfterAsyncWebResponse += TestHelpers.HandleAfterAsyncWebResponse;

            return client;
        }
    }
}
