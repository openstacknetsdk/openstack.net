using System.Collections.Generic;
using OpenStack.Serialization;

namespace OpenStack
{
    /// <summary />
    public class PageOptions : IQueryStringBuilder
    {
        /// <summary />
        public int? PageSize { get; set; }

        /// <summary />
        public Identifier StartingAt { get; set; }

        /// <summary />
        protected virtual IDictionary<string, object> BuildQueryString()
        {
            return new Dictionary<string, object>
            {
                {"marker", StartingAt},
                {"limit", PageSize}
            };
        }

        IDictionary<string , object> IQueryStringBuilder.Build()
        {
            return BuildQueryString();
        }
    }
}