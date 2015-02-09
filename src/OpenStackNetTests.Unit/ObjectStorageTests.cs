namespace OpenStackNetTests.Live
{
    using System;
    using Microsoft.Owin.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using OpenStackNetTests.Unit;
    using OpenStackNetTests.Unit.Simulator.IdentityService.V2;
    using OpenStackNetTests.Unit.Simulator.ObjectStorageService.V1;

    [TestClass]
    public partial class ObjectStorageTests
    {
        private IDisposable _identityService;
        private IDisposable _objectStorageService;

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
            _objectStorageService = WebApp.Start<ObjectStorageServiceConfiguration>("http://localhost:8080");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _identityService.Dispose();
            _identityService = null;

            _objectStorageService.Dispose();
            _objectStorageService = null;
        }
    }
}
