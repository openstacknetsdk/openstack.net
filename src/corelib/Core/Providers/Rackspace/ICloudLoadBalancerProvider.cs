using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core.Providers.Rackspace
{
    /// <summary>
    /// Describes the available Cloud Load Balancer actions and services
    /// </summary>
    public interface ICloudLoadBalancerProvider
    {

        /// <summary>
        /// Retrieves a list of the account's <see cref="SimpleLoadBalancer"/>
        /// </summary>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>An enumerable list of <see cref="SimpleLoadBalancer"/></returns>
        IEnumerable<SimpleLoadBalancer> ListLoadBalancers(string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Returns a <see cref="LoadBalancer" /> based on the specified id/>
        /// </summary>
        /// <param name="id">The Id of the specified Load Balancer</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="LoadBalancer"/></returns>
        LoadBalancer GetLoadBalancer(string id, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Creates a new Load Balancer
        /// </summary>
        /// <param name="name">Name of the load balancer to create. The name must be 128 characters or less in length, and all UTF-8 characters are valid. Refer to http://www.utf8-chartable.de/ for information about the UTF-8 character set. Refer to the request examples in this section for the required xml/json format.</param>
        /// <param name="port">Port number for the service you are load balancing.</param>
        /// <param name="protocol">Protocol of the service which is being load balanced. Refer to Section 4.15, “Protocols” for a table of available protocols.</param>
        /// <param name="virtualIps">Type of virtualIp to add along with the creation of a load balancer.<remarks>Available values: [PUBLIC,SERVICENET]</remarks></param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="LoadBalancer"/> of the new Load Balancer</returns>
        LoadBalancer CreateLoadBalancer(string name, int port, string protocol, IEnumerable<string> virtualIps, string region = null, CloudIdentity identity = null);
    }
}
