using System;

namespace net.openstack.Core.Exceptions
{
    internal class InvalidCloudIdentityException : Exception
    {
        public InvalidCloudIdentityException(string message) : base(message) {}
    }
}