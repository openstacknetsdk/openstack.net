using System;

namespace net.openstack.Core.Domain
{
    [Serializable]
    public class ServerIps
    {
        public ServerIps(string ipAddress)
        {
            AccessIPv4 = ipAddress;
        }

        public string AccessIPv4 { get; private set; }

        public string AccessIPv6 { get; private set; }
    }
}