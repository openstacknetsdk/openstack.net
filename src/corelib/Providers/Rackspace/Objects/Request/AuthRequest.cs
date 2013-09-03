namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class AuthRequest
    {
        [JsonProperty("auth")]
        public AuthDetails Credentials { get; set; }

        public static AuthRequest FromCloudIdentity(CloudIdentity identity)
        {
            var creds = new AuthDetails();
            if (string.IsNullOrWhiteSpace(identity.Password))
                creds.APIKeyCredentials = new Credentials() { Username = identity.Username, APIKey = identity.APIKey};
            else
                creds.PasswordCredentials = new Credentials(){Username = identity.Username, Password = identity.Password};

            var raxIdentity = identity as RackspaceCloudIdentity;
            if (raxIdentity != null)
            {
                creds.Domain = raxIdentity.Domain;
            }

            return new AuthRequest { Credentials = creds };
        }
    }
}
