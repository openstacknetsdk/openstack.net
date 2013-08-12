namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Container
    {
        /// <summary>
        /// Gets the name of the container.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the number of objects in the container.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets the total space utilized by the objects in this container.
        /// </summary>
        [JsonProperty("bytes")]
        public long Bytes { get; set; }

        //internal IObjectStorageProvider CloudFilesProvider { get; set; }

        //public void AddHeader(string name)
        //{
        //    CloudFilesProvider.Add
        //}
    }
}
