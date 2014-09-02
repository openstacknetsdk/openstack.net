namespace OpenStackNetTests.Unit.Simulator
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class SimulatedService : IDisposable
    {
        private readonly HttpListener _listener;

        public SimulatedService(int port)
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

        protected virtual Uri GetDispatchUri(Uri requestUri)
        {
            if (requestUri == null)
                throw new ArgumentNullException("requestUri");

            Uri result = requestUri;
            if (result.AbsoluteUri.IndexOf('?') >= 0)
                result = new Uri(result.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped));

            if (result.AbsoluteUri.EndsWith("/"))
                result = RemoveTrailingSlash(result);

            return BaseAddress.MakeRelativeUri(result);
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
                Uri dispatchUri = GetDispatchUri(context.Request.Url);
                HttpResponseMessage response = await ProcessRequestImplAsync(context, dispatchUri, cancellationToken).ConfigureAwait(false);
                context.Response.StatusCode = (int)response.StatusCode;
                foreach (var header in response.Headers)
                {
                    throw new NotImplementedException();
                }

                foreach (var header in response.Content.Headers)
                {
                    switch (header.Key.ToLowerInvariant())
                    {
                    case "content-type":
                        context.Response.ContentType = string.Join(", ", header.Value);
                        break;

                    case "content-length":
                        context.Response.ContentLength64 = long.Parse(header.Value.Single());
                        break;

                    default:
                        throw new NotImplementedException();
                    }
                }

                await response.Content.CopyToAsync(context.Response.OutputStream);
                context.Response.Close();
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Close();
            }
        }

        protected virtual Task<HttpResponseMessage> ProcessRequestImplAsync(HttpListenerContext context, Uri dispatchUri, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
            return Task.FromResult(response);
        }
    }
}
