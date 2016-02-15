using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary>
    /// Represents a collection of flavor resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <exclude />
    public class FlavorCollection<T> : ResourceCollection<T>
        where T : IServiceResource
    {
        /// <summary />
        [JsonProperty("flavors")]
        protected IList<T> Flavors => Items;
    }

    /// <summary>
    /// Represents a collection of flavor summary resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <exclude />
    public class FlavorSummaryCollection : FlavorCollection<FlavorSummary>
    { }

    /// <inheritdoc cref="FlavorCollection{T}" />
    /// <exclude />
    public class FlavorCollection : FlavorCollection<Flavor>
    { }
}
