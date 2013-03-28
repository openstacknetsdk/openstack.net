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
    }
}
