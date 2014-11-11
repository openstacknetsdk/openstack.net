namespace OpenStackNetTests.Unit.Simulator.IdentityService.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Web.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Net;
    using OpenStack.Services.Identity;
    using OpenStack.Services.Identity.V2;

    public class IdentityController : BaseIdentityController
    {
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
        private static readonly object _lock = new object();

        private static TokenId _tokenId;
        private static DateTimeOffset? _tokenCreated;
        private static DateTimeOffset? _tokenExpires;

        internal static TokenId TokenId
        {
            get
            {
                return _tokenId;
            }
        }

        internal static DateTimeOffset? TokenExpires
        {
            get
            {
                return _tokenExpires;
            }
        }

        [AllowAnonymous]
        [ActionName("Authenticate")]
        public HttpResponseMessage PostAuthenticate([FromBody] AuthenticationRequest authenticationRequest)
        {
            MediaTypeHeaderValue acceptedType = new MediaTypeHeaderValue("application/json");
            MediaTypeHeaderValue contentType = Request.Content.Headers.ContentType;
            if (!HttpApiCall.IsAcceptable(acceptedType, contentType))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            ValidateRequest(Request);

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

        [AllowAnonymous]
        [ActionName("ListExtensions")]
        public HttpResponseMessage GetAllExtensions()
        {
            ValidateRequest(Request);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StringContent(IdentityServiceResources.ListExtensionsResponse, Encoding.UTF8, "application/json");
            return result;
        }

        [AllowAnonymous]
        [ActionName("GetExtension")]
        public HttpResponseMessage GetExtension([FromUri(Name = "alias")] string aliasString)
        {
            ValidateRequest(Request);

            ExtensionAlias alias = new ExtensionAlias(aliasString);
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

        [ActionName("ListTenants")]
        public HttpResponseMessage GetAllTenants()
        {
            ValidateRequest(Request);

            if (Request.GetQueryNameValuePairs().Any())
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
    }
}
