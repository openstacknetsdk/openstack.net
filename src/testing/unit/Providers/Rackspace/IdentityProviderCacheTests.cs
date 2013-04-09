using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using JSIStudios.SimpleRESTServices.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.openstack;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Objects.Response;

namespace OpenStackNet.Testing.Unit.Providers.Rackspace
{
    [TestClass]
    public class IdentityProviderCacheTests
    {
        [TestMethod]
        public void Should_Not_Hit_Cache_When_Authenticating_The_First_Time()
        {
            var cacheMock = new Mock<ICache<UserAccess>>();
            var restServiceMock = new Mock<IRestService>();

            restServiceMock.Setup(m => m.Execute<AuthenticationResponse>(It.IsAny<string>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<RequestSettings>())).Returns(new Response<AuthenticationResponse>(200, "OK", new AuthenticationResponse(), new List<HttpHeader>(), null));
            restServiceMock.Setup(m => m.Execute<AuthenticationResponse>(It.IsAny<Uri>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<RequestSettings>())).Returns(new Response<AuthenticationResponse>(200, "OK", new AuthenticationResponse(), new List<HttpHeader>(), null));

            var identityProvider = new CloudIdentityProvider(restServiceMock.Object, cacheMock.Object);

            identityProvider.Authenticate(new RackspaceCloudIdentity());

            cacheMock.Verify(m => m.Get(It.IsAny<string>(), It.IsAny<Func<UserAccess>>(), It.IsAny<bool>()), Times.Never());
        }

        [TestMethod]
        public void Should_Never_Hit_Cache_When_Authenticating()
        {
            var cacheMock = new Mock<ICache<UserAccess>>();
            var restServiceMock = new Mock<IRestService>();

            restServiceMock.Setup(m => m.Execute<AuthenticationResponse>(It.IsAny<string>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<RequestSettings>())).Returns(new Response<AuthenticationResponse>(200, "OK", new AuthenticationResponse(), new List<HttpHeader>(), null));
            restServiceMock.Setup(m => m.Execute<AuthenticationResponse>(It.IsAny<Uri>(), It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<RequestSettings>())).Returns(new Response<AuthenticationResponse>(200, "OK", new AuthenticationResponse(), new List<HttpHeader>(), null));
            
            var identityProvider = new CloudIdentityProvider(restServiceMock.Object, cacheMock.Object);

            for (int i = 0; i < 100; i++)
            {
                identityProvider.Authenticate(new RackspaceCloudIdentity());
            }

            cacheMock.Verify(m => m.Get(It.IsAny<string>(), It.IsAny<Func<UserAccess>>(), It.IsAny<bool>()), Times.Never());
        }
    }
}
