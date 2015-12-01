using System;
using OpenStack.Synchronous;
using OpenStack.Testing;
using Xunit;

namespace OpenStack.Compute.v2_6
{
    public class ServerTests
    {
        private readonly ComputeService _computeService;

        public ServerTests()
        {
            _computeService = new ComputeService(Stubs.AuthenticationProvider, "region");
        }

        [Fact]
        public void GetConsole()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Console {Type = ConsoleType.NoVnc});

                Console result = _computeService.GetConsole(serverId, ConsoleProtocol.VNC, ConsoleType.NoVnc);
                
                httpTest.ShouldHaveCalled($"*/servers/{serverId}/remote-consoles");
                Assert.NotNull(result);
                Assert.Equal(ConsoleType.NoVnc, result.Type);
            }
        }
    }
}
