using System;
using System.IO;
using System.Text;
using net.openstack.Core.Domain;

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
            var homeDir = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

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
        public CloudIdentity TestIdentity { get; set; }

        public CloudIdentity TestAdminIdentity { get; set; }

        public string RackspaceExtendedIdentityUSUrl { get; set; }
        public string RackspaceExtendedIdentityUKUrl { get; set; }
    }
}
