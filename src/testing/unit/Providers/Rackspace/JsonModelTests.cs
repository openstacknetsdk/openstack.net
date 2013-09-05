namespace OpenStackNet.Testing.Unit.Providers.Rackspace
{
    using System.Net;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Providers.Rackspace.Objects.Request;
    using net.openstack.Providers.Rackspace.Objects.Response;
    using Newtonsoft.Json;

    [TestClass]
    public class JsonModelTests
    {
        /// <seealso cref="PasswordCredential"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials (OpenStack Identity Service API v2.0 Reference)</seealso>
        [TestMethod]
        public void TestPasswordCredential()
        {
            string json = @"{ ""username"" : ""test_user"", ""password"" : ""mypass"" }";
            PasswordCredential credentials = JsonConvert.DeserializeObject<PasswordCredential>(json);
            Assert.IsNotNull(credentials);
            Assert.AreEqual("test_user", credentials.Username);
            Assert.AreEqual("mypass", credentials.Password);
        }

        /// <seealso cref="PasswordCredentialResponse"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials (OpenStack Identity Service API v2.0 Reference)</seealso>
        [TestMethod]
        public void TestPasswordCredentialResponse()
        {
            string json = @"{ ""passwordCredentials"" : { username : ""test_user"", password : ""mypass"" } }";
            PasswordCredentialResponse response = JsonConvert.DeserializeObject<PasswordCredentialResponse>(json);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.PasswordCredential);
            Assert.AreEqual("test_user", response.PasswordCredential.Username);
            Assert.AreEqual("mypass", response.PasswordCredential.Password);
        }

        /// <seealso href="http://docs.openstack.org/api/openstack-compute/2/content/ServerUpdate.html">Update Server (OpenStack Compute API v2 and Extensions Reference)</seealso>
        [TestMethod]
        public void TestUpdateServerRequest()
        {
            UpdateServerRequest request = new UpdateServerRequest("new-name", IPAddress.Parse("10.0.0.1"), IPAddress.Parse("2607:f0d0:1002:51::4"));
            string expectedJson = @"{""server"":{""name"":""new-name"",""accessIPv4"":""10.0.0.1"",""accessIPv6"":""2607:f0d0:1002:51::4""}}";
            string actual = JsonConvert.SerializeObject(request, Formatting.None);
            Assert.AreEqual(expectedJson, actual);
        }
    }
}
