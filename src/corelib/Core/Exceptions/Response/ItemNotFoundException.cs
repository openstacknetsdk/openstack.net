namespace net.openstack.Core.Exceptions.Response
{
    public class ItemNotFoundException : ResponseException
    {
        public ItemNotFoundException(SimpleRestServices.Client.Response response) : base("The item was not found or does not exist.", response){ }

        public ItemNotFoundException(string message, SimpleRestServices.Client.Response response) : base(message, response){ }
    }
}