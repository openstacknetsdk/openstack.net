namespace net.openstack.Core.Domain
{
    using System;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServerImage : SimpleServerImage
    {
        [JsonProperty("status")]
        private string _status;

        [JsonProperty("OS-DCF:diskConfig")]
        public string DiskConfig { get; private set; }

        public ImageState Status
        {
            get
            {
                if (string.IsNullOrEmpty(_status))
                    return null;

                return ImageState.FromName(_status);
            }

            private set
            {
                if (value == null)
                    _status = null;

                _status = value.Name;
            }
        }

        [JsonProperty]
        public DateTime Created { get; private set; }

        [JsonProperty]
        public int Progress { get; private set; }

        [JsonProperty]
        public DateTime Updated { get; private set; }

        [JsonProperty]
        public int MinDisk { get; private set; }

        private SimpleServer _server;
        [JsonProperty]
        public SimpleServer Server { 
            get {
                if (_server != null)
                {
                    _server.Provider = Provider;
                    _server.Region = Region;
                }

                return _server;
            } 
            private set { _server = value; } 
        }

        [JsonProperty]
        public int MinRAM { get; private set; }

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
        /// Retrieves a list of the image's metadata from the provider's service.
        /// </summary>
        /// <value>
        /// <see cref="GetMetadata"/>
        /// </value>
        public Metadata GetMetadata()
        {
            return Provider.ListImageMetadata(Id, Region);
        }

        /// <summary>
        /// Replaces the image's metadata with the metadata provided.
        /// </summary>
        /// <param name="metadata">List of metadata to associate with the image</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool SetMetadata(Metadata metadata)
        {
            return Provider.SetImageMetadata(Id, metadata, Region);
        }

        /// <summary>
        /// Adds a set of metadata to the image.
        /// </summary>
        /// <param name="metadata">List of metadata to associate with the image</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool AddMetadata(Metadata metadata)
        {
            return Provider.UpdateImageMetadata(Id, metadata, Region);
        }

        /// <summary>
        /// Adds a single metadata item to the image's metadata
        /// </summary>
        /// <param name="key">The new metadata key.</param>
        /// <param name="value">The new metadata value.</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool AddMetadata(string key, string value)
        {
            return Provider.SetImageMetadataItem(Id, key, value, Region);
        }

        /// <summary>
        /// Updates the image's metadata with the metadata provided. <remarks>For each metadata item, if the key exists, the value is updated, else the item is added.</remarks>
        /// </summary>
        /// <param name="metadata">List of metadata to associate with the image</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool UpdateMetadata(Metadata metadata)
        {
            return Provider.UpdateImageMetadata(Id, metadata, Region);
        }

        /// <summary>
        /// Removes the specified metadata items from the image
        /// </summary>
        /// <param name="metadata">List of metadata to associate with the image</param>
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
        /// Removes a metadata item from the image by the specified key
        /// </summary>
        /// <param name="key">The key of the metadata item to be deleted.</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool DeleteMetadataItem(string key)
        {
            return Provider.DeleteImageMetadataItem(Id, key, Region);
        }

        /// <summary>
        /// Updates a single metadata item to the image's metadata. <remarks>If the metadata item does not already exist, it will be added.</remarks>
        /// </summary>
        /// <param name="key">The key of the metadata item to update.</param>
        /// <param name="value">The value of the metadata item to update.</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool UpdateMetadataItem(string key, string value)
        {
            return Provider.SetImageMetadataItem(Id, key, value, Region);
        }
    }
}
