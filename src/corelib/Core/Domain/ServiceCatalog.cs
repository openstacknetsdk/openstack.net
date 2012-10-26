namespace net.openstack.Core.Domain
{
    public class ServiceCatalog
    {
        public Endpoint[] Endpoints { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }
}