using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Core.Domain;

namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    /// <summary>
    /// Summary description for ComputeTests
    /// </summary>
    [TestClass]
    public class ComputeTests
    {
        public ComputeTests()
        {
            _testIdentity = new CloudIdentity()
            {
                APIKey = ConfigurationManager.AppSettings["TestIdentityAPIKey"],
                Password = ConfigurationManager.AppSettings["TestIdentityPassword"],
                Region = ConfigurationManager.AppSettings["TestIdentityRegion"],
                Username = ConfigurationManager.AppSettings["TestIdentityUserName"],
            };
        }

        private TestContext testContextInstance;
        private static CloudIdentity _testIdentity;
        private static NewServer _testServer;

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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Test1_Should_Create_A_New_Server_In_DFW()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            _testServer = provider.CreateServer("test-server", "My Test Server", "d531a2dd-7ae9-4407-bb5a-e5ea03303d98", "2", _testIdentity);

            Assert.IsNotNull(_testServer);
            Assert.IsNotNull(_testServer.Id);
        }

        [TestMethod]
        public void Test2_Should_Get_Details_For_Newly_Created_Server_In_DFW()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var serverDetails = provider.GetDetails(_testServer.Id, _testIdentity);

            Assert.IsNotNull(serverDetails);
            Assert.AreEqual("test-server", serverDetails.Name);
            Assert.AreEqual("d531a2dd-7ae9-4407-bb5a-e5ea03303d98", serverDetails.Image.Id);
            Assert.AreEqual("2", serverDetails.Flavor.Id);
        }

        [TestMethod]
        public void Test3_Should_Wait_Until_Server_Becomes_Active_Or_A_Maximum_Of_10_Minutes()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var serverDetails = provider.GetDetails(_testServer.Id, _testIdentity);
            Assert.IsNotNull(serverDetails);
            
            int count = 0;
            while (!serverDetails.Status.Equals("ACTIVE") && !serverDetails.Status.Equals("ERROR") && !serverDetails.Status.Equals("UNKNOWN") && !serverDetails.Status.Equals("SUSPENDED") && count < 600)
            {
                Thread.Sleep(1000);
                serverDetails = provider.GetDetails(_testServer.Id, _testIdentity);
                count++;
            }

            Assert.IsNotNull(serverDetails);
            Assert.AreEqual("ACTIVE", serverDetails.Status);
        }

        [TestMethod]
        public void Test4_Should_Get_A_List_Of_Servers_Which_Should_Include_The_Newly_Created_Server()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var servers = provider.ListServers(_testIdentity);

            Assert.IsNotNull(servers);
            Assert.IsTrue(servers.Any());
            Assert.IsNotNull(servers.FirstOrDefault(s => s.Id == _testServer.Id));
        }

        [TestMethod]
        public void Test5_Should_Mark_The_Server_For_Deletion()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var result = provider.DeleteServer(_testServer.Id, _testIdentity);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test6_Should_Wait_A_Max_Of_10_Minutes_For_The_Server_Is_Deleted_Indicated_By_A_Null_Return_Value_For_Details()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var details = provider.GetDetails(_testServer.Id, _testIdentity);
            Assert.IsNotNull(details);
            
            while (details != null && (!details.Status.Equals("DELETED") && !details.Status.Equals("ERROR") && !details.Status.Equals("UNKNOWN") && !details.Status.Equals("SUSPENDED")))
            {
                Thread.Sleep(1000);
                details = provider.GetDetails(_testServer.Id, _testIdentity);
            }

            if(details != null)
                Assert.AreEqual("DELETED", details.Status);
        }
    }
}
