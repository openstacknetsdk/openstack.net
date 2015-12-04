using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Xunit;
using Xunit.Abstractions;

namespace OpenStack.Compute.v2_1
{
    public class ServerTests : IDisposable
    {
        private readonly ComputeService _compute;
        private readonly ComputeTestDataManager _testData;

        public ServerTests(ITestOutputHelper testLog)
        {
            var testOutput = new XunitTraceListener(testLog);
            Trace.Listeners.Add(testOutput);
            OpenStackNet.Tracing.Http.Listeners.Add(testOutput);

            var authenticationProvider = TestIdentityProvider.GetIdentityProvider();
            _compute = new ComputeService(authenticationProvider, "RegionOne");

            _testData = new ComputeTestDataManager(_compute);
        }

        public void Dispose()
        {
            Trace.Listeners.Clear();
            OpenStackNet.Tracing.Http.Listeners.Clear();

            _testData.Dispose();
        }

        [Fact]
        public async void CreateServerTest()
        {
            var definition = _testData.BuildServer();

            Trace.WriteLine(string.Format("Creating server named: {0}", definition.Name));
            var server = await _testData.CreateServer(definition);
            await server.WaitUntilActiveAsync();

            Trace.WriteLine("Verifying server matches requested definition...");
            Assert.NotNull(server);
            Assert.Equal(definition.Name, server.Name);
            Assert.NotNull(server.Flavor);
            Assert.Equal(definition.FlavorId, server.Flavor.Id);
            Assert.NotNull(server.AdminPassword);
            Assert.NotNull(server.Image);
            Assert.Equal(definition.ImageId, server.Image.Id);
            Assert.Equal(server.Status, ServerStatus.Active);
            Assert.NotNull(server.AvailabilityZone);
            Assert.NotNull(server.Created);
            Assert.NotNull(server.LastModified);
            Assert.NotNull(server.Launched);
            Assert.NotNull(server.DiskConfig);
            Assert.NotNull(server.HostId);
            Assert.NotNull(server.HostName);
            Assert.NotNull(server.PowerState);
            Assert.NotNull(server.VMState);
            Assert.NotNull(server.SecurityGroups);
        }

        [Fact]
        public async void DeleteServerTest()
        {
            var server = await _testData.CreateServer();
            Trace.WriteLine(string.Format("Created server named: {0}", server.Name));

            await server.DeleteAsync();
            await server.WaitUntilDeletedAsync();

            await Assert.ThrowsAsync<FlurlHttpException>(() => _compute.GetServerAsync(server.Id));
        }

        [Fact]
        public async Task ListServersTest()
        {
            var results = await _compute.ListServersAsync(new ListServersOptions {PageSize = 1});

            while (results.Any())
            {
                var result = results.First();
                Assert.NotNull(result.Name);
                results = await results.GetNextPageAsync();
            }
            Assert.NotNull(results);
        }

        [Fact]
        public async void FindServersTest()
        {
            var servers = await _testData.CreateServers();
            await Task.WhenAll(servers.Select(x => x.WaitUntilActiveAsync()));
            var serversNames = new HashSet<string>(servers.Select(s => s.Name));

            var results = await _compute.ListServersAsync(new ListServersOptions {Name = "ci-*"});
            var resultNames = new HashSet<string>(results.Select(s => s.Name));

            Assert.Subset(resultNames, serversNames);
        }

        [Fact]
        public async Task ListServerDetailsTest()
        {
            var results = await _compute.ListServerDetailsAsync(new ListServersOptions { PageSize = 1 });

            while (results.Any())
            {
                var result = results.First();
                Assert.NotNull(result.Image);
                results = await results.GetNextPageAsync();
            }
            Assert.NotNull(results);
        }
    }
}
