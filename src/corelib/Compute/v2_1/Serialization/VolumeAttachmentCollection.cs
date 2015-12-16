using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary>
    /// Represents a collection of server volume resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class VolumeAttachmentCollection<T> : ResourceCollection<T>
        where T : IServiceResource
    {
        /// <summary>
        /// The requested flavors.
        /// </summary>
        [JsonProperty("volumeAttachments")]
        protected IList<T> Volumes => Items;
    }

    /// <summary>
    /// Represents a collection of references to flavor resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class VolumeAttachmentCollection : FlavorCollection<VolumeAttachment>
    { }
}