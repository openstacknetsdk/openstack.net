using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary>
    /// Represents a collection of flavor resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class FlavorCollection<T> : ResourceCollection<T>
        where T : IServiceResource
    {
        /// <summary>
        /// The requested flavors.
        /// </summary>
        [JsonProperty("flavors")]
        protected IList<T> Flavors => Items;
    }

    /// <summary>
    /// Represents a collection of references to flavor resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class FlavorReferenceCollection : FlavorCollection<FlavorReference>
    { }

    /// <inheritdoc cref="FlavorCollection{T}" />
    public class FlavorCollection : FlavorCollection<Flavor>
    { }
}