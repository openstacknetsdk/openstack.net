using System.Net;
using JSIStudios.SimpleRESTServices.Client;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Core.Validators;

namespace net.openstack.Providers.Rackspace.Validators
{
    internal class HttpResponseCodeValidator : IHttpResponseCodeValidator
    {
        public bool Validate(Response response)
        {
            if (response.StatusCode <= (HttpStatusCode)299)
                return true;

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new BadServiceRequestException(response);
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.MethodNotAllowed:
                    throw new UserNotAuthorizedException(response);
                case HttpStatusCode.NotFound:
                    throw new ItemNotFoundException(response);
                case HttpStatusCode.Conflict:
                    throw new ServiceConflictException(response);
                case HttpStatusCode.RequestEntityTooLarge:
                    throw new ServiceLimitReachedException(response);
                case HttpStatusCode.InternalServerError:
                    throw new ServiceFaultException(response);
                case HttpStatusCode.NotImplemented:
                    throw new MethodNotImplementedException(response);
                case HttpStatusCode.ServiceUnavailable:
                    throw new ServiceUnavailableException(response);
            }

            return true;
        }
    }
}
