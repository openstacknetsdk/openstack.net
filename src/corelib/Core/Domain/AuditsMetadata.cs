using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class AuditsMetadata
    {

        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "limit")]
        public int Limit { get; set; }

        [DataMember(Name = "marker")]
        public object Marker { get; set; }

        [DataMember(Name = "next_marker")]
        public object NextMarker { get; set; }

        [DataMember(Name = "next_href")]
        public object NextHref { get; set; }
    }
}