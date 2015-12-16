﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using net.openstack.Core.Domain;
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

            Trace.WriteLine($"Creating server named: {definition.Name}");
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

        [Fact]
        public async void UpdateServerTest()
        {
            var server = await _testData.CreateServer();
            await server.WaitUntilActiveAsync();
            Trace.WriteLine($"Created server named: {server.Name}");

            var desiredName = server.Name + "UPDATED";
            server.Name = desiredName;
            Trace.WriteLine($"Updating server name to: {server.Name}...");
            await server.UpdateAsync();

            Trace.WriteLine("Verifying server instance was updated...");
            Assert.NotNull(server);
            Assert.Equal(desiredName, server.Name);

            Trace.WriteLine("Verifying server matches updated definition...");
            server = await _compute.GetServerAsync(server.Id);
            Assert.Equal(desiredName, server.Name);
        }

        [Fact]
        public async void DeleteServerTest()
        {
            var server = await _testData.CreateServer();
            await server.WaitUntilActiveAsync();
            Trace.WriteLine($"Created server named: {server.Name}");

            await server.DeleteAsync();
            await server.WaitUntilDeletedAsync();

            await Assert.ThrowsAsync<FlurlHttpException>(() => _compute.GetServerAsync(server.Id));
        }

        [Fact]
        public async void LookupServerAddressesTest()
        {
            var server = await _testData.CreateServer();
            await server.WaitUntilActiveAsync();
            Trace.WriteLine($"Created server named: {server.Name}");

            var results = await server.ListAddressesAsync();
            Assert.NotEmpty(results);

            var networkLabel = results.First().Key;
            var result = (await server.GetAddressAsync(networkLabel)).First();
            Assert.NotNull(result.IP);
        }

        [Fact]
        public async void SnapshotServerTest()
        {
            var server = await _testData.CreateServer();
            await server.WaitUntilActiveAsync();
            Trace.WriteLine($"Created server named: {server.Name}");

            var request = new SnapshotServerRequest(server.Name + "-SNAPSHOT")
            {
                Metadata =
                {
                    ["category"] = "ci"
                }
            };

            Trace.WriteLine("Taking a snapshot of the server...");
            var image = await server.SnapshotAsync(request);
            _testData.Register(image);
            await image.WaitUntilActiveAsync();

            Assert.NotNull(image);
            Assert.Equal(request.Name, image.Name);
            Assert.True(image.Metadata["category"] == "ci");
        }

        [Fact]
        public async void RestartServerTest()
        {
            var server = await _testData.CreateServer();
            await server.WaitUntilActiveAsync();
            Trace.WriteLine($"Created server named: {server.Name}");

            Trace.WriteLine("Stopping the server...");
            await server.StopAsync();
            await server.WaitForStatusAsync(ServerStatus.Stopped);
            Assert.Equal(server.Status, ServerStatus.Stopped);

            Trace.WriteLine("Starting the server...");
            await server.StartAsync();
            await server.WaitUntilActiveAsync();
            Assert.Equal(server.Status, ServerStatus.Active);
        }

        [Fact]
        public async void RebootServerTest()
        {
            var server = await _testData.CreateServer();
            await server.WaitUntilActiveAsync();
            Trace.WriteLine($"Created server named: {server.Name}");

            Trace.WriteLine("Rebooting the server...");
            await server.RebootAsync();
            await server.WaitForStatusAsync(ServerStatus.Reboot);
            await server.WaitUntilActiveAsync();
        }

        [Fact]
        public async void EvacuateServerTest()
        {
            var server = await _testData.CreateServer();
            await server.WaitUntilActiveAsync();
            Trace.WriteLine($"Created server named: {server.Name}");

            Trace.WriteLine("Evacuating the server to a new host...");
            var request = new EvacuateServerRequest(false)
            {
                AdminPassword = "top-secret-password"
            };

            // Our service isn't down, so we can't really evacuate.
            // Just check that the request would have gone through (i.e. was valid)
            // and only was rejected because the compute service is healthy
            try
            {
                await server.EvacuateAsync(request);
            }
            catch (FlurlHttpException httpError) when (httpError.Call.ErrorResponseBody.Contains("is still in use"))
            {
                // Hurray! the test passed
            }
        }

        [Fact]
        public async void ServerVolumesTest()
        {
            var server = await _testData.CreateServer();
            await server.WaitUntilActiveAsync();
            Trace.WriteLine($"Created server named: {server.Name}");

            Trace.WriteLine("Creating a test volume...");
            var volume = _testData.BlockStorage.CreateVolume();
            Identifier volumeId = volume.Id;
            _testData.BlockStorage.StorageProvider.WaitForVolumeAvailable(volumeId);

            Trace.WriteLine("Attaching the volume...");
            await server.AttachVolumeAsync(new VolumeAttachmentDefinition(volumeId));
            _testData.BlockStorage.StorageProvider.WaitForVolumeState(volumeId, VolumeState.InUse, new[] { VolumeState.Error });

            var volumeRef = server.AttachedVolumes.FirstOrDefault();
            Assert.NotNull(volumeRef);

            Trace.WriteLine("Verifying volume was attached successfully...");
            var attachedVolumes = await server.ListVolumesAsync();
            var attachedVolume = attachedVolumes.FirstOrDefault(v => v.Id == volumeId);
            Assert.NotNull(attachedVolume);

            Trace.WriteLine("Retrieving attached volume details...");
            attachedVolume = await volumeRef.GetServerVolumeAsync();
            Assert.Equal(volumeId, attachedVolume.VolumeId);
            Assert.Equal(server.Id, attachedVolume.ServerId);

            Trace.WriteLine("Detaching the volume...");
            await attachedVolume.DetachAsync();
            Assert.False(server.AttachedVolumes.Any(v => v.Id == volumeId));
            _testData.BlockStorage.StorageProvider.WaitForVolumeAvailable(volumeId);
        }
    }
}
