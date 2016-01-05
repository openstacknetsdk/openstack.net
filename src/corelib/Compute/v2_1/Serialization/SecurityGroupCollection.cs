using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary>
    /// Represents a collection of security group resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class SecurityGroupCollection<T> : ResourceCollection<T>
        where T : IServiceResource
    {
        /// <summary>
        /// The requested flavors.
        /// </summary>
        [JsonProperty("security_groups")]
        protected IList<T> SecurityGroups => Items;
    }
    
    /// <inheritdoc cref="FlavorCollection{T}" />
    public class SecurityGroupCollection : SecurityGroupCollection<SecurityGroup>
    { }
}