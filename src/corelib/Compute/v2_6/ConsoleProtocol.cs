using System.Runtime.Serialization;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_6
{
    /// <summary>
    /// The remote console protocol
    /// </summary>
    [JsonConverter(typeof(TolerantEnumConverter))]
    public enum ConsoleProtocol
    {
        /// <summary>
        /// VNC
        /// </summary>
        [EnumMember(Value = "vnc")]
        VNC,

        /// <summary>
        /// RDP
        /// </summary>
        [EnumMember(Value = "rdp-html5")]
        RDP,

        /// <summary>
        /// Serial
        /// </summary>
        [EnumMember(Value = "serial")]
        Serial,

        /// <summary>
        /// Spice
        /// </summary>
        [EnumMember(Value = "spice-html5")]
        Spice,
    }
}