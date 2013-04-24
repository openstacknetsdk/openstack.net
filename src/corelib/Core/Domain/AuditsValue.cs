using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class Value
    {

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "headers")]
        public AuditsHeaders Headers { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "app")]
        public string App { get; set; }

        [DataMember(Name = "query")]
        public object Query { get; set; }

        [DataMember(Name = "txnId")]
        public string TxnId { get; set; }

        [DataMember(Name = "payload")]
        public string Payload { get; set; }

        [DataMember(Name = "method")]
        public string Method { get; set; }

        [DataMember(Name = "account_id")]
        public string AccountId { get; set; }

        [DataMember(Name = "who")]
        public object Who { get; set; }

        [DataMember(Name = "why")]
        public object Why { get; set; }

        [DataMember(Name = "statusCode")]
        public int StatusCode { get; set; }

        [DataMember(Name = "timestamp")]
        public long Timestamp { get; set; }
    }
}
