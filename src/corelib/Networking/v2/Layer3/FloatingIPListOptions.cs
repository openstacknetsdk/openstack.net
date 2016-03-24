using System.Collections.Generic;

namespace OpenStack.Networking.v2.Layer3
{
    /// <summary>
    /// Optional filter and paging options when listing ports.
    /// </summary>
    public class FloatingIPListOptions : FilterOptions
    {
        /// <summary>
        /// Filter by status.
        /// </summary>
        public FloatingIPStatus Status { get; set; }

        /// <summary />
        protected override IDictionary<string, object> BuildQueryString()
        {
            var queryString = new Dictionary<string, object>
            {
                ["status"] = Status
            };

            return queryString;
        }
    }
}
