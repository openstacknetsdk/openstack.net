using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
	[DataContract]
    public class Account
    {

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "metadata")]
        public AccountMetaData Metadata { get; set; }

        [DataMember(Name = "webhook_token")]
        public string WebhookToken { get; set; }
    }
}
