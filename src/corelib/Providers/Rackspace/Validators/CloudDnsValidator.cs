using System;
using System.Web;
using System.Linq;
using net.openstack.Core;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Validators;

namespace net.openstack.Providers.Rackspace.Validators
{
    public class CloudDnsValidator : IDnsValidator
    {
        /// <summary>
        ///  A default instance of <see cref="CloudDnsValidator"/>.
        /// </summary>
        private static readonly CloudDnsValidator _default = new CloudDnsValidator();

        /// <summary>
        /// Gets a default instance of <see cref="CloudDnsValidator"/>.
        /// </summary>
        public static CloudDnsValidator Default
        {
            get
            {
                return _default;
            }
        }

        public void ValidateRecordType(string recordType)
        {
            var recordTypeString = string.Format("Record Type:[{0}]", recordType);
            if (string.IsNullOrEmpty(recordType))
                throw new ArgumentNullException("RecordType", "ERROR: Record Type cannot be Null.");
            if (!new string[] { "A", "CNAME", "MX", "NS", "SRV", "TXT" }.Contains(recordType.ToUpper()))
                throw new InvalidArgumentException(string.Format("ERROR: Invalid Record type {0}", recordTypeString));
        }

        public void ValidateTTL(int ttl)
        {
            if (ttl < 300)
                throw new TTLLengthException("TTL range must be greater than 300 seconds TTL: " + ttl.ToString());
        }
    }
}