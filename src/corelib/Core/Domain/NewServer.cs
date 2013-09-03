namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class NewServer : ServerBase
    {
        [JsonProperty("OS-DCF:diskConfig")]
        public string DiskConfig { get; set; }

        [JsonProperty("adminPass")]
        public string AdminPassword { get; set; }

        protected override void UpdateThis(ServerBase server)
        {
            base.UpdateThis(server);

            var details = server as NewServer;

            if (details == null)
                return;

            DiskConfig = details.DiskConfig;
            AdminPassword = details.AdminPassword;
        }
    }
}