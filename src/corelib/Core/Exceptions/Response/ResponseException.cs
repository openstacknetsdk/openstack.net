using System;

namespace net.openstack.Core.Exceptions.Response
{
    public class ResponseException : Exception
    {
        public SimpleRestServices.Client.Response Response { get; private set; }
        public ResponseException(string message, SimpleRestServices.Client.Response response) : base(message)
        {
            Response = response;
        }
    }
}