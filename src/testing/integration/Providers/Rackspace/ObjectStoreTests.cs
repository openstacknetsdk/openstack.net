using System;
using System.Configuration;
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
            var containerList = provider.ListContainers(_testIdentity);

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_Limit()
        {
            var provider = new ObjectStoreProvider();
            var containerList = provider.ListContainers(_testIdentity, 1);

            Assert.IsNotNull(containerList);
            Assert.AreEqual(1, containerList.Count());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_Start_Marker_Lower_Case()
        {
            var provider = new ObjectStoreProvider();
            var containerList = provider.ListContainers(_testIdentity, null, "a");

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_Start_Marker_Upper_Case()
        {
            var provider = new ObjectStoreProvider();
            var containerList = provider.ListContainers(_testIdentity, null, "A");

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }


        [TestMethod]
        public void Should_Return_Container_List_With_End_Marker_Upper_Case()
        {
            var provider = new ObjectStoreProvider();
            var containerList = provider.ListContainers(_testIdentity, null, null, "L");

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_End_Marker_Lower_Case()
        {
            var provider = new ObjectStoreProvider();
            var containerList = provider.ListContainers(_testIdentity, null, null, "l");

            Assert.IsNotNull(containerList);
            Assert.IsTrue(containerList.Any());
        }

        [TestMethod]
        public void Should_Create_Container()
        {
            const string containerName = "TestContainer";
            var provider = new ObjectStoreProvider();
            var containerCreatedResponse = provider.CreateContainer(_testIdentity, containerName);

            Assert.AreEqual(ObjectStore.ContainerCreated, containerCreatedResponse);
        }

        [TestMethod]
        public void Should_Not_Create_Container_Already_Exists()
        {
            const string containerName = "TestContainer";
            var provider = new ObjectStoreProvider();
            var containerCreatedResponse = provider.CreateContainer(_testIdentity, containerName);

            Assert.AreEqual(ObjectStore.ContainerExists, containerCreatedResponse);
        }

        [TestMethod]
        public void Should_Delete_Container()
        {
            const string containerName = "TestContainer";
            var provider = new ObjectStoreProvider();
            var containerCreatedResponse = provider.DeleteContainer(_testIdentity, containerName);

            Assert.AreEqual(ObjectStore.ContainerDeleted, containerCreatedResponse);
        }

        [TestMethod]
        public void Should_Get_Objects_From_Container()
        {
            const string containerName = "lb_19087_ADMLab_-_LB80_Apr_2012";
            var provider = new ObjectStoreProvider();
            var containerGetObjectsResponse = provider.GetObjects(_testIdentity, containerName);

            Assert.IsNotNull(containerGetObjectsResponse);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Throw_An_Exception_When_Calling_Get_Objects_From_Container_And_Container_Does_Not_Exist()
        {
            const string containerName = "No_Container_Present";
            var provider = new ObjectStoreProvider();
            var containerGetObjectsResponse = provider.GetObjects(_testIdentity, containerName);
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Throw_An_Exception_When_Calling_Get_Objects_From_Container_And_Objects_Does_Not_Exist()
        {
            const string containerName = "RK_Teat";
            var provider = new ObjectStoreProvider();
            var containerGetObjectsResponse = provider.GetObjects(_testIdentity, containerName);
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void Should_Get_Headers_For_Container()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetHeaderForContainer(_testIdentity, containerName);

            Assert.IsNotNull(objectHeadersResponse);
            //Assert.AreEqual("Christian Bale", objectHeadersResponse.Where(x => x.Key.Equals("X-Object-Meta-Actor", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        public void Should_Get_MetaData_For_Container()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetMetaDataForContainer(_testIdentity, containerName);

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
            provider.AddContainerMetadata(_testIdentity, containerName, metaData);
        }

        [TestMethod]
        public void Should_Add_Headers_For_Container()
        {
            const string containerName = "DarkKnight";
            var metaData = new Dictionary<string, string>();
            metaData.Add("X-Container-Meta-Movie", "Batman");
            metaData.Add("X-Container-Meta-Genre", "Action");
            var provider = new ObjectStoreProvider();
            provider.AddContainerHeaders(_testIdentity, containerName, metaData);
        }

        [TestMethod]
        public void Should_Get_CDN_Headers_For_Container()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetCDNHeaderForContainer(_testIdentity, containerName);

            Assert.IsNotNull(objectHeadersResponse);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Not_Get_CDN_Headers_For_Container()
        {
            const string containerName = "cloudservers";
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetCDNHeaderForContainer(_testIdentity, containerName);

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
            provider.AddContainerCdnHeaders(_testIdentity, containerName, headers);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void Should_Not_Add_CDN_Headers_For_Container()
        {
            const string containerName = "cloudservers";
            var provider = new ObjectStoreProvider();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Log-Retention", "false");
            provider.AddContainerCdnHeaders(_testIdentity, containerName, headers);
            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        public void Should_Get_CDN_Enabled_Containers()
        {
            var provider = new ObjectStoreProvider();
            var cdnContainerList = provider.ListCDNContainers(_testIdentity, null, null, null, true);

            Assert.IsNotNull(cdnContainerList);
            Assert.IsTrue(cdnContainerList.All(x => x.CDNEnabled == true));
        }

        [TestMethod]
        public void Should_Get_CDN_All_Containers()
        {
            var provider = new ObjectStoreProvider();
            var cdnContainerList = provider.ListCDNContainers(_testIdentity);

            Assert.IsNotNull(cdnContainerList);
            Assert.IsTrue(cdnContainerList.Any(x => x.CDNEnabled == true));
            Assert.IsTrue(cdnContainerList.Any(x => x.CDNEnabled == false));
        }

        [TestMethod]
        public void Should_Make_Container_CDN_Enabled_With_TTL()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var cdnEnabledResponse = provider.EnableCDNOnContainer(_testIdentity, containerName, 1000);

            var cdnContainerHeaderResponse = provider.GetCDNHeaderForContainer(_testIdentity, containerName);

            Assert.AreEqual(1000,int.Parse(cdnContainerHeaderResponse.Where(x => x.Key.Equals("X-Ttl", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));
            Assert.IsTrue(bool.Parse(cdnContainerHeaderResponse.Where(x => x.Key.Equals("X-Cdn-Enabled", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));

        }

        [TestMethod]
        public void Should_Make_Container_CDN_Enabled_With_Log_Retention()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var cdnEnabledResponse = provider.EnableCDNOnContainer(_testIdentity, containerName, true);

            var cdnContainerHeaderResponse = provider.GetCDNHeaderForContainer(_testIdentity, containerName);

            Assert.AreEqual(259200, int.Parse(cdnContainerHeaderResponse.Where(x => x.Key.Equals("X-Ttl", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));
            Assert.IsTrue(bool.Parse(cdnContainerHeaderResponse.Where(x => x.Key.Equals("X-Log-Retention", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));
            Assert.IsTrue(bool.Parse(cdnContainerHeaderResponse.Where(x => x.Key.Equals("X-Cdn-Enabled", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));

        }

        [TestMethod]
        public void Should_Make_Container_CDN_Disabled()
        {
            const string containerName = "DarkKnight";
            var provider = new ObjectStoreProvider();
            var cdnEnabledResponse = provider.DisableCDNOnContainer(_testIdentity, containerName);

            var cdnContainerHeaderResponse = provider.GetCDNHeaderForContainer(_testIdentity, containerName);

            Assert.IsFalse(bool.Parse(cdnContainerHeaderResponse.Where(x => x.Key.Equals("X-Cdn-Enabled", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value));
        }

        #endregion Container Tests


        [TestMethod]
        public void Should_Get_Headers_For_Object()
        {
            const string containerName = "DarkKnight";
            const string objectName = "BatmanBegins.jpg";
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetObjectHeaders(_testIdentity, containerName, objectName);

            Assert.IsNotNull(objectHeadersResponse);
            Assert.AreEqual("Christian Bale", objectHeadersResponse.Where(x => x.Key.Equals("X-Object-Meta-Actor", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_Exception_When_Calling_Get_Headers_For_Object_And_Container_Name_Is_Empty()
        {
            string containerName = string.Empty;
            const string objectName = "BatmanBegins.jpg";
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetObjectHeaders(_testIdentity, containerName, objectName);

            Assert.Fail("Expected exception was not thrown.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_Exception_When_Calling_Get_Headers_For_Object_And_Object_Name_Is_Empty()
        {
            const string containerName = "DarkKnight";
            string objectName = string.Empty;
            var provider = new ObjectStoreProvider();
            var objectHeadersResponse = provider.GetObjectHeaders(_testIdentity, containerName, objectName);

            Assert.Fail("Expected exception was not thrown.");
        }

    }
}
