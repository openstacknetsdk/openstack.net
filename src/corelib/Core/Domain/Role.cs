namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Role
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("description")]
        public string Description { get; private set; }

        public Role(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}