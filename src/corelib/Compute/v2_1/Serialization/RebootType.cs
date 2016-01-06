using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary />
    /// <exclude />
    public class RebootType<T> : StringEnumeration
        where T : RebootType<T>, new()
    {
        /// <summary>
        /// A soft reboot is a graceful shutdown and restart of your server�s operating system.
        /// </summary>
        public static readonly T Soft = new T {DisplayName = "SOFT"};

        /// <summary>
        /// A hard reboot power cycles your server, which performs an immediate shutdown and restart.
        /// </summary>
        public static readonly T Hard = new T {DisplayName = "HARD"};
    }
}
