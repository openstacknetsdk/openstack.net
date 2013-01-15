using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleRestServices.Client;
using SimpleRestServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public class ProviderBase
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IRestService _restService;

        protected ProviderBase(IIdentityProvider identityProvider, IRestService restService)
        {
            _identityProvider = identityProvider;
            _restService = restService;
        }

        protected Response<T> ExecuteRESTRequest<T>(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null, bool isRetry = false, JsonRequestSettings requestSettings = null) where T : new()
        {
            if (requestSettings == null)
                requestSettings = BuildDefaultRequestSettings();

            var headers = new Dictionary<string, string>
                              {
                                  { "X-Auth-Token", _identityProvider.GetToken(identity, isRetry)}
                              };

            string bodyStr = null;
            if (body != null)
            {
                if (body is JObject)
                    bodyStr = body.ToString();
                else
                    bodyStr = JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            var response = _restService.Execute<T>(absoluteUri, method, bodyStr, headers, queryStringParameter, requestSettings);

            // on errors try again 1 time.
            if (response.StatusCode == 401)
            {
                if (!isRetry)
                {
                    return ExecuteRESTRequest<T>(identity, absoluteUri, method, body, queryStringParameter, true);
                }
            }

            return response; 
            
        }

        protected Response ExecuteRESTRequest(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null, bool isRetry = false, JsonRequestSettings requestSettings = null)
        {
            if (requestSettings == null)
                requestSettings = BuildDefaultRequestSettings();

            var headers = new Dictionary<string, string>
                              {
                                  { "X-Auth-Token", _identityProvider.GetToken(identity, isRetry)}
                              };

            string bodyStr = null;
            if (body != null)
            {
                if (body is JObject)
                    bodyStr = body.ToString();
                else
                    bodyStr = JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            var response = _restService.Execute(absoluteUri, method, bodyStr, headers, queryStringParameter, requestSettings);

            // on errors try again 1 time.
            if (response.StatusCode == 401)
            {
                if (!isRetry)
                {
                    return ExecuteRESTRequest(identity, absoluteUri, method, body, queryStringParameter, true);
                }
            }

            return response;

        }

        internal JsonRequestSettings BuildDefaultRequestSettings(IEnumerable<int> non200SuccessCodes = null)
        {
            var non200SuccessCodesAggregate = new List<int>{ 401, 409 };
            if(non200SuccessCodes != null)
                non200SuccessCodesAggregate.AddRange(non200SuccessCodes);

            return new JsonRequestSettings { RetryCount = 2, RetryDelayInMS = 200, Non200SuccessCodes = non200SuccessCodesAggregate};
        }

        protected virtual string GetServiceEndpoint(CloudIdentity identity, string serviceName, string region = null)
        {
            var userAccess = _identityProvider.Authenticate(identity);

            if (userAccess == null || userAccess.ServiceCatalog == null)
                throw new UserAuthenticationException("Unable to authenticate user and retrieve authorized service endpoints");

            var serviceDetails = userAccess.ServiceCatalog.FirstOrDefault(sc => sc.Name == serviceName);

            if (serviceDetails == null || serviceDetails.Endpoints == null || serviceDetails.Endpoints.Length == 0)
                throw new UserAuthorizationException("The user does not have access to the requested service.");

            if (string.IsNullOrWhiteSpace(region))
                region = userAccess.User.DefaultRegion;

            var endpoint = serviceDetails.Endpoints.FirstOrDefault(e => e.Region.Equals(region, StringComparison.OrdinalIgnoreCase));

            if(endpoint == null)
                throw new UserAuthorizationException("The user does not have access to the requested service or region.");

            return endpoint.PublicURL;
        }
    }

    public class UserAuthorizationException : Exception
    {
        public UserAuthorizationException(string message) : base(message){}
    }

    public class UserAuthenticationException : Exception
    {
        public UserAuthenticationException(string message) : base(message){}
    }
}
