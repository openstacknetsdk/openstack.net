namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class SimpleServer : ServerBase
    {
        [JsonProperty]
        public string Name { get; private set; }

        protected override void UpdateThis(ServerBase server)
        {
            base.UpdateThis(server);

            var details = server as SimpleServer;

            if (details == null)
                return;

            Name = details.Name;
        }
    }
}