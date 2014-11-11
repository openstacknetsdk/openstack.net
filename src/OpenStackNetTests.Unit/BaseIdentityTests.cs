namespace OpenStackNetTests.Live
{
    using System;
    using Microsoft.Owin.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using OpenStackNetTests.Unit;
    using OpenStackNetTests.Unit.Simulator.IdentityService;

    [TestClass]
    public partial class BaseIdentityTests
    {
        private IDisposable _simulator;

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
            _simulator = WebApp.Start<BaseIdentityServiceConfiguration>("http://localhost:5000");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _simulator.Dispose();
            _simulator = null;
        }
    }
}
