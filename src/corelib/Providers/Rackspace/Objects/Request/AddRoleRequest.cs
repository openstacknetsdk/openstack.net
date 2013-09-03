﻿namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class AddRoleRequest
    {
        [JsonProperty("role")]
        public Role Role { get; set; } 

    }
}
