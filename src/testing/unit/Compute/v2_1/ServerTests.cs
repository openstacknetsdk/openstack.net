using System;
using System.Linq;
using Newtonsoft.Json;
using OpenStack.Compute.v2_1.Serialization;
using OpenStack.Serialization;
using OpenStack.Synchronous;
using OpenStack.Testing;
using Xunit;

namespace OpenStack.Compute.v2_1
{
    public class ServerTests
    {
        private readonly ComputeService _computeService;

        public ServerTests()
        {
            _computeService = new ComputeService(Stubs.AuthenticationProvider, "region");
        }

        [Fact]
        public void ListServers()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new ServerCollection
                {
                   Items = { new ServerReference {Id = serverId}},
                   Links = { new PageLink("next", "http://api.com/next") }
                });

                var results = _computeService.ListServers();

                httpTest.ShouldHaveCalled("*/servers");
                Assert.Equal(1, results.Count());
                Assert.Equal(serverId, results.First().Id);
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

                _computeService.ListServers(new ListServersOptions { Name = name, FlavorId = flavorId, ImageId = imageId, LastModified = lastModified, Status = status});

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
                _computeService.ListServers(new ListServersOptions { PageSize = pageSize, StartingAt = startingAt });

                httpTest.ShouldHaveCalled($"*marker={startingAt}*");
                httpTest.ShouldHaveCalled($"*limit={pageSize}*");
            }
        }

        [Fact]
        public void GetVncConsole()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Console {Type = ConsoleType.NoVnc});

                Console result = _computeService.GetVncConsole(serverId, ConsoleType.NoVnc);
                
                httpTest.ShouldHaveCalled($"*/servers/{serverId}/action");
                Assert.NotNull(result);
                Assert.Equal(ConsoleType.NoVnc, result.Type);
            }
        }
    }
}
