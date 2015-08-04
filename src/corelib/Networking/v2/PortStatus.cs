using System.Runtime.Serialization;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// <see cref="Port"/> Status
    /// </summary>
    [JsonConverter(typeof (TolerantEnumConverter))]
    public enum PortStatus
    {
        /// <summary>
        /// The port status is unknown.
        /// </summary>
        [EnumMember(Value = "UNKNOWN")]
        Unknown,

        /// <summary>
        /// The port is active.
        /// </summary>
        [EnumMember(Value = "ACTIVE")]
        Active,

        /// <summary>
        /// The port is down.
        /// </summary>
        [EnumMember(Value = "DOWN")]
        Down
    }
}