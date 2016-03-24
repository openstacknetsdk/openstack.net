using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Networking.v2.Layer3;
using OpenStack.Serialization;

namespace OpenStack.Networking.v2.Serialization
{
    /// <summary>
    /// Represents a collection of keypair resources of the <see cref="NetworkingService"/>.
    /// </summary>
    /// <exclude />
    public class FloatingIPCollection<T> : ResourceCollection<T>
        where T : IServiceResource
    {
        /// <summary/>
        [JsonProperty("floatingips")]
        protected IList<T> FloatingIPs => Items;
    }

    /// <summary>
    /// Represents a collection of key pair summary resources of the <see cref="NetworkingService"/>.
    /// </summary>
    /// <exclude />
    public class FloatingIPCollection : FloatingIPCollection<FloatingIP>
    { }
}
