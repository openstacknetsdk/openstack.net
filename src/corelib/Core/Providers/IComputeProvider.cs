using System;
using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core.Providers
{
    /// <summary>
    /// Provides simple access to the Rackspace Cloud Servers Services.
    /// </summary>
    public interface IComputeProvider
    {
        /// <summary>
        /// Returns a list of all servers for the account.
        /// </summary>
        /// <param name="imageId">Filters the list to those servers created by the given Image ID. </param>
        /// <param name="flavorId">Filters the list to those servers created by the given flavor ID. </param>
        /// <param name="name">Filters the list to those with a name that matches.</param>
        /// <param name="status">Filters the list to those with a status that matches.</param>
        /// <param name="markerId">The ID of the last item in the previous list. <remarks>Used for pagination.</remarks></param>
        /// <param name="limit">Indicates the number of items to return <remarks>Used for pagination.</remarks></param>
        /// <param name="changesSince">Filters the list to those that have changed since the given date.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>An enumerable list of Servers <see cref="SimpleServer"/></returns>
        IEnumerable<SimpleServer> ListServers(string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Returns a list of all servers and their full details for the account.
        /// </summary>
        /// <param name="imageId">Filters the list to those servers created by the given Image ID. </param>
        /// <param name="flavorId">Filters the list to those servers created by the given flavor ID. </param>
        /// <param name="name">Filters the list to those with a name that matches.</param>
        /// <param name="status">Filters the list to those with a status that matches.</param>
        /// <param name="markerId">The marker ID.</param>
        /// <param name="limit">The number of items to return.</param>
        /// <param name="changesSince">Filters the list to those that have changed since the given date.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>An enumerable list of Servers' Details <see cref="Server"/></returns>
        IEnumerable<Server> ListServersWithDetails(string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Creates a new Cloud Server
        /// </summary>
        /// <param name="cloudServerName">Name of the cloud server.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="flavor">The flavor.</param>
        /// <param name="diskConfig">The disk configuration. <remarks>Available values are [AUTO, MANUAL]</remarks></param>
        /// <param name="metadata">A list of metadata to associate with the server. </param>
        /// <param name="personality">File path and contents.</param>
        /// <param name="attachToServiceNetwork">If set to <c>true</c> the Internal Service Network will be attached to the server. <remarks>default value = <c>false</c></remarks></param>
        /// <param name="attachToPublicNetwork">If set to <c>true</c> the Public Network will be attached to the server. <remarks>default value = <c>false</c></remarks></param>
        /// <param name="networks">A list of networks to attach to the server </param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>
        /// Details for the new server <see cref="net.openstack.Core.Domain.NewServer" /> <remarks>NOTE: This is the only time the servers admin password is retruned.</remarks>
        /// </returns>
        NewServer CreateServer(string cloudServerName, string imageName, string flavor, string diskConfig = null, Metadata metadata = null, Personality personality = null, bool attachToServiceNetwork = false, bool attachToPublicNetwork = false, IEnumerable<Guid> networks = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Retrieves the details for a given server.
        /// </summary>
        /// <param name="cloudServerId">The cloud server ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>Details for the given server <see cref="Server"/></returns>
        Server GetDetails(string cloudServerId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Updates the editable attributes for the specified server.
        /// </summary>
        /// <param name="cloudServerId">The cloud server ID.</param>
        /// <param name="name">The new name for the server. <remarks>If not provided (<c>null</c>) this value will be ignored and not updated on the server.</remarks></param>
        /// <param name="accessIPv4">The new IP v4 address for the server. <remarks>If not provided (<c>null</c>) this value will be ignored and not updated on the server.</remarks></param>
        /// <param name="accessIPv6">The new IP v6 address for the server. <remarks>If not provided (<c>null</c>) this value will be ignored and not updated on the server.</remarks></param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool UpdateServer(string cloudServerId, string name = null, string accessIPv4 = null, string accessIPv6 = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Marks a server to be deleted. <remarks>Actual deletion of the server may take several minutes during which time, you can still access the server</remarks>
        /// </summary>
        /// <param name="cloudServerId">The cloud server ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool DeleteServer(string cloudServerId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Lists all networks and server addresses associated with a specified server.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="ServerAddresses"/> containing the list of network addresses</returns>
        ServerAddresses ListAddresses(string serverId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Lists addresses associated with a specified server and network.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="network">The network name.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of network <see cref="AddressDetails"/></returns>
        IEnumerable<AddressDetails> ListAddressesByNetwork(string serverId, string network, string region = null, CloudIdentity identity = null);


        /// <summary>
        /// Changes the administrator password for a specified server.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="password">The new administrator password.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool ChangeAdministratorPassword(string serverId, string password, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Performs a soft or hard reboot of the specified server.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="rebootType">Type of the reboot. <see cref="net.openstack.Core.Domain.RebootType"/></param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool RebootServer(string serverId, RebootType rebootType, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Rebuilds the specified server.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="serverName">The new name for the server.</param>
        /// <param name="imageName">The ID of the new image for the server.</param>
        /// <param name="flavor">The new flavor for server.</param>
        /// <param name="adminPassword">The new admin password for the server.</param>
        /// <param name="accessIPv4">The new access IP v4 address for the server. </param>
        /// <param name="accessIPv6">The new access IP v6 address for the server. </param>
        /// <param name="metadata">The list of any metadata to associate with the server. </param>
        /// <param name="diskConfig">The disk configuration value. <remarks>Available values are [AUTO, MANUAL]</remarks></param>
        /// <param name="personality">The file path and file contents. </param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="Server"/></returns>
        Server RebuildServer(string serverId, string serverName, string imageName, string flavor, string adminPassword, string accessIPv4 = null, string accessIPv6 = null, Metadata metadata = null, string diskConfig = null, Personality personality = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Resizes the specified server.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="serverName">The new name for the resized server.</param>
        /// <param name="flavor">The new flavor.</param>
        /// <param name="diskConfig">The disk configuration value. <remarks>Available values are {AUTO|MANUAL}</remarks></param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool ResizeServer(string serverId, string serverName, string flavor, string diskConfig = null, string region = null, CloudIdentity identity = null);


        /// <summary>
        /// Confirms a pending resize action.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool ConfirmServerResize(string serverId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Cancels and reverts a pending resize action.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool RevertServerResize(string serverId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Places a server in rescue mode.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The root password is assigned for use during rescue mode.</returns>
        string RescueServer(string serverId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Takes a server out of rescue mode.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool UnRescueServer(string serverId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Creates a new snapshot image for a specified server at its current state.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="imageName">Name of the new image.</param>
        /// <param name="metadata">A list of any metadata to associate to the new image. </param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool CreateImage(string serverId, string imageName, Metadata metadata = null, string region = null, CloudIdentity identity = null);


        /// <summary>
        /// Attaches a volume to the specified server.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="volumeId">The volume ID.</param>
        /// <param name="storageDevice">The name of the device, such as /dev/xvdb. <remarks>If null, this value will be auto assigned</remarks></param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="ServerVolume"/> details.</returns>
        ServerVolume AttachServerVolume(string serverId, string volumeId, string storageDevice = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Lists the volume attachments for the specified server.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="ServerVolume"/></returns>
        IEnumerable<ServerVolume> ListServerVolumes(string serverId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Lists volume details for the specified volume attachment ID.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="volumeId">The volume ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="ServerVolume"/> details.</returns>
        ServerVolume GetServerVolumeDetails(string serverId, string volumeId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Detaches the specified volume from the specified server.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="volumeId">The volume ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool DetachServerVolume(string serverId, string volumeId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Lists all available flavors.
        /// </summary>
        /// <param name="minDiskInGB">Filters the list of flavors to those with the specified minimum number of gigabytes of disk storage. </param>
        /// <param name="minRamInMB">Filters the list of flavors to those with the specified minimum amount of RAM in megabytes. </param>
        /// <param name="markerId">The ID of the last item in the previous list. <remarks>Used for pagination.</remarks></param>
        /// <param name="limit">Indicates the number of items to return <remarks>Used for pagination.</remarks></param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="Flavor"/></returns>
        IEnumerable<Flavor> ListFlavors(int minDiskInGB = 0, int minRamInMB = 0, string markerId = null, int limit = 0, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Lists full details for all available flavors.
        /// </summary>
        /// <param name="minDiskInGB">Filters the list of flavors to those with the specified minimum number of gigabytes of disk storage. </param>
        /// <param name="minRamInMB">Filters the list of flavors to those with the specified minimum amount of RAM in megabytes. </param>
        /// <param name="markerId">The ID of the last item in the previous list. <remarks>Used for pagination.</remarks></param>
        /// <param name="limit">Indicates the number of items to return <remarks>Used for pagination.</remarks></param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="Flavor"/></returns>
        IEnumerable<FlavorDetails> ListFlavorsWithDetails(int minDiskInGB = 0, int minRamInMB = 0, string markerId = null, int limit = 0, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Gets details for the specified flavor.
        /// </summary>
        /// <param name="id">The ID of the flavor.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="Flavor"/> details</returns>
        FlavorDetails GetFlavor(string id, string region = null, CloudIdentity identity = null);

        // Images
        /// <summary>
        /// Lists all available images.
        /// </summary>
        /// <param name="server">Filters the list of images by server. Specify the server reference by ID or by full URL. </param>
        /// <param name="imageName">Filters the list of images by image name. </param>
        /// <param name="imageStatus">Filters the list of images by status. </param>
        /// <param name="changesSince">Filters the list of images to those that have changed since the specified <c>datetime</c>. </param>
        /// <param name="markerId">The ID of the last item in the previous list. <remarks>Used for pagination. </remarks></param>
        /// <param name="limit">Indicates the number of items to return <remarks>Used for pagination. </remarks></param>
        /// <param name="imageType">Filters base Rackspace images or any custom server images that you have created. <remarks>Available values are {BASE|SNAPSHOT}</remarks></param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="SimpleServerImage"/></returns>
        IEnumerable<SimpleServerImage> ListImages(string server = null, string imageName = null, string imageStatus = null, DateTime changesSince = default(DateTime), string markerId = null, int limit = 0, string imageType = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Lists full details for all available images.
        /// </summary>
        /// <param name="server">Filters the list of images by server. Specify the server reference by ID or by full URL. </param>
        /// <param name="imageName">Filters the list of images by image name. </param>
        /// <param name="imageStatus">Filters the list of images by status. </param>
        /// <param name="changesSince">Filters the list of images to those that have changed since the specified <c>datetime</c>. </param>
        /// <param name="markerId">The ID of the last item in the previous list. <remarks>Used for pagination. </remarks></param>
        /// <param name="limit">Indicates the number of items to return <remarks>Used for pagination. </remarks></param>
        /// <param name="imageType">Filters base Rackspace images or any custom server images that you have created. <remarks>Available values are {BASE|SNAPSHOT}</remarks></param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="SimpleServerImage"/></returns>
        IEnumerable<ServerImage> ListImagesWithDetails(string server = null, string imageName = null, string imageStatus = null, DateTime changesSince = default(DateTime), string markerId = null, int limit = 0, string imageType = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Retrieves the details for the specified image
        /// </summary>
        /// <param name="imageId">The image ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="SimpleServerImage"/> details</returns>
        ServerImage GetImage(string imageId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the specified image.
        /// </summary>
        /// <param name="imageId">The image ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool DeleteImage(string imageId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Lists the metadata for the sprcified server
        /// </summary>
        /// <param name="serverId">The cloud server ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="Metadata"/></returns>
        Metadata ListServerMetadata(string serverId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Sets the metadata for the specified server. <remarks>This replaces all metadata for the server with the specified metadata.</remarks>
        /// </summary>
        /// <param name="serverId">The cloud server ID.</param>
        /// <param name="metadata">List of metadata to associate with the server</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool SetServerMetadata(string serverId, Metadata metadata, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Updates the metadata for the specified server. <remarks>For each metadata item, if the key exists, the value is updated, else the item is added.</remarks>
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="metadata">List of metadata to associate with the server</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool UpdateServerMetadata(string serverId, Metadata metadata, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Gets the specified metadata item
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="key">The metadata key</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The value for the <see cref="Metadata"/> item</returns>
        string GetServerMetadataItem(string serverId, string key, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Updates the value for the specified metadata item.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="key">The metadata key</param>
        /// <param name="value">The new value for the metadata item.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool SetServerMetadataItem(string serverId, string key, string value, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the specified metadata item from the server.
        /// </summary>
        /// <param name="serverId">The server ID.</param>
        /// <param name="key">The metadata key</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool DeleteServerMetadataItem(string serverId, string key, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Lists the metadata for the sprcified image
        /// </summary>
        /// <param name="imageId">The image ID.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="Metadata"/></returns>
        Metadata ListImageMetadata(string imageId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Sets the metadata for the specified image. <remarks>This replaces all metadata for the image with the specified metadata.</remarks>
        /// </summary>
        /// <param name="imageId">The image ID.</param>
        /// <param name="metadata">List of metadata to associate with the image</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool SetImageMetadata(string imageId, Metadata metadata, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Updates the metadata for the specified image. <remarks>For each metadata item, if the key exists, the value is updated, else the item is added.</remarks>
        /// </summary>
        /// <param name="imageId">The image ID.</param>
        /// <param name="metadata">List of metadata to associate with the image</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool UpdateImageMetadata(string imageId, Metadata metadata, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Gets the specified metadata item
        /// </summary>
        /// <param name="imageId">The image ID.</param>
        /// <param name="key">The metadata key</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The value for the <see cref="Metadata"/> item</returns>
        string GetImageMetadataItem(string imageId, string key, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Updates the value for the specified metadata item.
        /// </summary>
        /// <param name="imageId">The image ID.</param>
        /// <param name="key">The metadata key</param>
        /// <param name="value">The new value for the metadata item.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool SetImageMetadataItem(string imageId, string key, string value, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the specified metadata item from the image.
        /// </summary>
        /// <param name="imageId">The image ID.</param>
        /// <param name="key">The metadata key</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool DeleteImageMetadataItem(string imageId, string key, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Waits for the server to enter a specified state. <remarks>NOTE: This is a blocking operation and will new return until the server enters either the expected state, an error state, or the retry count is exceeded</remarks>
        /// </summary>
        /// <param name="serverId">The cloud server ID.</param>
        /// <param name="expectedState">The expected state.</param>
        /// <param name="errorStates">A list of states in which to throw an exception if the server enters. </param>
        /// <param name="refreshCount">Number of times to check the servers status</param>
        /// <param name="refreshDelayInMS">The number of milisecods to wait each time before requesting the status for the server.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>Returns the deatils of the <see cref="Server"/> after the process completes</returns>
        /// <param name="refreshDelayInMS">The number of milliseconds to wait each time before requesting the status for the server.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>Returns the details of the <see cref="Server"/> after the process completes</returns>
        Server WaitForServerState(string serverId, string expectedState, string[] errorStates, int refreshCount = 600, int refreshDelayInMS = 2400, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Waits for the server to enter a set of specified states. <remarks>NOTE: This is a blocking operation and will new return until the server enters either the expected state, an error state, or the retry count is exceeded</remarks>
        /// </summary>
        /// <param name="serverId">The cloud server ID.</param>
        /// <param name="expectedStates">The set expected state.</param>
        /// <param name="errorStates">A list of states in which to throw an exception if the server enters. </param>
        /// <param name="refreshCount">Number of times to check the servers status</param>
        /// <param name="refreshDelayInMS">The number of milliseconds to wait each time before requesting the status for the server.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>Returns the details of the <see cref="Server"/> after the process completes</returns>
        Server WaitForServerState(string serverId, string[] expectedStates, string[] errorStates, int refreshCount = 600, int refreshDelayInMS = 2400, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Waits for the server to enter the ACTIVE state. <remarks>NOTE: This is a blocking operation and will new return until the server enters either the expected state, an error state, or the retry count is exceeded</remarks>
        /// </summary>
        /// <param name="serverId">The cloud server ID.</param>
        /// <param name="refreshCount">Number of times to check the servers status</param>
        /// <param name="refreshDelayInMS">The number of milliseconds to wait each time before requesting the status for the server.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>Returns the details of the <see cref="Server"/> after the process completes</returns>
        Server WaitForServerActive(string serverId, int refreshCount = 600, int refreshDelayInMS = 2400, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Waits for the server to enter the DELETED state or to be removed. <remarks>NOTE: This is a blocking operation and will new return until the server enters either the expected state, an error state, or the retry count is exceeded</remarks>
        /// </summary>
        /// <param name="serverId">The cloud server ID.</param>
        /// <param name="refreshCount">Number of times to check the servers status</param>
        /// <param name="refreshDelayInMS">The number of milliseconds to wait each time before requesting the status for the server.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns></returns>
        void WaitForServerDeleted(string serverId, int refreshCount = 600, int refreshDelayInMS = 2400, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Waits for the image to enter a specified state. <remarks>NOTE: This is a blocking operation and will new return until the image enters either the expected state, an error state, or the retry count is exceeded</remarks>
        /// </summary>
        /// <param name="imageId">The image ID.</param>
        /// <param name="expectedState">The expected <see cref="ServerState"/></param>
        /// <param name="errorStates">A list of <see cref="ServerState"/>s in which to throw an exception if the server enters. </param>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelayInMS">The number of milliseconds to wait each time before requesting the status for the image.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>Returns the details of the <see cref="Server"/> after the process completes</returns>
        ServerImage WaitForImageState(string imageId, string expectedState, string[] errorStates, int refreshCount = 600, int refreshDelayInMS = 2400, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Waits for the image to enter a set of specified states. <remarks>NOTE: This is a blocking operation and will new return until the image enters either the expected state, an error state, or the retry count is exceeded</remarks>
        /// </summary>
        /// <param name="imageId">The image ID.</param>
        /// <param name="expectedStates">The set expected <see cref="ServerState"/>s</param>
        /// <param name="errorStates">A list of <see cref="ServerState"/>s in which to throw an exception if the server enters. </param>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelayInMS">The number of milliseconds to wait each time before requesting the status for the image.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>Returns the details of the <see cref="Server"/> after the process completes</returns>
        ServerImage WaitForImageState(string imageId, string[] expectedStates, string[] errorStates, int refreshCount = 600, int refreshDelayInMS = 2400, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Waits for the image to enter the ACTIVE state. <remarks>NOTE: This is a blocking operation and will new return until the image enters either the expected state, an error state, or the retry count is exceeded</remarks>
        /// </summary>
        /// <param name="imageId">The image ID.</param>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelayInMS">The number of milliseconds to wait each time before requesting the status for the image.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>Returns the details of the <see cref="Server"/> after the process completes</returns>
        ServerImage WaitForImageActive(string imageId, int refreshCount = 600, int refreshDelayInMS = 2400, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Waits for the image to enter the DELETED state or to be removed. <remarks>NOTE: This is a blocking operation and will new return until the image enters either the expected state, an error state, or the retry count is exceeded</remarks>
        /// </summary>
        /// <param name="imageId">The image ID.</param>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelayInMS">The number of milliseconds to wait each time before requesting the status for the image.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        void WaitForImageDeleted(string imageId, int refreshCount = 600, int refreshDelayInMS = 2400, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null);

    }
}
