namespace OpenStackNetTests.Live
{
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using OpenStackNetTests.Unit;
    using OpenStackNetTests.Unit.Simulator.IdentityService.V2;

    [TestClass]
    public partial class IdentityV2Tests
    {
        private SimulatedIdentityService _simulator;

        internal TestCredentials Credentials
        {
            get
            {
                return JsonConvert.DeserializeObject<TestCredentials>(Resources.SimulatedCredentials);
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _simulator = new SimulatedIdentityService();
            _simulator.StartAsync(CancellationToken.None);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _simulator.Dispose();
            _simulator = null;
        }
    }
}
