using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using OpenStack.Synchronous;
using Xunit;
using Xunit.Abstractions;

namespace OpenStack.Networking.v2
{
    public class NetworkTests
    {
        private readonly NetworkingService _networkingService;
        private readonly ITestOutputHelper _log;

        public NetworkTests(ITestOutputHelper log)
        {
            _log = log;
            OpenStackNet.Configure(flurl =>
            {
                flurl.OnError = call => { throw new FlurlHttpException(call, new Exception(call.ErrorResponseBody)); };
            });
            var authenticationProvider = TestIdentityProvider.GetIdentityProvider();
            _networkingService = new NetworkingService(authenticationProvider, "IAD");
        }

        [Fact]
        public async void CreateUpdateThenDeleteNetwork()
        {
            var definition = BuildNetworkDefinition();
            _log.WriteLine("Creating Network named {0}", definition.Name);
            var network = await _networkingService.CreateNetworkAsync(definition);
            _log.WriteLine("Network was created: {0}", network.Id);

            try
            {
                _log.WriteLine("Verifying network matches requested definition...");
                Assert.Equal(definition.Name, network.Name);
                Assert.True(network.IsUp);

                _log.WriteLine("Updating the network...");
                definition.Name += "updated";
                network = await _networkingService.UpdateNetworkAsync(network.Id, definition);

                _log.WriteLine("Verifying network matches updated definition...");
                Assert.Equal(definition.Name, network.Name);
            }
            finally
            {
                _log.WriteLine("Cleaning up any test data...");

                _log.WriteLine("Removing the network...");
                _networkingService.DeleteNetwork(network.Id);
                _log.WriteLine("The service was cleaned up sucessfully.");
            }
        }

        [Fact]
        public async void FindNetworks()
        {
            var networkIds = new List<string>();
            try
            {
                var create1 = CreateNetwork().ContinueWith(t => networkIds.Add(t.Result.Id));
                var create2 = CreateNetwork().ContinueWith(t => networkIds.Add(t.Result.Id));
                var create3 = CreateNetwork().ContinueWith(t => networkIds.Add(t.Result.Id));
                await Task.WhenAll(create1, create2, create3);

                _log.WriteLine("Listing networks...");
                var allNetworks = await _networkingService.ListNetworksAsync();
                Assert.All(networkIds, id => allNetworks.Any(n => n.Id == id));

                _log.WriteLine("Retrieving a network...");
                var networkId = networkIds.First();
                var network = await _networkingService.GetNetworkAsync(networkId);
                Assert.NotNull(network);
                Assert.Equal(networkId, network.Id);
            }
            finally
            {
                _log.WriteLine("Cleaning up any test data...");

                _log.WriteLine("Removing the networks...");
                var deletes = networkIds.Select(id => _networkingService
                    .DeleteNetworkAsync(id)
                    .ContinueWith(t => _log.WriteLine("Network was deleted: {0}", id)))
                    .ToArray();

                Task.WaitAll(deletes);
                _log.WriteLine("The networks were cleaned up sucessfully.");

            }
        }

        private static NetworkDefinition BuildNetworkDefinition()
        {
            return new NetworkDefinition { Name = string.Format("ci-test-{0}", Guid.NewGuid()) };
        }

        private async Task<Network> CreateNetwork()
        {
            var definition = BuildNetworkDefinition();
            _log.WriteLine("Creating Network named {0}", definition.Name);
            var network = await _networkingService.CreateNetworkAsync(definition);
            _log.WriteLine("Network was created: {0}", network.Id);
            return network;
        }
    }
}
