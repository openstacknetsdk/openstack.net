using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class ChangeServerAdminPasswordRequest
    {
        [DataMember(Name = "changePassword")]
        public ChangeAdminPasswordDetails Details { get; set; }
    }

    [DataContract]
    internal class ChangeAdminPasswordDetails
    {
        [DataMember(Name = "adminPass")]
        public string AdminPassword { get; set; }
    }
}
