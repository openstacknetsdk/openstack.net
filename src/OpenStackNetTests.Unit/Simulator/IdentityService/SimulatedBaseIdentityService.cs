namespace OpenStackNetTests.Unit.Simulator.IdentityService
{
    using System;
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

    public class SimulatedBaseIdentityService : IDisposable
    {
        private readonly HttpListener _listener;

        public SimulatedBaseIdentityService(int port)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(string.Format("http://localhost:{0}/", port));
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
                string[] segments = context.Request.Url.Segments;
                switch (segments.Length)
                {
                case 1:
                    await ListApiVersionsAsync(context, cancellationToken);
                    break;

                case 2:
                    await GetApiVersionAsync(context, new ApiVersionId(segments[1].TrimEnd('/')), cancellationToken);
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    throw new NotImplementedException();
                }
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
