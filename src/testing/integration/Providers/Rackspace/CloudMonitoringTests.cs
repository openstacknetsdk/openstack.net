using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Providers.Rackspace;

namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    [TestClass]
    public class CloudMonitoringTests
    {
        private static RackspaceCloudIdentity _testIdentity;
        private static RackspaceCloudIdentity _testAdminIdentity;
        private TestContext testContextInstance;

        private const string volumeDisplayName = "Integration Test Volume";
        private const string volumeDisplayDescription = "Integration Test Volume Description";
        private const string snapshotDisplayName = "Integration Test Snapshot";
        private const string snapshotDisplayDescription = "Integration Test Snapshot Description";

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

        #region Account
        #region Get Account

        [TestMethod]
        public void Should_Get_Account_Information()
        {
            var provider = new CloudMonitoringProvider();
            var accountInformationResponse = provider.GetAccountInformation(null, _testIdentity);

            Assert.IsNotNull(accountInformationResponse.Id);
            Assert.AreEqual(accountInformationResponse.Id, "ac7SzakhPj");

            Assert.IsNotNull(accountInformationResponse.WebhookToken);
            Assert.AreEqual(accountInformationResponse.WebhookToken, "qq4KBwlXbJvxERwTg9nNtudRcGidAABY");

        }
        
        [TestMethod]
        public void Should_Fail_Authentication_Account_Information()
        {
            var provider = new CloudMonitoringProvider();
            var badIdentity = new RackspaceCloudIdentity()
                                  {
                                      APIKey = "123123123",
                                      Username = "rooty",
                                      Password = "h0peN0OneKn0wzM3"
                                  };            
            try
            {
                var accountInformationResponse = provider.GetAccountInformation(null, badIdentity);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UserNotAuthorizedException);
            }
            
        }
        #endregion

        #region Update Account

        #endregion

        #region Get Limit

        #endregion

        #region List Audits

        #endregion

        #endregion
    }
}
