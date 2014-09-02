namespace OpenStackNetTests.Unit.Simulator.IdentityService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Net;
    using OpenStack.Services.Identity;
    using Rackspace.Net;

    public class SimulatedBaseIdentityService : SimulatedService
    {
        private static readonly UriTemplate _listApiVersionsTemplate = new UriTemplate("");
        private static readonly UriTemplate _getApiVersionTemplate = new UriTemplate("{version}");

        public SimulatedBaseIdentityService(int port)
            : base(port)
        {
        }

        protected override async Task<HttpResponseMessage> ProcessRequestImplAsync(HttpListenerContext context, Uri dispatchUri, CancellationToken cancellationToken)
        {
            UriTemplateMatch match;
            match = _listApiVersionsTemplate.Match(dispatchUri);
            if (match != null)
            {
                return await ListApiVersionsAsync(context, cancellationToken).ConfigureAwait(false);
            }

            match = _getApiVersionTemplate.Match(dispatchUri);
            if (match != null)
            {
                KeyValuePair<VariableReference, object> versionParameter;
                if (match.Bindings.TryGetValue("version", out versionParameter))
                {
                    string version = versionParameter.Value as string;
                    if (!string.IsNullOrEmpty(version))
                        return await GetApiVersionAsync(context, new ApiVersionId(version), cancellationToken).ConfigureAwait(false);
                }
            }

            return await base.ProcessRequestImplAsync(context, dispatchUri, cancellationToken);
        }

        public Task<HttpResponseMessage> ListApiVersionsAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ValidateRequest(context.Request);

            context.Response.ContentType = "application/json";
            if (!string.Equals("GET", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed));

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.MultipleChoices);
            response.Content = new StringContent(IdentityServiceResources.ListApiVersionsResponse, Encoding.UTF8, "application/json");
            return Task.FromResult(response);
        }

        public Task<HttpResponseMessage> GetApiVersionAsync(HttpListenerContext context, ApiVersionId id, CancellationToken cancellationToken)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (id == null)
                throw new ArgumentNullException("id");

            ValidateRequest(context.Request);

            if (!string.Equals("GET", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed));

            JObject allVersions = JsonConvert.DeserializeObject<JObject>(IdentityServiceResources.ListApiVersionsResponse);
            ApiVersion[] versions = allVersions["versions"]["values"].ToObject<ApiVersion[]>();
            ApiVersion version = versions.FirstOrDefault(i => i.Id.Equals(id));

            if (version == null)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            else
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

                JObject responseObject = new JObject(new JProperty("version", JObject.FromObject(version)));
                response.Content = new StringContent(responseObject.ToString(Formatting.None), Encoding.UTF8, "application/json");

                return Task.FromResult(response);
            }
        }

        protected virtual void ValidateRequest(HttpListenerRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            MediaTypeHeaderValue[] acceptTypes = Array.ConvertAll(request.AcceptTypes, MediaTypeHeaderValue.Parse);
            MediaTypeHeaderValue contentType = new MediaTypeHeaderValue("application/json");
            if (!acceptTypes.Any(acceptType => HttpApiCall.IsAcceptable(acceptType, contentType)))
                throw new NotSupportedException();
        }
    }
}
