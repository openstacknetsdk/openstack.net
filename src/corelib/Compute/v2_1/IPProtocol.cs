using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class IPProtocol : StringEnumeration
    {
        /// <summary />
        protected IPProtocol(string displayName)
            : base(displayName)
        { }

        /// <summary>
        /// ICMP
        /// </summary>
        public static readonly IPProtocol ICMP = new IPProtocol("icmp");

        /// <summary>
        /// TCP
        /// </summary>
        public static readonly IPProtocol TCP = new IPProtocol("tcp");

        /// <summary>
        /// UDP
        /// </summary>
        public static readonly IPProtocol UDP = new IPProtocol("udp");
    }
}