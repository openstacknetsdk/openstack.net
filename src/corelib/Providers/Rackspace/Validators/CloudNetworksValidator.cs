using System;
using System.Net;
using net.openstack.Core;
using net.openstack.Core.Exceptions;

namespace net.openstack.Providers.Rackspace.Validators
{
    public class CloudNetworksValidator : ICloudNetworksValidator
    {
        public void ValidateCidr(string cidr)
        {
            if (string.IsNullOrWhiteSpace(cidr))
                throw new ArgumentNullException("cidr", "ERROR: CIDR cannot be null");

            if (!cidr.Contains("/"))
                throw new CidrFormatException(string.Format("ERROR: CIDR {0} is missing /", cidr));

            var parts = cidr.Split('/');

            if (parts.Length != 2)
                throw new CidrFormatException(string.Format("ERROR: CIDR {0} must have exactly one / character", cidr));

            var ipAddress = parts[0];
            var cidr_range = parts[1];

            if (!IsIpAddress(ipAddress))
                throw new CidrFormatException(string.Format("ERROR: IP address segment ({0}) of CIDR is not a valid IP address", ipAddress));

            int cidrInt;
            if (!int.TryParse(cidr_range, out cidrInt))
                throw new CidrFormatException(string.Format("ERROR: CIDR range segment {0} must be an integer", cidr_range));

            if (cidrInt < 1 || cidrInt > 32)
                throw new CidrFormatException(string.Format("ERROR: CIDR range segment {0} must be between 1 and 32", cidr_range));
        }

        /// <summary>
        /// Returns true if the string matches the ip address pattern (xxx.xxx.xxx.xxx) 
        /// and all octets are numeric and less than 256
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private bool IsIpAddress(string address)
        {
            IPAddress ipAddress;
            return IPAddress.TryParse(address, out ipAddress);
        }

    }
}
