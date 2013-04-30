using System;

namespace net.openstack.Core.Exceptions
{
    public class NoDefaultRegionSetException : Exception
    {
        public NoDefaultRegionSetException(string message) : base(message)
        {
        }
    }
}