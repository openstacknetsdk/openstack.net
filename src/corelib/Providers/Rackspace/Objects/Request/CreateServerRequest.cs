namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using System;
    using System.Collections.Generic;
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateServerRequest
    {
        [JsonProperty("server")]
        public CreateServerDetails Details { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateServerDetails
    {
        /// <summary>
        /// The name of the new server to create.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The image reference to the desired image for the server instance. This is
        /// specified as an image ID (see <see cref="SimpleServerImage.Id"/>) or a full URL.
        /// </summary>
        [JsonProperty("imageRef")]
        public string ImageName { get; set; }

        /// <summary>
        /// The flavor reference for the desired flavor for your server instance. This
        /// is specified as a flavor ID (see <see cref="net.openstack.Core.Domain.Flavor.Id"/>) or a full URL.
        /// </summary>
        [JsonProperty("flavorRef")]
        public string Flavor { get; set; }

        [JsonProperty("OS-DCF:diskConfig")]
        public string DiskConfig { get; set; }

        [JsonProperty("metadata", DefaultValueHandling = DefaultValueHandling.Include)]
        public Dictionary<string, string> Metadata { get; set; }

        [JsonProperty("accessIPv4", DefaultValueHandling = DefaultValueHandling.Include)]
        public string AccessIPv4 { get; set; }

        [JsonProperty("accessIPv6", DefaultValueHandling = DefaultValueHandling.Include)]
        public string AccessIPv6 { get; set; }

        [JsonProperty("networks", DefaultValueHandling = DefaultValueHandling.Include)]
        public NewServerNetwork[] Networks { get; set; }

        [JsonProperty("personality", DefaultValueHandling = DefaultValueHandling.Include)]
        public Personality[] Personality { get; set; }

        public CreateServerDetails()
        {
            Metadata = new Dictionary<string, string>();
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class NewServerNetwork
    {
        [JsonProperty("uuid")]
        public Guid Id { get; set; }
    }
}
