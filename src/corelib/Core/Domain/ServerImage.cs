using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    public class ServerImage
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public Link[] Links { get; set; }

        [DataMember]
        public string Name { get; set; }
    }

    public class ServerImageDetails : ServerImage
    {
        [DataMember(Name = "OS-DCF:diskConfig")]
        public string DiskConfig { get; set; }

        [DataMember]
        public Metadata Metadata { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public DateTime Created { get; set; }

        [DataMember]
        public int Progress { get; set; }

        [DataMember]
        public DateTime Updated { get; set; }

        [DataMember]
        public int MinDisk { get; set; }

        [DataMember]
        public Server Server { get; set; }

        [DataMember]
        public int MinRAM { get; set; }
    }
}