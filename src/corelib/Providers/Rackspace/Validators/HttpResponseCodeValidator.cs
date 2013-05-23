using JSIStudios.SimpleRESTServices.Client;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Core.Validators;

namespace net.openstack.Providers.Rackspace.Validators
{
    internal class HttpResponseCodeValidator : IHttpResponseCodeValidator
    {
        public bool Validate(Response response)
        {
            if (response.StatusCode <= 299)
                return true;

            switch (response.StatusCode)
            {
                case 400:
                    throw new BadServiceRequestException(response);
                case 401:
                case 403:
                case 405:
                    throw new UserNotAuthorizedException(response);
                case 404:
                    throw new ItemNotFoundException(response);
                case 409:
                    throw new ServiceConflictException(response);
                case 413:
                    throw new ServiceLimitReachedException(response);
                case 500:
                    throw new ServiceFaultException(response);
                case 501:
                    throw new MethodNotImplementedException(response);
                case 503:
                    throw new ServiceUnavailableException(response);
            }

            return true;
        }
    }
}
