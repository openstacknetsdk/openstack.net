using System;
using System.IO;
using System.Text;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace.Objects;

namespace Net.OpenStack.Testing.Integration
{
    public class Bootstrapper
    {
        private static OpenstackNetSetings _settings;
        public static OpenstackNetSetings Settings
        {
            get
            {
                if(_settings == null)
                    Initialize();

                return _settings;
            }
        }

        public static void Initialize()
        {
            var homeDir = Environment.ExpandEnvironmentVariables("C:\\");

            var path = Path.Combine(homeDir, ".openstack_net");

            var contents = new StringBuilder();

            using(var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using(var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if(!line.Trim().StartsWith("//"))
                            contents.Append(line);
                    }
                }
            }

            var appCredentials = Newtonsoft.Json.JsonConvert.DeserializeObject<OpenstackNetSetings>(contents.ToString());

            _settings = appCredentials;
        }
    }

    public class OpenstackNetSetings
    {
        public ExtendedCloudIdentity TestIdentity { get; set; }

        public ExtendedCloudIdentity TestAdminIdentity { get; set; }

        public ExtendedCloudIdentity TestDomainIdentity { get; set; }

        public string RackspaceExtendedIdentityUSUrl { get; set; }
        public string RackspaceExtendedIdentityUKUrl { get; set; }
    }

    public class ExtendedCloudIdentity : CloudIdentity
    {
        public string TenantId { get; set; }

        public string Domain { get; set; }
    }

    public class ExtendedRackspaceCloudIdentity : RackspaceCloudIdentity
    {
        public string TenantId { get; set; }

        public ExtendedRackspaceCloudIdentity(ExtendedCloudIdentity cloudIdentity)
        {
            this.APIKey = cloudIdentity.APIKey;
            this.Password = cloudIdentity.Password;
            this.Username = cloudIdentity.Username;
            this.TenantId = cloudIdentity.TenantId;
            this.Domain = string.IsNullOrWhiteSpace(cloudIdentity.Domain) ? null : new Domain {Name = cloudIdentity.Domain};
        }
    }
}
