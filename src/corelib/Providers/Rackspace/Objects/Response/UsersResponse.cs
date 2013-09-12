using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    internal class UsersResponse
    {
        public User[] Users { get; private set; }
    }
}