namespace OpenStackNetTests.Live
{
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using OpenStackNetTests.Unit;
    using OpenStackNetTests.Unit.Simulator.IdentityService.V2;
    using OpenStackNetTests.Unit.Simulator.ObjectStorageService.V1;

    [TestClass]
    public partial class ObjectStorageTests
    {
        private SimulatedIdentityService _identityService;
        private SimulatedObjectStorageService _objectStorageService;

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
            _identityService = new SimulatedIdentityService();
            _identityService.StartAsync(CancellationToken.None);

            _objectStorageService = new SimulatedObjectStorageService(_identityService);
            _objectStorageService.StartAsync(CancellationToken.None);
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
