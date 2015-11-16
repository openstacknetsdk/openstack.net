using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        [Theory]
        [InlineData(ConsoleProtocol.VNC, ConsoleType.NoVnc)]
        [InlineData(ConsoleProtocol.VNC, ConsoleType.XpVnc)]
        [InlineData(ConsoleProtocol.RDP, ConsoleType.RdpHtml5)]
        [InlineData(ConsoleProtocol.Serial, ConsoleType.Serial)]
        [InlineData(ConsoleProtocol.Spice, ConsoleType.SpiceHtml5)]
        public void GetConsole(ConsoleProtocol protocol, ConsoleType type)
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Console {Type = type});

                Console result = _computeService.GetConsole(serverId, protocol, type);
                
                httpTest.ShouldHaveCalled($"*/servers/{serverId}/remote-consoles");
                Assert.NotNull(result);
                Assert.Equal(type, result.Type);
            }
        }
    }
}
