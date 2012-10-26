using System;
using System.Collections.Generic;
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
        private readonly Uri _urlBase;
        private readonly IIdentityProvider _identityProvider;
        private readonly IRestService _restService;

        protected ProviderBase(Uri urlBase, IIdentityProvider identityProvider, IRestService restService)
        {
            _urlBase = urlBase;
            _identityProvider = identityProvider;
            _restService = restService;
        }

        protected Response<T> ExecuteRESTRequest<T>(string urlPath, HttpMethod method, object body, CloudIdentity identity, bool isRetry = false, string token = null, int retryCount = 2, int retryDelay = 200) where T : new()
        {
            var url = new Uri(_urlBase, urlPath);

            var headers = new Dictionary<string, string>
                              {
                                  { "X-Auth-Token", string.IsNullOrWhiteSpace(token) ? _identityProvider.GetToken(identity) : token }
                              };

            string bodyStr = null;
            if (body != null)
            {
                if (body is JObject)
                    bodyStr = body.ToString();
                else
                    bodyStr = JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            var response = _restService.Execute<T>(url, method, bodyStr, headers, new JsonRequestSettings() { RetryCount = retryCount, RetryDelayInMS = retryDelay, Non200SuccessCodes = new[] { 401, 409 } });

            // on errors try again 1 time.
            if (response.StatusCode == 401)
            {
                if (!isRetry)
                {
                    return ExecuteRESTRequest<T>(urlPath, method, body, identity, true, _identityProvider.GetToken(identity));
                }
            }

            return response;
        }
    }
}
