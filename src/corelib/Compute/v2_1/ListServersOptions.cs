using System;
using System.Collections.Generic;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class ListServersOptions : PageOptions
    {
        /// <summary />
        public DateTimeOffset? UpdatedAfter { get; set; }

        /// <summary />
        public Identifier ImageId { get; set; }

        /// <summary />
        public string FlavorId { get; set; }

        /// <summary />
        public string Name { get; set; }

        /// <summary />
        public ServerStatus Status { get; set; }

        /// <summary />
        protected override IDictionary<string, object> BuildQueryString()
        {
            var queryString = base.BuildQueryString();
            queryString["changes-since"] = UpdatedAfter?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            queryString["image"] = ImageId;
            queryString["flavor"] = FlavorId;
            queryString["name"] = Name;
            queryString["status"] = Status;

            return queryString;
        }
    }
}