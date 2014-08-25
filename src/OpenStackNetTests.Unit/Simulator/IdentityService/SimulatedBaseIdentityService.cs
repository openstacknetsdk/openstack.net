namespace OpenStackNetTests.Unit.Simulator.IdentityService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Net;
    using OpenStack.Services.Identity;
    using Rackspace.Net;

    public class SimulatedBaseIdentityService : IDisposable
    {
        private static readonly UriTemplate _listApiVersionsTemplate = new UriTemplate("{?params*}");
        private static readonly UriTemplate _getApiVersionTemplate = new UriTemplate("{version}{?params*}");

        private readonly HttpListener _listener;

        public SimulatedBaseIdentityService(int port)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(string.Format("http://localhost:{0}/", port));
        }

        public Uri BaseAddress
        {
            get
            {
                return new Uri(_listener.Prefixes.Single());
            }
        }

        protected static Uri RemoveTrailingSlash(Uri uri)
        {
            UriBuilder builder = new UriBuilder(uri);
            builder.Path = builder.Path.TrimEnd('/');
            return builder.Uri;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _listener.Close();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _listener.Start();
            return Task.Run(() => ProcessRequests(cancellationToken), cancellationToken);
        }

        public async Task ProcessRequests(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                HttpListenerContext context = await _listener.GetContextAsync().ConfigureAwait(false);
                Task childTask = Task.Run(() => ProcessRequestAsync(context, cancellationToken), cancellationToken);
            }
        }

        public virtual async Task ProcessRequestAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            try
            {
                Uri uri = RemoveTrailingSlash(context.Request.Url);

                UriTemplateMatch match;
                match = _listApiVersionsTemplate.Match(BaseAddress, uri);
                if (match != null)
                {
                    await ListApiVersionsAsync(context, cancellationToken).ConfigureAwait(false);
                    return;
                }

                match = _getApiVersionTemplate.Match(BaseAddress, uri);
                if (match != null)
                {
                    KeyValuePair<VariableReference, object> versionParameter;
                    if (match.Bindings.TryGetValue("version", out versionParameter))
                    {
                        string version = versionParameter.Value as string;
                        if (!string.IsNullOrEmpty(version))
                        {
                            await GetApiVersionAsync(context, new ApiVersionId(version), cancellationToken).ConfigureAwait(false);
                            return;
                        }
                    }
                }

                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Close();
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Close();
            }
        }

        public async Task ListApiVersionsAsync(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ValidateRequest(context.Request);

            context.Response.ContentType = "application/json";
            if (!string.Equals("GET", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                context.Response.Close();
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.MultipleChoices;

            byte[] responseData = Encoding.UTF8.GetBytes(IdentityServiceResources.ListApiVersionsResponse);

            await context.Response.OutputStream.WriteAsync(responseData, 0, responseData.Length, cancellationToken);
            context.Response.Close();
        }

        public async Task GetApiVersionAsync(HttpListenerContext context, ApiVersionId id, CancellationToken cancellationToken)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (id == null)
                throw new ArgumentNullException("id");

            ValidateRequest(context.Request);

            context.Response.ContentType = "application/json";
            if (!string.Equals("GET", context.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                context.Response.Close();
                return;
            }

            JObject allVersions = JsonConvert.DeserializeObject<JObject>(IdentityServiceResources.ListApiVersionsResponse);
            ApiVersion[] versions = allVersions["versions"]["values"].ToObject<ApiVersion[]>();
            ApiVersion version = versions.FirstOrDefault(i => i.Id.Equals(id));

            if (version == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                JObject responseObject = new JObject(new JProperty("version", JObject.FromObject(version)));
                byte[] responseData = Encoding.UTF8.GetBytes(responseObject.ToString(Formatting.None));
                await context.Response.OutputStream.WriteAsync(responseData, 0, responseData.Length, cancellationToken);
            }

            context.Response.Close();
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
