namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class NewUser
    {
        [JsonProperty("OS-KSADM:password")]
        public string Password { get; internal set; }

        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Id { get; private set; }

        [JsonProperty("username")]
        public string Username { get; private set; }

        [JsonProperty("email")]
        public string Email { get; private set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; private set; }

        public NewUser(string username, string email, string password = null, bool enabled = true)
        {
            Username = username;
            Email = email;
            Password = password;
            Enabled = enabled;
        }
    }
}
