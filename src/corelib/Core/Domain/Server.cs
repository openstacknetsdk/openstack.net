using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace net.openstack.Core.Domain
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Server : SimpleServer
    {
        [JsonProperty("status")]
        private string _status;

        [JsonProperty("OS-DCF:diskConfig" )]
        public string DiskConfig { get; private set; }

        [JsonProperty("OS-EXT-STS:power_state")]
        public bool PowerState { get; private set; }

        [JsonProperty("OS-EXT-STS:task_state")]
        public string TaskState { get; private set; }

        [JsonProperty("OS-EXT-STS:vm_state")]
        public string VMState { get; private set; }

        [JsonProperty]
        public string AccessIPv4 { get; private set; }

        [JsonProperty]
        public string AccessIPv6 { get; private set; }

        [JsonProperty("user_id")]
        public string UserId { get; private set; }

        private SimpleServerImage _image;
        [JsonProperty]
        public SimpleServerImage Image 
        { 
            get {
                if (_image != null)
                {
                    _image.Provider = Provider;
                    _image.Region = Region;
                }

                return _image;
            }
            private set { _image = value;  }
        }

        public ServerState Status
        {
            get
            {
                if (string.IsNullOrEmpty(_status))
                    return null;

                return ServerState.FromName(_status);
            }

            private set
            {
                if (value == null)
                    _status = null;

                _status = value.Name;
            }
        }

        [JsonProperty]
        public Flavor Flavor { get; private set; }

        [JsonProperty]
        public ServerAddresses Addresses { get; private set; }

        [JsonProperty]
        public DateTime Created { get; private set; }

        [JsonProperty]
        public string HostId { get; private set; }

        [JsonProperty]
        public int Progress { get; private set; }

        [JsonProperty("rax-bandwidth:bandwidth")]
        public string[] Bandwidth { get; private set; }

        [JsonProperty("tenant_id")]
        public string TenantId { get; private set; }

        [JsonProperty]
        public DateTime Updated { get; private set; }

        protected override void UpdateThis(ServerBase server)
        {
            base.UpdateThis(server);

            var details = server as Server;

            if (details == null)
                return;

            DiskConfig = details.DiskConfig;
            PowerState = details.PowerState;
            TaskState = details.TaskState;
            VMState = details.VMState;
            AccessIPv4 = details.AccessIPv4;
            AccessIPv6 = details.AccessIPv6;
            UserId = details.UserId;
            Image = details.Image;
            Status = details.Status;
            Flavor = details.Flavor;
            Addresses = details.Addresses;
            Created = details.Created;
            HostId = details.HostId;
            Progress = details.Progress;
            Bandwidth = details.Bandwidth;
            TenantId = details.TenantId;
            Updated = details.Updated;
        }

        /// <summary>
        /// Retrieves a list of the server's attached volumes from the provider's service.
        /// </summary>
        /// <value>
        /// A list of <see cref="ServerVolume"/> details
        /// </value>
        public IEnumerable<ServerVolume> GetVolumes()
        {
            return Provider.ListServerVolumes(Id, Region);
        }

        /// <summary>
        /// Attaches the specified volume to the server.
        /// </summary>
        /// <param name="volumeId">The volume ID.</param>
        /// <param name="storageDevice">The name of the device, such as /dev/xvdb. <remarks>If null, this value will be auto assigned</remarks></param>
        /// <returns>The <see cref="ServerVolume"/> details.</returns>
        public ServerVolume AttachVolume(string volumeId, string storageDevice = null)
        {
            return Provider.AttachServerVolume(Id, volumeId, storageDevice, Region);
        }

        /// <summary>
        /// Detaches the specified volume from the server.
        /// </summary>
        /// <param name="volumeId">The volume ID.</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool DetachVolume(string volumeId)
        {
            return Provider.DetachServerVolume(Id, volumeId, Region);
        }

        /// <summary>
        /// Retrieves a list of the server's metadata from the provider's service.
        /// </summary>
        /// <value>
        /// <see cref="GetMetadata"/>
        /// </value>
        public Metadata GetMetadata()
        {
            return Provider.ListServerMetadata(Id, Region);
        }

        /// <summary>
        /// Replaces the server's metadata with the metadata provided.
        /// </summary>
        /// <param name="metadata">List of metadata to associate with the server</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool SetMetadata(Metadata metadata)
        {
            return Provider.SetServerMetadata(Id, metadata, Region);
        }

        /// <summary>
        /// Adds a set of metadata to the server.
        /// </summary>
        /// <param name="metadata">List of metadata to associate with the server</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool AddMetadata(Metadata metadata)
        {
            return Provider.UpdateServerMetadata(Id, metadata, Region);
        }

        /// <summary>
        /// Adds a single metadata item to the server's metadata
        /// </summary>
        /// <param name="key">The new metadata key.</param>
        /// <param name="value">The new metadata value.</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool AddMetadata(string key, string value)
        {
            return Provider.SetServerMetadataItem(Id, key, value, Region);
        }

        /// <summary>
        /// Updates the server's metadata with the metadata provided. <remarks>For each metadata item, if the key exists, the value is updated, else the item is added.</remarks>
        /// </summary>
        /// <param name="metadata">List of metadata to associate with the server</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool UpdateMetadata(Metadata metadata)
        {
            return Provider.UpdateServerMetadata(Id, metadata, Region);
        }

        /// <summary>
        /// Removes the specified metadata items from the server
        /// </summary>
        /// <param name="metadata">List of metadata to associate with the server</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool DeleteMetadata(Metadata metadata)
        {
            foreach (var item in metadata)
            {
                DeleteMetadataItem(item.Key);
            }

            return true;
        }

        /// <summary>
        /// Removes a metadata item from the server by the specified key
        /// </summary>
        /// <param name="key">The key of the metadata item to be deleted.</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool DeleteMetadataItem(string key)
        {
            return Provider.DeleteServerMetadataItem(Id, key, Region);
        }

        /// <summary>
        /// Updates a single metadata item to the server's metadata. <remarks>If the metadata item does not already exist, it will be added.</remarks>
        /// </summary>
        /// <param name="key">The key of the metadata item to update.</param>
        /// <param name="value">The value of the metadata item to update.</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool UpdateMetadataItem(string key, string value)
        {
            return Provider.SetServerMetadataItem(Id, key, value, Region);
        }

        /// <summary>
        /// Lists all networks and server addresses associated with this server.
        /// </summary>
        /// <returns><see cref="ServerAddresses"/> containing the list of network addresses</returns>
        public ServerAddresses ListAddresses()
        {
            return Provider.ListAddresses(Id, Region);
        }

        /// <summary>
        /// Lists of network addresses associated with the serverk.
        /// </summary>
        /// <param name="networkName">The network name.</param>
        /// <returns>List of network <see cref="AddressDetails"/></returns>
        public IEnumerable<AddressDetails> ListAddressesByNetwork(string networkName)
        {
            return Provider.ListAddressesByNetwork(Id, networkName, Region);
        }

        /// <summary>
        /// Creates a new snapshot image of the server's current state
        /// </summary>
        /// <param name="imageName">Name of the new image.</param>
        /// <param name="metadata">A list of any metadata to associate to the new image. </param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public ServerImage Snapshot(string imageName, Metadata metadata = null)
        {
            if (!Provider.CreateImage(Id, imageName, metadata, Region))
                return null;

            return Provider.ListImagesWithDetails(Id, imageName, imageType: ImageType.Snapshot)
                        .OrderByDescending(i => i.Created)
                        .FirstOrDefault();
        }

        /// <summary>
        /// Marks the server to be deleted. <remarks>Actual deletion of the server may take several minutes during which time, you can still access the server</remarks>
        /// </summary>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool Delete()
        {
            return Provider.DeleteServer(Id, Region);
        }

        /// <summary>
        /// Lists the server's virtual interfaces.
        /// </summary>
        /// <returns>List of <see cref="VirtualInterface"/></returns>
        public IEnumerable<VirtualInterface> ListVirtualInterfaces()
        {
            return Provider.ListVirtualInterfaces(Id, Region);
        }

        /// <summary>
        /// Creates a virtual interface for the specified network and attaches the network to the server
        /// </summary>
        /// <param name="networkId">The network ID</param>
        /// <returns>The newly created <see cref="VirtualInterface"/></returns>
        public VirtualInterface CreateVirtualInterface(string networkId)
        {
            return Provider.CreateVirtualInterface(Id, networkId, Region);
        }

        /// <summary>
        /// Deletes the specified virtual interface from the server
        /// </summary>
        /// <param name="virtualInterfaceId">The virtual interface ID</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool DeleteVirtualInterface(string virtualInterfaceId)
        {
            return Provider.DeleteVirtualInterface(Id, virtualInterfaceId, Region);
        }
    }
}