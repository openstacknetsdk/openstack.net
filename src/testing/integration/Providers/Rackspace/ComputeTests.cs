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
        private static NewServer _testServer2;
        private static bool _rebootSuccess;
        private static bool _resizeSuccess;
        private static bool _confirmResizeSuccess;
        private static ServerDetails _rebuildServer;
        private static ServerImage _testImage;
        private static string _testImageName;
        private static bool _revertResizeSuccess;
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

        [Timeout(1800000), TestMethod]
        public void Test001_Should_Create_A_New_Server_In_DFW()
        {
            var provider = new ComputeProvider(_testIdentity);
            _testServer = provider.CreateServer("net-sdk-test-server", "d531a2dd-7ae9-4407-bb5a-e5ea03303d98", "2");

            Assert.IsNotNull(_testServer);
            Assert.IsNotNull(_testServer.Id);
        }

        [TestMethod]
        public void Should_Create_A_New_Complex_Server_In_DFW()
        {
            var provider = new ComputeProvider(_testIdentity);
            _testServer2 = provider.CreateServer("net-sdk-test-server2", "d531a2dd-7ae9-4407-bb5a-e5ea03303d98", "2", metadata: new Metadata{{"Key1", "Value1"}, {"Key2", "Value2"}}, attachToPublicNetwork: true, attachToServiceNetwork: true);

            Assert.IsNotNull(_testServer2);
            Assert.IsNotNull(_testServer2.Id);
        }

        [TestMethod]
        public void Test002_Should_Get_Details_For_Newly_Created_Server_In_DFW()
        {
            var provider = new ComputeProvider(_testIdentity);
            var serverDetails = provider.GetDetails(_testServer.Id);

            Assert.IsNotNull(serverDetails);
            Assert.AreEqual("net-sdk-test-server", serverDetails.Name);
            Assert.AreEqual("d531a2dd-7ae9-4407-bb5a-e5ea03303d98", serverDetails.Image.Id);
            Assert.AreEqual("2", serverDetails.Flavor.Id);  
        }

        [TestMethod]
        public void Should_Get_Details_For_Newly_Created_Complex_Server_In_DFW()
        {
            var provider = new ComputeProvider(_testIdentity);
            var serverDetails = provider.GetDetails(_testServer2.Id);

            Assert.IsNotNull(serverDetails);
            Assert.AreEqual("net-sdk-test-server2", serverDetails.Name);
            Assert.AreEqual("d531a2dd-7ae9-4407-bb5a-e5ea03303d98", serverDetails.Image.Id);
            Assert.AreEqual("2", serverDetails.Flavor.Id);
        }

        [TestMethod]
        public void Test003_Should_Wait_Until_Server_Becomes_Active_Or_A_Maximum_Of_10_Minutes()
        {
            var provider = new ComputeProvider(_testIdentity);

            var serverDetails = provider.WaitForServerActive(_testServer.Id);

            Assert.IsNotNull(serverDetails);
            Assert.AreEqual("ACTIVE", serverDetails.Status);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_Until_Server_Becomes_Active_Or_A_Maximum_Of_10_Minutes_For_Server2()
        {
            var provider = new ComputeProvider(_testIdentity);

            var serverDetails = provider.WaitForServerActive(_testServer2.Id);

            Assert.IsNotNull(serverDetails);
            Assert.AreEqual("ACTIVE", serverDetails.Status);
        }

        [TestMethod]
        public void Test004_Should_Get_A_List_Of_Servers_Which_Should_Include_The_Newly_Created_Server()
        {
            var provider = new ComputeProvider(_testIdentity);
            var servers = provider.ListServers(identity: _testIdentity);

            Assert.IsNotNull(servers);
            Assert.IsTrue(servers.Any());
            Assert.IsNotNull(servers.FirstOrDefault(s => s.Id == _testServer.Id));
        }

        [TestMethod]
        public void Test005_Should_Get_A_List_Of_Servers_With_Details_Which_Should_Include_The_Newly_Created_Server()
        {
            var provider = new ComputeProvider(_testIdentity);
            var servers = provider.ListServersWithDetails(identity: _testIdentity);

            Assert.IsNotNull(servers);
            Assert.IsTrue(servers.Any());
            Assert.IsNotNull(servers.FirstOrDefault(s => s.Id == _testServer.Id));
        }

        [TestMethod]
        public void Test006_Should_Get_Empty_Server_Metadata_List()
        {
            var provider = new ComputeProvider(_testIdentity);
            var metadata = provider.ListServerMetadata(_testServer.Id);

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 0);
        }

        [TestMethod]
        public void Test007_Should_Set_Initial_Server_Metadata()
        {
            var provider = new ComputeProvider(_testIdentity);
            var metadata = new Metadata() {{"Metadata_Key_1", "My_Value_1"}, {"Metadata_Key_2", "My_Value_2"}};
            var response = provider.SetServerMetadata(_testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test008_Should_Get_The_Initial_Test_Server_Metadata()
        {
            var provider = new ComputeProvider(_testIdentity);
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
            var provider = new ComputeProvider(_testIdentity);
            var metadata = new Metadata() { { "Metadata_Key_3", "My_Value_3" }, { "Metadata_Key_4", "My_Value_4" } };
            var response = provider.SetServerMetadata(_testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test010_Should_Get_The_Update_Reset_Test_Server_Metadata()
        {
            var provider = new ComputeProvider(_testIdentity);
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
            var provider = new ComputeProvider(_testIdentity);
            var metadata = new Metadata() { { "Metadata_Key_3", "My_Value_3_Updated" }, { "Metadata_Key_4", "My_Value_4_Updated" } };
            var response = provider.UpdateServerMetadata(_testServer.Id, metadata);

            Thread.Sleep(10000);   // Sleep a few seconds because there is an edge case that the system does not reflect the new metadata changes quick enough
            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test012_Should_Get_The_Updated_Test_Server_Metadata()
        {
            var provider = new ComputeProvider(_testIdentity);
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
            var provider = new ComputeProvider(_testIdentity);
            var response = provider.SetServerMetadataItem(_testServer.Id, "Metadata_Key_5", "My_Value_5");

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test014_Should_Get_The_Test_Server_Metadata_Including_The_New_Item()
        {
            var provider = new ComputeProvider(_testIdentity);
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
            var provider = new ComputeProvider(_testIdentity);
            var metadata = new Metadata() { { "Metadata_Key_3", "My_Value_3_Updated_Again" }, { "Metadata_Key_5", "My_Value_5_Updated" } };
            var response = provider.UpdateServerMetadata(_testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test016_Should_Get_The_Updated_Test_Server_Metadata_Including_The_Updated_New_Item()
        {
            var provider = new ComputeProvider(_testIdentity);
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
            var provider = new ComputeProvider(_testIdentity);
            var response = provider.SetServerMetadataItem(_testServer.Id, "Metadata_Key_4", "My_Value_4_Updated_Again");

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test018_Should_Get_The_Updated_Test_Server_Metadata_Including_The_Updated_Item()
        {
            var provider = new ComputeProvider(_testIdentity);
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
            var provider = new ComputeProvider(_testIdentity);
            var response = provider.DeleteServerMetadataItem(_testServer.Id, "Metadata_Key_4");

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test020_Should_Get_The_Updated_Test_Server_Metadata_Excluding_The_Deleted_Item()
        {
            var provider = new ComputeProvider(_testIdentity);
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
            var provider = new ComputeProvider(_testIdentity);
            var metadata = new Metadata() { { "Metadata_Key_4", "My_Value_4" }, { "Metadata_Key_5", "My_Value_5_Updated_Again" } };
            var response = provider.UpdateServerMetadata(_testServer.Id, metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Test022_Should_Get_The_Updated_Test_Server_Metadata()
        {
            var provider = new ComputeProvider(_testIdentity);
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
            var provider = new ComputeProvider(_testIdentity);
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
            var provider = new ComputeProvider(_testIdentity);
            var result = provider.ChangeAdministratorPassword(_testServer.Id, NewPassword);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test025_Should_Successfully_To_And_Login_With_New_Password()
        {
            var provider = new ComputeProvider(_testIdentity);
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
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImages(imageType: ImageType.Base);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test027_Should_Get_List_Of_Snapshot_Images()
        {
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImages(imageType: ImageType.Snapshot);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test028_Should_Get_List_All_Images_Which_should_Equal_Base_And_Snapshot_Combined()
        {
            var provider = new ComputeProvider(_testIdentity);
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
            var provider = new ComputeProvider(_testIdentity);
            _allImages = provider.ListImagesWithDetails(identity: _testIdentity);

            Assert.IsTrue(_allImages.Any());
        }

        [TestMethod]
        public void Test030_Should_Return_One_Image_When_Searching_By_Valid_Id()
        {
            var validImage = _allImages.First(i => i.Server != null);
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImages(serverId: validImage.Server.Id);

            Assert.IsTrue(images.Any());
        }
        
        [TestMethod]
        public void Test031_Should_Return_At_Least_One_Image_When_Searching_By_Valid_Name()
        {
            var validImage = _allImages.First();
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImages(imageName: validImage.Name);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test032_Should_Return_At_Least_One_Image_When_Searching_By_Valid_Status()
        {
            var validImage = _allImages.First();
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImages(imageStatus: validImage.Status);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test033_Should_Return_At_Least_One_Image_When_Searching_By_Valid_Change_Since_Date()
        {
            var validImage = _allImages.First();
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImages(changesSince: validImage.Updated.AddSeconds(-1));

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test034_Should_Return_One_Image_With_Details_When_Searching_By_Valid_Id()
        {
            var validImage = _allImages.First(i => i.Server != null);
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(serverId: validImage.Server.Id);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test035_Should_Return_At_Least_One_Image_With_Details_When_Searching_By_Valid_Name()
        {
            var validImage = _allImages.First();
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(imageName: _testImageName);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test036_Should_Return_At_Least_One_Image_With_Details_When_Searching_By_Valid_Status()
        {
            var validImage = _allImages.First();
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(imageStatus: validImage.Status);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test037_Should_Return_At_Least_One_Image_With_Details_When_Searching_By_Valid_Change_Since_Date()
        {
            var validImage = _allImages.First();
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(changesSince: validImage.Updated.AddSeconds(-1));

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Test038_Should_Return_Exactly_Two_Images()
        {
            var validImage = _allImages.First();
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImages(limit: 2);

            Assert.IsTrue(images.Count() == 2);
        }

        [TestMethod]
        public void Test039_Should_Return_Exactly_Two_Images_With_Details()
        {
            var validImage = _allImages.First();
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(limit: 2);

            Assert.IsTrue(images.Count() == 2);
        }

        [TestMethod]
        public void Test040_Should_Return_All_Images_When_Using_A_Limit_Greater_Than_What_Actually_Exists()
        {
            var validImage = _allImages.First();
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImages(limit: _allImages.Count() * 2);

            Assert.IsTrue(images.Count() == _allImages.Count());
        }

        [TestMethod]
        public void Test041_Should_Return_All_Images_With_Details_When_Using_A_Limit_Greater_Than_What_Actually_Exists()
        {
            var validImage = _allImages.First();
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(limit: _allImages.Count() * 2);

            Assert.IsTrue(images.Count() == _allImages.Count());
        }

        [TestMethod]
        public void Should_Contain_AccessIP_v4_Address()
        {
            var provider = new ComputeProvider(_testIdentity);
            var server = provider.GetDetails(_testServer.Id);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(server.AccessIPv4));
        }

        [TestMethod]
        public void Should_Contain_AccessIP_v6_Address()
        {
            var provider = new ComputeProvider(_testIdentity);
            var server = provider.GetDetails(_testServer.Id);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(server.AccessIPv6));
        }

        [TestMethod]
        public void Should_Contain_AccessIP_v4_Address_For_Server2()
        {
            var provider = new ComputeProvider(_testIdentity);
            var server = provider.GetDetails(_testServer2.Id);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(server.AccessIPv4));
        }

        [TestMethod]
        public void Should_Contain_AccessIP_v6_Address_For_Server2()
        {
            var provider = new ComputeProvider(_testIdentity);
            var server = provider.GetDetails(_testServer2.Id);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(server.AccessIPv6));
        }

        [TestMethod]
        public void Should_Contain_Network_Addresses_For_Server2()
        {
            var provider = new ComputeProvider(_testIdentity);
            var server = provider.GetDetails(_testServer2.Id);

            Assert.IsTrue(server.Addresses.Public.Any());
            Assert.IsTrue(server.Addresses.Private.Any());
        }

        [TestMethod]
        public void Should_Match_AccessIPv4_and_Public_Network_IP()
        {
            var provider = new ComputeProvider(_testIdentity);
            var server = provider.GetDetails(_testServer2.Id);

            Assert.AreEqual(server.AccessIPv4, server.Addresses.Public.First(a => a.Version.Equals("4")).Address);
        }

        [TestMethod]
        public void Should_Match_AccessIPv6_and_Public_Network_IP()
        {
            var provider = new ComputeProvider(_testIdentity);
            var server = provider.GetDetails(_testServer2.Id);

            Assert.AreEqual(server.AccessIPv6, server.Addresses.Public.First(a => a.Version.Equals("6")).Address);
        }

        [TestMethod]
        public void Should_Return_Addresses_For_Server2()
        {
            var provider = new ComputeProvider(_testIdentity);
            var addresses = provider.ListAddresses(_testServer2.Id);

            Assert.IsTrue(addresses.Public.Any());
        }

        [TestMethod]
        public void Should_Return_Addresses_By_Network_Name_For_Server2()
        {
            var provider = new ComputeProvider(_testIdentity);
            var addresses = provider.ListAddressesByNetwork(_testServer2.Id, "public");

            Assert.IsTrue(addresses.Any());
        }

        [TestMethod]
        public void Test100_Should_Mark_The_Server_For_Deletion()
        {
            var provider = new ComputeProvider(_testIdentity);
            var result = provider.DeleteServer(_testServer.Id);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test101_Should_Wait_A_Max_Of_10_Minutes_For_The_Server_Is_Deleted_Indicated_By_A_Null_Return_Value_For_Details()
        {
            var provider = new ComputeProvider(_testIdentity);

            provider.WaitForServerDeleted(_testServer.Id);
        }

        [TestMethod]
        public void Should_Mark_The_Server_For_Deletion_For_Server2()
        {
            var provider = new ComputeProvider(_testIdentity);
            var result = provider.DeleteServer(_testServer2.Id);

            Assert.IsTrue(result);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_A_Max_Of_10_Minutes_For_The_Server_Is_Deleted_Indicated_By_A_Null_Return_Value_For_Details_For_Server2()
        {
            var provider = new ComputeProvider(_testIdentity);

            provider.WaitForServerDeleted(_testServer2.Id);
        }

        [TestMethod]
        public void Should_Soft_Reboot_Server()
        {
            var provider = new ComputeProvider(_testIdentity);

            _rebootSuccess = provider.RebootServer(_testServer.Id, RebootType.SOFT);

            Assert.IsTrue(_rebootSuccess);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_Until_Server_Goes_Into_Reboot_State()
        {
            Assert.IsTrue(_rebootSuccess); // If the reboot was not successful in the previous test, then fail this one too.

            var provider = new ComputeProvider(_testIdentity);

            var details = provider.WaitForServerState(_testServer.Id, ServerState.REBOOT, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED });

            Assert.IsNotNull(details);
        }

        [TestMethod]
        public void Should_Hard_Reboot_Server()
        {
            var provider = new ComputeProvider(_testIdentity);

            _rebootSuccess = provider.RebootServer(_testServer.Id, RebootType.HARD);

            Assert.IsTrue(_rebootSuccess);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_Until_Server_Goes_Into_Hard_Reboot_State()
        {
            Assert.IsTrue(_rebootSuccess); // If the reboot was not successful in the previous test, then fail this one too.

            var provider = new ComputeProvider(_testIdentity);

            var details = provider.WaitForServerState(_testServer.Id, ServerState.HARD_REBOOT, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED });

            Assert.IsNotNull(details);
        }

        [TestMethod]
        public void Should_Rebuild_Server()
        {
            var provider = new ComputeProvider(_testIdentity);
            var details = provider.GetDetails(_testServer.Id);
            var image = provider.ListImages().First(i => i.Name.Contains("CentOS") && i.Id != details.Image.Id);
            var flavor = int.Parse(details.Flavor.Id) + 1;

            _rebuildServer = provider.RebuildServer(_testServer.Id, string.Format("{0}_REBUILD", details.Name), image.Id, flavor.ToString(), _testServer.AdminPassword);

            Assert.IsNotNull(_rebuildServer);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Rebuild_Status()
        {
            Assert.IsNotNull(_rebuildServer);

            var provider = new ComputeProvider(_testIdentity);
            var details = provider.WaitForServerState(_testServer.Id, ServerState.REBUILD, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED });

            Assert.IsNotNull(details);
            Assert.AreEqual(_testServer.Id, _rebuildServer.Id);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Active_Status_After_Rebuild()
        {
            Assert.IsNotNull(_rebuildServer);

            var provider = new ComputeProvider(_testIdentity);
            var details = provider.WaitForServerActive(_testServer.Id);

            Assert.IsNotNull(details);
        }

        [TestMethod]
        public void Should_Resize_Server()
        {
            var provider = new ComputeProvider(_testIdentity);
            var details = provider.GetDetails(_testServer.Id);
            var flavor = int.Parse(details.Flavor.Id) + 1;

            _resizeSuccess = provider.ResizeServer(_testServer.Id, string.Format("{0}_RESIZE", details.Name), flavor.ToString());

            Assert.IsTrue(_resizeSuccess);
        }

        [TestMethod]
        public void Should_Resize_Server_Back()
        {
            Assert.IsTrue(_resizeSuccess);

            var provider = new ComputeProvider(_testIdentity);
            var details = provider.GetDetails(_testServer.Id);
            var flavor = int.Parse(details.Flavor.Id) - 1;

            _resizeSuccess = provider.ResizeServer(_testServer.Id, string.Format("{0}_RESIZED_BACK", details.Name), flavor.ToString());

            Assert.IsTrue(_resizeSuccess);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Resize_Status()
        {
            Assert.IsTrue(_resizeSuccess);

            var provider = new ComputeProvider(_testIdentity);
            var details = provider.WaitForServerState(_testServer.Id, ServerState.RESIZE, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED });

            Assert.IsNotNull(details);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Verify_Resize_Status()
        {
            Assert.IsTrue(_resizeSuccess);

            var provider = new ComputeProvider(_testIdentity);
            var details = provider.WaitForServerState(_testServer.Id, ServerState.VERIFY_RESIZE, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED });

            Assert.IsNotNull(details);
        }

        [TestMethod]
        public void Should_Confirm_Resize_Server()
        {
            Assert.IsTrue(_resizeSuccess);

            var provider = new ComputeProvider(_testIdentity);

            _confirmResizeSuccess = provider.ConfirmServerResize(_testServer.Id);

            Assert.IsTrue(_confirmResizeSuccess);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Active_Status_After_Confirm_Resize()
        {
            Assert.IsTrue(_confirmResizeSuccess);

            var provider = new ComputeProvider(_testIdentity);

            var details = provider.WaitForServerActive(_testServer.Id);

            Assert.IsNotNull(details);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Confirm_Resize_Status()
        {
            Assert.IsTrue(_confirmResizeSuccess);

            var provider = new ComputeProvider(_testIdentity);
            var details = provider.WaitForServerState(_testServer.Id, ServerState.VERIFY_RESIZE, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED });

            Assert.IsNotNull(details);
        }

        [TestMethod]
        public void Should_Revert_Resize_Server()
        {
            Assert.IsTrue(_resizeSuccess);
            var provider = new ComputeProvider(_testIdentity);

            _revertResizeSuccess = provider.RevertServerResize(_testServer.Id);

            Assert.IsTrue(_revertResizeSuccess);
        }

        [TestMethod]
        public void Should_Wait_For_Server_To_Be_In_Active_State_After_Reverting_Resize()
        {
            Assert.IsTrue(_revertResizeSuccess);

            var provider = new ComputeProvider(_testIdentity);
            var details = provider.WaitForServerActive(_testServer.Id);

            Assert.IsNotNull(details);
        }

        [TestMethod]
        public void Should_Create_Image_From_Server()
        {
            var provider = new ComputeProvider(_testIdentity);
            var detail = provider.GetDetails(_testServer.Id);

            _testImageName = "Image_of_" + detail.Id;
            var sucess = provider.CreateImage(detail.Id, _testImageName);

            Assert.IsTrue(sucess);
        }

        [TestMethod]
        public void Should_Retrieve_Image_By_Name()
        {
            var provider = new ComputeProvider(_testIdentity);
            var images = provider.ListImagesWithDetails();

            _testImage = images.First(i => i.Name == _testImageName);

            Assert.IsNotNull(_testImage);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Image_To_Be_In_Active_State()
        {
            var provider = new ComputeProvider(_testIdentity);

            var details = provider.WaitForImageActive(_testImage.Id);

            Assert.IsNotNull(details);
        }

        [TestMethod]
        public void Should_Mark_Image_For_Deletetion()
        {
            var provider = new ComputeProvider(_testIdentity);

            var success = provider.DeleteImage(_testImage.Id);

            Assert.IsTrue(success);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Image_To_Be_In_Deleted_State()
        {
            var provider = new ComputeProvider(_testIdentity);

            provider.WaitForImageDeleted(_testImage.Id);
        }
    }
}
