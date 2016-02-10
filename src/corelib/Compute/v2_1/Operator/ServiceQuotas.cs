using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Operator
{
    /// <summary />
    [JsonConverterWithConstructor(typeof (RootWrapperConverter), "quota_set")]
    public class ServiceQuotas : IHaveExtraData
    {
        /// <summary />
        [JsonProperty("injected_file_content_bytes")]
        public int InjectedFileContentBytes { get; set; }

        /// <summary />
        [JsonProperty("metadata_items")]
        public int MetadataItems { get; set; }

        /// <summary />
        [JsonProperty("server_group_members")]
        public int ServerGroupMembers { get; set; }

        /// <summary />
        [JsonProperty("server_groups")]
        public int ServerGroups { get; set; }

        /// <summary />
        [JsonProperty("ram")]
        public int Memory { get; set; }

        /// <summary />
        [JsonProperty("floating_ips")]
        public int FloatingIPs { get; set; }

        /// <summary />
        [JsonProperty("key_pairs")]
        public int KeyPairs { get; set; }

        /// <summary />
        [JsonProperty("id")]
        public Identifier Id { get; set; }

        /// <summary />
        [JsonProperty("instances")]
        public int Instances { get; set; }

        /// <summary />
        [JsonProperty("security_group_rules")]
        public int SecurityGroupRules { get; set; }

        /// <summary />
        [JsonProperty("injected_files")]
        public int InjectedFiles { get; set; }

        /// <summary />
        [JsonProperty("cores")]
        public int Cores { get; set; }

        /// <summary />
        [JsonProperty("fixed_ips")]
        public int FixedIPs { get; set; }

        /// <summary />
        [JsonProperty("injected_file_path_bytes")]
        public int InjectedFilePathBytes { get; set; }

        /// <summary />
        [JsonProperty("security_groups")]
        public int SecurityGroups { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();
    }
}
