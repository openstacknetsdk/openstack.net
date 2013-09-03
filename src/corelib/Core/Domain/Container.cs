namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Container
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("bytes")]
        public long Bytes { get; set; }

        //internal IObjectStorageProvider CloudFilesProvider { get; set; }

        //public void AddHeader(string name)
        //{
        //    CloudFilesProvider.Add
        //}
    }
}
