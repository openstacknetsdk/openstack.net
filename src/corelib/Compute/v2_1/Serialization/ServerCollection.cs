using System.Collections.Generic;
using net.openstack.Core.Domain;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary>
    /// Represents a collection of server resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class ServerCollection<T> : Page<T>
    {
        /// <summary>
        /// The requested servers.
        /// </summary>
        [JsonProperty("servers")]
        public IList<T> Servers => Items;

        /// <summary>
        /// The paging navigation links.
        /// </summary>
        [JsonProperty("servers_links")]
        public IList<Link> ServerLinks => Links;
    }

    /// <inheritdoc cref="ServerCollection{T}" />
    public class ServerCollection : ServerCollection<ServerReference>
    {
        
    }
}