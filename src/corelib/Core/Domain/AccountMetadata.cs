using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class AccountMetaData
    {

        [DataMember(Name = "key")]
        public string Key { get; set; }
    }
}
