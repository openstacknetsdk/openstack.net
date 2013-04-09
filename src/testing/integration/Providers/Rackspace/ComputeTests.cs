using System;
using System.Collections.Generic;
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
        private TestContext testContextInstance;
        private static CloudIdentity _testIdentity;
        private static NewServer _testServer;
        private static IEnumerable<ServerImageDetails> _allImages;
        private const string NewPassword = "my_new_password";

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

        [TestMethod]
        public void Test001_Should_Create_A_New_Server_In_DFW()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            _testServer = provider.CreateServer("net-sdk-test-server", "d531a2dd-7ae9-4407-bb5a-e5ea03303d98", "2");

            Assert.IsNotNull(_testServer);
            Assert.IsNotNull(_testServer.Id);
        }

        [TestMethod]
        public void Test002_Should_Get_Details_For_Newly_Created_Server_In_DFW()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var serverDetails = provider.GetDetails(_testServer.Id);

            Assert.IsNotNull(serverDetails);
            Assert.AreEqual("net-sdk-test-server", serverDetails.Name);
            Assert.AreEqual("d531a2dd-7ae9-4407-bb5a-e5ea03303d98", serverDetails.Image.Id);
            Assert.AreEqual("2", serverDetails.Flavor.Id);  
        }

        [TestMethod]
        public void Test003_Should_Wait_Until_Server_Becomes_Active_Or_A_Maximum_Of_10_Minutes()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);

            var serverDetails = provider.WaitForServerActive(_testServer.Id);

            Assert.IsNotNull(serverDetails);
            Assert.AreEqual("ACTIVE", serverDetails.Status);
        }

        [TestMethod]
        public void Test004_Should_Get_A_List_Of_Servers_Which_Should_Include_The_Newly_Created_Server()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var servers = provider.ListServers(identity: _testIdentity);

            Assert.IsNotNull(servers);
            Assert.IsTrue(servers.Any());
            Assert.IsNotNull(servers.FirstOrDefault(s => s.Id == _testServer.Id));
        }

        [TestMethod]
        public void Test005_Should_Get_A_List_Of_Servers_With_Details_Which_Should_Include_The_Newly_Created_Server()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var servers = provider.ListServersWithDetails(identity: _testIdentity);

            Assert.IsNotNull(servers);
            Assert.IsTrue(servers.Any());
            Assert.IsNotNull(servers.FirstOrDefault(s => s.Id == _testServer.Id));
        }

        [TestMethod]
        public void Test006_Should_Get_Empty_Server_Metadata_List()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = provider.ListServerMetadata(_testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 0);
        }

        [TestMethod]
        public void Test007_Should_Set_Initial_Server_Metadata()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = new Metadata() {{"Metadata_Key_1", "My_Value_1"}, {"Metadata_Key_2", "My_Value_2"}};
            var response = provider.SetServerMetadata(_testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test008_Should_Get_The_Initial_Test_Server_Metadata()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = provider.ListServerMetadata(_testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 2);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_1"));
            Assert.AreEqual("My_Value_1", metadata.First(md => md.Key == "Metadata_Key_1").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_2"));
            Assert.AreEqual("My_Value_2", metadata.First(md => md.Key == "Metadata_Key_2").Value);
        }

        [TestMethod]
        public void Test009_Should_Set_Updated_Reset_Server_Metadata()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = new Metadata() { { "Metadata_Key_3", "My_Value_3" }, { "Metadata_Key_4", "My_Value_4" } };
            var response = provider.SetServerMetadata(_testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test010_Should_Get_The_Update_Reset_Test_Server_Metadata()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = provider.ListServerMetadata(_testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 2);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.AreEqual("My_Value_3", metadata.First(md => md.Key == "Metadata_Key_3").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.AreEqual("My_Value_4", metadata.First(md => md.Key == "Metadata_Key_4").Value);
        }

        [TestMethod]
        public void Test011_Should_Set_Update_All_Server_Metadata_Items()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = new Metadata() { { "Metadata_Key_3", "My_Value_3_Updated" }, { "Metadata_Key_4", "My_Value_4_Updated" } };
            var response = provider.UpdateServerMetadata(_testServer.Id, metadata);

            Thread.Sleep(10000);   // Sleep a few seconds because there is an edge case that the system does not reflect the new metadata changes quick enough
            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test012_Should_Get_The_Updated_Test_Server_Metadata()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = provider.ListServerMetadata(_testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 2);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.AreEqual("My_Value_3_Updated", metadata.First(md => md.Key == "Metadata_Key_3").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.AreEqual("My_Value_4_Updated", metadata.First(md => md.Key == "Metadata_Key_4").Value);
        }

        [TestMethod]
        public void Test013_Should_Add_A_New_Metadata_Item()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var response = provider.SetServerMetadataItem(_testServer.Id, "Metadata_Key_5", "My_Value_5");

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test014_Should_Get_The_Test_Server_Metadata_Including_The_New_Item()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = provider.ListServerMetadata(_testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 3);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.AreEqual("My_Value_3_Updated", metadata.First(md => md.Key == "Metadata_Key_3").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.AreEqual("My_Value_4_Updated", metadata.First(md => md.Key == "Metadata_Key_4").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_5"));
            Assert.AreEqual("My_Value_5", metadata.First(md => md.Key == "Metadata_Key_5").Value);
        }

        [TestMethod]
        public void Test015_Should_Set_Update_Two_Of_The_Three_Server_Metadata_Items()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = new Metadata() { { "Metadata_Key_3", "My_Value_3_Updated_Again" }, { "Metadata_Key_5", "My_Value_5_Updated" } };
            var response = provider.UpdateServerMetadata(_testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test016_Should_Get_The_Updated_Test_Server_Metadata_Including_The_Updated_New_Item()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = provider.ListServerMetadata(_testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 3);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.AreEqual("My_Value_3_Updated_Again", metadata.First(md => md.Key == "Metadata_Key_3").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.AreEqual("My_Value_4_Updated", metadata.First(md => md.Key == "Metadata_Key_4").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_5"));
            Assert.AreEqual("My_Value_5_Updated", metadata.First(md => md.Key == "Metadata_Key_5").Value);
        }

        [TestMethod]
        public void Test017_Should_Set_Update_A_Single_Server_Metadata_item()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var response = provider.SetServerMetadataItem(_testServer.Id, "Metadata_Key_4", "My_Value_4_Updated_Again");

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test018_Should_Get_The_Updated_Test_Server_Metadata_Including_The_Updated_Item()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = provider.ListServerMetadata(_testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 3);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.AreEqual("My_Value_3_Updated_Again", metadata.First(md => md.Key == "Metadata_Key_3").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.AreEqual("My_Value_4_Updated_Again", metadata.First(md => md.Key == "Metadata_Key_4").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_5"));
            Assert.AreEqual("My_Value_5_Updated", metadata.First(md => md.Key == "Metadata_Key_5").Value);
        }

        [TestMethod]
        public void Test019_Should_Delete_A_Single_Server_Metadata_item()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider();
            var response = provider.DeleteServerMetadataItem(_testServer.Id, "Metadata_Key_4");

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test020_Should_Get_The_Updated_Test_Server_Metadata_Excluding_The_Deleted_Item()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = provider.ListServerMetadata(_testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 2);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.AreEqual("My_Value_3_Updated_Again", metadata.First(md => md.Key == "Metadata_Key_3").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_5"));
            Assert.AreEqual("My_Value_5_Updated", metadata.First(md => md.Key == "Metadata_Key_5").Value);
        }

        [TestMethod]
        public void Test021_Should_Set_Update_Two_Of_The_Three_Server_Metadata_Items()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = new Metadata() { { "Metadata_Key_4", "My_Value_4" }, { "Metadata_Key_5", "My_Value_5_Updated_Again" } };
            var response = provider.UpdateServerMetadata(_testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test022_Should_Get_The_Updated_Test_Server_Metadata()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var metadata = provider.ListServerMetadata(_testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 3);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.AreEqual("My_Value_3_Updated_Again", metadata.First(md => md.Key == "Metadata_Key_3").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.AreEqual("My_Value_4", metadata.First(md => md.Key == "Metadata_Key_4").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_5"));
            Assert.AreEqual("My_Value_5_Updated_Again", metadata.First(md => md.Key == "Metadata_Key_5").Value);
        }

        [TestMethod]
        public void Test023_Should_Successfully_To_And_Login_With_Old_Password()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var serverDetails = provider.GetDetails(_testServer.Id);
            using(var client = new Renci.SshNet.SshClient(serverDetails.AccessIPv4, "root", _testServer.AdminPassword))
            {
                client.Connect();

                Assert.IsTrue(client.IsConnected);
            }
        }

        [TestMethod]
        public void Test024_Should_Update_The_Admin_Password()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var result = provider.ChangeAdministratorPassword(_testServer.Id, NewPassword);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test025_Should_Successfully_To_And_Login_With_New_Password()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var serverDetails = provider.GetDetails(_testServer.Id);
            bool sucess = false;
            for (int i = 0; i < 10; i++ )
            {
                using (var client = new Renci.SshNet.SshClient(serverDetails.AccessIPv4, "root", NewPassword))
                {
                    client.Connect();

                    sucess = client.IsConnected;

                    if (sucess)
                        break;
                }
                Thread.Sleep(1000);
            }
            
            Assert.IsTrue(sucess);
        }

        [TestMethod]
        public void Test026_Should_Get_List_Of_Base_Images()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImages(imageType: ImageType.Base);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test027_Should_Get_List_Of_Snapshot_Images()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImages(imageType: ImageType.Snapshot);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test028_Should_Get_List_All_Images_Which_should_Equal_Base_And_Snapshot_Combined()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var allImages = provider.ListImages(identity: _testIdentity);
            var baseImages = provider.ListImages(imageType: ImageType.Base);
            var snapImages = provider.ListImages(imageType: ImageType.Snapshot);

            Assert.IsTrue(allImages.Any());
            Assert.IsTrue(allImages.Count() == (baseImages.Count() + snapImages.Count()));
            foreach (var image in allImages)
            {
                Assert.IsTrue(baseImages.Any(i => i.Id.Equals(image.Id, StringComparison.OrdinalIgnoreCase)) ^ snapImages.Any(i => i.Id.Equals(image.Id, StringComparison.OrdinalIgnoreCase)));
            }
        }

        [TestMethod]
        public void Test029_Should_Get_List_All_Images_With_Details()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            _allImages = provider.ListImagesWithDetails(identity: _testIdentity);

            Assert.IsTrue(_allImages.Any());
        }

        [TestMethod]
        public void Test030_Should_Return_One_Image_When_Searching_By_Valid_Id()
        {
            var validImage = _allImages.First(i => i.Server != null);
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImages(serverId: validImage.Server.Id);

            Assert.IsTrue(images.Any());
        }
        
        [TestMethod]
        public void Test031_Should_Return_At_Least_One_Image_When_Searching_By_Valid_Name()
        {
            var validImage = _allImages.First();
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImages(imageName: validImage.Name);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test032_Should_Return_At_Least_One_Image_When_Searching_By_Valid_Status()
        {
            var validImage = _allImages.First();
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImages(imageStatus: validImage.Status);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test033_Should_Return_At_Least_One_Image_When_Searching_By_Valid_Change_Since_Date()
        {
            var validImage = _allImages.First();
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImages(changesSince: validImage.Updated.AddSeconds(-1));

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test034_Should_Return_One_Image_With_Details_When_Searching_By_Valid_Id()
        {
            var validImage = _allImages.First(i => i.Server != null);
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(serverId: validImage.Server.Id);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test035_Should_Return_At_Least_One_Image_With_Details_When_Searching_By_Valid_Name()
        {
            var validImage = _allImages.First();
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(imageName: validImage.Name);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test036_Should_Return_At_Least_One_Image_With_Details_When_Searching_By_Valid_Status()
        {
            var validImage = _allImages.First();
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(imageStatus: validImage.Status);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test037_Should_Return_At_Least_One_Image_With_Details_When_Searching_By_Valid_Change_Since_Date()
        {
            var validImage = _allImages.First();
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(changesSince: validImage.Updated.AddSeconds(-1));

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test038_Should_Return_Exactly_Two_Images()
        {
            var validImage = _allImages.First();
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImages(limit: 2);

            Assert.IsTrue(images.Count() == 2);
        }

        [TestMethod]
        public void Test039_Should_Return_Exactly_Two_Images_With_Details()
        {
            var validImage = _allImages.First();
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(limit: 2);

            Assert.IsTrue(images.Count() == 2);
        }

        [TestMethod]
        public void Test040_Should_Return_All_Images_When_Using_A_Limit_Greater_Than_What_Actually_Exists()
        {
            var validImage = _allImages.First();
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImages(limit: _allImages.Count() * 2);

            Assert.IsTrue(images.Count() == _allImages.Count());
        }

        [TestMethod]
        public void Test041_Should_Return_All_Images_With_Details_When_Using_A_Limit_Greater_Than_What_Actually_Exists()
        {
            var validImage = _allImages.First();
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(limit: _allImages.Count() * 2);

            Assert.IsTrue(images.Count() == _allImages.Count());
        }

        [TestMethod]
        public void Test042_Should_Wait_For_Image_To_Be_ACTIVE()
        {
            
        }

        [TestMethod]
        public void Test100_Should_Mark_The_Server_For_Deletion()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);
            var result = provider.DeleteServer(_testServer.Id);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test101_Should_Wait_A_Max_Of_10_Minutes_For_The_Server_Is_Deleted_Indicated_By_A_Null_Return_Value_For_Details()
        {
            var provider = new net.openstack.Providers.Rackspace.CloudServersProvider(_testIdentity);

            provider.WaitForServerDeleted(_testServer.Id);
        }
    }
}
