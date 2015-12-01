using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary>
    /// Represents a collection of server resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class ServerCollection<TPage, TItem> : Page<TPage, TItem, PageLink>
        where TPage : ServerCollection<TPage, TItem>
    {
        /// <summary>
        /// The requested servers.
        /// </summary>
        [JsonProperty("servers")]
        protected IList<TItem> Servers => Items;

        /// <summary>
        /// The paging navigation links.
        /// </summary>
        [JsonProperty("servers_links")]
        protected IList<PageLink> ServerLinks => Links;
    }

    /// <inheritdoc cref="ServerCollection{TPage, TItem}" />
    public class ServerCollection : ServerCollection<ServerCollection, ServerReference>
    {
        
    }
}