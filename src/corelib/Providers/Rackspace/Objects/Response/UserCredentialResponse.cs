namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using System.Collections.Generic;
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class UserCredentialResponse
    {
        [JsonProperty("RAX-KSKEY:apiKeyCredentials")]
        public UserCredential UserCredential { get; set; }
    }
    
    [JsonObject(MemberSerialization.OptIn)]
    internal class UserCredentialsResponse
    {
        [JsonProperty("credentials")]
        public List<UserCredential> Credentials { get; set; }
    }
}
