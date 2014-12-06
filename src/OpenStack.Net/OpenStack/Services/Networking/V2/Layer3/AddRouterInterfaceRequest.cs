﻿namespace OpenStack.Services.Networking.V2.Layer3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class AddRouterInterfaceRequest : ExtensibleJsonObject
    {
        [JsonProperty("subnet_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SubnetId _subnetId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddRouterInterfaceRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected AddRouterInterfaceRequest()
        {
        }

        public AddRouterInterfaceRequest(SubnetId subnetId)
        {
            _subnetId = subnetId;
        }

        public SubnetId SubnetId
        {
            get
            {
                return _subnetId;
            }
        }
    }
}
