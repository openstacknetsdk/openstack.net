namespace OpenStackNet.Testing.Unit.Providers.Rackspace
{
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
    }
}
