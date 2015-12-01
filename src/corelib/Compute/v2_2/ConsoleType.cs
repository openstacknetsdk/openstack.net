namespace OpenStack.Compute.v2_2
{
    /// <summary>
    /// The remote console type
    /// </summary>
    public class ConsoleType : v2_1.ConsoleType
    {
        /// <summary />
        protected ConsoleType(string displayName) : base(displayName)
        { }

        /// <summary>
        /// noVNC
        /// </summary>
        public new static ConsoleType NoVnc = new ConsoleType("novnc");

        /// <summary>
        /// XP VNC
        /// </summary>
        public new static ConsoleType XpVnc = new ConsoleType("xpvnc");

        /// <summary>
        /// RDP
        /// </summary>
        public static ConsoleType RdpHtml5 = new ConsoleType("rdp-html5");

        /// <summary>
        /// Serial
        /// </summary>
        public static ConsoleType Serial = new ConsoleType("serial");

        /// <summary>
        /// Spice HTML5
        /// </summary>
        public static ConsoleType SpiceHtml5 = new ConsoleType("spice-html5");
    }
}