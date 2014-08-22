namespace OpenStackNetTests.Live
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using OpenStack.Net;

    internal static class TestHelpers
    {
        public static void HandleBeforeAsyncWebRequest(object sender, HttpRequestEventArgs e)
        {
            HttpRequestMessage request = e.Request;

            Console.Error.WriteLine("{0} (Request) {1} {2}", DateTime.Now, e.Request.Method, e.Request.RequestUri);

            foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
            {
                Console.Error.WriteLine(string.Format("{0}: {1}", header.Key, string.Join(", ", header.Value)));
            }

            if (request.Content != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in request.Content.Headers)
                {
                    Console.Error.WriteLine(string.Format("{0}: {1}", header.Key, string.Join(", ", header.Value)));
                }

                if (request.Content is StreamContent)
                {
                    Console.Error.WriteLine("<== [STREAM CONTENT]");
                }
                else
                {
                    Console.Error.WriteLine("<== " + Encoding.UTF8.GetString(request.Content.ReadAsByteArrayAsync().Result));
                }
            }
        }

        public static void HandleAfterAsyncWebResponse(object sender, HttpResponseEventArgs e)
        {
            if (e.Response.Status == TaskStatus.RanToCompletion)
            {
                Console.Error.WriteLine("{0} (Result {1})", DateTime.Now, e.Response.Result.StatusCode);

                if (e.Response.Result.Content != null && e.Response.Result.Content.Headers.ContentType != null)
                {
                    switch (e.Response.Result.Content.Headers.ContentType.MediaType)
                    {
                    case "application/json":
                        LogResult(e.Response.Result, e.Response.Result.Content.ReadAsStringAsync().Result, true);
                        break;

                    case "application/xml":
                    case "text/plain":
                    case "text/html":
                        LogResult(e.Response.Result, e.Response.Result.Content.ReadAsStringAsync().Result, false);
                        break;

                    default:
                        LogResult(e.Response.Result, "[STREAMING CONTENT]", false);
                        break;
                    }
                }
                else
                {
                    LogResult(e.Response.Result, "[STREAMING CONTENT]", false);
                }
            }
            else
            {
                Console.Error.WriteLine("{0} (Result {1})", DateTime.Now, e.Response.Status);
            }
        }

        private static void LogResult(HttpResponseMessage response, string rawBody, bool reformat)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers)
            {
                Console.Error.WriteLine(string.Format("{0}: {1}", header.Key, string.Join(", ", header.Value)));
            }

            if (response.Content != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in response.Content.Headers)
                {
                    Console.Error.WriteLine(string.Format("{0}: {1}", header.Key, string.Join(", ", header.Value)));
                }
            }

            if (!string.IsNullOrEmpty(rawBody))
            {
                string formatted = rawBody;
                if (reformat)
                {
                    try
                    {
                        object parsed = JsonConvert.DeserializeObject(rawBody);
                        formatted = JsonConvert.SerializeObject(parsed, Formatting.Indented);
                    }
                    catch (JsonException)
                    {
                        // couldn't reformat as JSON
                    }
                }

                Console.Error.WriteLine("==> " + formatted);
            }
        }
    }
}