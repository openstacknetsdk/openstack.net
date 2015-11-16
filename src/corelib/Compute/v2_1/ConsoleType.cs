using System.Runtime.Serialization;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary>
    /// The VNC type
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
        XpVnc
    }
}