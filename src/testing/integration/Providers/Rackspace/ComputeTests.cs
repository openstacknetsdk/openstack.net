using System.Configuration;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;

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
            CloudInstance cloudInstance;
            CloudInstance.TryParse(ConfigurationManager.AppSettings["TestIdentityGeo"], true, out cloudInstance);

            _testIdentity = new RackspaceCloudIdentity
            {
                APIKey = ConfigurationManager.AppSettings["TestIdentityAPIKey"],
                Password = ConfigurationManager.AppSettings["TestIdentityPassword"],
                CloudInstance = cloudInstance,
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

        [TestMethod]
        public void Test1_Should_Create_A_New_Server_In_DFW()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            _testServer = provider.CreateServer(_testIdentity, "net-sdk-test-server", "d531a2dd-7ae9-4407-bb5a-e5ea03303d98", "2");

            Assert.IsNotNull(_testServer);
            Assert.IsNotNull(_testServer.Id);
        }

        [TestMethod]
        public void Test2_Should_Get_Details_For_Newly_Created_Server_In_DFW()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var serverDetails = provider.GetDetails(_testIdentity, _testServer.Id);

            Assert.IsNotNull(serverDetails);
            Assert.AreEqual("net-sdk-test-server", serverDetails.Name);
            Assert.AreEqual("d531a2dd-7ae9-4407-bb5a-e5ea03303d98", serverDetails.Image.Id);
            Assert.AreEqual("2", serverDetails.Flavor.Id);
        }

        [TestMethod]
        public void Test3_Should_Wait_Until_Server_Becomes_Active_Or_A_Maximum_Of_10_Minutes()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var serverDetails = provider.GetDetails(_testIdentity, _testServer.Id);
            Assert.IsNotNull(serverDetails);
            
            int count = 0;
            while (!serverDetails.Status.Equals("ACTIVE") && !serverDetails.Status.Equals("ERROR") && !serverDetails.Status.Equals("UNKNOWN") && !serverDetails.Status.Equals("SUSPENDED") && count < 600)
            {
                Thread.Sleep(2400);
                serverDetails = provider.GetDetails(_testIdentity, _testServer.Id);
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
        public void Test5_Should_Get_A_List_Of_Servers_With_Details_Which_Should_Include_The_Newly_Created_Server()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var servers = provider.ListServersWithDetails(_testIdentity);

            Assert.IsNotNull(servers);
            Assert.IsTrue(servers.Any());
            Assert.IsNotNull(servers.FirstOrDefault(s => s.Id == _testServer.Id));
        }

        [TestMethod]
        public void Test6_Should_Get_Empty_Server_Metadata_List()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = provider.ListServerMetadata(_testIdentity, _testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 0);
        }

        [TestMethod]
        public void Test7_Should_Set_Initial_Server_Metadata()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = new Metadata() {{"Metadata_Key_1", "My_Value_1"}, {"Metadata_Key_2", "My_Value_2"}};
            var response = provider.SetServerMetadata(_testIdentity, _testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test8_Should_Get_The_Initial_Test_Server_Metadata()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = provider.ListServerMetadata(_testIdentity, _testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 2);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_1"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_1").Value == "My_Value_1");
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_2"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_2").Value == "My_Value_2");
        }

        [TestMethod]
        public void Test9_Should_Set_Updated_Reset_Server_Metadata()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = new Metadata() { { "Metadata_Key_3", "My_Value_3" }, { "Metadata_Key_4", "My_Value_4" } };
            var response = provider.SetServerMetadata(_testIdentity, _testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test10_Should_Get_The_Update_Reset_Test_Server_Metadata()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = provider.ListServerMetadata(_testIdentity, _testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 2);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_3").Value == "My_Value_3");
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_4").Value == "My_Value_4");
        }

        [TestMethod]
        public void Test11_Should_Set_Update_All_Server_Metadata_Items()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = new Metadata() { { "Metadata_Key_3", "My_Value_3_Updated" }, { "Metadata_Key_4", "My_Value_4_Updated" } };
            var response = provider.UpdateServerMetadata(_testIdentity, _testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test12_Should_Get_The_Updated_Test_Server_Metadata()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = provider.ListServerMetadata(_testIdentity, _testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 2);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_3").Value == "My_Value_3_Updated");
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_4").Value == "My_Value_4_Updated");
        }

        [TestMethod]
        public void Test13_Should_Add_A_New_Metadata_Item()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var response = provider.SetServerMetadataItem(_testIdentity, _testServer.Id, "Metadata_Key_5", "My_Value_5");

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test14_Should_Get_The_Test_Server_Metadata_Including_The_New_Item()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = provider.ListServerMetadata(_testIdentity, _testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 3);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_3").Value == "My_Value_3_Updated");
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_4").Value == "My_Value_4_Updated");
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_5"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_5").Value == "My_Value_5");
        }

        [TestMethod]
        public void Test15_Should_Set_Update_Two_Of_The_Three_Server_Metadata_Items()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = new Metadata() { { "Metadata_Key_3", "My_Value_3_Updated_Again" }, { "Metadata_Key_5", "My_Value_5_Updated" } };
            var response = provider.UpdateServerMetadata(_testIdentity, _testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test16_Should_Get_The_Updated_Test_Server_Metadata_Including_The_Updated_New_Item()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = provider.ListServerMetadata(_testIdentity, _testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 3);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_3").Value == "My_Value_3_Updated_Again");
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_4").Value == "My_Value_4_Updated");
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_5"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_5").Value == "My_Value_5_Updated");
        }

        [TestMethod]
        public void Test17_Should_Set_Update_A_Single_Server_Metadata_item()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var response = provider.SetServerMetadataItem(_testIdentity, _testServer.Id, "Metadata_Key_4", "My_Value_4_Updated_Again");

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test18_Should_Get_The_Updated_Test_Server_Metadata_Including_The_Updated_Item()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = provider.ListServerMetadata(_testIdentity, _testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 3);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_3").Value == "My_Value_3_Updated_Again");
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_4").Value == "My_Value_4_Updated_Again");
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_5"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_5").Value == "My_Value_5_Updated");
        }

        [TestMethod]
        public void Test19_Should_Delete_A_Single_Server_Metadata_item()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var response = provider.DeleteServerMetadataItem(_testIdentity, _testServer.Id, "Metadata_Key_4");

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test20_Should_Get_The_Updated_Test_Server_Metadata_Excluding_The_Deleted_Item()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = provider.ListServerMetadata(_testIdentity, _testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 2);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_3").Value == "My_Value_3_Updated_Again");
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_5"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_5").Value == "My_Value_5_Updated");
        }

        [TestMethod]
        public void Test21_Should_Set_Update_Two_Of_The_Three_Server_Metadata_Items()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = new Metadata() { { "Metadata_Key_4", "My_Value_4" }, { "Metadata_Key_5", "My_Value_5_Updated_Again" } };
            var response = provider.UpdateServerMetadata(_testIdentity, _testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test22_Should_Get_The_Updated_Test_Server_Metadata()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var metadata = provider.ListServerMetadata(_testIdentity, _testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 3);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_3").Value == "My_Value_3_Updated_Again");
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_4").Value == "My_Value_4");
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_5"));
            Assert.IsTrue(metadata.First(md => md.Key == "Metadata_Key_5").Value == "My_Value_5_Updated_Again");
        }

        [TestMethod]
        public void Test23_Should_Mark_The_Server_For_Deletion()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var result = provider.DeleteServer(_testIdentity, _testServer.Id);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test24_Should_Wait_A_Max_Of_10_Minutes_For_The_Server_Is_Deleted_Indicated_By_A_Null_Return_Value_For_Details()
        {
            var provider = new net.openstack.Providers.Rackspace.ComputeProvider();
            var details = provider.GetDetails(_testIdentity, _testServer.Id);
            Assert.IsNotNull(details);
            
            while (details != null && (!details.Status.Equals("DELETED") && !details.Status.Equals("ERROR") && !details.Status.Equals("UNKNOWN") && !details.Status.Equals("SUSPENDED")))
            {
                Thread.Sleep(1000);
                details = provider.GetDetails(_testIdentity, _testServer.Id);
            }

            if(details != null)
                Assert.AreEqual("DELETED", details.Status);
        }
    }
}
