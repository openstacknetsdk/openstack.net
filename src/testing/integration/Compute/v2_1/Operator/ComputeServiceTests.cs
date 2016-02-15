using System.Diagnostics;
using OpenStack.Serialization;
using Xunit;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace OpenStack.Compute.v2_1.Operator
{
    public class ComputeServiceTests
    {
        private readonly ComputeService _compute;

        public ComputeServiceTests(ITestOutputHelper testLog)
        {
            var testOutput = new XunitTraceListener(testLog);
            Trace.Listeners.Add(testOutput);
            OpenStackNet.Tracing.Http.Listeners.Add(testOutput);

            var authenticationProvider = TestIdentityProvider.GetOperatorIdentity();
            _compute = new ComputeService(authenticationProvider, "RegionOne");
        }

        public void Dispose()
        {
            Trace.Listeners.Clear();
            OpenStackNet.Tracing.Http.Listeners.Clear();
        }

        [Fact]
        public async Task GetCurrentQuotas()
        {
            var quotas = await _compute.GetCurrentQuotasAsync();
            Assert.NotNull(quotas);
            Assert.Equal("detail", quotas.Id);
            Assert.Empty(((IHaveExtraData)quotas).Data);
        }

        [Fact]
        public async Task GetDefaultQuotas()
        {
            var quotas = await _compute.GetDefaultQuotasAsync();
            Assert.NotNull(quotas);
            Assert.Equal("defaults", quotas.Id);
            Assert.Empty(((IHaveExtraData)quotas).Data);
        }
    }
}
