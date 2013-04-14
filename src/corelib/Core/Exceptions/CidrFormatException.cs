using System;

namespace net.openstack.Core.Exceptions
{
    public class CidrFormatException : Exception
    {
        public CidrFormatException() { }
        public CidrFormatException(string message) : base(message) { }
    }
}
