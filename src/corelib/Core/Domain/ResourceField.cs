using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    /// <summary>
    /// <param name="checks">Time-indexed audit limit.</param>
    /// <param name="alarms">Time-indexed alarm limit.</param>
    /// </summary>
    [DataContract]
    public class ResourceField
    {

        [DataMember(Name = "checks")]
        public int Checks { get; set; }

        [DataMember(Name = "alarms")]
        public int Alarms { get; set; }
    }
}