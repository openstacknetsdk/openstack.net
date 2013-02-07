using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.Core.Exceptions.Response
{
    class MethodNotImplementedException : ResponseException
    {
        public MethodNotImplementedException(SimpleRestServices.Client.Response response) : base("The requested method is not implemented at the service.", response) { }

        public MethodNotImplementedException(string message, SimpleRestServices.Client.Response response) : base(message, response) {}
    }
}
