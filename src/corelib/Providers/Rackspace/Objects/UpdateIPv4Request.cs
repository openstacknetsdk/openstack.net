using System;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    [Serializable]
    public class UpdateIPv4Request
    {
        public UpdateIPv4Request(string ipAddress)
        {
            Server = new ServerIps(ipAddress);
        }

        public ServerIps Server { get; set; }
    }
}