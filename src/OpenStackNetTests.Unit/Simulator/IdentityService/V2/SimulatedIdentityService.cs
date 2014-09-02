namespace OpenStackNetTests.Unit.Simulator.IdentityService.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Net;
    using OpenStack.Services.Identity;
    using OpenStack.Services.Identity.V2;
    using Rackspace.Net;
    using Encoding = System.Text.Encoding;
    using StreamReader = System.IO.StreamReader;

    public class SimulatedIdentityService : SimulatedBaseIdentityService
    {
        private static readonly UriTemplate _authenticateTemplate = new UriTemplate("v2.0/tokens");
        private static readonly UriTemplate _listExtensionsTemplate = new UriTemplate("v2.0/extensions");
        private static readonly UriTemplate _getExtensionTemplate = new UriTemplate("v2.0/extensions/{alias}");
        private static readonly UriTemplate _listTenantsTemplate = new UriTemplate("v2.0/tenants");

        private static readonly string _username = "simulated_user";
        private static readonly string _password = "simulated_password";
        private static readonly string _tenantName = "simulated_tenant";
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This value was generated specifically for use in this simulator.
        /// </remarks>
        private static readonly string _tenantId = "{BC5A63CD-F5B3-4D8F-AF4A-15FECE463D4D}";
        private static readonly string _userFullName = _username;
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This value was generated specifically for use in this simulator.
        /// </remarks>
        private static readonly string _userId = "{97DD18E0-4763-4286-93B1-17B784FE8465}";

        /// <summary>
        /// This is used for synchronizing access to the authentication fields below.
        /// </summary>
        private readonly object _lock = new object();

        private TokenId _tokenId;
        private DateTimeOffset? _tokenCreated;
        private DateTimeOffset? _tokenExpires;

        public SimulatedIdentityService()
            : base(5000)
        {
        }

        protected override async Task<HttpResponseMessage> ProcessRequestImplAsync(HttpListenerContext context, Uri dispatchUri, CancellationToken cancellationToken)
        {
            UriTemplateMatch match;
            match = _authenticateTemplate.Match(dispatchUri);
            if (match != null)
            {
                return await AuthenticateAsync(context, cancellationToken).ConfigureAwait(false);
            }

            match = _listExtensionsTemplate.Match(dispatchUri);
            if (match != null)
            {
                return ProcessListExtensionsRequestAsync(context, cancellationToken);
            }

            match = _getExtensionTemplate.Match(dispatchUri);
            if (match != null)
            {
                KeyValuePair<VariableReference, object> aliasArgument;
                if (match.Bindings.TryGetValue("alias", out aliasArgument))
                {
                    string alias = aliasArgument.Value as string;
                    if (!string.IsNullOrEmpty(alias))
                    {
                        return ProcessGetExtensionRequestAsync(context, new ExtensionAlias(alias), cancellationToken);
                    }
                }
            }

            match = _listTenantsTemplate.Match(dispatchUri);
            if (match != null)
            {
                return ProcessListTenantsRequestAsync(context, cancellationToken);
            }

            return await base.ProcessRequestImplAsync(context, dispatchUri, cancellationToken);
        }

        private HttpResponseMessage ProcessListExtensionsRequestAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (!string.Equals("GET", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
                return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);

            ValidateRequest(context.Request);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StringContent(IdentityServiceResources.ListExtensionsResponse, Encoding.UTF8, "application/json");
            return result;
        }

        private HttpResponseMessage ProcessGetExtensionRequestAsync(HttpListenerContext context, ExtensionAlias alias, CancellationToken cancellationToken)
        {
            if (!string.Equals("GET", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
                return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);

            ValidateRequest(context.Request);

            JObject allExtensions = JsonConvert.DeserializeObject<JObject>(IdentityServiceResources.ListExtensionsResponse);
            Extension[] extensions = allExtensions["extensions"]["values"].ToObject<Extension[]>();
            Extension extension = extensions.FirstOrDefault(i => i.Alias.Equals(alias));
            if (extension == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            else
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

                JObject responseObject = new JObject(new JProperty("extension", JObject.FromObject(extension)));
                result.Content = new StringContent(responseObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                return result;
            }
        }

        private HttpResponseMessage ProcessListTenantsRequestAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (!string.Equals("GET", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
                return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);

            ValidateAuthenticatedRequest(context.Request);

            if (context.Request.Url.AbsoluteUri.IndexOf('?') >= 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NotImplemented)
                {
                    Content = new StringContent("The call supports query parameters which are not implemented by the simulator.", Encoding.UTF8, "text/plain")
                };
            }

            string responseBody = IdentityServiceResources.ListTenantsResponseTemplate;

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["tenantId"] = JsonConvert.SerializeObject(_tenantId);
            parameters["tenantName"] = JsonConvert.SerializeObject(_tenantName);
            foreach (var pair in parameters)
                responseBody = responseBody.Replace("{" + pair.Key + "}", JsonConvert.DeserializeObject<string>(pair.Value));

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StringContent(responseBody, Encoding.UTF8, "application/json");
            return result;
        }

        private async Task<HttpResponseMessage> AuthenticateAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (!string.Equals("POST", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
                return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);

            MediaTypeHeaderValue acceptedType = new MediaTypeHeaderValue("application/json");
            MediaTypeHeaderValue contentType = MediaTypeHeaderValue.Parse(context.Request.ContentType);
            if (!HttpApiCall.IsAcceptable(acceptedType, contentType))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            ValidateRequest(context.Request);

            StreamReader reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
            AuthenticationRequest authenticationRequest = JsonConvert.DeserializeObject<AuthenticationRequest>(await reader.ReadToEndAsync().ConfigureAwait(false));
            if (authenticationRequest == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            AuthenticationData authenticationData = authenticationRequest.AuthenticationData;
            if (authenticationData == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            if (authenticationData.TenantName != null
                && !string.Equals(authenticationData.TenantName, _tenantName, StringComparison.OrdinalIgnoreCase))
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);

            if (authenticationData.TenantId != null
                && authenticationData.TenantId != new ProjectId(_tenantId))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            if (authenticationData.Token != null)
                throw new NotImplementedException();

            PasswordCredentials passwordCredentials = authenticationData.PasswordCredentials;
            if (passwordCredentials == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            if (!string.Equals(passwordCredentials.Username, _username, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(passwordCredentials.Password, _password, StringComparison.Ordinal))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            bool hasTenant = authenticationData.TenantId != null || authenticationData.TenantName != null;

            string responseBody;
            if (hasTenant)
                responseBody = IdentityServiceResources.AuthenticateResponseTemplate;
            else
                responseBody = IdentityServiceResources.AuthenticateWithoutTenantResponseTemplate;

            lock (_lock)
            {
                var parameters = new Dictionary<string, string>();

                // expire the token 5 minutes early
                if (!_tokenExpires.HasValue || _tokenExpires < DateTimeOffset.Now - TimeSpan.FromMinutes(5))
                {
                    // generate a new token
                    _tokenCreated = DateTimeOffset.Now;
                    _tokenExpires = _tokenCreated + TimeSpan.FromHours(24);
                    _tokenId = new TokenId(Guid.NewGuid().ToString());
                }

                parameters["issued_at"] = JsonConvert.SerializeObject(_tokenCreated.Value);
                parameters["expires"] = JsonConvert.SerializeObject(_tokenExpires.Value);
                parameters["tokenId"] = JsonConvert.SerializeObject(_tokenId);
                parameters["tenantId"] = JsonConvert.SerializeObject(_tenantId);
                parameters["tenantName"] = JsonConvert.SerializeObject(_tenantName);
                parameters["username"] = JsonConvert.SerializeObject(_username);
                parameters["userId"] = JsonConvert.SerializeObject(_userId);
                parameters["userFullName"] = JsonConvert.SerializeObject(_userFullName);

                foreach (var pair in parameters)
                    responseBody = responseBody.Replace("{" + pair.Key + "}", JsonConvert.DeserializeObject<string>(pair.Value));
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StringContent(responseBody, Encoding.UTF8, "application/json");
            return result;
        }

        public virtual void ValidateAuthenticatedRequest(HttpListenerRequest request)
        {
            ValidateRequest(request);

            if (_tokenId == null || _tokenExpires < DateTimeOffset.Now)
                throw new InvalidOperationException();

            string authHeader = request.Headers.Get("X-Auth-Token");
            if (authHeader == null)
                throw new InvalidOperationException();

            TokenId tokenId = new TokenId(authHeader.Trim());
            if (tokenId != _tokenId)
                throw new InvalidOperationException();
        }
    }
}
