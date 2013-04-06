using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Providers.Rackspace;

namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    [TestClass]
    public class ObjectStoreTests
    {

        private static RackspaceCloudIdentity _testIdentity;
        private static RackspaceCloudIdentity _testAdminIdentity;

        public ObjectStoreTests()
        {
            CloudInstance cloudInstance;
        }

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

        #region Container Tests

        [TestMethod]
        public void Should_Return_Container_List()
        {
            var provider = new ObjectStoreProvider();
            var containerList = provider.ListContainers(identity:_testIdentity);

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_Limit()
        {
            var provider = new ObjectStoreProvider();
            var containerList = provider.ListContainers( 1,identity:_testIdentity);

            Assert.IsNotNull(containerList);
            Assert.AreEqual(1, containerList.Count());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_Start_Marker_Lower_Case()
        {
            var provider = new ObjectStoreProvider();
            var containerList = provider.ListContainers(null, "a", identity: _testIdentity);

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_Start_Marker_Upper_Case()
        {
            var provider = new ObjectStoreProvider();
            var containerList = provider.ListContainers(null, "A", identity: _testIdentity);

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }


        [TestMethod]
        public void Should_Return_Container_List_With_End_Marker_Upper_Case()
        {
            var provider = new ObjectStoreProvider();
            var containerList = provider.ListContainers(null, null, "L", identity: _testIdentity);

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_End_Marker_Lower_Case()
        {
            var provider = new ObjectStoreProvider();
            var containerList = provider.ListContainers(null, null, "l", identity: _testIdentity);

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Create_Container()
        {
            const string containerName = "TestContainer";
            var provider = new ObjectStoreProvider();
            var containerCreatedResponse = provider.CreateContainer(containerName, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ContainerCreated, containerCreatedResponse);
        }

        [TestMethod]
        public void Should_Not_Create_Container_Already_Exists()
        {
            const string containerName = "TestContainer";
            var provider = new ObjectStoreProvider();
            var containerCreatedResponse = provider.CreateContainer(containerName, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ContainerExists, containerCreatedResponse);
        }

        [TestMethod]
        public void Should_Delete_Container()
        {
            const string containerName = "TestContainer";
            var provider = new ObjectStoreProvider();
            var containerCreatedResponse = provider.DeleteContainer(containerName, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ContainerDeleted, containerCreatedResponse);
        }

        [TestMethod]
        public void Should_Get_Objects_From_Container()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var containerGetObjectsResponse = provider.GetObjects(containerName, identity: _testIdentity);

            Assert.IsNotNull(containerGetObjectsResponse);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Throw_An_Exception_When_Calling_Get_Objects_From_Container_And_Container_Does_Not_Exist()
        {
            const string containerName = "No_Container_Present";
            var provider = new ObjectStoreProvider();
            var containerGetObjectsResponse = provider.GetObjects(containerName, identity: _testIdentity);
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Throw_An_Exception_When_Calling_Get_Objects_From_Container_And_Objects_Does_Not_Exist()
        {
            const string containerName = "RK_Teat";
            var provider = new ObjectStoreProvider();
            var containerGetObjectsResponse = provider.GetObjects(containerName, identity: _testIdentity);
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void Should_Get_Headers_For_Container()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetContainerHeader(containerName, identity: _testIdentity);

            Assert.IsNotNull(objectHeadersResponse);
            //Assert.AreEqual("Christian Bale", objectHeadersResponse.Where(x => x.Key.Equals("X-Object-Meta-Actor", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        public void Should_Get_MetaData_For_Container()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetContainerMetaData(containerName, identity: _testIdentity);

            Assert.IsNotNull(objectHeadersResponse);
            Assert.IsFalse(bool.Parse(objectHeadersResponse.Where(x => x.Key.Equals("Access-Log-Delivery", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));
        }

        [TestMethod]
        public void Should_Add_MetaData_For_Container()
        {
            const string containerName = "DarkKnight";
            var metaData = new Dictionary<string, string>();
            metaData.Add("X-Container-Meta-XXXX", "Test");
            var provider = new ObjectStoreProvider();
            provider.AddContainerMetadata(containerName, metaData, identity: _testIdentity);
        }

        [TestMethod]
        public void Should_Add_Headers_For_Container()
        {
            const string containerName = "DarkKnight";
            var metaData = new Dictionary<string, string>();
            metaData.Add("X-Container-Meta-Movie", "Batman");
            metaData.Add("X-Container-Meta-Genre", "Action");
            var provider = new ObjectStoreProvider();
            provider.AddContainerHeaders(containerName, metaData, identity: _testIdentity);
        }

        [TestMethod]
        public void Should_Get_CDN_Headers_For_Container()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetContainerCDNHeader(containerName, identity: _testIdentity);

            Assert.IsNotNull(objectHeadersResponse);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Not_Get_CDN_Headers_For_Container()
        {
            const string containerName = "cloudservers";
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetContainerCDNHeader(containerName, identity: _testIdentity);

            Assert.Fail("Expected exception was not thrown.");
        }

        //X-Log-Retention, X-CDN-enabled, and X-TTL.
        [TestMethod]
        public void Should_Add_CDN_Headers_For_Container()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Log-Retention", "false");
            provider.AddContainerCdnHeaders(containerName, headers, identity: _testIdentity);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Not_Add_CDN_Headers_For_Container()
        {
            const string containerName = "cloudservers";
            var provider = new ObjectStoreProvider();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Log-Retention", "false");
            provider.AddContainerCdnHeaders(containerName, headers, identity: _testIdentity);
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void Should_Get_CDN_Enabled_Containers()
        {
            var provider = new ObjectStoreProvider();
            var cdnContainerList = provider.ListCDNContainers(null, null, null, true, identity: _testIdentity);

            Assert.IsNotNull(cdnContainerList);
            Assert.IsTrue(cdnContainerList.All(x => x.CDNEnabled == true));
        }

        [TestMethod]
        public void Should_Get_CDN_All_Containers()
        {
            var provider = new ObjectStoreProvider();
            var cdnContainerList = provider.ListCDNContainers(identity: _testIdentity);

            Assert.IsNotNull(cdnContainerList);
            Assert.IsTrue(cdnContainerList.Any(x => x.CDNEnabled == true));
            Assert.IsTrue(cdnContainerList.Any(x => x.CDNEnabled == false));
        }

        [TestMethod]
        public void Should_Make_Container_CDN_Enabled_With_TTL()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var cdnEnabledResponse = provider.EnableCDNOnContainer(containerName, 1000, identity: _testIdentity);

            var cdnContainerHeaderResponse = provider.GetContainerCDNHeader(containerName, identity: _testIdentity);

            Assert.AreEqual(1000, int.Parse(cdnContainerHeaderResponse.Where(x => x.Key.Equals("X-Ttl", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));
            Assert.IsTrue(bool.Parse(cdnContainerHeaderResponse.Where(x => x.Key.Equals("X-Cdn-Enabled", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));

        }

        [TestMethod]
        public void Should_Make_Container_CDN_Enabled_With_Log_Retention()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var cdnEnabledResponse = provider.EnableCDNOnContainer(containerName, true, identity: _testIdentity);

            var cdnContainerHeaderResponse = provider.GetContainerCDNHeader(containerName, identity: _testIdentity);

            Assert.AreEqual(259200, int.Parse(cdnContainerHeaderResponse.Where(x => x.Key.Equals("X-Ttl", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));
            Assert.IsTrue(bool.Parse(cdnContainerHeaderResponse.Where(x => x.Key.Equals("X-Log-Retention", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));
            Assert.IsTrue(bool.Parse(cdnContainerHeaderResponse.Where(x => x.Key.Equals("X-Cdn-Enabled", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));

        }

        [TestMethod]
        public void Should_Make_Container_CDN_Disabled()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var cdnEnabledResponse = provider.DisableCDNOnContainer(containerName, identity: _testIdentity);

            var cdnContainerHeaderResponse = provider.GetContainerCDNHeader(containerName, identity: _testIdentity);

            Assert.IsFalse(bool.Parse(cdnContainerHeaderResponse.Where(x => x.Key.Equals("X-Cdn-Enabled", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));
        }

        [TestMethod]
        public void Should_Enable_Static_Web_On_Container_With_Index_Error_CSS_And_Listing_As_True()
        {
            const string containerName = "DarkKnight";
            const string webIndex = "index.html";
            const string webError = "error.html";
            const string webListingsCSS = "index.css";
            const bool webListing = true;

            var provider = new ObjectStoreProvider();
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
            const string containerName = "DarkKnight";
            const string webListingsCSS = "index.css";
            const bool webListing = true;

            var provider = new ObjectStoreProvider();
            //var cdnEnabledResponse = provider.EnableCDNOnContainer( containerName, true, identity: _testIdentity);

            provider.EnableStaticWebOnContainer(containerName, webListingsCSS, webListing, null, false, _testIdentity);
            var cdnContainerMetaDataResponse = provider.GetContainerMetaData(containerName, identity: _testIdentity);

            Assert.AreEqual(webListingsCSS, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-listings-css", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.IsTrue(bool.Parse(cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-listings", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));

        }

        [TestMethod]
        public void Should_Enable_Static_Web_On_Container_With_Index_Error_And_Listing_As_True()
        {
            const string containerName = "DarkKnight";
            const string webIndex = "index.html";
            const string webError = "error.html";
            const bool webListing = true;

            var provider = new ObjectStoreProvider();
            //var cdnEnabledResponse = provider.EnableCDNOnContainer( containerName, true, identity: _testIdentity);

            provider.EnableStaticWebOnContainer(containerName, webIndex, webError, webListing, null, false, _testIdentity);
            var cdnContainerMetaDataResponse = provider.GetContainerMetaData( containerName);

            Assert.AreEqual(webIndex, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("Web-index", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual(webError, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-error", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.IsTrue(bool.Parse(cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-listings", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));

        }

        [TestMethod]
        public void Should_Enable_Static_Web_On_Container_With_Index_And_Error()
        {
            const string containerName = "DarkKnight";
            const string webIndex = "index.html";
            const string webError = "error.html";
        
            var provider = new ObjectStoreProvider();
            //var cdnEnabledResponse = provider.EnableCDNOnContainer( containerName, true, identity: _testIdentity);

            provider.EnableStaticWebOnContainer(containerName, webIndex, webError, null, false, _testIdentity);
            var cdnContainerMetaDataResponse = provider.GetContainerMetaData(containerName, identity: _testIdentity);

            Assert.AreEqual(webIndex, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("Web-index", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
            Assert.AreEqual(webError, cdnContainerMetaDataResponse.Where(x => x.Key.Equals("web-error", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        public void Should_Disable_Static_Web_On_Container()
        {
            const string containerName = "DarkKnight";
           
            var provider = new ObjectStoreProvider();
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
            const string containerName = "DarkKnight";
            const string objectName = "BatmanBegins.jpg";
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetObjectHeaders(containerName, objectName, identity: _testIdentity);

            Assert.IsNotNull(objectHeadersResponse);
            //Assert.AreEqual("Christian Bale", objectHeadersResponse.Where(x => x.Key.Equals("X-Object-Meta-Actor", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_Exception_When_Calling_Get_Headers_For_Object_And_Container_Name_Is_Empty()
        {
            string containerName = string.Empty;
            const string objectName = "BatmanBegins.jpg";
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetObjectHeaders(containerName, objectName, identity: _testIdentity);

            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_Exception_When_Calling_Get_Headers_For_Object_And_Object_Name_Is_Empty()
        {
            const string containerName = "DarkKnight";
            string objectName = string.Empty;
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetObjectHeaders(containerName, objectName, identity: _testIdentity);

            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void Should_Create_Object_From_Stream_With_Headers()
        {
            const string containerName = "DarkKnight";
            const string filePath = @"C:\Users\Public\Pictures\Sample Pictures\Koala.jpg";
            string fileName = Path.GetFileName(filePath);
            Stream stream = System.IO.File.OpenRead(filePath);
            var etag = GetMD5Hash(filePath);
            stream.Position = 0;
            var headers = new Dictionary<string, string>();
            headers.Add("ETag", etag);
            int cnt = 0;
            var info = new FileInfo(filePath);
            var totalBytest = info.Length;
            var provider = new ObjectStoreProvider(_testIdentity);
            provider.CreateObjectFromStream(containerName, stream, fileName, 65536, headers, null, (bytesWritten) =>
            {
                cnt = cnt + 1;
                if (cnt % 10 != 0)
                    return;

                var x = (float)bytesWritten / (float)totalBytest;
                var percentCompleted = (float)x * 100.00;

                Console.WriteLine(string.Format("{0:0.00} % Completed (Writen: {1} of {2})", percentCompleted, bytesWritten, totalBytest));
            });

            var containerGetObjectsResponse = provider.GetObjects(containerName, identity: _testIdentity);
            Assert.AreEqual(fileName, containerGetObjectsResponse.Where(x => x.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Name);

            var objectHeadersResponse = provider.GetObjectHeaders(containerName, fileName, identity: _testIdentity);

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
        public void Should_Create_Object_From_Stream_Without_Headers()
        {
            const string containerName = "DarkKnight";
            const string filePath = @"C:\Users\Public\Pictures\Sample Pictures\Tulips.jpg";
            string fileName = Path.GetFileName(filePath);
            Stream stream = System.IO.File.OpenRead(filePath);
            var etag = GetMD5Hash(filePath);
            stream.Position = 0;
            var headers = new Dictionary<string, string>();
            var provider = new ObjectStoreProvider(_testIdentity);
            provider.CreateObjectFromStream(containerName, stream, fileName, 65536, headers);

            var containerGetObjectsResponse = provider.GetObjects(containerName, identity: _testIdentity);
            Assert.AreEqual(fileName, containerGetObjectsResponse.Where(x => x.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Name);

        }

        [TestMethod]
        public void Should_Create_Object_From_File_With_Headers()
        {
            const string containerName = "DarkKnight";
            const string filePath = @"C:\Users\Public\Pictures\Sample Pictures\Hydrangeas.jpg";
            string fileName = Path.GetFileName(filePath);
            Stream stream = System.IO.File.OpenRead(filePath);
            var etag = GetMD5Hash(filePath);
            stream.Position = 0;
            var headers = new Dictionary<string, string>();
            headers.Add("ETag", etag);
            int cnt = 0;
            var info = new FileInfo(filePath);
            var totalBytest = info.Length;
            var provider = new ObjectStoreProvider(_testIdentity);
            provider.CreateObjectFromFile(containerName, filePath, fileName, 65536, headers, null, (bytesWritten) =>
            {
                cnt = cnt + 1;
                if (cnt % 10 != 0)
                    return;

                var x = (float)bytesWritten / (float)totalBytest;
                var percentCompleted = (float)x * 100.00;

                Console.WriteLine(string.Format("{0:0.00} % Completed (Writen: {1} of {2})", percentCompleted, bytesWritten, totalBytest));
            });

            var containerGetObjectsResponse = provider.GetObjects(containerName, identity: _testIdentity);
            Assert.AreEqual(fileName, containerGetObjectsResponse.Where(x => x.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Name);

            var objectHeadersResponse = provider.GetObjectHeaders(containerName, fileName, identity: _testIdentity);

            Assert.IsNotNull(objectHeadersResponse);
            Assert.AreEqual(etag, objectHeadersResponse.Where(x => x.Key.Equals("ETag", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        public void Should_Create_Object_From_File_Without_Headers()
        {
            const string containerName = "DarkKnight";
            const string filePath = @"C:\Users\Public\Pictures\Sample Pictures\Desert.jpg";
            string fileName = Path.GetFileName(filePath);
            var headers = new Dictionary<string, string>();
            var provider = new ObjectStoreProvider(_testIdentity);
            provider.CreateObjectFromFile(containerName, filePath, fileName, 65536, headers, identity: _testIdentity);

            var containerGetObjectsResponse = provider.GetObjects(containerName, identity: _testIdentity);
            Assert.AreEqual(fileName, containerGetObjectsResponse.Where(x => x.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Name);

        }


        [TestMethod]
        public void Should_Get_Object_And_Save_To_File_Without_Headers()
        {
            const string containerName = "DarkKnight";
            const string filePath = @"C:\Users\Public\Pictures\Sample Pictures\Koala.jpg";
            const string saveDirectory = @"C:\Users\Public\Pictures\Sample Pictures\";

            string fileName = Path.GetFileName(filePath);
            var headers = new Dictionary<string, string>();
            var provider = new ObjectStoreProvider();
            provider.GetObjectSaveToFile(containerName, saveDirectory, fileName, null, 65536, null, null, false, _testIdentity);

            //var containerGetObjectsResponse = provider.GetObjects( containerName);
            //Assert.AreEqual(fileName, containerGetObjectsResponse.Where(x => x.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Name);

        }

        [TestMethod]
        public void Should_Get_Object_And_Save_To_File_Without_Headers_And_Verify_Etag()
        {
            const string containerName = "DarkKnight";
            const string filePath = @"C:\Users\Public\Pictures\Sample Pictures\Koala.jpg";
            const string saveDirectory = @"C:\Users\Public\Pictures\Sample Pictures\";

            string fileName = Path.GetFileName(filePath);
            var headers = new Dictionary<string, string>();
            var provider = new ObjectStoreProvider();
            provider.GetObjectSaveToFile(containerName, saveDirectory, fileName, null, 65536, null, null, true, _testIdentity);
        }

        //[TestMethod]
        //public void Should_Get_Object_And_Save_To_File_Without_Headers_And_Verify_Etag_Fails()
        //{
        //    const string containerName = "DarkKnight";
        //    const string filePath = @"C:\Users\Public\Pictures\Sample Pictures\Koala.jpg";
        //    const string saveDirectory = @"C:\Users\Public\Pictures\";

        //    string fileName = Path.GetFileName(filePath);
        //    var headers = new Dictionary<string, string>();
        //    var provider = new ObjectStoreProvider();
        //    provider.GetObjectSaveToFile(containerName, saveDirectory, fileName, "test", 65536, null, null, true, _testIdentity);
        //}


        [TestMethod]
        public void Should_Delete_Object()
        {
            const string containerName = "DarkKnight";
            const string filePath = @"C:\Users\Public\Pictures\Sample Pictures\Desert.jpg";
            string fileName = Path.GetFileName(filePath);
            var headers = new Dictionary<string, string>();
            var provider = new ObjectStoreProvider();
            var deleteResponse = provider.DeleteObject(containerName, fileName, headers, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectDeleted, deleteResponse);
        }


        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Throw_An_Exception_When_Deleting_Object()
        {
            // TODO: Also need to make 404 as an acceptable status.
            const string containerName = "DarkKnight";
            const string filePath = @"C:\Users\Public\Pictures\Sample Pictures\Desert.jpg";
            string fileName = Path.GetFileName(filePath);
            var headers = new Dictionary<string, string>();
            var provider = new ObjectStoreProvider();
            var deleteResponse = provider.DeleteObject(containerName, fileName, headers, identity: _testIdentity);

            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void Should_Copy_Object_When_Not_Passing_Content_Length()
        {
            const string sourceContainerName = "DarkKnight";
            const string sourceObjectName = "BatmanBegins.jpg";

            const string destinationContainerName = "rk_test";
            const string destinationObjectName = "BatmanBegins.jpg";

            var provider = new ObjectStoreProvider();
            var copyResponse = provider.CopyObject(sourceContainerName, sourceObjectName, destinationContainerName, destinationObjectName, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectCreated, copyResponse);
        }

        [TestMethod]
        public void Should_Copy_Object_When_Passing_Content_Length()
        {
            const string sourceContainerName = "DarkKnight";
            const string sourceObjectName = "BatmanBegins.jpg";

            const string destinationContainerName = "rk_test";
            const string destinationObjectName = "BatmanBegins.jpg";

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add(ObjectStoreConstants.ContentLength, "62504");

            var provider = new ObjectStoreProvider();
            var copyResponse = provider.CopyObject(sourceContainerName, sourceObjectName, destinationContainerName, destinationObjectName, header, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectCreated, copyResponse);
        }

        [TestMethod]
        public void Should_Copy_Object_When_Not_Passing_Content_Length_And_Passing_Expiring_Header()
        {
            // Object will expire 2 days from now.
            int epoch = (int)(DateTime.UtcNow.AddDays(2) - new DateTime(1970, 1, 1)).TotalSeconds;
            const string sourceContainerName = "DarkKnight";
            const string sourceObjectName = "BatmanBegins.jpg";

            const string destinationContainerName = "rk_test";
            const string destinationObjectName = "BatmanBegins.jpg";

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add(ObjectStoreConstants.ObjectDeleteAt, epoch.ToString());

            var provider = new ObjectStoreProvider();
            var copyResponse = provider.CopyObject(sourceContainerName, sourceObjectName, destinationContainerName, destinationObjectName, header, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectCreated, copyResponse);

        }
        [TestMethod]
        public void Should_Get_MetaData_For_Object1()
        {
            const string containerName = "DarkKnight";
            const string objectName = "BatmanBegins.jpg";

            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetObjectMetaData(containerName, objectName, null, false, _testIdentity);

            Assert.IsNotNull(objectHeadersResponse);
            //Assert.AreEqual("Christian Bale", objectHeadersResponse.Where(x => x.Key.Equals("X-Object-Meta-Actor", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        public void Should_Purge_CDN_Enabled_Object_No_Email_Notification()
        {
            const string containerName = "DarkKnight";
            const string objectName = "BatmanBegins.jpg";
            var provider = new ObjectStoreProvider();
            var objectDeleteResponse = provider.PurgeObjectFromCDN(containerName, objectName, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectPurged, objectDeleteResponse);
            
        }

        [TestMethod]
        public void Should_Purge_CDN_Enabled_Object_Single_Email_Notification()
        {
            const string containerName = "DarkKnight";
            const string objectName = "TheDarkKnight.jpg";
            const string emailTo = "123@abc.com";

            var provider = new ObjectStoreProvider();
            var objectDeleteResponse = provider.PurgeObjectFromCDN(containerName, objectName, email:emailTo, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectPurged, objectDeleteResponse);

        }

        [TestMethod]
        public void Should_Purge_CDN_Enabled_Object_Multiple_Email_Notification()
        {
            const string containerName = "DarkKnight";
            const string objectName = "TheDarkKnight.jpg";
             var emailTo = new[]{"abc@123.com,123@abc.com"};

            var provider = new ObjectStoreProvider();
            var objectDeleteResponse = provider.PurgeObjectFromCDN(containerName, objectName, emailTo, identity: _testIdentity);

            Assert.AreEqual(ObjectStore.ObjectPurged, objectDeleteResponse);

        }

        #endregion Object Tests

    }
}
