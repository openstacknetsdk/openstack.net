using System;
using System.Collections.Generic;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class ImageListOptions : PageOptions
    {
        /// <summary />
        public DateTimeOffset? UpdatedAfter { get; set; }

        /// <summary />
        public Identifier ServerId { get; set; }

        /// <summary />
        public string Name { get; set; }

        /// <summary />
        public int? MininumDiskSize { get; set; }

        /// <summary />
        public int? MininumMemorySize { get; set; }

        /// <summary />
        public ImageType Type { get; set; }

        /// <summary />
        protected override IDictionary<string, object> BuildQueryString()
        {
            var queryString = base.BuildQueryString();
            queryString["changes-since"] = UpdatedAfter?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            queryString["server"] = ServerId;
            queryString["name"] = Name;
            queryString["minDisk"] = MininumDiskSize;
            queryString["minRam"] = MininumMemorySize;
            queryString["type"] = Type;

            return queryString;
        }
    }
}