using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.Core.Exceptions
{
    public class CDNNotEnabledException : Exception
    {
        public CDNNotEnabledException() { }
        public CDNNotEnabledException(string message) : base(message) { }
    }	
}
