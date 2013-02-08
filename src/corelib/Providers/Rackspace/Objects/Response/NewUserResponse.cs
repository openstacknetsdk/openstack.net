using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class NewUserResponse
    {
        [DataMember(Name = "user")]
        public NewUser NewUser { get; set; }
    }
}