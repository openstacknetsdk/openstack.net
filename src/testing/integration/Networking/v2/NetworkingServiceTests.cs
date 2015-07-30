using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using OpenStack.Synchronous;
using Xunit;
using Xunit.Abstractions;

namespace OpenStack.Networking.v2
{
    public class NetworkingServiceTests : IDisposable
    {
        private readonly NetworkingService _networkingService;
        private readonly HashSet<object> _testData;
         
        public NetworkingServiceTests(ITestOutputHelper testLog)
        {
            var testOutput = new XunitTraceListener(testLog);
            Trace.Listeners.Add(testOutput);
            OpenStackNet.Tracing.Http.Listeners.Add(testOutput);

            var authenticationProvider = TestIdentityProvider.GetIdentityProvider();
            _networkingService = new NetworkingService(authenticationProvider, "RegionOne");

            _testData = new HashSet<object>();
        }

        private void RegisterTestData(IEnumerable<object> testItems)
        {
            foreach (var testItem in testItems)
            {
                RegisterTestData(testItem);
            }
        }

        private void RegisterTestData(object testItem)
        {
            _testData.Add(testItem);
        }

        public void Dispose()
        {
            Trace.Listeners.Clear();
            OpenStackNet.Tracing.Http.Listeners.Clear();

            //
            // Remove all test data
            //
            var errors = new List<Exception>();
            foreach (var subnet in _testData.OfType<Subnet>())
            {
                try
                {
                    _networkingService.DeleteSubnet(subnet.Id);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }

            foreach (var network in _testData.OfType<Network>())
            {
                try
                {
                    _networkingService.DeleteNetwork(network.Id);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }

            if (errors.Any())
                throw new AggregateException("Unable to remove all test data!", errors);
        }

        #region Networks

        [Fact]
        public async void CreateNetworkTest()
        {
            var definition = BuildNetwork();

            Trace.WriteLine(string.Format("Creating network named: {0}", definition.Name));
            var network = await _networkingService.CreateNetworkAsync(definition);

            Trace.WriteLine("Verifying network matches requested definition...");
            Assert.NotNull(network);
            Assert.Equal(definition.Name, network.Name);
            Assert.True(network.IsUp);
            Assert.False(network.IsShared);
            Assert.Equal(NetworkStatus.Active, network.Status);
        }

        [Fact]
        public async void BulkCreateNetworksTest()
        {
            var definitions = new[] {BuildNetwork(), BuildNetwork(), BuildNetwork()};

            Trace.WriteLine(string.Format("Creating networks: {0}", string.Join(", ", definitions.Select(d => d.Name))));
            var networks = await CreateNetworks(definitions);

            Trace.WriteLine("Verifying networks matches requested definitions...");

            Assert.NotNull(networks);
            Assert.Equal(3, networks.Count());
            Assert.All(networks, n => Assert.True(n.IsUp));
            Assert.All(networks, n => Assert.Equal(NetworkStatus.Active, n.Status));
        }

        [Fact]
        public async void UpdateNetworkTest()
        {
            Network network = await CreateNetwork();

            var networkUpdate = new NetworkDefinition
            {
                Name = string.Format("{0}-updated", network.Name)
            };

            Trace.WriteLine("Updating the network...");
            network = await _networkingService.UpdateNetworkAsync(network.Id, networkUpdate);

            Trace.WriteLine("Verifying network was updated as requested...");
            Assert.NotNull(network);
            Assert.Equal(networkUpdate.Name, network.Name);
        }

        [Fact]
        public async void ListNetworksTest()
        {
            var networks = await CreateNetworks();

            Trace.WriteLine("Listing networks...");
            var results = await _networkingService.ListNetworksAsync();

            Trace.WriteLine("Verifying list of networks...");
            Assert.NotNull(results);
            Assert.All(networks, network => Assert.True(results.Any(x => x.Id == network.Id)));
        }

        [Fact]
        public async void GetNetworkTest()
        {
            var network = await CreateNetwork();

            Trace.WriteLine("Retrieving network...");
            var result = await _networkingService.GetNetworkAsync(network.Id);

            Trace.WriteLine("Verifying network...");
            Assert.NotNull(result);
            Assert.Equal(network.Id, result.Id);
        }

        public static NetworkDefinition BuildNetwork()
        {
            return new NetworkDefinition
            {
                Name = string.Format("ci-test-{0}", Guid.NewGuid())
            };
        }

        public async Task<Network> CreateNetwork()
        {
            var definition = BuildNetwork();
            var network = await _networkingService.CreateNetworkAsync(definition);
            RegisterTestData(network);
            return network;
        }

        public async Task<IEnumerable<Network>> CreateNetworks()
        {
            var definitions = new[] { BuildNetwork(), BuildNetwork(), BuildNetwork() };
            return await CreateNetworks(definitions);
        }

        public async Task<IEnumerable<Network>> CreateNetworks(IEnumerable<NetworkDefinition> definitions)
        {
            var networks = await _networkingService.CreateNetworksAsync(definitions);
            RegisterTestData(networks);
            return networks;
        }

        public void DeleteNetworks(IEnumerable<Network> networks)
        {
            Task[] deletes = networks.Select(n => _networkingService.DeleteNetworkAsync(n.Id)).ToArray();
            Task.WaitAll(deletes);
        }
        #endregion

        #region Subnets
        [Fact]
        public async void CreateSubnetTest()
        {
            var network = await CreateNetwork();
            var definition = new SubnetCreateDefinition(network.Id, IPVersion.IP, "192.168.3.0/24")
            {
                Name = string.Format("ci-test-{0}", Guid.NewGuid()),
                IsDHCPEnabled = true,
                GatewayIP = "192.168.3.1",
                AllocationPools =
                {
                    new AllocationPool("192.168.3.10", "192.168.3.50")
                },
                Nameservers =
                {
                    "8.8.8.8"
                },
                HostRoutes =
                {
                    new HostRoute("1.2.3.4/24", "10.0.0.1")
                }
            };

            Trace.WriteLine(string.Format("Creating subnet named: {0}", definition.Name));
            var subnet = await CreateSubnet(definition);

            Trace.WriteLine("Verifying subnet matches requested definition...");
            Assert.NotNull(subnet);
            Assert.Equal(definition.NetworkId, subnet.NetworkId);
            Assert.Equal(definition.Name, subnet.Name);
            Assert.Equal(definition.CIDR, subnet.CIDR);
            Assert.Equal(definition.IPVersion, subnet.IPVersion);
            Assert.Equal(definition.IsDHCPEnabled, subnet.IsDHCPEnabled);
            Assert.Equal(definition.GatewayIP, subnet.GatewayIP);
            Assert.Equal(definition.AllocationPools, subnet.AllocationPools);
            Assert.Equal(definition.Nameservers, subnet.Nameservers);
            Assert.Equal(definition.HostRoutes, subnet.HostRoutes);
        }

        [Fact]
        public async void BulkCreateSubnetsTest()
        {
            var network = await CreateNetwork();
            var definitions = new[] {BuildSubnet(network), BuildSubnet(network), BuildSubnet(network)};

            Trace.WriteLine(string.Format("Creating subnets: {0}", string.Join(", ", definitions.Select(d => d.Name))));
            var subnets = await CreateSubnets(definitions);

            Trace.WriteLine("Verifying subnets matches requested definitions...");
            Assert.NotNull(subnets);
            Assert.Equal(3, subnets.Count());
        }

        [Fact]
        public async void UpdateSubnetTest()
        {
            Network network = await CreateNetwork();
            Subnet subnet = await CreateSubnet(network);

            var networkUpdate = new SubnetUpdateDefinition
            {
                Name = string.Format("{0}-updated", subnet.Name)
            };

            Trace.WriteLine("Updating the subnet...");
            subnet = await _networkingService.UpdateSubnetAsync(subnet.Id, networkUpdate);

            Trace.WriteLine("Verifying subnet was updated as requested...");
            Assert.NotNull(subnet);
            Assert.Equal(networkUpdate.Name, subnet.Name);
        }

        [Fact]
        public async void ListSubnetsTest()
        {
            var network = await CreateNetwork();
            var subnets = await CreateSubnets(network);

            Trace.WriteLine("Listing subnets...");
            var results = await _networkingService.ListSubnetsAsync();

            Trace.WriteLine("Verifying list of subnets...");
            Assert.NotNull(results);
            Assert.All(subnets, subnet => Assert.True(results.Any(x => x.Id == subnet.Id)));
        }

        [Fact]
        public async void GetSubnetTest()
        {
            var network = await CreateNetwork();
            var subnet = await CreateSubnet(network);

            Trace.WriteLine("Retrieving subnet...");
            var result = await _networkingService.GetSubnetAsync(subnet.Id);

            Trace.WriteLine("Verifying subnet...");
            Assert.NotNull(result);
            Assert.Equal(subnet.Id, result.Id);

        }

        private static int _subnetCounter = 0;
        public static SubnetCreateDefinition BuildSubnet(Network network)
        {
            var cidr = string.Format("192.168.{0}.0/24", _subnetCounter++);
            return new SubnetCreateDefinition(network.Id, IPVersion.IP, cidr)
            {
                Name = string.Format("ci-test-{0}", Guid.NewGuid())
            };
        }

        public async Task<Subnet> CreateSubnet(Network network)
        {
            var definition = BuildSubnet(network);
            return await CreateSubnet(definition);
        }

        public async Task<Subnet> CreateSubnet(SubnetCreateDefinition definition)
        {
            var subnet = await _networkingService.CreateSubnetAsync(definition);
            RegisterTestData(subnet);
            return subnet;
        }

        public async Task<IEnumerable<Subnet>> CreateSubnets(Network network)
        {
            var definitions = new[] { BuildSubnet(network), BuildSubnet(network), BuildSubnet(network) };
            return await CreateSubnets(definitions);
        }

        public async Task<IEnumerable<Subnet>> CreateSubnets(IEnumerable<SubnetCreateDefinition> definitions)
        {
            var subnets = await _networkingService.CreateSubnetsAsync(definitions);
            RegisterTestData(subnets);
            return subnets;
        }

        public void DeleteSubnets(IEnumerable<Subnet> networks)
        {
            Task[] deletes = networks.Select(n => _networkingService.DeleteSubnetAsync(n.Id)).ToArray();
            Task.WaitAll(deletes);
        }
        #endregion
    }
}
