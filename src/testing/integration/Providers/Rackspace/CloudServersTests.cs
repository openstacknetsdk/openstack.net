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
    /// Summary description for CloudServersTests
    /// </summary>
    [TestClass]
    public class CloudServersTests
    {
        private TestContext testContextInstance;
        private static CloudIdentity _testIdentity;
        private static NewServer _newTestServer;
        private static Server _testServer;
        private static IEnumerable<ServerImage> _allImages;
        private static NewServer _newTestServer2;
        private static Server _testServer2;
        private static bool _rebootSuccess;
        private static bool _resizeSuccess;
        private static bool _confirmResizeSuccess;
        private static bool _rebuildServerSuccess;
        private static ServerImage _testImage;
        private static string _testImageName;
        private static bool _revertResizeSuccess;
        private static Server _preBuildDetails;
        private static string _rescueAdminPass;
        private static bool _unRescueSuccess;
        private static ServerVolume _testVolume;
        private static SimpleServerImage _initImage;
        private static Flavor _initFlavor;
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

        #region Initialize and Build Test Servers

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _testIdentity = new RackspaceCloudIdentity(Bootstrapper.Settings.TestIdentity);
            
            var provider = new CloudServersProvider(_testIdentity);

            _initImage = provider.ListImages(imageName: "CentOS 6.3").First();
            _initFlavor = provider.ListFlavors().OrderBy(f => f.Id).First();
        }

        [Timeout(1800000), TestMethod]
        public void Should_Create_A_New_Server_In_DFW()
        {
            var provider = new CloudServersProvider(_testIdentity);
            _newTestServer = provider.CreateServer("net-sdk-test-server", _initImage.Id, _initFlavor.Id);

            Assert.IsNotNull(_newTestServer);
            Assert.IsNotNull(_newTestServer.Id);
        }

        [TestMethod]
        public void Should_Create_A_New_Complex_Server_In_DFW()
        {
            var provider = new CloudServersProvider(_testIdentity);
            _newTestServer2 = provider.CreateServer("net-sdk-test-server2", _initImage.Id, _initFlavor.Id, metadata: new Metadata { { "Key1", "Value1" }, { "Key2", "Value2" } }, attachToPublicNetwork: true, attachToServiceNetwork: true);

            Assert.IsNotNull(_newTestServer2);
            Assert.IsNotNull(_newTestServer2.Id);
        }

        #endregion

        #region Test Server Details
       
        [TestMethod]
        public void Should_Get_Details_For_Newly_Created_Server_In_DFW()
        {
            _testServer = _newTestServer.GetDetails();

            Assert.IsNotNull(_testServer);
            Assert.AreEqual("net-sdk-test-server", _testServer.Name);
            Assert.AreEqual(_initImage.Id, _testServer.Image.Id);
            Assert.AreEqual(_initFlavor.Id, _testServer.Flavor.Id);  
        }

        [TestMethod]
        public void Should_Get_Details_For_Newly_Created_Complex_Server_In_DFW()
        {
            _testServer2 = _newTestServer2.GetDetails();

            Assert.IsNotNull(_testServer2);
            Assert.AreEqual("net-sdk-test-server2", _testServer2.Name);
            Assert.AreEqual(_initImage.Id, _testServer2.Image.Id);
            Assert.AreEqual(_initFlavor.Id, _testServer2.Flavor.Id);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_Until_Server_Becomes_Active_Or_A_Maximum_Of_10_Minutes()
        {
            _testServer.WaitForActive();

            Assert.IsNotNull(_newTestServer);
            Assert.AreEqual("ACTIVE", _testServer.Status);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_Until_Server_Becomes_Active_Or_A_Maximum_Of_10_Minutes_For_Server2()
        {
            _testServer2.WaitForActive();

            Assert.IsNotNull(_testServer2);
            Assert.AreEqual("ACTIVE", _testServer2.Status);
        }

        [TestMethod]
        public void Should_Get_A_List_Of_Servers_Which_Should_Include_The_Newly_Created_Server()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var servers = provider.ListServers();

            Assert.IsNotNull(servers);
            Assert.IsTrue(servers.Any());
            Assert.IsNotNull(servers.FirstOrDefault(s => s.Id == _testServer.Id));
        }

        [TestMethod]
        public void Should_Get_A_List_Of_Servers_With_Details_Which_Should_Include_The_Newly_Created_Server()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var servers = provider.ListServersWithDetails();

            Assert.IsNotNull(servers);
            Assert.IsTrue(servers.Any());
            Assert.IsNotNull(servers.FirstOrDefault(s => s.Id == _testServer.Id));
        }

        [TestMethod]
        public void Should_Contain_AccessIP_v4_Address()
        {
            var server = _testServer.GetDetails();

            Assert.IsTrue(!string.IsNullOrWhiteSpace(server.AccessIPv4));
        }

        [TestMethod]
        public void Should_Contain_AccessIP_v6_Address()
        {
            var server = _testServer.GetDetails();

            Assert.IsTrue(!string.IsNullOrWhiteSpace(server.AccessIPv6));
        }

        [TestMethod]
        public void Should_Contain_AccessIP_v4_Address_For_Server2()
        {
            var server = _testServer2.GetDetails();

            Assert.IsTrue(!string.IsNullOrWhiteSpace(server.AccessIPv4));
        }

        [TestMethod]
        public void Should_Contain_AccessIP_v6_Address_For_Server2()
        {
            var server = _testServer2.GetDetails();

            Assert.IsTrue(!string.IsNullOrWhiteSpace(server.AccessIPv6));
        }

        [TestMethod]
        public void Should_Contain_Network_Addresses_For_Server2()
        {
            var server = _testServer2.GetDetails();

            Assert.IsTrue(server.Addresses.Public.Any());
            Assert.IsTrue(server.Addresses.Private.Any());
        }

        [TestMethod]
        public void Should_Match_AccessIPv4_and_Public_Network_IP()
        {
            var server = _testServer2.GetDetails();

            Assert.AreEqual(server.AccessIPv4, server.Addresses.Public.First(a => a.Version.Equals("4")).Address);
        }

        [TestMethod]
        public void Should_Match_AccessIPv6_and_Public_Network_IP()
        {
            var server = _testServer2.GetDetails();

            Assert.AreEqual(server.AccessIPv6, server.Addresses.Public.First(a => a.Version.Equals("6")).Address);
        }

        [TestMethod]
        public void Should_Return_Addresses_For_Server2()
        {
            var addresses = _testServer2.ListAddresses();

            Assert.IsTrue(addresses.Public.Any());
        }

        [TestMethod]
        public void Should_Return_Addresses_By_Network_Name_For_Server2()
        {
            var addresses = _testServer2.ListAddressesByNetwork("public");

            Assert.IsTrue(addresses.Any());
        }

        #endregion

        #region Test Metadata
        
        [TestMethod]
        public void Should_Get_Empty_Server_Metadata_List()
        {
            var metadata = _testServer.GetMetadata();

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 0);
        }

        [TestMethod]
        public void Should_Set_Initial_Server_Metadata_With_Keys_1_and_2()
        {
            var metadata = new Metadata() {{"Metadata_Key_1", "My_Value_1"}, {"Metadata_Key_2", "My_Value_2"}};
            var response = _testServer.SetMetadata(metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Should_Get_The_Initial_Test_Server_Metadata_With_Keys_1_and_2()
        {
            var metadata = _testServer.GetMetadata();

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 2);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_1"));
            Assert.AreEqual("My_Value_1", metadata.First(md => md.Key == "Metadata_Key_1").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_2"));
            Assert.AreEqual("My_Value_2", metadata.First(md => md.Key == "Metadata_Key_2").Value);
        }

        [TestMethod]
        public void Should_Reset_Server_Metadata_With_Keys_3_and_4()
        {
            var metadata = new Metadata() { { "Metadata_Key_3", "My_Value_3" }, { "Metadata_Key_4", "My_Value_4" } };
            var response = _testServer.SetMetadata(metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Should_Get_The_Reset_Test_Server_Metadata_With_Keys_3_and_4()
        {
            var metadata = _testServer.GetMetadata();

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 2);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.AreEqual("My_Value_3", metadata.First(md => md.Key == "Metadata_Key_3").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.AreEqual("My_Value_4", metadata.First(md => md.Key == "Metadata_Key_4").Value);
        }

        [TestMethod]
        public void Should_Update_Server_Metadata_Items_With_Keys_3_and_4()
        {
            var metadata = new Metadata() { { "Metadata_Key_3", "My_Value_3_Updated" }, { "Metadata_Key_4", "My_Value_4_Updated" } };
            var response = _testServer.UpdateMetadata(metadata);

            Thread.Sleep(10000);   // Sleep a few seconds because there is an edge case that the system does not reflect the new metadata changes quick enough
            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Should_Get_The_Updated_Server_Metadata_With_Keys_3_and_4()
        {
            var metadata = _testServer.GetMetadata();

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 2);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.AreEqual("My_Value_3_Updated", metadata.First(md => md.Key == "Metadata_Key_3").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_4"));
            Assert.AreEqual("My_Value_4_Updated", metadata.First(md => md.Key == "Metadata_Key_4").Value);
        }

        [TestMethod]
        public void Should_Add_A_New_Metadata_Item_With_Key_5()
        {
            var response = _testServer.AddMetadata("Metadata_Key_5", "My_Value_5");

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Should_Get_The_Server_Metadata_Including_The_New_Item_With_Key_5()
        {
            var metadata = _testServer.GetMetadata();

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
        public void Should_Update_Metadata_Items_With_Key_3_And_5_But_NOT_Key_4()
        {
            var metadata = new Metadata() { { "Metadata_Key_3", "My_Value_3_Updated_Again" }, { "Metadata_Key_5", "My_Value_5_Updated" } };
            var response = _testServer.UpdateMetadata(metadata);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Should_Get_The_Updates_To_Metadata_Items_With_Key_3_And_5_And_Validate_That_Metadata_Item_With_Key_4_Did_Not_Change()
        {
            var metadata = _testServer.GetMetadata();

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
        public void Should_Update_A_Single_Server_Metadata_Item_With_Key_4()
        {
            var response = _testServer.UpdateMetadataItem("Metadata_Key_4", "My_Value_4_Updated_Again");

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Should_Get_The_Updates_To_Metadata_Item_With_Key_4_And_Validate_That_Metadata_Items_ith_Keys_3_And_5_Have_Not_Changed()
        {
            var metadata = _testServer.GetMetadata();

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
        public void Should_Delete_A_Single_Server_Metadata_Item_With_Key_4()
        {
            var response = _testServer.DeleteMetadataItem("Metadata_Key_4");

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void Should_Get_The_List_Of_Metadata_Items_Including_Keys_3_And_5_But_Excluding_The_Deleted_Item_With_Key_4()
        {
            var metadata = _testServer.GetMetadata();

            Assert.IsNotNull(metadata);
            Assert.IsTrue(metadata.Count == 2);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_3"));
            Assert.AreEqual("My_Value_3_Updated_Again", metadata.First(md => md.Key == "Metadata_Key_3").Value);
            Assert.IsTrue(metadata.Any(md => md.Key == "Metadata_Key_5"));
            Assert.AreEqual("My_Value_5_Updated", metadata.First(md => md.Key == "Metadata_Key_5").Value);
        }

        #endregion

        #region Test Images
        
        [TestMethod]
        public void Should_Create_Image_From_Server()
        {
            _testImageName = "Image_of_" + _testServer.Id;
            var imageDetails = _testServer.Snapshot(_testImageName);

            Assert.IsNotNull(imageDetails);
        }

        [TestMethod]
        public void Should_Retrieve_Image_By_Name()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails();

            _testImage = images.First(i => i.Name == _testImageName);

            Assert.IsNotNull(_testImage);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Image_To_Be_In_Active_State()
        {
            _testImage.WaitForActive();

            Assert.AreEqual(ImageState.ACTIVE, _testImage.Status);
        }

        [TestMethod]
        public void Should_Mark_Image_For_Deletetion()
        {
            var success = _testImage.Delete();

            Assert.IsTrue(success);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Image_To_Be_In_Deleted_State()
        {
            _testImage.WaitForDelete();
        }

        [TestMethod]
        public void Should_Get_List_Of_Base_Images()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImages(imageType: ImageType.Base);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Should_Get_List_Of_Snapshot_Images()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImages(imageType: ImageType.Snapshot);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Should_Get_List_All_Images_Which_should_Equal_Base_And_Snapshot_Combined()
        {
            var provider = new CloudServersProvider(_testIdentity);
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
        public void Should_Get_List_All_Images_With_Details()
        {
            var provider = new CloudServersProvider(_testIdentity);
            _allImages = provider.ListImagesWithDetails(identity: _testIdentity);

            Assert.IsTrue(_allImages.Any());
        }

        [TestMethod]
        public void Should_Return_One_Image_When_Searching_By_Valid_Id()
        {
            var validImage = _allImages.First(i => i.Server != null);
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImages(server: validImage.Server.Id);

            Assert.IsTrue(images.Any());
        }
        
        [TestMethod]
        public void Should_Return_At_Least_One_Image_When_Searching_By_Valid_Name()
        {
            var validImage = _allImages.First();
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImages(imageName: validImage.Name);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Should_Return_At_Least_One_Image_When_Searching_By_Valid_Status()
        {
            var validImage = _allImages.First();
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImages(imageStatus: validImage.Status);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Should_Return_At_Least_One_Image_When_Searching_By_Valid_Change_Since_Date()
        {
            var validImage = _allImages.First();
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImages(changesSince: validImage.Updated.AddSeconds(-1));

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Should_Return_One_Image_With_Details_When_Searching_By_Valid_Id()
        {
            var validImage = _allImages.First(i => i.Server != null);
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(server: validImage.Server.Id);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Should_Return_At_Least_One_Image_With_Details_When_Searching_By_Valid_Name()
        {
            var validImage = _allImages.First();
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(imageName: _testImageName);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Should_Return_At_Least_One_Image_With_Details_When_Searching_By_Valid_Status()
        {
            var validImage = _allImages.First();
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(imageStatus: validImage.Status);

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Should_Return_At_Least_One_Image_With_Details_When_Searching_By_Valid_Change_Since_Date()
        {
            var validImage = _allImages.First();
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(changesSince: validImage.Updated.AddSeconds(-1));

            Assert.IsTrue(images.Any());
        }

        [TestMethod]
        public void Should_Return_Exactly_Two_Images()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImages(limit: 2);

            Assert.IsTrue(images.Count() == 2);
        }

        [TestMethod]
        public void Should_Return_Exactly_Two_Images_With_Details()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(limit: 2);

            Assert.IsTrue(images.Count() == 2);
        }

        [TestMethod]
        public void Should_Return_All_Images_When_Using_A_Limit_Greater_Than_What_Actually_Exists()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImages(limit: _allImages.Count() * 2);

            Assert.IsTrue(images.Count() == _allImages.Count());
        }

        [TestMethod]
        public void Should_Return_All_Images_With_Details_When_Using_A_Limit_Greater_Than_What_Actually_Exists()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var images = provider.ListImagesWithDetails(limit: _allImages.Count() * 2);

            Assert.IsTrue(images.Count() == _allImages.Count());
        }

        #endregion

        #region Test Server Actions

        [TestMethod]
        public void Should_Successfully_To_And_Login_With_Old_Password()
        {
            using (var client = new Renci.SshNet.SshClient(_testServer.AccessIPv4, "root", _newTestServer.AdminPassword))
            {
                client.Connect();

                Assert.IsTrue(client.IsConnected);
            }
        }

        [TestMethod]
        public void Should_Update_The_Admin_Password()
        {
            var provider = new CloudServersProvider(_testIdentity);
            var result = provider.ChangeAdministratorPassword(_testServer.Id, NewPassword);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Should_Successfully_To_And_Login_With_New_Password()
        {
            bool sucess = false;
            for (int i = 0; i < 10; i++)
            {
                using (var client = new Renci.SshNet.SshClient(_testServer.AccessIPv4, "root", NewPassword))
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
        public void Should_Soft_Reboot_Server()
        {
            _rebootSuccess = _testServer.SoftReboot();

            Assert.IsTrue(_rebootSuccess);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_Until_Server_Goes_Into_Reboot_State()
        {
            Assert.IsTrue(_rebootSuccess); // If the reboot was not successful in the previous test, then fail this one too.

            _testServer.WaitForState(ServerState.REBOOT, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED });

            Assert.AreEqual(ServerState.REBOOT, _testServer.Status);
        }

        [TestMethod]
        public void Should_Hard_Reboot_Server()
        {
            _rebootSuccess = _testServer.HardReboot();

            Assert.IsTrue(_rebootSuccess);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_Until_Server_Goes_Into_Hard_Reboot_State()
        {
            Assert.IsTrue(_rebootSuccess); // If the reboot was not successful in the previous test, then fail this one too.

            _testServer.WaitForState(ServerState.HARD_REBOOT, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED });

            Assert.AreEqual(ServerState.HARD_REBOOT, _testServer.Status);
        }

        [TestMethod]
        public void Should_Rebuild_Server()
        {
            _preBuildDetails = _testServer.GetDetails();
            var provider = new CloudServersProvider(_testIdentity);
            var image = provider.ListImages().First(i => i.Name.Contains("CentOS") && i.Id != _preBuildDetails.Image.Id);
            var flavor = int.Parse(_preBuildDetails.Flavor.Id) + 1;

            _rebuildServerSuccess = _testServer.Rebuild(string.Format("{0}_REBUILD", _preBuildDetails.Name), image.Id, flavor.ToString(), _newTestServer.AdminPassword);

            Assert.IsTrue(_rebuildServerSuccess);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Rebuild_Status()
        {
            Assert.IsTrue(_rebuildServerSuccess);

            _testServer.WaitForState(ServerState.REBUILD, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED });

            Assert.AreEqual(_preBuildDetails.Id, _testServer.Id);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Active_Status_After_Rebuild()
        {
            Assert.IsTrue(_rebuildServerSuccess);

            _testServer.WaitForActive();

            Assert.AreEqual(ServerState.ACTIVE, _testServer.Status);
        }

        [TestMethod]
        public void Should_Verify_That_The_Name_Image_And_Flavor_Changed_As_expected_After_Rebuild()
        {
            Assert.IsTrue(_rebuildServerSuccess);
            
            Assert.AreNotEqual(_preBuildDetails.Name, _testServer.Name);
            Assert.AreNotEqual(_preBuildDetails.Flavor.Id, _testServer.Flavor.Id);
            Assert.AreNotEqual(_preBuildDetails.Image.Id, _testServer.Image.Id);
        }

        [TestMethod]
        public void Should_Resize_Server()
        {
            var flavor = int.Parse(_testServer.Flavor.Id) + 1;

            _resizeSuccess = _testServer.Resize(string.Format("{0}_RESIZE", _testServer.Name), flavor.ToString());

            Assert.IsTrue(_resizeSuccess);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Verify_Resize_Status()
        {
            Assert.IsTrue(_resizeSuccess);

            _testServer.WaitForState(ServerState.VERIFY_RESIZE, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED });

            Assert.AreEqual(ServerState.VERIFY_RESIZE, _testServer.Status);
        }

        [TestMethod]
        public void Should_Confirm_Resize_Server()
        {
            Assert.IsTrue(_resizeSuccess);

            _confirmResizeSuccess = _testServer.ConfirmResize();

            Assert.IsTrue(_confirmResizeSuccess);
        }

        [TestMethod]
        public void Should_Resize_Server_Back()
        {
            Assert.IsTrue(_resizeSuccess);

            var flavor = int.Parse(_testServer.Flavor.Id) - 1;

            _resizeSuccess = _testServer.Resize(string.Format("{0}_RESIZED_BACK", _testServer.Name), flavor.ToString());

            Assert.IsTrue(_resizeSuccess);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Active_Status_After_Confirm_Resize()
        {
            Assert.IsTrue(_confirmResizeSuccess);


            _testServer.WaitForActive();

            Assert.AreEqual(ServerState.ACTIVE, _testServer.Status);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Confirm_Resize_Status()
        {
            Assert.IsTrue(_confirmResizeSuccess);

            _testServer.WaitForState(ServerState.VERIFY_RESIZE, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED });

            Assert.AreEqual(ServerState.VERIFY_RESIZE, _testServer.Status);
        }

        [TestMethod]
        public void Should_Revert_Resize_Server()
        {
            Assert.IsTrue(_resizeSuccess);

            _revertResizeSuccess = _testServer.RevertResize();

            Assert.IsTrue(_revertResizeSuccess);
        }

        [TestMethod]
        public void Should_Wait_For_Server_To_Be_In_Active_State_After_Reverting_Resize()
        {
            Assert.IsTrue(_revertResizeSuccess);

            _testServer.WaitForActive();

            Assert.AreEqual(ServerState.ACTIVE, _testServer.Status);
        }

        [TestMethod]
        public void Should_Mark_Server_To_Enter_Rescue_Mode()
        {
            _rescueAdminPass = _testServer.Rescue();

            Assert.IsFalse(string.IsNullOrWhiteSpace(_rescueAdminPass));
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Rescue_Status()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(_rescueAdminPass));

            _testServer.WaitForState(ServerState.RESCUE, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED });

            Assert.AreEqual(ServerState.RESCUE, _testServer.Status);
        }

        [TestMethod]
        public void Should_Mark_Server_To_Be_UnRescued()
        {
            _unRescueSuccess = _testServer.UnRescue();

            Assert.IsTrue(_unRescueSuccess);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_For_Server_To_Go_Into_Active_Status_After_UnRescue()
        {
            Assert.IsTrue(_unRescueSuccess);

            _testServer.WaitForActive();

            Assert.AreEqual(ServerState.ACTIVE, _testServer.Status);
        }

        #endregion

        #region Test Server Volumes
        
        [TestMethod]
        public void Should_Attach_Server_Volume()
        {
            _testVolume = _testServer.AttachVolume("2da9ce90-076e-450a-be3e-c822c9aa73f5");

            Assert.IsNotNull(_testVolume);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_Until_Volume_Is_Attached_To_The_Server()
        {
           var volumeIsInList = false;
            var count = 0;
            do
            {
                var volumes = _testServer.GetVolumes();

                if (volumes != null)
                    volumeIsInList = volumes.Any(v => v.Id == _testVolume.Id);

                count += 1;

                Thread.Sleep(2400);
            } while (!volumeIsInList && count < 600);

            Assert.IsTrue(volumeIsInList);
        }

        [TestMethod]
        public void Should_List_All_Volumes()
        {
            var volumes = _testServer.GetVolumes();

            Assert.IsTrue(volumes.Any());
        }

        [TestMethod]
        public void Should_Contain_Attached_Volumne_In_Server_Volume_List()
        {
            var volumes = _testServer.GetVolumes();

            Assert.IsTrue(volumes.Any(v => v.Id == _testVolume.Id));
        }

        [TestMethod]
        public void Should_Detach_Volume_From_Server()
        {
            var success = _testServer.DetachVolume(_testVolume.Id);

            Assert.IsTrue(success);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_Until_Volume_Is_Detached_From_The_Server()
        {
            var volumeIsInList = false;
            var count = 0;
            do
            {
                var volumes = _testServer.GetVolumes();

                if (volumes != null)
                    volumeIsInList = volumes.Any(v => v.Id == _testVolume.Id);

                count += 1;
            } while (volumeIsInList && count < 600);

            Assert.IsFalse(volumeIsInList);
        }

        [TestMethod]
        public void Should_NOT_Contain_Attached_Volume_In_Server_Volume_List()
        {
            var volumes = _testServer.GetVolumes();

            Assert.IsFalse(volumes.Any(v => v.Id == _testVolume.Id));
        }

        #endregion

        #region Cleanup
        
        [TestMethod]
        public void Should_Mark_The_Server_For_Deletion()
        {
            var result = _testServer.Delete();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Should_Wait_A_Max_Of_10_Minutes_For_The_Server_Is_Deleted_Indicated_By_A_Null_Return_Value_For_Details()
        {
            _testServer.WaitForDeleted();
        }

        [TestMethod]
        public void Should_Mark_The_Server_For_Deletion_For_Server2()
        {
            var result = _testServer2.Delete();

            Assert.IsTrue(result);
        }

        [Timeout(1800000), TestMethod]
        public void Should_Wait_A_Max_Of_10_Minutes_For_The_Server_Is_Deleted_Indicated_By_A_Null_Return_Value_For_Details_For_Server2()
        {
            _testServer2.WaitForDeleted();
        }

        #endregion
    }
}
