using System;

namespace net.openstack.Core.Exceptions
{
    public class ServerEnteredErrorStateException : Exception
    {
        public string Status { get; private set; }

        public ServerEnteredErrorStateException(string status)
            : base(string.Format("The server entered an error state: '{0}'", status))
        {
            Status = status;
        }
    }
}