using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using net.openstack.Core.Domain;

namespace Net.OpenStack.Testing.Integration
{
    public class Bootstrapper
    {
        private static CloudIdentity _testIdentity;
        public static CloudIdentity TestIdentity
        {
            get
            {
                if(_testIdentity == null)
                    Initialize();
                return _testIdentity;
            }
        }

        private static CloudIdentity _testAdminIdentity;
        public static CloudIdentity TestAdminIdentity
        {
            get
            {
                if (_testAdminIdentity == null)
                    Initialize();
                return _testAdminIdentity;
            }
        }

        public static void Initialize()
        {
            var homeDir = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            var path = Path.Combine(homeDir, ".openstack_net");

            string contents;

            using(var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using(var reader = new StreamReader(stream))
                {
                    contents = reader.ReadToEnd();
                }
            }

            var appCredentials = Newtonsoft.Json.JsonConvert.DeserializeObject<OpenstackNetCredencials>(contents);

            _testIdentity = appCredentials.TestIdentity;
            _testAdminIdentity = appCredentials.TestAdminIdentity;
        }
    }

    public class OpenstackNetCredencials
    {
        public CloudIdentity TestIdentity { get; set; }

        public CloudIdentity TestAdminIdentity { get; set; }
    }
}
