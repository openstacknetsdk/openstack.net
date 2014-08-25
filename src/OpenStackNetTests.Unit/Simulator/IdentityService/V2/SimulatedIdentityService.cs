namespace OpenStackNetTests.Unit.Simulator.IdentityService.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
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
        private static readonly UriTemplate _authenticateTemplate = new UriTemplate("v2.0/tokens{?params*}");
        private static readonly UriTemplate _listExtensionsTemplate = new UriTemplate("v2.0/extensions{?params*}");
        private static readonly UriTemplate _getExtensionTemplate = new UriTemplate("v2.0/extensions/{alias}{?params*}");
        private static readonly UriTemplate _listTenantsTemplate = new UriTemplate("v2.0/tenants{?limit,marker,params*}");

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

        public SimulatedIdentityService(int port)
            : base(port)
        {
        }

        public override async Task ProcessRequestAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            try
            {
                Uri uri = RemoveTrailingSlash(context.Request.Url);

                UriTemplateMatch match;
                match = _authenticateTemplate.Match(BaseAddress, uri);
                if (match != null)
                {
                    await AuthenticateAsync(context, cancellationToken).ConfigureAwait(false);
                    return;
                }

                match = _listExtensionsTemplate.Match(BaseAddress, uri);
                if (match != null)
                {
                    await ProcessListExtensionsRequestAsync(context, cancellationToken).ConfigureAwait(false);
                    return;
                }

                match = _getExtensionTemplate.Match(BaseAddress, uri);
                if (match != null)
                {
                    KeyValuePair<VariableReference, object> aliasArgument;
                    if (match.Bindings.TryGetValue("alias", out aliasArgument))
                    {
                        string alias = aliasArgument.Value as string;
                        if (!string.IsNullOrEmpty(alias))
                        {
                            await ProcessGetExtensionRequestAsync(context, new ExtensionAlias(alias), cancellationToken).ConfigureAwait(false);
                            return;
                        }
                    }
                }

                match = _listTenantsTemplate.Match(BaseAddress, uri);
                if (match != null)
                {
                    await ProcessListTenantsRequestAsync(context, match, cancellationToken).ConfigureAwait(false);
                    return;
                }

                await base.ProcessRequestAsync(context, cancellationToken).ConfigureAwait(false);
                return;
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Close();
            }
        }

        private async Task ProcessListExtensionsRequestAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (!string.Equals("GET", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                context.Response.Close();
                return;
            }

            ValidateRequest(context.Request);

            context.Response.ContentType = "application/json";

            byte[] buffer = Encoding.UTF8.GetBytes(IdentityServiceResources.ListExtensionsResponse);
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            context.Response.Close();
        }

        private async Task ProcessGetExtensionRequestAsync(HttpListenerContext context, ExtensionAlias alias, CancellationToken cancellationToken)
        {
            if (!string.Equals("GET", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                context.Response.Close();
                return;
            }

            ValidateRequest(context.Request);

            JObject allExtensions = JsonConvert.DeserializeObject<JObject>(IdentityServiceResources.ListExtensionsResponse);
            Extension[] extensions = allExtensions["extensions"]["values"].ToObject<Extension[]>();
            Extension extension = extensions.FirstOrDefault(i => i.Alias.Equals(alias));
            if (extension == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                JObject responseObject = new JObject(new JProperty("extension", JObject.FromObject(extension)));
                byte[] responseData = Encoding.UTF8.GetBytes(responseObject.ToString(Formatting.None));
                await context.Response.OutputStream.WriteAsync(responseData, 0, responseData.Length, cancellationToken);
            }

            context.Response.Close();
        }

        private async Task ProcessListTenantsRequestAsync(HttpListenerContext context, UriTemplateMatch match, CancellationToken cancellationToken)
        {
            if (!string.Equals("GET", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                context.Response.Close();
                return;
            }

            ValidateAuthenticatedRequest(context.Request);

            if (match.Bindings.ContainsKey("marker"))
                throw new NotImplementedException();
            if (match.Bindings.ContainsKey("limit"))
                throw new NotImplementedException();

            string responseBody = IdentityServiceResources.ListTenantsResponseTemplate;

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["tenantId"] = JsonConvert.SerializeObject(_tenantId);
            parameters["tenantName"] = JsonConvert.SerializeObject(_tenantName);
            foreach (var pair in parameters)
                responseBody = responseBody.Replace("{" + pair.Key + "}", JsonConvert.DeserializeObject<string>(pair.Value));

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json";
            byte[] responseData = Encoding.UTF8.GetBytes(responseBody);
            await context.Response.OutputStream.WriteAsync(responseData, 0, responseData.Length, cancellationToken);
            context.Response.Close();
        }

        private async Task AuthenticateAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (!string.Equals("POST", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                context.Response.Close();
                return;
            }

            MediaTypeHeaderValue acceptedType = new MediaTypeHeaderValue("application/json");
            MediaTypeHeaderValue contentType = MediaTypeHeaderValue.Parse(context.Request.ContentType);
            if (!HttpApiCall.IsAcceptable(acceptedType, contentType))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Close();
                return;
            }

            ValidateRequest(context.Request);

            StreamReader reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
            AuthenticationRequest authenticationRequest = JsonConvert.DeserializeObject<AuthenticationRequest>(await reader.ReadToEndAsync().ConfigureAwait(false));
            if (authenticationRequest == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Close();
                return;
            }

            AuthenticationData authenticationData = authenticationRequest.AuthenticationData;
            if (authenticationData == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Close();
                return;
            }

            if (authenticationData.TenantName != null
                && !string.Equals(authenticationData.TenantName, _tenantName, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.Close();
                return;
            }

            if (authenticationData.TenantId != null
                && authenticationData.TenantId != new ProjectId(_tenantId))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.Close();
                return;
            }

            if (authenticationData.Token != null)
                throw new NotImplementedException();

            PasswordCredentials passwordCredentials = authenticationData.PasswordCredentials;
            if (passwordCredentials == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Close();
                return;
            }

            if (!string.Equals(passwordCredentials.Username, _username, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(passwordCredentials.Password, _password, StringComparison.Ordinal))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.Close();
                return;
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

            context.Response.ContentType = "application/json";
            byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            context.Response.Close();
        }

        protected virtual void ValidateAuthenticatedRequest(HttpListenerRequest request)
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
