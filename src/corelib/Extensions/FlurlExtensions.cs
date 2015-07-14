using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Flurl.Http;
using OpenStack.Authentication;

namespace Flurl
{
    internal static class FlurlExtensions
    {
        public static FlurlClient AllowHttpStatus(this FlurlClient client, HttpStatusCode statusCode)
        {
            return client.AllowHttpStatus(((int)statusCode).ToString(CultureInfo.InvariantCulture));
        }

        public static FlurlClient AllowHttpStatus(this Url url, HttpStatusCode statusCode)
        {
            return new FlurlClient(url, autoDispose: true).AllowHttpStatus(statusCode);
        }

        public static FlurlClient AllowHttpStatus(this string url, HttpStatusCode statusCode)
        {
            return new Url(url).AllowHttpStatus(statusCode);
        }

        public static Uri ToUri(this Url url)
        {
            return new Uri(url.ToString());
        }

        public static Url RemoveNullOrEmptyQueryParams(this string url)
        {
            return new Url(url).RemoveNullOrEmptyQueryParams();    
        }

        public static Url RemoveNullOrEmptyQueryParams(this Url url)
        {
            foreach (KeyValuePair<string, object> queryParam in url.QueryParams.ToList())
            {
                if (queryParam.Value == null || queryParam.Value.ToString() == string.Empty)
                    url.QueryParams.Remove(queryParam);
            }

            return url;
        }

        public static FlurlClient Authenticate(this string url, IAuthenticationProvider authenticationProvider)
        {
            return new Url(url).Authenticate(authenticationProvider);
        }

        public static FlurlClient Authenticate(this Url url, IAuthenticationProvider authenticationProvider)
        {
            var client = new FlurlClient(url, autoDispose: true);
            return client.Authenticate(authenticationProvider);
        }

        public static FlurlClient Authenticate(this FlurlClient client, IAuthenticationProvider authenticationProvider)
        {
            var authenticatedMessageHandler = client.HttpMessageHandler as AuthenticatedMessageHandler;
            if (authenticatedMessageHandler != null)
            {
                authenticatedMessageHandler.AuthenticationProvider = authenticationProvider;
            }
            return client;
        }
    }
}
