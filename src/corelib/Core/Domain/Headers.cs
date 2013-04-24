using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class AuditsHeaders
    {

        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "user-agent")]
        public string UserAgent { get; set; }

        [DataMember(Name = "accept")]
        public string Accept { get; set; }

        [DataMember(Name = "content-type")]
        public string ContentType { get; set; }

        [DataMember(Name = "x-auth-token")]
        public string XAuthToken { get; set; }

        [DataMember(Name = "content-length")]
        public int ContentLength { get; set; }

        [DataMember(Name = "connection")]
        public string Connection { get; set; }

        [DataMember(Name = "expect")]
        public string Expect { get; set; }

        [DataMember(Name = "x-forwarded-proto")]
        public string XForwardedProto { get; set; }

        [DataMember(Name = "x-forwarded-port")]
        public string XForwardedPort { get; set; }

        [DataMember(Name = "x-lb")]
        public string XLb { get; set; }

        [DataMember(Name = "x-forwarded-host")]
        public string XForwardedHost { get; set; }

        [DataMember(Name = "x-forwarded-server")]
        public string XForwardedServer { get; set; }
    }
}