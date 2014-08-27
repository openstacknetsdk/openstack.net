namespace OpenStackNetTests.Unit
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenStack.Collections;
    using OpenStack.Services.Identity;
    using SimulatedBaseIdentityService = OpenStackNetTests.Unit.Simulator.IdentityService.SimulatedBaseIdentityService;
    using TestHelpers = OpenStackNetTests.Live.TestHelpers;

    [TestClass]
    public class SimulatedBaseIdentityTests
    {
        private SimulatedBaseIdentityService _simulator;

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
            _simulator = new SimulatedBaseIdentityService(5000);
            _simulator.StartAsync(CancellationToken.None);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _simulator.Dispose();
            _simulator = null;
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Identity)]
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
                        Assert.IsNotNull(version.MediaTypes);
                        Assert.IsNotNull(version.Links);
                        Assert.IsNotNull(version.Status);

                        Assert.AreNotEqual(0, version.MediaTypes.Count);
                        foreach (MediaType mediaType in version.MediaTypes)
                        {
                            Assert.IsNotNull(mediaType);
                            Assert.IsNotNull(mediaType.Base);
                            Assert.IsNotNull(mediaType.Type);
                        }

                        Assert.AreNotEqual(0, version.Links.Count);
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

        protected TimeSpan TestTimeout(TimeSpan timeSpan)
        {
            if (Debugger.IsAttached)
                return TimeSpan.FromDays(6);

            return timeSpan;
        }

        protected IBaseIdentityService CreateService()
        {
            BaseIdentityClient client = new BaseIdentityClient(BaseAddress);
            client.BeforeAsyncWebRequest += TestHelpers.HandleBeforeAsyncWebRequest;
            client.AfterAsyncWebResponse += TestHelpers.HandleAfterAsyncWebResponse;

            return client;
        }
    }
}
