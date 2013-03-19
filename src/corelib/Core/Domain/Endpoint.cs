namespace net.openstack.Core.Domain
{
    public class Endpoint
    {
        public string PublicURL { get; set; }

        public string Region { get; set; }

        public string TenantId { get; set; }

        public string VersionId { get; set; }

        public string VersionInfo { get; set; }

        public string VersionList { get; set; }

        public string InternalURL { get; set; }
    }
}