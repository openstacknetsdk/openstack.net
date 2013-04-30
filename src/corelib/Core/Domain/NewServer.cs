using System.Collections.Generic;
using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class NewServer : ServerBase
    {
        [DataMember(Name = "OS-DCF:diskConfig")]
        public string DiskConfig { get; set; }

        [DataMember(Name = "adminPass")]
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