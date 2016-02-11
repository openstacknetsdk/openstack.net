using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary>
    /// Represents a collection of volume type resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <exclude />
    public class VolumeTypeCollection<T> : ResourceCollection<T>
    {
        /// <summary />
        [JsonProperty("volume_types")]
        protected IList<T> VolumeTypes => Items;
    }

    /// <summary>
    /// Represents a collection of volume type resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <exclude />
    public class VolumeTypeCollection : VolumeTypeCollection<VolumeType>
    { }
}
