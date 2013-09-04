namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class UserCredential
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("username")]
        public string Username { get; private set; }

        [JsonProperty("apiKey")]
        public string APIKey { get; private set; }

        public UserCredential(string name, string username, string apiKey)
        {
            Name = name;
            Username = username;
            APIKey = apiKey;
        }
    }
}
