using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        protected readonly IIdentityProvider IdentityProvider;
        protected readonly IRestService RestService;
        protected readonly CloudIdentity DefaultIdentity;

        protected ProviderBase(CloudIdentity defaultIdentity, IIdentityProvider identityProvider, IRestService restService)
        {
            DefaultIdentity = defaultIdentity;
            IdentityProvider = identityProvider;
            RestService = restService;
        }
        
        protected Response<T> ExecuteRESTRequest<T>(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null,  bool isRetry = false, JsonRequestSettings settings = null)
        {
            return ExecuteRESTRequest<Response<T>>(identity, absoluteUri, method, body, queryStringParameter, headers, isRetry, settings,
                                                   (uri, requestMethod, requestBody, requestHeaders, requestQueryParams, requestSettings) => RestService.Execute<T>(uri, requestMethod, requestBody, requestHeaders, requestQueryParams, requestSettings));
        }

        protected Response ExecuteRESTRequest(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null, bool isRetry = false, JsonRequestSettings settings = null)
        {
            return ExecuteRESTRequest<Response>(identity, absoluteUri, method, body, queryStringParameter, headers, isRetry, settings,
                                       (uri, requestMethod, requestBody, requestHeaders, requestQueryParams, requestSettings) => RestService.Execute(uri, requestMethod, requestBody, requestHeaders, requestQueryParams, requestSettings));

        }

        protected Response ExecuteRESTRequest(CloudIdentity identity, Uri absoluteUri, HttpMethod method, Func<HttpWebResponse, bool, Response> buildResponseCallback, object body = null, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null, bool isRetry = false, JsonRequestSettings settings = null)
        {
            return ExecuteRESTRequest<Response>(identity, absoluteUri, method, body, queryStringParameter, headers, isRetry, settings,
                                       (uri, requestMethod, requestBody, requestHeaders, requestQueryParams, requestSettings) => RestService.Execute(uri, requestMethod, buildResponseCallback, requestBody, requestHeaders, requestQueryParams, requestSettings));

        }

        private T ExecuteRESTRequest<T>(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body, Dictionary<string, string> queryStringParameter, Dictionary<string, string> headers, bool isRetry, JsonRequestSettings requestSettings,
            Func<Uri, HttpMethod, string, Dictionary<string, string>, Dictionary<string, string>, JsonRequestSettings, T> callback) where T : Response
        {
            if (identity == null)
                identity = DefaultIdentity;

            if (requestSettings == null)
                requestSettings = BuildDefaultRequestSettings();

            if (headers == null)
                headers = new Dictionary<string, string>();

            headers.Add("X-Auth-Token", IdentityProvider.GetToken(identity, isRetry));

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

            var response = callback(absoluteUri, method, bodyStr, headers, queryStringParameter, requestSettings);

            // on errors try again 1 time.
            if (response.StatusCode == 401)
            {
                if (!isRetry)
                {
                    return ExecuteRESTRequest<T>(identity, absoluteUri, method, body, queryStringParameter, headers, true, requestSettings, callback);
                }
            }

            CheckResponse(response);

            return response;
        }

        protected Response StreamRESTRequest(CloudIdentity identity, Uri absoluteUri, HttpMethod method, Stream stream, int chunkSize, long maxReadLength = 0, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null, bool isRetry = false, RequestSettings requestSettings = null, Action<long> progressUpdated = null)
        {
            if (identity == null)
                identity = DefaultIdentity;

            if (requestSettings == null)
                requestSettings = BuildDefaultRequestSettings();

            requestSettings.Timeout = 14400000; // Need to pass this in.

            if (headers == null)
                headers = new Dictionary<string, string>();

            headers.Add("X-Auth-Token", IdentityProvider.GetToken(identity, isRetry));

            if (string.IsNullOrWhiteSpace(requestSettings.UserAgent))
                requestSettings.UserAgent = GetUserAgentHeaderValue();

            var response = RestService.Stream(absoluteUri, method, stream, chunkSize, maxReadLength, headers, queryStringParameter, requestSettings, progressUpdated);

            // on errors try again 1 time.
            if (response.StatusCode == 401)
            {
                if (!isRetry)
                {
                    return StreamRESTRequest(identity, absoluteUri, method, stream, chunkSize, maxReadLength, queryStringParameter, headers, isRetry, requestSettings, progressUpdated);
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

        private Endpoint GetServiceEndpoint(CloudIdentity identity, string serviceName, string region = null)
        {
            if (identity == null)
                identity = DefaultIdentity;

            var userAccess = IdentityProvider.GetUserAccess(identity);

            if (userAccess == null || userAccess.ServiceCatalog == null)
                throw new UserAuthenticationException("Unable to authenticate user and retrieve authorized service endpoints");

            var serviceDetails = userAccess.ServiceCatalog.FirstOrDefault(sc => sc.Name == serviceName);

            if (serviceDetails == null || serviceDetails.Endpoints == null || serviceDetails.Endpoints.Length == 0)
                throw new UserAuthorizationException("The user does not have access to the requested service.");

            if (string.IsNullOrWhiteSpace(region))
                region = userAccess.User.DefaultRegion;

            var endpoint = serviceDetails.Endpoints.FirstOrDefault(e => e.Region.Equals(region, StringComparison.OrdinalIgnoreCase));

            if (endpoint == null)
                throw new UserAuthorizationException("The user does not have access to the requested service or region.");

            return endpoint;
        }

        protected virtual string GetPublicServiceEndpoint(CloudIdentity identity, string serviceName, string region = null)
        {
            var endpoint = GetServiceEndpoint(identity, serviceName, region);

            return endpoint.PublicURL;
        }

        protected virtual string GetInternalServiceEndpoint(CloudIdentity identity, string serviceName, string region = null)
        {
            var endpoint = GetServiceEndpoint(identity, serviceName, region);

            return endpoint.InternalURL;
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
        }

        private static Version _currentVersion; 
        internal static string GetUserAgentHeaderValue()
        {
            if (_currentVersion == null)
                _currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            return string.Format("openstack.net/{0}", _currentVersion.ToString());
        }
    }
}
