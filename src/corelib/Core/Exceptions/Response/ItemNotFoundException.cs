using System;
using System.Runtime.Serialization;
using RestResponse = JSIStudios.SimpleRESTServices.Client.Response;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class ItemNotFoundException : ResponseException
    {
        public ItemNotFoundException(RestResponse response) : base("The item was not found or does not exist.", response) { }

        public ItemNotFoundException(string message, RestResponse response) : base(message, response) { }

        protected ItemNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
