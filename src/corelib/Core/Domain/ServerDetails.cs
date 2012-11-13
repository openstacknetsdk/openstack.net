using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class ServerDetails
    {
        [DataMember(Name = "OS-DCF:diskConfig" )]
        public string DiskConfig { get; set; }

        [DataMember(Name = "OS-EXT-STS:power_state")]
        public bool PowerState { get; set; }

        [DataMember(Name = "OS-EXT-STS:task_state")]
        public string TaskState { get; set; }

        [DataMember(Name = "OS-EXT-STS:vm_state")]
        public string VMState { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string AccessIPv4 { get; set; }

        [DataMember]
        public string AccessIPv6 { get; set; }

        [DataMember]
        public Metadata Metadata { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember(Name = "user_id")]
        public string UserId { get; set; }

        [DataMember]
        public ServerImage Image { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public Flavor Flavor { get; set; }

        [DataMember]
        public ServerAddresses Addresses { get; set; }

        [DataMember]
        public DateTime Created { get; set; }

        [DataMember]
        public string HostId { get; set; }

        [DataMember]
        public Link[] Links { get; set; }

        [DataMember]
        public int Progress { get; set; }

        [DataMember(Name = "rax-bandwidth:bandwidth")]
        public string[] Bandwidth { get; set; }

        [DataMember(Name = "tenant_id")]
        public string TenantId { get; set; }

        [DataMember]
        public DateTime Updated { get; set; }
    }
}