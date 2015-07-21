using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using OpenStack.Authentication;

// ReSharper disable once CheckNamespace
namespace Flurl.Extensions
{
    /// <summary>
    /// Useful Flurl extension methods for custom implementations.
    /// </summary>
    public static class FlurlExtensions
    {
        /// <summary>
        /// Allow a specific HTTP status code.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="statusCode">The allowed status code.</param>
        /// <returns></returns>
        public static FlurlClient AllowHttpStatus(this FlurlClient client, HttpStatusCode statusCode)
        {
            return client.AllowHttpStatus(((int)statusCode).ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Allow a specific HTTP status code.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="statusCode">The allowed status code.</param>
        /// <returns></returns>
        public static FlurlClient AllowHttpStatus(this Url url, HttpStatusCode statusCode)
        {
            return new FlurlClient(url, autoDispose: true).AllowHttpStatus(statusCode);
        }

        /// <summary>
        /// Allow a specific HTTP status code.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="statusCode">The allowed status code.</param>
        /// <returns></returns>
        public static FlurlClient AllowHttpStatus(this string url, HttpStatusCode statusCode)
        {
            return new Url(url).AllowHttpStatus(statusCode);
        }

        /// <summary>
        /// Converts a <see cref="Url"/> to a <see cref="Uri"/>.
        /// </summary>
        /// <param name="url">The URL.</param>
        public static Uri ToUri(this Url url)
        {
            return new Uri(url.ToString());
        }

        /// <summary>
        /// Removes any query parameters which have a null or empty value.
        /// </summary>
        /// <param name="url">The URL.</param>
        public static Url RemoveNullOrEmptyQueryParams(this string url)
        {
            return new Url(url).RemoveNullOrEmptyQueryParams();    
        }

        /// <summary>
        /// Removes any query parameters which have a null or empty value.
        /// </summary>
        /// <param name="url">The URL.</param>
        public static Url RemoveNullOrEmptyQueryParams(this Url url)
        {
            foreach (KeyValuePair<string, object> queryParam in url.QueryParams.ToList())
            {
                if (queryParam.Value == null || queryParam.Value.ToString() == string.Empty)
                    url.QueryParams.Remove(queryParam);
            }

            return url;
        }

        /// <summary>
        /// Applies OpenStack authentication to a request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <returns>
        /// An authenticated request.
        /// </returns>
        public static PreparedRequest Authenticate(this string url, IAuthenticationProvider authenticationProvider)
        {
            return new Url(url).Authenticate(authenticationProvider);
        }

        /// <summary>
        /// Applies OpenStack authentication to a request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <returns>
        /// An authenticated request.
        /// </returns>
        public static PreparedRequest Authenticate(this Url url, IAuthenticationProvider authenticationProvider)
        {
            var client = new PreparedRequest(url, autoDispose: true);
            return client.Authenticate(authenticationProvider);
        }

        /// <summary>
        /// Applies OpenStack authentication to a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <returns>
        /// An authenticated request.
        /// </returns>
        public static PreparedRequest Authenticate(this PreparedRequest request, IAuthenticationProvider authenticationProvider)
        {
            var authenticatedMessageHandler = request.HttpMessageHandler as AuthenticatedMessageHandler;
            if (authenticatedMessageHandler != null)
            {
                authenticatedMessageHandler.AuthenticationProvider = authenticationProvider;
            }
            return request;
        }

        /// <summary>
        /// Sends the <see cref="PreparedRequest"/>.
        /// </summary>
        /// <param name="requestTask">A task which returns the request.</param>
        /// <returns>The HTTP response message.</returns>
        public static async Task<HttpResponseMessage> SendAsync(this Task<PreparedRequest> requestTask)
        {
            PreparedRequest request = await requestTask.ConfigureAwait(false);
            return await request.SendAsync().ConfigureAwait(false);
        }
    }
}
