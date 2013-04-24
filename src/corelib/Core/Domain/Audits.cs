using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public partial class Audits
    {

        [DataMember(Name = "values")]
        public Value[] Values { get; set; }

        [DataMember(Name = "metadata")] 
        public AuditsMetadata Metadata;
    }

}
