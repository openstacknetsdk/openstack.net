using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    public class SimpleServer : ServerBase
    {
        [DataMember]
        public string Name { get; internal set; }

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