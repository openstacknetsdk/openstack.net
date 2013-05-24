using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Exceptions;
using net.openstack.Providers.Rackspace.Objects;

namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    [TestClass]
    public class CloudFilesTests
    {

        private static RackspaceCloudIdentity _testIdentity;

        private static readonly string containerName = string.Format("Cloud Files Integration Tests_{0}",Guid.NewGuid().ToString());
        private static readonly string containerName2 = string.Format("Cloud Files Integration Tests 2_{0}",Guid.NewGuid().ToString());
        const string webIndex = "index.html";
        const string webError = "error.html";
        const string webListingsCSS = "index.css";
        const bool webListing = true;
        const string objectName = "DarkKnightRises.jpg";
        const string saveFileNane1 = "DarkKnightRises_SAVE1.jpg";
        const string saveFileNane2 = "DarkKnightRises_SAVE2.jpg";

        private static readonly string sourceContainerName = containerName;
        private const string sourceObjectName = objectName;

        private static readonly string destinationContainerName = string.Format("{0}_DarkKnightRises", containerName);
        private const string destinationObjectName = "Test " + objectName;


        const string emailTo = "123@abc.com";
        string[] emailToList = new[] { "abc@123.com,123@abc.com" };


        public CloudFilesTests()
        {
        }

        private TestContext testContextInstance;
        private static long _originalThreshold;


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

            var file = Path.Combine(Directory.GetCurrentDirectory(), saveFileNane1);
            if(File.Exists(file))
                File.Delete(file);

            file = Path.Combine(Directory.GetCurrentDirectory(), saveFileNane2);
            if (File.Exists(file))
                File.Delete(file);
        }

        #region Container Tests

        [TestMethod]
        public void Should_Return_Container_List()
        {
            var provider = new CloudFilesProvider();
            var containerList = provider.ListContainers(identity: _testIdentity);

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_Internal_Url()
        {
            var provider = new CloudFilesProvider();
            var containerList = provider.ListContainers(useInternalUrl:true, identity: _testIdentity);

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_Limit()
        {
            var provider = new CloudFilesProvider();
            var containerList = provider.ListContainers(1, identity: _testIdentity);

            Assert.IsNotNull(containerList);
            Assert.AreEqual(1, containerList.Count());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_Start_Marker_Lower_Case()
        {
            var provider = new CloudFilesProvider();
            var containerList = provider.ListContainers(null, "a", identity: _testIdentity);

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_Start_Marker_Upper_Case()
        {
            var provider = new CloudFilesProvider();
            var containerList = provider.ListContainers(null, "A", identity: _testIdentity);

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }


        [TestMethod]
        public void Should_Return_Container_List_With_End_Marker_Upper_Case()
        {
            var provider = new CloudFilesProvider();
            var containerList = provider.ListContainers(null, null, "L", identity: _testIdentity);

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_End_Marker_Lower_Case()
        {
            var provider = new CloudFilesProvider();
            var containerList = provider.ListContainers(null, null, "l", identity: _testIdentity);

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Create_Container()
        {

            var provider = new CloudFilesProvider();
            var containerCreatedResponse = provider.CreateContainer(containerName, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ContainerCreated, containerCreatedResponse);
        }

        [TestMethod]
        public void Should_Create_Destination_Container()
        {
            var provider = new CloudFilesProvider();
            var containerCreatedResponse = provider.CreateContainer(destinationContainerName, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ContainerCreated, containerCreatedResponse);
        }


        [TestMethod]
        public void Should_Not_Create_Container_Already_Exists()
        {
            var provider = new CloudFilesProvider();
            var containerCreatedResponse = provider.CreateContainer(containerName, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ContainerExists, containerCreatedResponse);
        }

        [TestMethod]
        public void Should_Delete_Container()
        {
            var provider = new CloudFilesProvider();
            var containerCreatedResponse = provider.DeleteContainer(containerName, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ContainerDeleted, containerCreatedResponse);
        }

        [TestMethod]
        public void Should_Delete_Destination_Container()
        {
            var provider = new CloudFilesProvider();
            var containerCreatedResponse = provider.DeleteContainer(destinationContainerName, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ContainerDeleted, containerCreatedResponse);
        }


        [TestMethod]
        public void Should_Get_Objects_From_Container()
        {
            var provider = new CloudFilesProvider();
            var containerGetObjectsResponse = provider.ListObjects(containerName, identity: _testIdentity);

            Assert.IsNotNull(containerGetObjectsResponse);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Throw_An_Exception_When_Calling_Get_Objects_From_Container_And_Container_Does_Not_Exist()
        {
            var provider = new CloudFilesProvider();
            var containerGetObjectsResponse = provider.ListObjects(containerName, identity: _testIdentity);
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Throw_An_Exception_When_Calling_Get_Objects_From_Container_And_Objects_Does_Not_Exist()
        {
            var provider = new CloudFilesProvider();
            provider.ListObjects(containerName, identity: _testIdentity);
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void Should_Add_MetaData_For_Container()
        {
            var metaData = new Dictionary<string, string>();
            metaData.Add("key1", "value1");
            metaData.Add("key2", "value2");
            metaData.Add("key3", "value3");
            metaData.Add("key4", "value4");
            var provider = new CloudFilesProvider();
            provider.UpdateContainerMetadata(containerName, metaData, identity: _testIdentity);
        }

        [TestMethod]
        public void Should_Get_MetaData_For_Container_And_Include_Key1_And_Key2_And_Key3_And_Key4()
        {
            var provider = new CloudFilesProvider( _testIdentity);
            var metaData = provider.GetContainerMetaData(containerName);
            Assert.IsNotNull(metaData);
            Assert.IsTrue(metaData.Any());
            Assert.AreEqual("value1", metaData.Where(x => x.Key.Equals("key1", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual("value2", metaData.Where(x => x.Key.Equals("key2", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual("value3", metaData.Where(x => x.Key.Equals("key3", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual("value4", metaData.Where(x => x.Key.Equals("key4", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        public void Should_Remove_Multiple_Metadata_For_Container_Items_Key2_and_Key3()
        {
            var provider = new CloudFilesProvider(_testIdentity);
            var metaData = new Dictionary<string, string> {{"key2", "value2"}, {"key3", "value3"}};
            
            provider.DeleteContainerMetadata(containerName, metaData);
        }

        [TestMethod]
        public void Should_Get_MetaData_For_Container_After_Multiple_Delete_And_Include_Key1_And_Key4()
        {
            var provider = new CloudFilesProvider();
            var metaData = provider.GetContainerMetaData(containerName, identity: _testIdentity);
            Assert.IsNotNull(metaData);
            Assert.AreEqual("value1", metaData.Where(x => x.Key.Equals("key1", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual("value4", metaData.Where(x => x.Key.Equals("key4", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        public void Should_Remove_Single_Metadata_For_Container_Item_Key1()
        {
            var provider = new CloudFilesProvider(_testIdentity);

            provider.DeleteContainerMetadata(containerName, "key1");
        }

        [TestMethod]
        public void Should_Get_MetaData_For_Container_After_Single_Delete_And_Include__Key4()
        {
            var provider = new CloudFilesProvider();
            var metaData = provider.GetContainerMetaData(containerName, identity: _testIdentity);
            Assert.IsNotNull(metaData);
            Assert.AreEqual("value4", metaData.Where(x => x.Key.Equals("key4", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        public void Should_Add_Headers_For_Container()
        {
            var metaData = new Dictionary<string, string>();
            metaData.Add("X-Container-Meta-Movie", "Batman");
            metaData.Add("X-Container-Meta-Genre", "Action");
            var provider = new CloudFilesProvider();
            provider.AddContainerHeaders(containerName, metaData, identity: _testIdentity);
        }

        [TestMethod]
        public void Should_Get_Headers_For_Container()
        {
            var provider = new CloudFilesProvider();
            var objectHeadersResponse = provider.GetContainerHeader(containerName, identity: _testIdentity);

            Assert.IsNotNull(objectHeadersResponse);
            Assert.IsTrue(objectHeadersResponse.Any());
        }

        [TestMethod]
        public void Should_Get_CDN_Headers_For_Container()
        {
            var provider = new CloudFilesProvider();
            var objectHeadersResponse = provider.GetContainerCDNHeader(containerName, identity: _testIdentity);

            Assert.IsNotNull(objectHeadersResponse);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Not_Get_CDN_Headers_For_Container()
        {
            var provider = new CloudFilesProvider();
            var objectHeadersResponse = provider.GetContainerCDNHeader(containerName, identity: _testIdentity);

            Assert.Fail("Expected exception was not thrown.");
        }

        //X-Log-Retention, X-CDN-enabled, and X-TTL.
        [TestMethod]
        public void Should_Add_CDN_Headers_For_Container()
        {
            var provider = new CloudFilesProvider();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Log-Retention", "false");
            provider.UpdateContainerCdnHeaders(containerName, headers, identity: _testIdentity);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Not_Add_CDN_Headers_For_Container()
        {
            var provider = new CloudFilesProvider();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Log-Retention", "false");
            provider.UpdateContainerCdnHeaders(containerName, headers, identity: _testIdentity);
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void Should_Get_CDN_Enabled_Containers()
        {
            var provider = new CloudFilesProvider();
            var cdnContainerList = provider.ListCDNContainers(null, null, null, true, identity: _testIdentity);

            Assert.IsNotNull(cdnContainerList);
            Assert.IsTrue(cdnContainerList.All(x => x.CDNEnabled == true));
        }

        [TestMethod]
        public void Should_Get_CDN_All_Containers()
        {
            var provider = new CloudFilesProvider();
            var cdnContainerList = provider.ListCDNContainers(identity: _testIdentity);

            Assert.IsNotNull(cdnContainerList);
            Assert.IsTrue(cdnContainerList.Any(x => x.CDNEnabled == true));
            Assert.IsTrue(cdnContainerList.Any(x => x.CDNEnabled == false));
        }

        [TestMethod]
        public void Should_Make_Container_CDN_Enabled_With_TTL()
        {
            var provider = new CloudFilesProvider();
            var cdnEnabledResponse = provider.EnableCDNOnContainer(containerName, 1000, identity: _testIdentity);

            var cdnContainerHeaderResponse = provider.GetContainerCDNHeader(containerName, identity: _testIdentity);

            Assert.AreEqual(1000, cdnContainerHeaderResponse.Ttl);
            Assert.IsTrue(cdnContainerHeaderResponse.CDNEnabled);

        }

        [TestMethod]
        public void Should_Make_Container_CDN_Enabled_With_Log_Retention()
        {
            var provider = new CloudFilesProvider();
            var cdnEnabledResponse = provider.EnableCDNOnContainer(containerName, true, identity: _testIdentity);

            var cdnContainerHeaderResponse = provider.GetContainerCDNHeader(containerName, identity: _testIdentity);

            Assert.AreEqual(259200, cdnContainerHeaderResponse.Ttl);
            Assert.IsTrue(cdnContainerHeaderResponse.LogRetention);
            Assert.IsTrue(cdnContainerHeaderResponse.CDNEnabled);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(cdnContainerHeaderResponse.CDNIosUri));
            Assert.IsTrue(containerName.Equals(cdnContainerHeaderResponse.Name, StringComparison.InvariantCultureIgnoreCase));

        }

        [TestMethod]
        public void Should_Make_Container_CDN_Disabled()
        {
            var provider = new CloudFilesProvider();
            var cdnEnabledResponse = provider.DisableCDNOnContainer(containerName, identity: _testIdentity);

            var cdnContainerHeaderResponse = provider.GetContainerCDNHeader(containerName, identity: _testIdentity);

            Assert.IsFalse(cdnContainerHeaderResponse.CDNEnabled);
        }

        [TestMethod]
        public void Should_Enable_Static_Web_On_Container_With_Index_Error_CSS_And_Listing_As_True()
        {
            var provider = new CloudFilesProvider();
            //var cdnEnabledResponse = provider.EnableCDNOnContainer( containerName, true);

            provider.EnableStaticWebOnContainer(containerName, webIndex, webError, webListingsCSS, webListing, null, false, _testIdentity);
            var cdnContainerMetaDataResponse = provider.GetContainerMetaData(containerName, identity: _testIdentity);

            Assert.AreEqual(webIndex, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("Web-index", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual(webError, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-error", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual(webListingsCSS, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-listings-css", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.IsTrue(bool.Parse(cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-listings", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));

        }

        [TestMethod]
        public void Should_Enable_Static_Web_On_Container_With_CSS_And_Listing_As_True()
        {
            var provider = new CloudFilesProvider();
            //var cdnEnabledResponse = provider.EnableCDNOnContainer( containerName, true, identity: _testIdentity);

            provider.EnableStaticWebOnContainer(containerName, webListingsCSS, webListing, null, false, _testIdentity);
            var cdnContainerMetaDataResponse = provider.GetContainerMetaData(containerName, identity: _testIdentity);

            Assert.AreEqual(webListingsCSS, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-listings-css", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.IsTrue(bool.Parse(cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-listings", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));

        }

        [TestMethod]
        public void Should_Enable_Static_Web_On_Container_With_Index_Error_And_Listing_As_True()
        {
            var provider = new CloudFilesProvider();
            //var cdnEnabledResponse = provider.EnableCDNOnContainer( containerName, true, identity: _testIdentity);

            provider.EnableStaticWebOnContainer(containerName, webIndex, webError, webListing, null, false, _testIdentity);
            var cdnContainerMetaDataResponse = provider.GetContainerMetaData(containerName, identity: _testIdentity);

            Assert.AreEqual(webIndex, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("Web-index", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual(webError, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-error", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.IsTrue(bool.Parse(cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-listings", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));

        }

        [TestMethod]
        public void Should_Enable_Static_Web_On_Container_With_Index_And_Error()
        {
            var provider = new CloudFilesProvider();
            //var cdnEnabledResponse = provider.EnableCDNOnContainer( containerName, true, identity: _testIdentity);

            provider.EnableStaticWebOnContainer(containerName, webIndex, webError, null, false, _testIdentity);
            var cdnContainerMetaDataResponse = provider.GetContainerMetaData(containerName, identity: _testIdentity);

            Assert.AreEqual(webIndex, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("Web-index", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual(webError, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-error", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        public void Should_Disable_Static_Web_On_Container()
        {

            var provider = new CloudFilesProvider();
            //var cdnEnabledResponse = provider.EnableCDNOnContainer( containerName, true, identity: _testIdentity);

            provider.DisableStaticWebOnContainer(containerName, null, false, _testIdentity);
            var cdnContainerMetaDataResponse = provider.GetContainerMetaData(containerName, identity: _testIdentity);


            Assert.IsFalse(cdnContainerMetaDataResponse.ContainsKey("Web-Index"));
            Assert.IsFalse(cdnContainerMetaDataResponse.ContainsKey("Web-Error"));
            Assert.IsFalse(cdnContainerMetaDataResponse.ContainsKey("Web-Listings-Css"));
            Assert.IsFalse(cdnContainerMetaDataResponse.ContainsKey("Web-Listings"));

        }

        #endregion Container Tests

        #region Object Tests


        [TestMethod]
        public void Should_Get_Headers_For_Object()
        {

            var provider = new CloudFilesProvider();
            var objectHeadersResponse = provider.GetObjectHeaders(containerName, objectName, identity: _testIdentity);

            Assert.IsNotNull(objectHeadersResponse);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_Exception_When_Calling_Get_Headers_For_Object_And_Container_Name_Is_Empty()
        {
            string containerNameLocal = string.Empty;
            var provider = new CloudFilesProvider();
            var objectHeadersResponse = provider.GetObjectHeaders(containerNameLocal, objectName, identity: _testIdentity);

            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_Exception_When_Calling_Get_Headers_For_Object_And_Object_Name_Is_Empty()
        {
            string objectNameLocal = string.Empty;
            var provider = new CloudFilesProvider();
            var objectHeadersResponse = provider.GetObjectHeaders(containerName, objectNameLocal, identity: _testIdentity);

            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        [DeploymentItem("DarkKnightRises.jpg")]
        public void Should_Create_Object_From_Stream_With_Headers()
        {
            var etag = GetMD5Hash(objectName);
            var headers = new Dictionary<string, string> {{"ETag", etag}};
            var provider = new CloudFilesProvider(_testIdentity);
            using (var stream = File.OpenRead(objectName))
            {
                provider.CreateObject(containerName, stream, objectName, headers: headers);
            }

            var containerGetObjectsResponse = provider.ListObjects(containerName, identity: _testIdentity);
            Assert.AreEqual(objectName, containerGetObjectsResponse.Where(x => x.Name.Equals(objectName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Name);

            var objectHeadersResponse = provider.GetObjectHeaders(containerName, objectName, identity: _testIdentity);

            Assert.IsNotNull(objectHeadersResponse);
            Assert.AreEqual(etag, objectHeadersResponse.Where(x => x.Key.Equals("ETag", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }


        private static string GetMD5Hash(string filePath)
        {
            byte[] computedHash = new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(filePath));
            var sBuilder = new StringBuilder();
            foreach (byte b in computedHash)
            {
                sBuilder.Append(b.ToString("x2").ToLower());
            }
            return sBuilder.ToString();
        }

        [TestMethod]
        [DeploymentItem("DarkKnightRises.jpg")]
        public void Should_Create_Object_From_Stream_Without_Headers()
        {
            string fileName = objectName;
            Stream stream = File.OpenRead(objectName);
            stream.Position = 0;

            var headers = new Dictionary<string, string>();
            var provider = new CloudFilesProvider(_testIdentity);
            provider.CreateObject(containerName, stream, fileName, 65536, headers);

            var containerGetObjectsResponse = provider.ListObjects(containerName, identity: _testIdentity);
            Assert.AreEqual(fileName, containerGetObjectsResponse.Where(x => x.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Name);

        }

        [TestMethod]
        [DeploymentItem("DarkKnightRises.jpg")]
        public void Should_Create_Object_From_File_With_Headers()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), objectName);
            var etag = GetMD5Hash(filePath);
            var headers = new Dictionary<string, string> {{"ETag", etag}};
            var provider = new CloudFilesProvider(_testIdentity);
            provider.CreateObjectFromFile(containerName, filePath, objectName, headers: headers);

            var containerGetObjectsResponse = provider.ListObjects(containerName, identity: _testIdentity);
            Assert.AreEqual(objectName, containerGetObjectsResponse.Where(x => x.Name.Equals(objectName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Name);

            var objectHeadersResponse = provider.GetObjectHeaders(containerName, objectName, identity: _testIdentity);

            Assert.IsNotNull(objectHeadersResponse);
            Assert.AreEqual(etag, objectHeadersResponse.Where(x => x.Key.Equals("ETag", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        [DeploymentItem("DarkKnightRises.jpg")]
        public void Should_Create_Object_From_File_Without_Headers()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), objectName);
            string fileName = Path.GetFileName(filePath);
            var headers = new Dictionary<string, string>();
            var provider = new CloudFilesProvider(_testIdentity);
            provider.CreateObjectFromFile(containerName, filePath, fileName, 65536, headers, identity: _testIdentity);

            var containerGetObjectsResponse = provider.ListObjects(containerName, identity: _testIdentity);
            Assert.AreEqual(fileName, containerGetObjectsResponse.Where(x => x.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Name);
        }

        [TestMethod]
        [DeploymentItem("DarkKnightRises.jpg")]
        public void Should_Get_Object_And_Save_To_File_Without_Headers()
        {
            var provider = new CloudFilesProvider(_testIdentity);
            provider.GetObjectSaveToFile(containerName, Directory.GetCurrentDirectory(), objectName, saveFileNane1);
        }

        [TestMethod]
        [DeploymentItem("DarkKnightRises.jpg")]
        public void Should_Get_Object_And_Save_To_File_Without_Headers_And_Verify_Etag()
        {
            var provider = new CloudFilesProvider(_testIdentity);
            provider.GetObjectSaveToFile(containerName, Directory.GetCurrentDirectory(), objectName, saveFileNane2, verifyEtag: true);
        }

        [TestMethod]
        public void Should_Delete_Object()
        {
            string fileName = objectName;
            var headers = new Dictionary<string, string>();
            var provider = new CloudFilesProvider();
            var deleteResponse = provider.DeleteObject(containerName, fileName, headers, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectDeleted, deleteResponse);
        }

        [TestMethod]
        public void Should_Delete_Object_On_Destination_Container()
        {
            var headers = new Dictionary<string, string>();
            var provider = new CloudFilesProvider();
            var deleteResponse = provider.DeleteObject(destinationContainerName, destinationObjectName, headers, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectDeleted, deleteResponse);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Throw_An_Exception_When_Deleting_Object()
        {
            // TODO: Also need to make 404 as an acceptable status.
            var headers = new Dictionary<string, string>();
            var provider = new CloudFilesProvider();
            provider.DeleteObject(containerName, objectName, headers, identity: _testIdentity);

            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void Should_Copy_Object_When_Not_Passing_Content_Length()
        {
            var provider = new CloudFilesProvider();
            var copyResponse = provider.CopyObject(sourceContainerName, sourceObjectName, destinationContainerName, destinationObjectName, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectCreated, copyResponse);
        }

        [TestMethod]
        public void Should_Copy_Object_When_Passing_Content_Length()
        {
            var provider = new CloudFilesProvider(_testIdentity);
            var copyResponse = provider.CopyObject(sourceContainerName, sourceObjectName, destinationContainerName, destinationObjectName);

            Assert.AreEqual(ObjectStore.ObjectCreated, copyResponse);

            var sourceheader = provider.GetObjectHeaders(sourceContainerName, sourceObjectName);
            var destinationHeader = provider.GetObjectHeaders(destinationContainerName, destinationObjectName);

            Assert.AreEqual(sourceheader.First(h => h.Key.Equals("ETag", StringComparison.OrdinalIgnoreCase)).Value, destinationHeader.First(h => h.Key.Equals("ETag", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.AreEqual(sourceheader.First(h => h.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)).Value, destinationHeader.First(h => h.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)).Value);
        }

        [TestMethod]
        public void Should_Copy_Object_When_Not_Passing_Content_Length_And_Passing_Expiring_Header()
        {
            // Object will expire 2 days from now.
            var epoch = (int)(DateTime.UtcNow.AddDays(2) - new DateTime(1970, 1, 1)).TotalSeconds;

            var header = new Dictionary<string, string> { { CloudFilesProvider.ObjectDeleteAt, epoch.ToString() } };

            var provider = new CloudFilesProvider(_testIdentity);
            var copyResponse = provider.CopyObject(sourceContainerName, sourceObjectName, destinationContainerName, destinationObjectName, header);

            Assert.AreEqual(ObjectStore.ObjectCreated, copyResponse);

            var sourceheader = provider.GetObjectHeaders(sourceContainerName, sourceObjectName);
            var destinationHeader = provider.GetObjectHeaders(destinationContainerName, destinationObjectName);

            Assert.AreEqual(sourceheader.First(h => h.Key.Equals("ETag", StringComparison.OrdinalIgnoreCase)).Value, destinationHeader.First(h => h.Key.Equals("ETag", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.AreEqual(sourceheader.First(h => h.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)).Value, destinationHeader.First(h => h.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)).Value);

        }

        [TestMethod]
        public void Should_Add_MetaData_For_Object()
        {
            var metaData = new Dictionary<string, string>();
            metaData.Add("key1", "value1");
            metaData.Add("key2", "value2");
            metaData.Add("key3", "value3");
            metaData.Add("key4", "value4");
            var provider = new CloudFilesProvider();
            provider.UpdateObjectMetadata(containerName, objectName, metaData, identity: _testIdentity);
        }

        [TestMethod]
        public void Should_Get_MetaData_For_Object_And_Include_Key1_And_Key2_And_Key3_And_Key4()
        {
            var provider = new CloudFilesProvider(_testIdentity);
            var metaData = provider.GetObjectMetaData(containerName, objectName);
            Assert.IsNotNull(metaData);
            Assert.IsTrue(metaData.Any());
            Assert.AreEqual("value1", metaData.Where(x => x.Key.Equals("key1", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual("value2", metaData.Where(x => x.Key.Equals("key2", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual("value3", metaData.Where(x => x.Key.Equals("key3", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual("value4", metaData.Where(x => x.Key.Equals("key4", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        public void Should_Remove_Multiple_Metadata_For_Object_Items_Key2_and_Key3()
        {
            var provider = new CloudFilesProvider(_testIdentity);
            var metaData = new Dictionary<string, string> { { "key2", "value2" }, { "key3", "value3" } };

            provider.DeleteObjectMetadata(containerName, objectName, metaData);
        }

        [TestMethod]
        public void Should_Get_MetaData_For_Object_After_Multiple_Delete_And_Include_Key1_And_Key4()
        {
            var provider = new CloudFilesProvider(_testIdentity);
            var metaData = provider.GetObjectMetaData(containerName, objectName);
            Assert.IsNotNull(metaData);
            Assert.AreEqual("value1", metaData.Where(x => x.Key.Equals("key1", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual("value4", metaData.Where(x => x.Key.Equals("key4", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        public void Should_Remove_Single_Metadata_For_Object_Item_Key1()
        {
            var provider = new CloudFilesProvider(_testIdentity);

            provider.DeleteObjectMetadata(containerName, objectName, "key1");
        }

        [TestMethod]
        public void Should_Get_MetaData_For_Object_After_Single_Delete_And_Include__Key4()
        {
            var provider = new CloudFilesProvider(_testIdentity);
            var metaData = provider.GetObjectMetaData(containerName, objectName);
            Assert.IsNotNull(metaData);
            Assert.AreEqual("value4", metaData.Where(x => x.Key.Equals("key4", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        public void Should_Purge_CDN_Enabled_Object_No_Email_Notification()
        {
            var provider = new CloudFilesProvider();
            var objectDeleteResponse = provider.PurgeObjectFromCDN(containerName, objectName, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectPurged, objectDeleteResponse);
        }

        [TestMethod]
        public void Should_Purge_CDN_Enabled_Object_Single_Email_Notification()
        {
            var provider = new CloudFilesProvider();
            var objectDeleteResponse = provider.PurgeObjectFromCDN(containerName, objectName, email: emailTo, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectPurged, objectDeleteResponse);

        }

        [TestMethod]
        public void Should_Purge_CDN_Enabled_Object_Multiple_Email_Notification()
        {
            var provider = new CloudFilesProvider();
            var objectDeleteResponse = provider.PurgeObjectFromCDN(containerName, objectName, emailToList, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectPurged, objectDeleteResponse);
        }

        [TestMethod]
        public void Should_Reset_The_Batch_Threshold()
        {
            _originalThreshold = CloudFilesProvider.LargeFileBatchThreshold;
            CloudFilesProvider.LargeFileBatchThreshold = 81920;
        }


        [TestMethod]
        public void Should_Create_New_Test_Container_2()
        {
            var provider = new CloudFilesProvider(_testIdentity);

            provider.CreateContainer(containerName2);

            var containers = provider.ListContainers();
            Assert.IsTrue(containers.Any(c => c.Name.Equals(containerName2)));
        }

        [TestMethod]
        [DeploymentItem("DarkKnightRises.jpg")]
        public void Should_Upload_File_In_Segments()
        {
            var provider = new CloudFilesProvider(_testIdentity);

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), objectName);

            provider.CreateObjectFromFile(containerName2, filePath);

            var objects = provider.ListObjects(containerName2).ToArray();

            Assert.AreEqual(12, objects.Count());
            Assert.IsTrue(objects.Any(o => o.Name.Equals(objectName)));
            for (int i = 0; i < 11; i++)
            {
                Assert.IsTrue(objects.Any(o => o.Name.Equals(string.Format("{0}.seg{1}", objectName, i.ToString("0000")))));
            }
        }

        [TestMethod]
        public void Should_Delete_Object_And_All_Segments()
        {
            var provider = new CloudFilesProvider(_testIdentity);

            provider.DeleteObject(containerName2, objectName, deleteSegments: true);

            var objects = provider.ListObjects(containerName2).ToArray();

            Assert.AreEqual(0, objects.Count());
            Assert.IsFalse(objects.Any(o => o.Name.Equals(objectName)));
            for (int i = 0; i < 11; i++)
            {
                Assert.IsFalse(objects.Any(o => o.Name.Equals(string.Format("{0}.seg{1}", objectName, i.ToString("0000")))));
            }
        }

        [TestMethod]
        public void Should_Delete_Object_But_Not_The_Segments()
        {
            var provider = new CloudFilesProvider(_testIdentity);

            provider.DeleteObject(containerName2, objectName, deleteSegments: false);

            var objects = provider.ListObjects(containerName2).ToArray();

            Assert.AreEqual(11, objects.Count());
            Assert.IsFalse(objects.Any(o => o.Name.Equals(objectName)));
            for (int i = 0; i < 11; i++)
            {
                Assert.IsTrue(objects.Any(o => o.Name.Equals(string.Format("{0}.seg{1}", objectName, i.ToString("0000")))));
            }
        }

        [TestMethod]
        public void Should_Bulk_Delete_All_Objects()
        {
            var provider = new CloudFilesProvider(_testIdentity);

            provider.BulkDelete(provider.ListObjects(containerName2).Select(o => string.Format("{0}/{1}", containerName2, o.Name)));

            var objects = provider.ListObjects(containerName2).ToArray();

            Assert.AreEqual(0, objects.Count());
        }

        [TestMethod]
        public void Should_Purge_Objects_Before_Deleting_The_Conatiner()
        {
            var provider = new CloudFilesProvider(_testIdentity);

            provider.DeleteContainer(containerName2);

            var containers = provider.ListContainers();
            Assert.IsFalse(containers.Any(c => c.Name.Equals(containerName2)));
        }


        [TestMethod]
        public void Should_Reset_The_Batch_Threshold_To_Original()
        {
            CloudFilesProvider.LargeFileBatchThreshold = _originalThreshold;
        }

        #endregion Object Tests

        #region Account Tests
        [TestMethod]
        public void Should_Get_Headers_For_Account()
        {
            var provider = new CloudFilesProvider();
            var accountHeadersResponse = provider.GetAccountHeaders(identity: _testIdentity);

            Assert.IsNotNull(accountHeadersResponse);
            Assert.IsTrue(accountHeadersResponse.ContainsKey("x-account-object-count"));

        }

        [TestMethod]
        public void Should_Get_MetaData_For_Account()
        {

            var provider = new CloudFilesProvider();
            var accountHeadersResponse = provider.GetAccountMetaData(identity: _testIdentity);

            Assert.IsNotNull(accountHeadersResponse);
            Assert.IsTrue(accountHeadersResponse.ContainsKey("Temp-Url-Key"));
        }

        [TestMethod]
        public void Should_Add_MetaData_For_Account()
        {
            var metaData = new Dictionary<string, string>();
            metaData.Add("Test-Accountmetadata", "Test");
            var provider = new CloudFilesProvider();
            provider.UpdateAccountMetadata(metaData, identity: _testIdentity);
            var accountHeadersResponse = provider.GetAccountMetaData(identity: _testIdentity);

            Assert.IsNotNull(accountHeadersResponse);
            Assert.IsTrue(accountHeadersResponse.ContainsKey("Test-Accountmetadata"));
            Assert.AreEqual("Test", accountHeadersResponse.Where(x => x.Key.Equals("Test-Accountmetadata", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);

        }

        [TestMethod]
        public void Should_Update_Headers_For_Account()
        {
            var headers = new Dictionary<string, string>();
            headers.Add("X-Account-Meta-Test-Accountmetadata", "Test1");

            var provider = new CloudFilesProvider();
            provider.UpdateAccountHeaders(headers, identity: _testIdentity);
            var accountHeadersResponse = provider.GetAccountMetaData(identity: _testIdentity);

            Assert.IsNotNull(accountHeadersResponse);
            Assert.IsTrue(accountHeadersResponse.ContainsKey("Test-Accountmetadata"));
            Assert.AreEqual("Test1", accountHeadersResponse.Where(x => x.Key.Equals("Test-Accountmetadata", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);

        }

        #endregion
    }
}
