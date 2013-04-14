using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class CloudNetwork
    {
        /// <summary>
        /// The network ID.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// The CIDR for an isolated network.
        /// </summary>
        [DataMember]
        public string Cidr { get; set; }

        /// <summary>
        /// The name of the network. ServiceNet is labeled as private and PublicNet 
        /// is labeled as public in the network list.
        /// </summary>
        [DataMember]
        public string Label { get; set; }
    }
}
