namespace OpenStackNetTests.Live
{
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using OpenStackNetTests.Unit;
    using OpenStackNetTests.Unit.Simulator.IdentityService;

    [TestClass]
    public partial class BaseIdentityTests
    {
        private SimulatedBaseIdentityService _simulator;

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
            _simulator = new SimulatedBaseIdentityService(5000);
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
