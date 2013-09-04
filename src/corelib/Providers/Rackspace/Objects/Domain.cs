namespace net.openstack.Providers.Rackspace.Objects
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Domain
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        public Domain(string name)
        {
            Name = name;
        }
    }
}
