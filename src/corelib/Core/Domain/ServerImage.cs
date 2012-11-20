namespace net.openstack.Core.Domain
{
    public class ServerImage
    {
        public string Id { get; set; }

        public Link[] Links { get; set; }
    }

    public class ServerImageDetails : ServerImage{}
}