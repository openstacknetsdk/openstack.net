using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        #endregion

        #region Update Account

        #endregion

        #region Get Limit
        [TestMethod]
        public void Should_Get_Account_Limits()
        {
            var provider = new CloudMonitoringProvider();
            var accountLimitResponse = provider.GetAccountLimits(null, _testIdentity);
            
            Assert.IsNotNull(accountLimitResponse.Resource);
            Assert.AreEqual(accountLimitResponse.Resource.Alarms, 100);
            Assert.AreEqual(accountLimitResponse.Resource.Checks, 100);

            Assert.IsNotNull(accountLimitResponse.Rate.Global);
            Assert.AreEqual(accountLimitResponse.Rate.Global.Limit, 50000);
            Assert.AreEqual(accountLimitResponse.Rate.Global.Window, "24 hours");

            Assert.IsNotNull(accountLimitResponse.Rate.TestAlarm);
            Assert.AreEqual(accountLimitResponse.Rate.TestAlarm.Limit, 500);
            Assert.AreEqual(accountLimitResponse.Rate.TestAlarm.Window, "24 hours");

            Assert.IsNotNull(accountLimitResponse.Rate.TestCheck);
            Assert.AreEqual(accountLimitResponse.Rate.TestCheck.Limit, 500);
            Assert.AreEqual(accountLimitResponse.Rate.TestCheck.Window, "24 hours");

            Assert.IsNotNull(accountLimitResponse.Rate.TestNotification);
            Assert.AreEqual(accountLimitResponse.Rate.TestNotification.Limit, 200);
            Assert.AreEqual(accountLimitResponse.Rate.TestNotification.Window, "24 hours");

            Assert.IsNotNull(accountLimitResponse.Rate.Traceroute);
            Assert.AreEqual(accountLimitResponse.Rate.Traceroute.Limit, 300);
            Assert.AreEqual(accountLimitResponse.Rate.Traceroute.Window, "24 hours");
        }

        [TestMethod]
        public void Should_Fail_Authentication()
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
                var accountLimitResponse = provider.GetAccountLimits(null, badIdentity);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UserNotAuthorizedException);
            }
            
        }

        #endregion

        #region List Audits

        #endregion

    #endregion
    }
}
