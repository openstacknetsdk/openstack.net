using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface ICloudNetworksProvider
    {
        /// <summary>
        /// List the networks configured for the account.
        /// </summary>
        /// <param name="region">The region in which to retrieve the networks.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>A list of networks <see cref="net.openstack.Core.Domain.CloudNetwork" /></returns>
        IEnumerable<CloudNetwork> ListNetworks(string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Create a network with the given IP block.
        /// </summary>
        /// <param name="cidr">The IP block from which to allocate the network. For example, 172.16.0.0/24 or 2001:DB8::/64</param>
        /// <param name="label">The name of the new network. For example, my_new_network.</param>
        /// <param name="region">The region in which to create the network.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>Details for the newly created network <see cref="net.openstack.Core.Domain.CloudNetwork" /></returns>
        CloudNetwork CreateNetwork(string cidr, string label, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Retrieve details for the specified network
        /// </summary>
        /// <param name="network_id">ID (uuid) of the network to retrieve</param>
        /// <param name="region">The region in which to retrieve the network details.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>Details for the specified network <see cref="net.openstack.Core.Domain.CloudNetwork" /></returns>
        CloudNetwork ShowNetwork(string network_id, string region = null, CloudIdentity identity = null);


        /// <summary>
        /// Deletes the specified network. <remarks>You cannot delete an isolated network unless the network is not attached to any server.</remarks>
        /// </summary>
        /// <param name="network_id">ID (uuid) of the network to delete</param>
        /// <param name="region">The region in which to specify the delete.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the delete was successful</returns>
        bool DeleteNetwork(string network_id, string region = null, CloudIdentity identity = null);
    }
}
