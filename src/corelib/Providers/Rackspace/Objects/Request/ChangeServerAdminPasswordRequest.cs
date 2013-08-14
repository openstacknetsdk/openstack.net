namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ChangeServerAdminPasswordRequest
    {
        [JsonProperty("changePassword")]
        public ChangeAdminPasswordDetails Details { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class ChangeAdminPasswordDetails
    {
        [JsonProperty("adminPass")]
        public string AdminPassword { get; set; }
    }
}
