using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Providers;
using net.openstack.Core.Validators;
using net.openstack.Providers.Rackspace.Objects;
using net.openstack.Providers.Rackspace.Validators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// Adds common functionality for all Rackspace Providers.
    /// </summary>
    public abstract class ProviderBase<TProvider>
    {
        protected readonly IIdentityProvider IdentityProvider;
        protected readonly IRestService RestService;
        protected readonly CloudIdentity DefaultIdentity;
        protected readonly IHttpResponseCodeValidator ResponseCodeValidator;

        protected ProviderBase(CloudIdentity defaultIdentity, IIdentityProvider identityProvider, IRestService restService)
            : this(defaultIdentity, identityProvider, restService, null) { }

        protected ProviderBase(CloudIdentity defaultIdentity,  IIdentityProvider identityProvider, IRestService restService, IHttpResponseCodeValidator httpStatusCodeValidator)
        {
            if(identityProvider == null)
                identityProvider = new CloudIdentityProvider(defaultIdentity);

            if (restService == null)
                restService = new JsonRestServices();

            if (httpStatusCodeValidator == null)
                httpStatusCodeValidator = new HttpResponseCodeValidator();

            DefaultIdentity = defaultIdentity;
            IdentityProvider = identityProvider;
            RestService = restService;
            ResponseCodeValidator = httpStatusCodeValidator;
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
            identity = GetDefaultIdentity(identity);

            if (requestSettings == null)
                requestSettings = BuildDefaultRequestSettings();

            if (headers == null)
                headers = new Dictionary<string, string>();

            headers["X-Auth-Token"] = IdentityProvider.GetToken(identity, isRetry);

            string bodyStr = null;
            if (body != null)
            {
                if (body is JObject)
                    bodyStr = body.ToString();
                else if (body is string)
                    bodyStr = body as string;
                else
                    bodyStr = JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            if (string.IsNullOrWhiteSpace(requestSettings.UserAgent))
                requestSettings.UserAgent = GetUserAgentHeaderValue();

            var response = callback(absoluteUri, method, bodyStr, headers, queryStringParameter, requestSettings);

            // on errors try again 1 time.
            if (response.StatusCode == HttpStatusCode.Unauthorized)
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
            identity = GetDefaultIdentity(identity);

            if (requestSettings == null)
                requestSettings = BuildDefaultRequestSettings();

            requestSettings.Timeout = TimeSpan.FromMilliseconds(14400000); // Need to pass this in.

            if (headers == null)
                headers = new Dictionary<string, string>();

            headers["X-Auth-Token"] = IdentityProvider.GetToken(identity, isRetry);

            if (string.IsNullOrWhiteSpace(requestSettings.UserAgent))
                requestSettings.UserAgent = GetUserAgentHeaderValue();

            var response = RestService.Stream(absoluteUri, method, stream, chunkSize, maxReadLength, headers, queryStringParameter, requestSettings, progressUpdated);

            // on errors try again 1 time.
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (!isRetry)
                {
                    return StreamRESTRequest(identity, absoluteUri, method, stream, chunkSize, maxReadLength, queryStringParameter, headers, isRetry, requestSettings, progressUpdated);
                }
            }

            CheckResponse(response);

            return response;
        }

        internal JsonRequestSettings BuildDefaultRequestSettings(IEnumerable<HttpStatusCode> non200SuccessCodes = null)
        {
            var non200SuccessCodesAggregate = new List<HttpStatusCode>{ HttpStatusCode.Unauthorized, HttpStatusCode.Conflict };
            if(non200SuccessCodes != null)
                non200SuccessCodesAggregate.AddRange(non200SuccessCodes);

            return new JsonRequestSettings { RetryCount = 2, RetryDelay = TimeSpan.FromMilliseconds(200), Non200SuccessCodes = non200SuccessCodesAggregate, UserAgent = GetUserAgentHeaderValue()};
        }

        protected Endpoint GetServiceEndpoint(CloudIdentity identity, string serviceName, string region = null)
        {
            identity = GetDefaultIdentity(identity);

            var userAccess = IdentityProvider.GetUserAccess(identity);

            if (userAccess == null || userAccess.ServiceCatalog == null)
                throw new UserAuthenticationException("Unable to authenticate user and retrieve authorized service endpoints.");

            var serviceDetails = userAccess.ServiceCatalog.FirstOrDefault(sc => sc.Name == serviceName);

            if (serviceDetails == null || serviceDetails.Endpoints == null || serviceDetails.Endpoints.Length == 0)
                throw new UserAuthorizationException("The user does not have access to the requested service.");

            if (string.IsNullOrWhiteSpace(region))
            {
                var isLondon = IsLondonIdentity(identity);
                region = string.IsNullOrWhiteSpace(userAccess.User.DefaultRegion) ?
                    isLondon ? "LON" : null : userAccess.User.DefaultRegion;

                if (string.IsNullOrWhiteSpace(region))
                    throw new NoDefaultRegionSetException("No region was provided and there is no default region set for the user's account.");
            }

            var endpoint = serviceDetails.Endpoints.FirstOrDefault(e => e.Region.Equals(region, StringComparison.OrdinalIgnoreCase)) ??
                           serviceDetails.Endpoints.FirstOrDefault(e => string.IsNullOrWhiteSpace(e.Region));

            if (endpoint == null)
                throw new UserAuthorizationException("The user does not have access to the requested service or region.");

            return endpoint;
        }

        protected virtual string GetPublicServiceEndpoint(CloudIdentity identity, string serviceName, string region)
        {
            var endpoint = GetServiceEndpoint(identity, serviceName, region);

            return endpoint.PublicURL;
        }

        protected virtual string GetInternalServiceEndpoint(CloudIdentity identity, string serviceName, string region)
        {
            var endpoint = GetServiceEndpoint(identity, serviceName, region);

            return endpoint.InternalURL;
        }

        internal void CheckResponse(Response response)
        {
            ResponseCodeValidator.Validate(response);
        }

        
        internal static string GetUserAgentHeaderValue()
        {
            return UserAgentGenerator.UserAgent;
        }

        protected T BuildCloudServersProviderAwareObject<T>(T input, string region, CloudIdentity identity) where T : ProviderStateBase<TProvider>
        {
            return BuildCloudServersProviderAwareObject(input, region, identity, CheckIdentityAndBuildProvider(identity));
        }

        protected IEnumerable<T> BuildCloudServersProviderAwareObject<T>(IEnumerable<T> input, string region, CloudIdentity identity) where T : ProviderStateBase<TProvider>
        {
            var provider = CheckIdentityAndBuildProvider(identity);

            return input.Select(obj => BuildCloudServersProviderAwareObject(obj, region, identity, provider)).ToList();
        }

        protected T BuildCloudServersProviderAwareObject<T>(T input, string region, CloudIdentity identity, TProvider provider) where T : ProviderStateBase<TProvider>
        {
            input.Provider = provider;
            input.Region = region;
            return input;
        }

        private TProvider CheckIdentityAndBuildProvider(CloudIdentity identity)
        {
            identity = GetDefaultIdentity(identity);

            return BuildProvider(identity);
        }

        protected abstract TProvider BuildProvider(CloudIdentity identity);

        protected CloudIdentity GetDefaultIdentity(CloudIdentity identity)
        {
            if (identity != null)
                return identity;

            if (DefaultIdentity != null)
                return DefaultIdentity;

            return IdentityProvider.DefaultIdentity;
        }

        private static bool IsLondonIdentity(CloudIdentity identity)
        {
            var rsCloudIdentity = identity as RackspaceCloudIdentity;

            if (rsCloudIdentity == null)
                return false;

            return rsCloudIdentity.CloudInstance == CloudInstance.UK;
        }
    }
}
