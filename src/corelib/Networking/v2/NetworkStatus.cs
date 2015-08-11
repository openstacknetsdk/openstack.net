using System.Runtime.Serialization;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// <see cref="Network"/> Status
    /// </summary>
    [JsonConverter(typeof(TolerantEnumConverter))]
    public enum NetworkStatus
    {
        /// <summary>
        /// The network status is unknown.
        /// </summary>
        [EnumMember(Value = "UNKNOWN")]
        Unknown,

        /// <summary>
        /// The network is active.
        /// </summary>
        [EnumMember(Value = "ACTIVE")]
        Active
    }
}