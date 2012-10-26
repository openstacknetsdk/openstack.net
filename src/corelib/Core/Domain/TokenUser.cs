namespace net.openstack.Core.Domain
{
    public class TokenUser
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Role[] Roles { get; set; }
    }
}