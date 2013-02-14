using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Core.Domain;
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
            var containerList = provider.ListContainers(_testIdentity,1);

            Assert.IsNotNull(containerList);
            Assert.AreEqual(1, containerList.Count());
        }

        [TestMethod]
        public void Should_Return_Container_List_With_Start_Marker_Lower_Case()
        {
            var provider = new ObjectStoreProvider();
            var containerList = provider.ListContainers(_testIdentity, null,"a");

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
            var containerList = provider.ListContainers(_testIdentity, null, null,"L");

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


    }
}
