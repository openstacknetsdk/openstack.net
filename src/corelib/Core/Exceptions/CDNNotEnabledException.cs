using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions
{
    [Serializable]
    public class CDNNotEnabledException : Exception
    {
        public CDNNotEnabledException() { }

        public CDNNotEnabledException(string message) : base(message) { }

        protected CDNNotEnabledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }	
}
