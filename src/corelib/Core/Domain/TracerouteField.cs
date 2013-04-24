using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    /// <summary>
    /// <param name="limit">Maximum tolerable limit before alarm occurs.</param>
    /// <param name="used">How many times an instance happened within a given window.</param>
    /// <param name="window">Time window for limits.</param>
    /// </summary>
    [DataContract]
    public class TracerouteField
    {

        [DataMember(Name = "limit")]
        public int Limit { get; set; }

        [DataMember(Name = "used")]
        public int Used { get; set; }

        [DataMember(Name = "window")]
        public string Window { get; set; }
    }
}