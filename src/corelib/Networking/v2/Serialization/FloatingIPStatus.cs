using OpenStack.Serialization;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// Server statuses.
    /// </summary>
    /// <exclude />
    public class FloatingIPStatus<T> : ResourceStatus
        where T : FloatingIPStatus<T>, new()
    {
        /// <summary>
        /// The IP address is in an unknown state.
        /// </summary>
        public static readonly T Unknown = new T { DisplayName = "UNKNOWN" };

        /// <summary>
        /// The IP address is active.
        /// </summary>
        public static readonly T Active = new T { DisplayName = "ACTIVE" };

        /// <summary>
        /// The IP address is unavilable.
        /// </summary>
        public static readonly T Down = new T { DisplayName = "DOWN" };
    }
}