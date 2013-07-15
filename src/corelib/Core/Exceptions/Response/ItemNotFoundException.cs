using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class ItemNotFoundException : ResponseException
    {
        public ItemNotFoundException(JSIStudios.SimpleRESTServices.Client.Response response) : base("The item was not found or does not exist.", response) { }

        public ItemNotFoundException(string message, JSIStudios.SimpleRESTServices.Client.Response response) : base(message, response) { }

        protected ItemNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
