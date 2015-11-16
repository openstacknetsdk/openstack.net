using System.Runtime.Serialization;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_2
{
    /// <summary>
    /// The remote console type
    /// </summary>
    [JsonConverter(typeof (TolerantEnumConverter))]
    public enum ConsoleType
    {
        /// <summary>
        /// noVNC
        /// </summary>
        [EnumMember(Value = "novnc")]
        NoVnc,

        /// <summary>
        /// XP VNC
        /// </summary>
        [EnumMember(Value = "xpvnc")]
        XpVnc,

        /// <summary>
        /// RDP
        /// </summary>
        [EnumMember(Value = "rdp-html5")]
        RdpHtml5,

        /// <summary>
        /// Serial
        /// </summary>
        [EnumMember(Value = "serial")]
        Serial,

        /// <summary>
        /// Spice HTML5
        /// </summary>
        [EnumMember(Value = "spice-html5")]
        SpiceHtml5,
    }
}