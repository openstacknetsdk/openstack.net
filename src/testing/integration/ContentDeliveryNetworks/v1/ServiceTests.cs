using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Marvin.JsonPatch;
using net.openstack.Providers.Rackspace;
using Newtonsoft.Json;
using OpenStack.Synchronous;
using Xunit;
using Xunit.Abstractions;

namespace OpenStack.ContentDeliveryNetworks.v1
{
    public class ServiceTests
    {
        private readonly ITestOutputHelper _log;
        private readonly ContentDeliveryNetworkService _cdnService;

        public ServiceTests(ITestOutputHelper log)
        {
            _log = log;

            var identity = TestIdentityProvider.GetIdentityFromEnvironment();
            var authenticationProvider = new CloudIdentityProvider(identity)
            {
                ApplicationUserAgent = "CI-BOT"
            };
            _cdnService = new ContentDeliveryNetworkService(authenticationProvider, "DFW");
        }

        [Fact]
        public async void CreateAServiceOnAkamai_UsingDefaults()
        {
            try
            {
                _log.WriteLine("Looking for a CDN flavor provided by Akamai...");
                var flavors = await _cdnService.ListFlavorsAsync();
                var flavor = flavors.FirstOrDefault(x => x.Providers.Any(p => string.Equals(p.Name, "Akamai", StringComparison.OrdinalIgnoreCase)));
                Assert.NotNull(flavor);
                var akamaiFlavor = flavor.Id;
                _log.WriteLine("Found the {0} flavor", akamaiFlavor);

                _log.WriteLine("Creating a CDN service using defaults for anything I can omit...");
                var domains = new[] {new ServiceDomain("mirror.example.com")};
                var origins = new[] {new ServiceOrigin("example.com")};
                var serviceDefinition = new ServiceDefinition("ci-test", akamaiFlavor, domains, origins);
                var serviceId = await _cdnService.CreateServiceAsync(serviceDefinition);
                _log.WriteLine("Service was created: {0}", serviceId);

                try
                {
                    _log.WriteLine("Waiting for the service to be deployed...");
                    var service = await _cdnService.WaitForServiceDeployedAsync(serviceId, progress: new Progress<bool>(x => _log.WriteLine("...")));

                    _log.WriteLine("Verifying service matches the requested definition...");
                    Assert.Equal("ci-test", service.Name);
                    Assert.Equal(serviceDefinition.FlavorId, service.FlavorId);
                    
                    Assert.Equal(serviceDefinition.Origins.Count, service.Origins.Count());
                    Assert.Equal(serviceDefinition.Origins.First().Origin, service.Origins.First().Origin);

                    Assert.Equal(serviceDefinition.Domains.Count, service.Domains.Count());
                    Assert.Equal(serviceDefinition.Domains.First().Domain, service.Domains.First().Domain);

                    _log.WriteLine("Updating the service...");
                    var patch = new JsonPatchDocument<ServiceDefinition>();
                    patch.Replace(x => x.Name, "ci-test2");
                    var intranetOnly = new ServiceRestriction("intranet", new[] {new ServiceRestrictionRule("intranet", "intranet.example.com")});
                    patch.Add(x => x.Restrictions, intranetOnly, 0);
                    await _cdnService.UpdateServiceAsync(serviceId, patch);

                    _log.WriteLine("Waiting for the service changes to be deployed...");
                    service = await _cdnService.WaitForServiceDeployedAsync(serviceId, progress: new Progress<bool>(x => _log.WriteLine("...")));

                    _log.WriteLine("Verifying service matches updated definition...");
                    Assert.Equal("ci-test2", service.Name);
                    Assert.Equal(JsonConvert.SerializeObject(intranetOnly), JsonConvert.SerializeObject(service.Restrictions.First()));

                    _log.WriteLine("Purging all assets on service");
                    await _cdnService.PurgeCachedAssetsAsync(serviceId);
                }
                finally
                {
                    _log.WriteLine("Cleaning up any test data...");

                    _log.WriteLine("Removing the service...");
                    _cdnService.DeleteService(serviceId);
                    _cdnService.WaitForServiceDeleted(serviceId);
                    _log.WriteLine("The service was cleaned up sucessfully.");
                }
            }
            catch (FlurlHttpException ex)
            {
                throw new FlurlHttpException(ex.Call, new Exception(ex.GetResponseString()));
            }
        }

        [Fact]
        public async void FindServiceOnAPage()
        {
            var serviceIds = new List<string>();
            try
            {
                var create1 = CreateService("ci-test1", "mirror.example1.com", "example1.com").ContinueWith(t => serviceIds.Add(t.Result));
                var create2 = CreateService("ci-test2", "mirror.example2.com", "example2.com").ContinueWith(t => serviceIds.Add(t.Result));
                var create3 = CreateService("ci-test3", "mirror.example3.com", "example3.com").ContinueWith(t => serviceIds.Add(t.Result));

                Task.WaitAll(create1, create2, create3);

                var currentPage = await _cdnService.ListServicesAsync(pageSize: 1);
                while (currentPage.Any())
                {
                    if (currentPage.Any(x => x.Name == "ci-test3"))
                    {
                        _log.WriteLine("Found the desired service");
                        break;
                    }

                    currentPage = await currentPage.GetNextPageAsync();
                }
            }
            catch (FlurlHttpException ex)
            {
                throw new FlurlHttpException(ex.Call, new Exception(ex.GetResponseString()));
            }
            finally
            {
                _log.WriteLine("Cleaning up any test data...");

                _log.WriteLine("Removing the services...");
                var deletes = serviceIds.Select(serviceId => _cdnService
                    .DeleteServiceAsync(serviceId)
                    .ContinueWith(t => _cdnService.WaitForServiceDeletedAsync(serviceId))
                    .ContinueWith(t => _log.WriteLine("Service was deleted: {0}", serviceId)))
                    .ToArray();

                Task.WaitAll(deletes);
                _log.WriteLine("The services were cleaned up sucessfully.");

            }
        }

        private async Task<string> CreateService(string name, string domain, string origin)
        {
            var flavors = await _cdnService.ListFlavorsAsync();
            var flavor = flavors.First();

            _log.WriteLine("Creating CDN Service: {0} for {1} originating from {2}", name, domain, origin);
            var domains = new[] { new ServiceDomain(domain) };
            var origins = new[] { new ServiceOrigin(origin) };
            var serviceDefinition = new ServiceDefinition(name, flavor.Id, domains, origins);
            var serviceId = await _cdnService.CreateServiceAsync(serviceDefinition);
            _log.WriteLine("Service was created: {0}", serviceId);
            return serviceId;
        }
    }
}
