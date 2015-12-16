using System;
using System.Linq;
using System.Net;
using OpenStack.Compute.v2_1.Serialization;
using OpenStack.Serialization;
using OpenStack.Synchronous;
using OpenStack.Testing;
using Xunit;

namespace OpenStack.Compute.v2_1
{
    public class ServerTests
    {
        private readonly ComputeService _compute;

        public ServerTests()
        {
            _compute = new ComputeService(Stubs.AuthenticationProvider, "region");
        }

        [Fact]
        public void CreateServer()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server {Id = serverId});

                var definition = new ServerCreateDefinition("{name}", Guid.NewGuid(), "{flavor-id}");
                var result = _compute.CreateServer(definition);

                httpTest.ShouldHaveCalled("*/servers");
                Assert.NotNull(result);
                Assert.Equal(serverId,result.Id);
                Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Fact]
        public void GetServer()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId });

                var result = _compute.GetServer(serverId);

                httpTest.ShouldHaveCalled($"*/servers/{serverId}");
                Assert.NotNull(result);
                Assert.Equal(serverId, result.Id);
                Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Fact]
        public void GetServerExtension()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new ServerReferenceCollection
                {
                    Items = { new ServerReference { Id = serverId } }
                });
                httpTest.RespondWithJson(new Server { Id = serverId });

                var serverRef = _compute.ListServers().First();
                var result = serverRef.GetServer();
                
                Assert.NotNull(result);
                Assert.Equal(serverId, result.Id);
            }
        }

        [Fact]
        public void WaitForServerActive()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId, Status = ServerStatus.Building});
                httpTest.RespondWithJson(new Server { Id = serverId, Status = ServerStatus.Active });

                var result = _compute.GetServer(serverId);
                result.WaitUntilActive();

                httpTest.ShouldHaveCalled($"*/servers/{serverId}");
                Assert.NotNull(result);
                Assert.Equal(serverId, result.Id);
                Assert.Equal(ServerStatus.Active, result.Status);
                Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Fact]
        public void ListServers()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new ServerReferenceCollection
                {
                   Items = { new ServerReference {Id = serverId}},
                   Links = { new PageLink("next", "http://api.com/next") }
                });

                var results = _compute.ListServers();

                httpTest.ShouldHaveCalled("*/servers");
                Assert.Equal(1, results.Count());
                var result = results.First();
                Assert.Equal(serverId, result.Id);
                Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Fact]
        public void ListServersWithFilter()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new ServerCollection());

                const string name = "foo";
                const string flavorId = "1";
                Identifier imageId = Guid.NewGuid();
                var lastModified = DateTimeOffset.Now.AddDays(-1);
                ServerStatus status = ServerStatus.Active;

                _compute.ListServers(new ListServersOptions { Name = name, FlavorId = flavorId, ImageId = imageId, LastModified = lastModified, Status = status});

                httpTest.ShouldHaveCalled($"*name={name}");
                httpTest.ShouldHaveCalled($"*flavor={flavorId}");
                httpTest.ShouldHaveCalled($"*image={imageId}");
                httpTest.ShouldHaveCalled($"*status={status}");
                httpTest.ShouldHaveCalled("*changes-since=");
            }
        }

        [Fact]
        public void ListServersWithPaging()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new ServerCollection());

                Identifier startingAt = Guid.NewGuid();
                const int pageSize = 10;
                _compute.ListServers(new ListServersOptions { PageSize = pageSize, StartingAt = startingAt });

                httpTest.ShouldHaveCalled($"*marker={startingAt}*");
                httpTest.ShouldHaveCalled($"*limit={pageSize}*");
            }
        }

        [Fact]
        public void UpdateServer()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId, Name = "{new-name}"});

                var request = new ServerUpdateDefinition {Name = "{new-name}"};
                var result = _compute.UpdateServer(serverId, request);

                httpTest.ShouldHaveCalled($"*/servers/{serverId}");
                Assert.NotNull(result);
                Assert.Equal(serverId, result.Id);
                Assert.Equal(request.Name, result.Name);
                Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Fact]
        public void UpdateServerExtension()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId, Name = "{old-name}" });
                var lastModified = DateTimeOffset.Now;
                httpTest.RespondWithJson(new Server { Id = serverId, Name = "{new-name}", LastModified = lastModified});

                var server = _compute.GetServer(serverId);
                server.Name = "{new-name}";
                server.Update();

                Assert.Equal(serverId, server.Id);
                Assert.Equal("{new-name}", server.Name);
                Assert.Equal(lastModified, server.LastModified);
                Assert.IsType<ComputeApiBuilder>(((IServiceResource)server).Owner);
            }
        }

        [Fact]
        public void DeleteServer()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWith((int)HttpStatusCode.NoContent, "All gone!");

                _compute.DeleteServer(serverId);

                httpTest.ShouldHaveCalled($"*/servers/{serverId}");
            }
        }

        [Fact]
        public void DeleteServerExtension()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server {Id = serverId});
                httpTest.RespondWith((int)HttpStatusCode.NoContent, "All gone!");
                httpTest.RespondWithJson(new Server { Id = serverId, Status = ServerStatus.Deleted});

                var server =_compute.GetServer(serverId);
                server.Delete();
                Assert.NotEqual(server.Status, ServerStatus.Deleted);

                server.WaitUntilDeleted();
                Assert.Equal(server.Status, ServerStatus.Deleted);
            }
        }

        [Fact]
        public void WhenDeleteServer_Returns404NotFound_ShouldConsiderRequestSuccessful()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWith((int)HttpStatusCode.NotFound, "Not here, boss...");

                _compute.DeleteServer(serverId);

                httpTest.ShouldHaveCalled($"*/servers/{serverId}");
            }
        }

        [Fact]
        public void WaitForServerDeleted()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId, Status = ServerStatus.Active });
                httpTest.RespondWith((int)HttpStatusCode.NoContent, "All gone!");
                httpTest.RespondWithJson(new Server { Id = serverId, Status = ServerStatus.Deleted });

                var result = _compute.GetServer(serverId);
                result.Delete();
                result.WaitUntilDeleted();
                
                Assert.Equal(ServerStatus.Deleted, result.Status);
            }
        }

        [Fact]
        public void WaitForServerDeleted_Returns404NotFound_ShouldConsiderRequestSuccessful()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId, Status = ServerStatus.Active });
                httpTest.RespondWith((int)HttpStatusCode.NoContent, "All gone!");
                httpTest.RespondWith((int)HttpStatusCode.NotFound, "Nothing here, boss");

                var result = _compute.GetServer(serverId);
                result.Delete();
                result.WaitUntilDeleted();

                Assert.Equal(ServerStatus.Deleted, result.Status);
            }
        }

        [Fact]
        public void SnapshotServer()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                Identifier imageId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId });
                httpTest.RespondWith((int)HttpStatusCode.Accepted, "Roger that, boss");
                httpTest.ResponseQueue.Last().Headers.Location = new Uri($"http://api.example.com/images/{imageId}");
                httpTest.RespondWithJson(new Image { Id = imageId });

                var server = _compute.GetServer(serverId);
                Image result = server.Snapshot(new SnapshotServerRequest("{image-name"));

                httpTest.ShouldHaveCalled($"*/servers/{serverId}/action");
                Assert.True(httpTest.CallLog.First(x => x.Url.EndsWith("/action")).RequestBody.Contains("createImage"));
                Assert.NotNull(result);
                Assert.Equal(imageId, result.Id);
            }
        }

        [Fact]
        public void StartServer()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId });
                httpTest.RespondWith((int)HttpStatusCode.Accepted, "Roger that, boss");

                var server = _compute.GetServer(serverId);
                server.Start();

                httpTest.ShouldHaveCalled($"*/servers/{serverId}/action");
                Assert.True(httpTest.CallLog.Last().RequestBody.Contains("os-start"));
            }
        }

        [Fact]
        public void StopServer()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId });
                httpTest.RespondWith((int)HttpStatusCode.Accepted, "Roger that, boss");

                var server = _compute.GetServer(serverId);
                server.Stop();

                httpTest.ShouldHaveCalled($"*/servers/{serverId}/action");
                Assert.True(httpTest.CallLog.Last().RequestBody.Contains("os-stop"));
            }
        }

        [Fact]
        public void RebootServer()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId });
                httpTest.RespondWith((int)HttpStatusCode.Accepted, "Roger that, boss");

                var server = _compute.GetServer(serverId);
                server.Reboot(new RebootServerRequest {Type = RebootType.Hard});

                httpTest.ShouldHaveCalled($"*/servers/{serverId}/action");
                string lastRequest = httpTest.CallLog.Last().RequestBody;
                Assert.True(lastRequest.Contains("reboot"));
                Assert.True(lastRequest.Contains("HARD"));
            }
        }

        [Fact]
        public void EvacuateServer()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId });
                httpTest.RespondWith((int)HttpStatusCode.Accepted, "Roger that, boss");

                var server = _compute.GetServer(serverId);
                server.Evacuate(new EvacuateServerRequest(false));

                httpTest.ShouldHaveCalled($"*/servers/{serverId}/action");
                string lastRequest = httpTest.CallLog.Last().RequestBody;
                Assert.True(lastRequest.Contains("evacuate"));
            }
        }

        [Fact]
        public void AttachVolume()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier volumeId = Guid.NewGuid();
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId });
                httpTest.RespondWithJson(new ServerVolume {Id = volumeId, DeviceName = "/dev/vdd"});

                var server = _compute.GetServer(serverId);
                var result = server.AttachVolume(new ServerVolumeDefinition(volumeId));

                httpTest.ShouldHaveCalled($"*/servers/{serverId}/os-volume_attachments");
                Assert.NotNull(result);
                Assert.Equal(volumeId, result.Id);
                Assert.True(server.AttachedVolumes.Any(v => v.Id == volumeId));
                Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
                Assert.NotNull(result.Server);
            }
        }

        [Fact]
        public void DetachVolume()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier volumeId = Guid.NewGuid();
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server
                {
                    Id = serverId,
                    AttachedVolumes = { new ServerVolume { Id = volumeId, DeviceName = "/dev/vdd" } }
                });
                httpTest.RespondWith((int)HttpStatusCode.Accepted, "Roger that, good buddy");

                var server = _compute.GetServer(serverId);
                ServerVolumeReference attachedVolume = server.AttachedVolumes[0];
                attachedVolume.Detach();

                httpTest.ShouldHaveCalled($"*/servers/{serverId}/os-volume_attachments/{volumeId}");
                Assert.False(server.AttachedVolumes.Any(v => v.Id == volumeId));
            }
        }

        [Fact]
        public void GetVolume()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier volumeId = Guid.NewGuid();
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server {Id = serverId, AttachedVolumes = {new ServerVolumeReference {Id = volumeId}}});
                httpTest.RespondWithJson(new ServerVolume {Id = volumeId, DeviceName = "/dev/vdd"});

                var server = _compute.GetServer(serverId);
                var result = server.AttachedVolumes[0].GetServerVolume();

                httpTest.ShouldHaveCalled($"*/servers/{serverId}/os-volume_attachments/{volumeId}");
                Assert.NotNull(result);
                Assert.Equal(volumeId, result.Id);
            }
        }

        [Fact]
        public void ListVolumes()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier volumeId = Guid.NewGuid();
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server {Id = serverId});
                httpTest.RespondWithJson(new ServerVolumeCollection
                {
                    new ServerVolume
                    {
                        Id = volumeId, DeviceName = "/dev/vdd"
                    }
                });

                var server = _compute.GetServer(serverId);
                var results = server.ListVolumes();

                httpTest.ShouldHaveCalled($"*/servers/{serverId}/os-volume_attachments");
                Assert.NotNull(results);
                Assert.Equal(1, results.Count());
                Assert.Equal(volumeId, results.First().Id);
            }
        }

        [Theory]
        [InlineData("novnc")]
        [InlineData("xvpvnc")]
        public void GetVncConsole(string typeName)
        {
            var type = StringEnumeration.FromDisplayName<RemoteConsoleType>(typeName);
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server {Id = serverId});
                httpTest.RespondWithJson(new RemoteConsole {Type = type});

                var server = _compute.GetServer(serverId);
                var result = server.GetVncConsole(RemoteConsoleType.NoVnc);
                
                httpTest.ShouldHaveCalled($"*/servers/{serverId}/action");
                Assert.Contains("os-getVNCConsole", httpTest.CallLog.Last().RequestBody);
                Assert.NotNull(result);
                Assert.Equal(type, result.Type);
            }
        }

        [Fact]
        public void GetSpiceConsole()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId });
                httpTest.RespondWithJson(new RemoteConsole { Type = RemoteConsoleType.SpiceHtml5 });

                var server = _compute.GetServer(serverId);
                var result = server.GetSpiceConsole();

                httpTest.ShouldHaveCalled($"*/servers/{serverId}/action");
                Assert.Contains("os-getSPICEConsole", httpTest.CallLog.Last().RequestBody);
                Assert.NotNull(result);
                Assert.Equal(RemoteConsoleType.SpiceHtml5, result.Type);
            }
        }

        [Fact]
        public void GetSerialConsole()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId });
                httpTest.RespondWithJson(new RemoteConsole { Type = RemoteConsoleType.Serial });

                var server = _compute.GetServer(serverId);
                var result = server.GetSerialConsole();

                httpTest.ShouldHaveCalled($"*/servers/{serverId}/action");
                Assert.Contains("os-getSerialConsole", httpTest.CallLog.Last().RequestBody);
                Assert.NotNull(result);
                Assert.Equal(RemoteConsoleType.Serial, result.Type);
            }
        }

        [Fact]
        public void GetRdpConsole()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Server { Id = serverId });
                httpTest.RespondWithJson(new RemoteConsole { Type = RemoteConsoleType.RdpHtml5 });

                var server = _compute.GetServer(serverId);
                var result = server.GetRdpConsole();

                httpTest.ShouldHaveCalled($"*/servers/{serverId}/action");
                Assert.Contains("os-getRDPConsole", httpTest.CallLog.Last().RequestBody);
                Assert.NotNull(result);
                Assert.Equal(RemoteConsoleType.RdpHtml5, result.Type);
            }
        }
    }
}
