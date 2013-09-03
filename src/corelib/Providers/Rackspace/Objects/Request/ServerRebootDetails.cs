namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ServerRebootDetails
    {
        [JsonProperty("type")]
        private string _type;

        public RebootType Type
        {
            get
            {
                if (string.IsNullOrEmpty(_type))
                    return null;

                return RebootType.FromName(_type);
            }

            set
            {
                if (value == null)
                    _type = null;
                else
                    _type = value.Name;
            }
        }
    }
}