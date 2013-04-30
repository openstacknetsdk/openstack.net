using JSIStudios.SimpleRESTServices.Client;

namespace net.openstack.Core.Validators
{
    public interface IHttpResponseCodeValidator
    {
        bool Validate(Response response);
    }
}
