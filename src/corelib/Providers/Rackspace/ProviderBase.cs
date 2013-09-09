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
using net.openstack.Core.Exceptions.Response;
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
        where TProvider : class
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
                httpStatusCodeValidator = HttpResponseCodeValidator.Default;

            DefaultIdentity = defaultIdentity;
            IdentityProvider = identityProvider;
            RestService = restService;
            ResponseCodeValidator = httpStatusCodeValidator;
        }        

        protected Response<T> ExecuteRESTRequest<T>(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null,  bool isRetry = false, RequestSettings settings = null)
        {
            return ExecuteRESTRequest<Response<T>>(identity, absoluteUri, method, body, queryStringParameter, headers, isRetry, settings, RestService.Execute<T>);
        }

        protected Response ExecuteRESTRequest(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null, bool isRetry = false, RequestSettings settings = null)
        {
            return ExecuteRESTRequest<Response>(identity, absoluteUri, method, body, queryStringParameter, headers, isRetry, settings, RestService.Execute);
        }

        protected Response ExecuteRESTRequest(CloudIdentity identity, Uri absoluteUri, HttpMethod method, Func<HttpWebResponse, bool, Response> buildResponseCallback, object body = null, Dictionary<string, string> queryStringParameter = null, Dictionary<string, string> headers = null, bool isRetry = false, RequestSettings settings = null)
        {
            return ExecuteRESTRequest<Response>(identity, absoluteUri, method, body, queryStringParameter, headers, isRetry, settings,
                (uri, requestMethod, requestBody, requestHeaders, requestQueryParams, requestSettings) => RestService.Execute(uri, requestMethod, buildResponseCallback, requestBody, requestHeaders, requestQueryParams, requestSettings));
        }

        private T ExecuteRESTRequest<T>(CloudIdentity identity, Uri absoluteUri, HttpMethod method, object body, Dictionary<string, string> queryStringParameter, Dictionary<string, string> headers, bool isRetry, RequestSettings requestSettings,
            Func<Uri, HttpMethod, string, Dictionary<string, string>, Dictionary<string, string>, RequestSettings, T> callback) where T : Response
        {
            identity = GetDefaultIdentity(identity);

            if (requestSettings == null)
                requestSettings = BuildDefaultRequestSettings();

            if (headers == null)
                headers = new Dictionary<string, string>();

            headers["X-Auth-Token"] = IdentityProvider.GetToken(identity, isRetry).Id;

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

            if (string.IsNullOrEmpty(requestSettings.UserAgent))
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

            headers["X-Auth-Token"] = IdentityProvider.GetToken(identity, isRetry).Id;

            if (string.IsNullOrEmpty(requestSettings.UserAgent))
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

        internal RequestSettings BuildDefaultRequestSettings(IEnumerable<HttpStatusCode> non200SuccessCodes = null)
        {
            var non200SuccessCodesAggregate = new List<HttpStatusCode>{ HttpStatusCode.Unauthorized, HttpStatusCode.Conflict };
            if(non200SuccessCodes != null)
                non200SuccessCodesAggregate.AddRange(non200SuccessCodes);

            return new JsonRequestSettings { RetryCount = 2, RetryDelay = TimeSpan.FromMilliseconds(200), Non200SuccessCodes = non200SuccessCodesAggregate, UserAgent = GetUserAgentHeaderValue()};
        }

        /// <summary>
        /// Gets the <see cref="Endpoint"/> associated with the specified service in the user's service catalog.
        /// </summary>
        /// <remarks>
        /// The endpoint returned by this method may not be an exact match for all arguments to this method.
        /// This method filters the service catalog in the following order to locate an acceptable endpoint.
        /// If more than one acceptable endpoint remains after all filters are applied, it is unspecified
        /// which one is returned by this method.
        ///
        /// <list type="number">
        /// <item>This method only considers services which match the specified <paramref name="serviceType"/>.</item>
        /// <item>This method attempts to filter the remaining items to those matching <paramref name="serviceName"/>. If <paramref name="serviceName"/> is <c>null</c>, or if no services match the specified name, <em>this argument is ignored</em>.</item>
        /// <item>This method attempts to filter the remaining items to those matching <paramref name="region"/>. If <paramref name="region"/> is <c>null</c>, the user's default region is used. If no services match the specified region, <em>this argument is ignored</em>.</item>
        /// </list>
        /// </remarks>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="serviceType">The service type (see <see cref="ServiceCatalog.Type"/>).</param>
        /// <param name="serviceName">The preferred name of the service (see <see cref="ServiceCatalog.Name"/>).</param>
        /// <param name="region">The preferred region for the service. If this value is <c>null</c>, the user's default region will be used.</param>
        /// <returns>An <see cref="Endpoint"/> object containing the details of the requested service.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="serviceType"/> is empty.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="NoDefaultRegionSetException">If <paramref name="region"/> is <c>null</c> and no default region is available for the identity or provider.</exception>
        /// <exception cref="UserAuthenticationException">If no service catalog is available for the user.</exception>
        /// <exception cref="UserAuthorizationException">If no endpoint is available for the requested service.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected Endpoint GetServiceEndpoint(CloudIdentity identity, string serviceType, string serviceName, string region)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (string.IsNullOrEmpty(serviceType))
                throw new ArgumentException("serviceType cannot be empty");
            CheckIdentity(identity);

            identity = GetDefaultIdentity(identity);

            var userAccess = IdentityProvider.GetUserAccess(identity);

            if (userAccess == null || userAccess.ServiceCatalog == null)
                throw new UserAuthenticationException("Unable to authenticate user and retrieve authorized service endpoints.");

            IEnumerable<ServiceCatalog> services = userAccess.ServiceCatalog.Where(sc => string.Equals(sc.Type, serviceType, StringComparison.OrdinalIgnoreCase));

            if (serviceName != null)
            {
                IEnumerable<ServiceCatalog> namedServices = services.Where(sc => string.Equals(sc.Name, serviceName, StringComparison.OrdinalIgnoreCase));
                if (namedServices.Any())
                    services = namedServices;
            }

            IEnumerable<Tuple<ServiceCatalog, Endpoint>> endpoints =
                services.SelectMany(service => service.Endpoints.Select(endpoint => Tuple.Create(service, endpoint)));

            string effectiveRegion = region;
            if (string.IsNullOrEmpty(effectiveRegion))
            {
                if (!string.IsNullOrEmpty(userAccess.User.DefaultRegion))
                    effectiveRegion = userAccess.User.DefaultRegion;
                else if (IsLondonIdentity(identity))
                    effectiveRegion = "LON";

                if (string.IsNullOrEmpty(effectiveRegion))
                    throw new NoDefaultRegionSetException("No region was provided and there is no default region set for the user's account.");
            }

            IEnumerable<Tuple<ServiceCatalog, Endpoint>> regionEndpoints =
                endpoints.Where(i => string.Equals(i.Item2.Region, effectiveRegion, StringComparison.OrdinalIgnoreCase));

            if (regionEndpoints.Any())
                endpoints = regionEndpoints;

            Tuple<ServiceCatalog, Endpoint> serviceEndpoint = endpoints.FirstOrDefault();
            if (serviceEndpoint == null)
                throw new UserAuthorizationException("The user does not have access to the requested service or region.");

            return serviceEndpoint.Item2;
        }

        /// <summary>
        /// Gets the <see cref="Endpoint.PublicURL"/> for the <see cref="Endpoint"/> associated with the
        /// specified service in the user's service catalog.
        /// </summary>
        /// <remarks>
        /// For details on how endpoint resolution is performed, see <see cref="GetServiceEndpoint"/>.
        /// </remarks>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="serviceType">The service type (see <see cref="ServiceCatalog.Type"/>).</param>
        /// <param name="serviceName">The preferred name of the service (see <see cref="ServiceCatalog.Name"/>).</param>
        /// <param name="region">The preferred region for the service. If this value is <c>null</c>, the user's default region will be used.</param>
        /// <returns>The <see cref="Endpoint.PublicURL"/> value for the <see cref="Endpoint"/> object containing the details of the requested service.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="serviceType"/> is empty.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="NoDefaultRegionSetException">If <paramref name="region"/> is <c>null</c> and no default region is available for the identity or provider.</exception>
        /// <exception cref="UserAuthenticationException">If no service catalog is available for the user.</exception>
        /// <exception cref="UserAuthorizationException">If no endpoint is available for the requested service.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected virtual string GetPublicServiceEndpoint(CloudIdentity identity, string serviceType, string serviceName, string region)
        {
            var endpoint = GetServiceEndpoint(identity, serviceType, serviceName, region);

            return endpoint.PublicURL;
        }

        /// <summary>
        /// Gets the <see cref="Endpoint.InternalURL"/> for the <see cref="Endpoint"/> associated with the
        /// specified service in the user's service catalog.
        /// </summary>
        /// <remarks>
        /// For details on how endpoint resolution is performed, see <see cref="GetServiceEndpoint"/>.
        /// </remarks>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="serviceType">The service type (see <see cref="ServiceCatalog.Type"/>).</param>
        /// <param name="serviceName">The preferred name of the service (see <see cref="ServiceCatalog.Name"/>).</param>
        /// <param name="region">The preferred region for the service. If this value is <c>null</c>, the user's default region will be used.</param>
        /// <returns>The <see cref="Endpoint.InternalURL"/> value for the <see cref="Endpoint"/> object containing the details of the requested service.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="serviceType"/> is empty.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="NoDefaultRegionSetException">If <paramref name="region"/> is <c>null</c> and no default region is available for the identity or provider.</exception>
        /// <exception cref="UserAuthenticationException">If no service catalog is available for the user.</exception>
        /// <exception cref="UserAuthorizationException">If no endpoint is available for the requested service.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected virtual string GetInternalServiceEndpoint(CloudIdentity identity, string serviceType, string serviceName, string region)
        {
            var endpoint = GetServiceEndpoint(identity, serviceType, serviceName, region);

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

        protected IEnumerable<T> BuildCloudServersProviderAwareObject<T>(IEnumerable<T> input, string region, CloudIdentity identity) where T : ProviderStateBase<TProvider>
        {
            return input.Select(obj => BuildCloudServersProviderAwareObject(obj, region, identity)).ToList();
        }

        protected T BuildCloudServersProviderAwareObject<T>(T input, string region, CloudIdentity identity)
            where T : ProviderStateBase<TProvider>
        {
            input.Provider = this as TProvider;
            input.Region = region;
            input.Identity = identity;
            return input;
        }

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

        protected Dictionary<string, string> BuildOptionalParameterList(Dictionary<string, string> optionalParameters)
        {
            if (optionalParameters == null)
                return null;

            var paramList = optionalParameters.Where(optionalParameter => !string.IsNullOrEmpty(optionalParameter.Value)).ToDictionary(optionalParameter => optionalParameter.Key, optionalParameter => optionalParameter.Value);

            if (!paramList.Any())
                return null;

            return paramList;
        }

        protected virtual void CheckIdentity(CloudIdentity identity)
        {
            if (identity == null && DefaultIdentity == null)
                throw new InvalidOperationException("No identity was specified for the request, and no default is available for the provider.");
        }
    }
}
