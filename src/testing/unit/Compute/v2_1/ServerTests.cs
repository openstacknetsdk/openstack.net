using System;
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

        [Theory]
        [InlineData(ConsoleType.NoVnc)]
        [InlineData(ConsoleType.XpVnc)]
        public void GetVncConsole(ConsoleType type)
        {
            using (var httpTest = new HttpTest())
            {
                Identifier serverId = Guid.NewGuid();
                httpTest.RespondWithJson(new Console {Type = type});

                Console result = _computeService.GetVncConsole(serverId, type);
                
                httpTest.ShouldHaveCalled($"*/servers/{serverId}/action");
                Assert.NotNull(result);
                Assert.Equal(type, result.Type);
            }
        }
    }
}
