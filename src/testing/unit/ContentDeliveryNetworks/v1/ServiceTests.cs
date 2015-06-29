using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http.Testing;
using Marvin.JsonPatch;
using net.openstack.Core.Domain;
using OpenStack.ContentDeliveryNetworks.v1.Synchronous;
using OpenStack.Synchronous;
using Xunit;

namespace OpenStack.ContentDeliveryNetworks.v1
{
    public class ServiceTests
    {
        private readonly ContentDeliveryNetworkService _cdnService;

        public ServiceTests()
        {
            _cdnService = new ContentDeliveryNetworkService(Stubs.AuthenticationProvider, "DFW");
        }

        [Fact]
        public void FindServiceOnAPage()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new ServiceCollection(new[] {new Service()})
                {
                    Links = new[] {new Link("next", "http://api.com/next")}
                });
                httpTest.RespondWithJson(new ServiceCollection(new[] {new Service {Name = "MyService"}}));

                var currentPage = _cdnService.ListServices();
                Service myService;
                do
                {
                    myService = currentPage.FirstOrDefault(x => x.Name == "MyService");
                    if (myService != null)
                        break;

                    currentPage = currentPage.GetNextPage();
                } while (currentPage.Any());

                Assert.NotNull(myService);
            }
        }

        [Fact]
        public void GetService()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Service{Id = "service-id"});

                var service = _cdnService.GetService("service-id");

                Assert.NotNull(service);
                Assert.Equal("service-id", service.Id);
            }
        }

        [Fact]
        public void CreateService()
        {
            using (var httpTest = new HttpTest())
            {
                var response = new HttpResponseMessage(HttpStatusCode.Created);
                response.Headers.Location = "http://api.com".AppendPathSegments("services", "service-id").ToUri();
                httpTest.ResponseQueue.Enqueue(response);

                var service = new ServiceDefinition("service-name", "flavor-id", null, null)
                {
                    Domains =
                    {
                        new ServiceDomain("example.com")
                    },
                    Origins =
                    {
                        new ServiceOrigin("assets.example.com")
                    },
                    Caches =
                    {
                        new ServiceCache("keep-one-day", TimeSpan.FromDays(1))
                    },
                    Restrictions =
                    {
                        new ServiceRestriction("internal-users-only", new[]
                        {
                            new ServiceRestrictionRule("intranet", "intranet.example.com")
                        })
                    }
                };
                var serviceId = _cdnService.CreateService(service);

                Assert.Equal("service-id", serviceId);
            }
        }

        [Fact]
        public void DeleteService()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith((int)HttpStatusCode.NoContent, "All gone!");

                _cdnService.DeleteService("service-id");
            }
        }

        [Fact]
        public void WaitForServiceDeployed()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Service { Status = ServiceStatus.CreateInProgress });
                httpTest.RespondWithJson(new Service { Status = ServiceStatus.UpdateInProgress });
                httpTest.RespondWithJson(new Service { Status = ServiceStatus.Deployed });

                var service = _cdnService.WaitForServiceDeployed("service-id", TimeSpan.FromMilliseconds(1));
                Assert.NotNull(service);
            }
        }

        [Fact]
        public void WaitForServiceDeploy_ThrowsAnException_WhenTheOperationFailed()
        {
            using (var httpTest = new HttpTest())
            {
                var failedServiceDeployment = new Service
                {
                    Status = ServiceStatus.Failed,
                    Errors = new[] { new ServiceError { Message = "The domain is already in use." } }
                };
                httpTest.RespondWithJson(failedServiceDeployment);

                var ex = Assert.Throws<ServiceOperationFailedException>(() => _cdnService.WaitForServiceDeployed("failed-service-id"));
                Assert.NotEmpty(ex.Errors);
            }
        }

        [Fact]
        public async void WaitForServiceDeployed_StopsWhenUserTokenIsCancelled()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Service {Status = ServiceStatus.CreateInProgress});

                var timedToken = new CancellationTokenSource(TimeSpan.FromSeconds(1));

                await Assert.ThrowsAsync<TaskCanceledException>(() => _cdnService.WaitForServiceDeployedAsync("service-id", Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan, null, timedToken.Token));
            }
        }

        [Fact]
        public async void WaitForServiceDeployed_StopsWhenTimeoutIsReached()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Service { Status = ServiceStatus.CreateInProgress });

                await Assert.ThrowsAsync<TimeoutException>(() => _cdnService.WaitForServiceDeployedAsync("service-id", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1)));
            }
        }

        [Fact]
        public void WaitForServiceDeleted()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Service { Status = ServiceStatus.DeleteInProgress });
                httpTest.RespondWith((int)HttpStatusCode.NotFound, "All gone!");

                _cdnService.WaitForServiceDeleted("service-id", TimeSpan.FromMilliseconds(1));
            }
        }

        [Fact]
        public void WaitForServiceDeleted_StopsWhenUserTokenIsCancelled()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Service { Status = ServiceStatus.DeleteInProgress });

                var timedToken = new CancellationTokenSource(TimeSpan.FromSeconds(1));

                Assert.ThrowsAsync<TaskCanceledException>(() => _cdnService.WaitForServiceDeletedAsync("service-id", Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan, null, timedToken.Token));
            }
        }

        [Fact]
        public async void WaitForServiceDeleted_StopsWhenTimeoutIsReached()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new Service { Status = ServiceStatus.CreateInProgress });

                await Assert.ThrowsAsync<TimeoutException>(() => _cdnService.WaitForServiceDeletedAsync("service-id", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1)));
            }
        }

        [Fact]
        public void WaitForServiceDeleted_ThrowsAnException_WhenTheOperationFailed()
        {
            using (var httpTest = new HttpTest())
            {
                var failedServiceDeployment = new Service
                {
                    Status = ServiceStatus.Failed,
                    Errors = new[] { new ServiceError { Message = "Random error." } }
                };
                httpTest.RespondWithJson(failedServiceDeployment);

                var ex = Assert.Throws<ServiceOperationFailedException>(() => _cdnService.WaitForServiceDeleted("failed-service-id"));
                Assert.NotEmpty(ex.Errors);
            }
        }

        [Fact]
        public void WhenIDeleteANonExistingService_TheOperationShouldBeConsideredSuccessful()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith((int)HttpStatusCode.NotFound, "Nothing to see here!");

                _cdnService.DeleteService("bad-service-id");
            }
        }

        [Fact]
        public void UpdateService()
        {
            using (new HttpTest())
            {
                var patch = new JsonPatchDocument<ServiceDefinition>();
                patch.Replace(x => x.Name, "My Cool New Name");

                _cdnService.UpdateService("service-id", patch);
            }
        }

        [Fact]
        public void PurgeAsset()
        {
            using (new HttpTest())
            {
                _cdnService.PurgeCachedAsset("service-id", "asset-url");
            }
        }

        [Fact]
        public void PurgeAssets()
        {
            using (new HttpTest())
            {
                _cdnService.PurgeCachedAssets("service-id");
            }
        }

        [Fact]
        public void WhenIPurgeANonExistingAsset_TheOperationShouldBeConsideredSuccessful()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith((int)HttpStatusCode.NotFound, "Nothing to see here!");

                _cdnService.PurgeCachedAsset("service-id", "missing-asset");
            }
        }
    }
}
