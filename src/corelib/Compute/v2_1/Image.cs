using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "image")]
    public class Image : ImageReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Image"/> class.
        /// </summary>
        public Image()
        {
            Metadata = new Dictionary<string, string>();
        }

        /// <summary />
        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        /// <summary />
        [JsonProperty("updated")]
        public DateTimeOffset LastModified { get; set; }

        /// <summary />
        [JsonProperty("minDisk")]
        public int MinimumDiskSize { get; set; }

        /// <summary />
        [JsonProperty("minRam")]
        public int MinimumMemorySize { get; set; }

        /// <summary />
        [JsonProperty("OS-EXT-IMG-SIZE:size")]
        public int? Size { get; set; }

        /// <summary />
        [JsonProperty("server")]
        public ServerReference Server { get; set; }

        /// <summary />
        [JsonProperty("metadata")]
        public IDictionary<string, string> Metadata { get; set; }

        /// <summary />
        [JsonIgnore]
        public ImageType Type
        {
            get
            {
                string type;
                if (Metadata != null && Metadata.TryGetValue("image_type", out type))
                    return StringEnumeration.FromDisplayName<ImageType>(type);

                return ImageType.Base;
            }
        }
    }
}
