using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Domain.Rackspace
{
    [DataContract]
    public class Record
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "data")]
        public string Data { get; set; }

        [DataMember(Name = "ttl")]
        public int Ttl { get; set; }

        [DataMember(Name = "priority")]
        public int? Priority { get; set; }

        [DataMember(Name = "updated")]
        public DateTime? Updated { get; internal set; }

        [DataMember(Name = "created")]
        public DateTime? Created { get; internal set; }

        [DataMember(Name = "comment")]
        public string Comment { get; set; }
    }
}
