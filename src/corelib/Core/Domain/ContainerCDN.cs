using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class ContainerCDN
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember(Name = "cdn_streaming_uri")]
        public string CDNStreamingUri { get; set; }

        [DataMember(Name = "cdn_ssl_uri")]
        public string CDNSslUri { get; set; }

        [DataMember(Name = "cdn_enabled")]
        public bool CDNEnabled { get; set; }

        [DataMember(Name = "ttl")]
        public long Ttl { get; set; }

        [DataMember(Name = "log_retention")]
        public bool LogRetention { get; set; }

        [DataMember(Name = "cdn_uri")]
        public string CDNUri { get; set; }
    }
}
