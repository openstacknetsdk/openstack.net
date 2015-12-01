namespace OpenStack.Compute.v2_6
{
    /// <inheritdoc />
    public class ConsoleType : v2_2.ConsoleType
    {
        /// <summary />
        protected ConsoleType()
        { }

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
        public new static ConsoleType RdpHtml5 = new ConsoleType("rdp-html5");

        /// <summary>
        /// Serial
        /// </summary>
        public new static ConsoleType Serial = new ConsoleType("serial");

        /// <summary>
        /// Spice HTML5
        /// </summary>
        public new static ConsoleType SpiceHtml5 = new ConsoleType("spice-html5");
    }
}