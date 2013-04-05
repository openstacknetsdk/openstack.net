using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;

namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    [TestClass]
    public class CloudBlockStorageTests
    {
        private static RackspaceCloudIdentity _testIdentity;
        private static RackspaceCloudIdentity _testAdminIdentity;
        private TestContext testContextInstance;
        private const string volumeDisplayName = "Test Volume";
        private const string volumeDisplayDescription = "Test Volume Description";

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }   

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _testIdentity = new RackspaceCloudIdentity(Bootstrapper.Settings.TestIdentity);

        }


        #region Create Volume Tests
        [TestMethod]
        public void Should_Create_Volume_Only_Required_Parameters()
        {
            var provider = new CloudBlockStorageProvider();
            var volumeCreatedResponse = provider.CreateVolume(100, identity: _testIdentity);
            Assert.IsTrue(volumeCreatedResponse);
        }

        [TestMethod]
        public void Should_Create_Volume_Full_Parameters()
        {
            var provider = new CloudBlockStorageProvider();
            var volumeCreatedResponse = provider.CreateVolume(100, volumeDisplayDescription, volumeDisplayName, null, CloudBlockStorageVolumeType.SATA, null, _testIdentity);
            Assert.IsTrue(volumeCreatedResponse);
        } 
        #endregion
    }
}
