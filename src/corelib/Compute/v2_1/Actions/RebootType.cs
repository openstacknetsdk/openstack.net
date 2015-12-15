using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class RebootType : StringEnumeration
    {
        /// <summary />
        protected RebootType(string displayName)
            : base(displayName)
        {
        }

        /// <summary>
        /// A soft reboot is a graceful shutdown and restart of your server’s operating system.
        /// </summary>
        public static readonly RebootType Soft = new RebootType("SOFT");

        /// <summary>
        /// A hard reboot power cycles your server, which performs an immediate shutdown and restart.
        /// </summary>
        public static readonly RebootType Hard = new RebootType("HARD");
    }
}