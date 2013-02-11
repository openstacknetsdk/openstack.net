using System;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class ResponseException : Exception
    {
        public JSIStudios.SimpleRESTServices.Client.Response Response { get; private set; }
        public ResponseException(string message, JSIStudios.SimpleRESTServices.Client.Response response)
            : base(message)
        {
            Response = response;
        }
    }
}