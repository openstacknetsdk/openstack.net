using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Exceptions.Response;

namespace net.openstack.Providers.Rackspace
{
    public class ProviderBase
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IRestService _restService;
        private static Version _currentVersion;

        protected ProviderBase(IIdentityProvider identityProvider, IRestService restService)
        {
            _identityProvider = identityProvider;
            _restService = restService;
        }

        protected Response<T> ExecuteRESTRequest<T>(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null,  bool isRetry = false, JsonRequestSettings requestSettings = null) 
        {
            if (requestSettings == null)
                requestSettings = BuildDefaultRequestSettings();

            if(headers == null)
                headers = new Dictionary<string, string>();

            headers.Add("X-Auth-Token", _identityProvider.GetToken(identity, isRetry));

            string bodyStr = null;
            if (body != null)
            {
                if (body is JObject)
                    bodyStr = body.ToString();
                else
                    bodyStr = JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            if (string.IsNullOrWhiteSpace(requestSettings.UserAgent))
                requestSettings.UserAgent = GetUserAgentHeaderValue();

            var response = _restService.Execute<T>(absoluteUri, method, bodyStr, headers, queryStringParameter, requestSettings);

            // on errors try again 1 time.
            if (response.StatusCode == 401)
            {
                if (!isRetry)
                {
                    return ExecuteRESTRequest<T>(identity, absoluteUri, method, body, queryStringParameter, headers, true);
                }
            }

            CheckResponse(response);

            return response;
        }

        protected Response ExecuteRESTRequest(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null, bool isRetry = false, JsonRequestSettings requestSettings = null)
        {
            if (requestSettings == null)
                requestSettings = BuildDefaultRequestSettings();

            if (headers == null)
                headers = new Dictionary<string, string>();

            headers.Add("X-Auth-Token", _identityProvider.GetToken(identity, isRetry));

            string bodyStr = null;
            if (body != null)
            {
                if (body is JObject)
                    bodyStr = body.ToString();
                else
                    bodyStr = JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            if (string.IsNullOrWhiteSpace(requestSettings.UserAgent))
                requestSettings.UserAgent = GetUserAgentHeaderValue();

            var response = _restService.Execute(absoluteUri, method, bodyStr, headers, queryStringParameter, requestSettings);

            // on errors try again 1 time.
            if (response.StatusCode == 401)
            {
                if (!isRetry)
                {
                    return ExecuteRESTRequest(identity, absoluteUri, method, body, queryStringParameter, headers, true);
                }
            }

            CheckResponse(response);

            return response;
        }

        internal JsonRequestSettings BuildDefaultRequestSettings(IEnumerable<int> non200SuccessCodes = null)
        {
            var non200SuccessCodesAggregate = new List<int>{ 401, 409 };
            if(non200SuccessCodes != null)
                non200SuccessCodesAggregate.AddRange(non200SuccessCodes);

            return new JsonRequestSettings { RetryCount = 2, RetryDelayInMS = 200, Non200SuccessCodes = non200SuccessCodesAggregate, UserAgent = GetUserAgentHeaderValue()};
        }

        protected virtual string GetServiceEndpoint(CloudIdentity identity, string serviceName, string region = null)
        {
            var userAccess = _identityProvider.GetUserAccess(identity);

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

        internal static void CheckResponse(Response response)
        {
            if(response.StatusCode <= 299)
                return;

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
                case 413:
                    throw new ServiceLimitReachedException(response);
                case 500:
                    throw new ServiceFaultException(response);
                case 501:
                    throw new MethodNotImplementedException(response);
                case 503:
                    throw new ServiceUnavailableException(response);
            }
        }

        internal static string GetUserAgentHeaderValue()
        {
            if (_currentVersion == null)
                _currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            return string.Format("openstack.net/{0}", _currentVersion.ToString());
        }
    }
}
