using System;
using OpenStack.Synchronous;
using OpenStack.Testing;
using Xunit;

namespace OpenStack.Compute.v2_1
{
    public class KeyPairTests
    {
        private readonly ComputeService _computeService;

        public KeyPairTests()
        {
            _computeService = new ComputeService(Stubs.AuthenticationProvider, "region");
        }
        
        [Fact]
        public void CreateKeyPair()
        {
            using (var httpTest = new HttpTest())
            {
                const string name = "{name}";
                httpTest.RespondWithJson(new KeyPair {Name = name});
                KeyPair result = _computeService.CreateKeyPair(name);

                httpTest.ShouldHaveCalled("*/os-keypair");
                Assert.NotNull(result);
                Assert.Equal(name, result.Name);
            }
        }
    }
}
