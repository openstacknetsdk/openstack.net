using System.Collections.Generic;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// Optional filter and paging options when listing ports.
    /// </summary>
    public class PortListOptions : FilterOptions
    {
        /// <summary>
        /// Filter by the associated device identifier
        /// </summary>
        public Identifier DeviceId { get; set; }

        /// <summary />
        protected override IDictionary<string, object> BuildQueryString()
        {
            var queryString = new Dictionary<string, object>
            {
                ["device_id"] = DeviceId
            };

            return queryString;
        }
    }
}
