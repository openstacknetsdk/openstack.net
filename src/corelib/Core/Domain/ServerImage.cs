using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class ServerImage : SimpleServerImage
    {
        [DataMember(Name = "OS-DCF:diskConfig")]
        public string DiskConfig { get; internal set; }

        [DataMember]
        public string Status { get; internal set; }

        [DataMember]
        public DateTime Created { get; internal set; }

        [DataMember]
        public int Progress { get; internal set; }

        [DataMember]
        public DateTime Updated { get; internal set; }

        [DataMember]
        public int MinDisk { get; internal set; }

        private SimpleServer _server;
        [DataMember]
        public SimpleServer Server { 
            get {
                if (_server != null)
                {
                    _server.Provider = Provider;
                    _server.Region = Region;
                }

                return _server;
            } 
            internal set { _server = value; } 
        }

        [DataMember]
        public int MinRAM { get; internal set; }

        protected override void UpdateThis(SimpleServerImage serverImage)
        {
            base.UpdateThis(serverImage);

            var details = serverImage as ServerImage;

            if (details == null)
                return;

            DiskConfig = details.DiskConfig;
            Status = details.Status;
            Created = details.Created;
            Progress = details.Progress;
            Updated = details.Updated;
            MinDisk = details.MinDisk;
            MinRAM = details.MinRAM;
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
    }
}