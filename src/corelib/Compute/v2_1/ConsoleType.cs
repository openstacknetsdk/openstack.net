using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary>
    /// The remote console type
    /// </summary>
    public class ConsoleType : StringEnumeration
    {
        /// <summary />
        protected ConsoleType(string displayName) : base(displayName)
        { }

        /// <summary>
        /// noVNC
        /// </summary>
        public static readonly ConsoleType NoVnc = new ConsoleType("novnc");

        /// <summary>
        /// XP VNC
        /// </summary>
        public static readonly ConsoleType XpVnc = new ConsoleType("xpvnc");
    }
}