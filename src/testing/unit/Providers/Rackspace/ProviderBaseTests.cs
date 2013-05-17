using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using JSIStudios.SimpleRESTServices.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Providers;
using net.openstack.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Objects;

namespace OpenStackNet.Testing.Unit.Providers.Rackspace
{
    [TestClass]
    public class ProviderBaseTests
    {
        private const string _testService = "test";

        [TestMethod]
        public void Should_Return_Correct_Endpoint_When_Identity_Is_Explicitly_Set_And_Region_Is_Explicitly_Declared()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "DFW" }, new Endpoint { Region = "ORD" } } } },
                    User = new UserDetails { DefaultRegion = "DFW" }
                });
            var provider = new MockProvider(null, identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, "DFW", new CloudIdentity());

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("DFW", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_Endpoint_When_Identity_Is_Explicitly_Set_And_Region_Is_Different_Than_Default_Region()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "DFW" }, new Endpoint { Region = "ORD" } } } },
                    User = new UserDetails { DefaultRegion = "DFW" }
                });
            var provider = new MockProvider(null, identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, "ORD", new CloudIdentity());

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("ORD", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_Endpoint_When_Identity_Set_On_Provider_And_Region_Is_Different_Than_Default_Region()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "DFW" }, new Endpoint { Region = "ORD" } } } },
                    User = new UserDetails { DefaultRegion = "DFW" }
                });
            var provider = new MockProvider(new CloudIdentity(), identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, "ORD", null);

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("ORD", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_Endpoint_When_Identity_Is_Explicitly_Set_And_Region_Is_NOT_Explicitly_Declared()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "DFW" }, new Endpoint { Region = "ORD" } } } },
                    User = new UserDetails { DefaultRegion = "DFW" }
                });
            var provider = new MockProvider(null, identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, null, new CloudIdentity());

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("DFW", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_Endpoint_When_Identity_Is_Set_On_Provider_And_Region_Is_Explicitly_Declared()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "DFW" }, new Endpoint { Region = "ORD" } } } },
                    User = new UserDetails { DefaultRegion = "DFW" }
                });
            var provider = new MockProvider(new CloudIdentity(), identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, "DFW", null);

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("DFW", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_Endpoint_When_Identity_Is_Set_On_Provider_And_Region_Is_NOT_Explicitly_Declared()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "DFW" }, new Endpoint { Region = "ORD" } } } },
                    User = new UserDetails { DefaultRegion = "DFW" }
                });
            var provider = new MockProvider(new CloudIdentity(), identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, null, null);

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("DFW", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_LON_Endpoint_When_Identity_Is_Explicitly_Set_And_Region_Is_Explicitly_Declared()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "LON" }, new Endpoint { Region = "LON2" } } } },
                    User = new UserDetails { DefaultRegion = "LON" }
                });
            var provider = new MockProvider(null, identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, "LON", new RackspaceCloudIdentity {CloudInstance = CloudInstance.UK});

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("LON", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_LON_Endpoint_When_Identity_Is_Explicitly_Set_And_Region_Is_NOT_Explicitly_Declared()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "LON" }, new Endpoint { Region = "LON2" } } } },
                    User = new UserDetails { DefaultRegion = "LON" }
                });
            var provider = new MockProvider(null, identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, null, new RackspaceCloudIdentity {CloudInstance = CloudInstance.UK});

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("LON", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_LON_Endpoint_When_Identity_Is_Set_On_Provider_And_Region_Is_Explicitly_Declared()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "LON" }, new Endpoint { Region = "LON2" } } } },
                    User = new UserDetails { DefaultRegion = "LON" }
                });
            var provider = new MockProvider(new RackspaceCloudIdentity {CloudInstance = CloudInstance.UK}, identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, "LON", null);

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("LON", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_LON_Endpoint_When_Identity_Is_Set_On_Provider_And_Region_Is_NOT_Explicitly_Declared()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "LON" }, new Endpoint { Region = "LON2" } } } },
                    User = new UserDetails { DefaultRegion = "LON" }
                });
            var provider = new MockProvider(new RackspaceCloudIdentity {CloudInstance = CloudInstance.UK}, identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, null, null);

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("LON", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_LON_Endpoint_When_Identity_Is_Explicitly_And_Region_Is_Always_Empty()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "LON" }, new Endpoint { Region = "LON2" } } } },
                    User = new UserDetails { DefaultRegion = "" }
                });
            var provider = new MockProvider(null, identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, null, new RackspaceCloudIdentity { CloudInstance = CloudInstance.UK });

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("LON", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_LON_Endpoint_When_Identity_Is_Set_On_Provider_And_Region_Is_Always_Empty()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "LON" }, new Endpoint { Region = "LON2" } } } },
                    User = new UserDetails { DefaultRegion = "" }
                });
            var provider = new MockProvider(new RackspaceCloudIdentity { CloudInstance = CloudInstance.UK }, identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, null, null);

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("LON", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_LON_Endpoint_When_Identity_Is_Explicitly_And_Region_Is_Always_Null()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "LON" }, new Endpoint { Region = "LON2" } } } },
                    User = new UserDetails { DefaultRegion = null }
                });
            var provider = new MockProvider(null, identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, null, new RackspaceCloudIdentity { CloudInstance = CloudInstance.UK });

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("LON", endpoint.Region);
        }

        [TestMethod]
        public void Should_Return_Correct_LON_Endpoint_When_Identity_Is_Set_On_Provider_And_Region_Is_Always_Null()
        {
            var identityProviderMock = new Mock<IIdentityProvider>();
            identityProviderMock.Setup(m => m.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(
                new UserAccess
                {
                    ServiceCatalog = new[] { new ServiceCatalog() { Name = _testService, Endpoints = new[] { new Endpoint { Region = "LON" }, new Endpoint { Region = "LON2" } } } },
                    User = new UserDetails { DefaultRegion = null }
                });
            var provider = new MockProvider(new RackspaceCloudIdentity { CloudInstance = CloudInstance.UK }, identityProviderMock.Object, null);

            var endpoint = provider.GetEndpoint(_testService, null, null);

            Assert.IsNotNull(endpoint);
            Assert.AreEqual("LON", endpoint.Region);
        }

        public class MockProvider : ProviderBase<IIdentityProvider>
        {
            internal MockProvider(CloudIdentity defaultIdentity, IIdentityProvider identityProvider, IRestService restService) : base(defaultIdentity, identityProvider, restService)
            {
            }

            public Endpoint GetEndpoint(string serviceName, string region, CloudIdentity identity)
            {
                return base.GetServiceEndpoint(identity, serviceName, region);
            }

            protected override IIdentityProvider BuildProvider(CloudIdentity identity)
            {
                throw new NotImplementedException();
            }
        }
    }
}
