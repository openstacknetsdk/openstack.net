using System;
using System.IO;
using System.Text;
using net.openstack.Core.Domain;
using net.openstack.Core.Providers;
using net.openstack.Providers.Rackspace;
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

        public static IIdentityProvider CreateIdentityProvider()
        {
            return CreateIdentityProvider(Bootstrapper.Settings.TestIdentity);
        }

        public static IIdentityProvider CreateIdentityProvider(CloudIdentity identity)
        {
            return new CloudIdentityProvider(identity);
        }

        public static IComputeProvider CreateComputeProvider()
        {
            return new CloudServersProvider(Bootstrapper.Settings.TestIdentity);
        }

        public static INetworksProvider CreateNetworksProvider()
        {
            return new CloudNetworksProvider(Bootstrapper.Settings.TestIdentity);
        }

        public static IBlockStorageProvider CreateBlockStorageProvider()
        {
            return new CloudBlockStorageProvider(Bootstrapper.Settings.TestIdentity);
        }

        public static IObjectStorageProvider CreateObjectStorageProvider()
        {
            return new CloudFilesProvider(Bootstrapper.Settings.TestIdentity);
        }
    }

    public class OpenstackNetSetings
    {
        public ExtendedCloudIdentity TestIdentity { get; set; }

        public ExtendedCloudIdentity TestAdminIdentity { get; set; }

        public ExtendedCloudIdentity TestDomainIdentity { get; set; }

        public string RackspaceExtendedIdentityUrl { get; set; }
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
            this.Domain = string.IsNullOrEmpty(cloudIdentity.Domain) ? null : new Domain(cloudIdentity.Domain);
        }
    }
}
