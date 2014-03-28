namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    internal static class TestHelpers
    {
        public static byte[] EncodeRequestBody<TBody>(HttpWebRequest request, TBody body, Func<HttpWebRequest, TBody, byte[]> encodeRequestBodyImpl)
        {
            byte[] encoded = encodeRequestBodyImpl(request, body);

            foreach (string header in request.Headers)
            {
                Console.Error.WriteLine(string.Format("{0}: {1}", header, request.Headers[header]));
            }

            Console.Error.WriteLine("<== " + Encoding.UTF8.GetString(encoded));
            return encoded;
        }

        public static Tuple<HttpWebResponse, string> ReadResult(Task<WebResponse> task, CancellationToken cancellationToken, Func<Task<WebResponse>, CancellationToken, Tuple<HttpWebResponse, string>> readResultImpl, bool reformat = true)
        {
            try
            {
                Tuple<HttpWebResponse, string> result = readResultImpl(task, cancellationToken);
                LogResult(result.Item1, result.Item2, reformat);
                return result;
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                if (response != null && response.ContentLength != 0)
                    LogResult(response, ex.Message, reformat);

                throw;
            }
        }

        private static void LogResult(HttpWebResponse response, string rawBody, bool reformat)
        {
            foreach (string header in response.Headers)
            {
                Console.Error.WriteLine(string.Format("{0}: {1}", header, response.Headers[header]));
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
