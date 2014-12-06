namespace OpenStackNetTests.Live
{
    using System;
    using Microsoft.Owin.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using OpenStackNetTests.Unit;
    using OpenStackNetTests.Unit.Simulator.IdentityService.V2;
    using OpenStackNetTests.Unit.Simulator.Networking.V2;

    [TestClass]
    public partial class NetworkingV2Tests
    {
        private IDisposable _identityService;
        private IDisposable _networkingService;

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
            _identityService = WebApp.Start<IdentityServiceConfiguration>("http://localhost:5000");
            _networkingService = WebApp.Start<NetworkingServiceConfiguration>("http://localhost:9696");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _identityService.Dispose();
            _identityService = null;

            _networkingService.Dispose();
            _networkingService = null;
        }
    }
}
