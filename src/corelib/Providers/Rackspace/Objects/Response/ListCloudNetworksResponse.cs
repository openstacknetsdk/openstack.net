﻿namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListCloudNetworksResponse
    {
        [JsonProperty("networks")]
        public CloudNetwork[] Networks { get; set; }
    }
}
