using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary>
    /// Represents a collection of server events of the <see cref="ComputeService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class ServerEventCollection<T> : ResourceCollection<T>
        where T : IServiceResource
    {
        /// <summary>
        /// The requested server events.
        /// </summary>
        [JsonProperty("instanceActions")]
        protected IList<T> Events => Items;
    }

    /// <summary>
    /// Represents a collection of references to server event resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class ServerActionReferenceCollection : ServerEventCollection<ServerActionReference>
    { }
}