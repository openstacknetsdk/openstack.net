using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace net.openstack.corelib.Web.Json
{
    public class JsonRestServices : IRestService
    {
        private readonly IRetryLogic<WebResponse, int> _retryLogic;
        private readonly IWebRequestLogger _logger;

        public JsonRestServices() : this(new WebRequestRetryLogic(), null) { }
        public JsonRestServices(IRetryLogic<WebResponse, int> retryLogic, IWebRequestLogger logger)
        {
            _retryLogic = retryLogic;
            _logger = logger;
        }

        public WebResponse<T> Execute<T, TBody>(string url, HttpMethod method, TBody body, Dictionary<string, string> headers, string contentType, int retryCount, int delayInMilliseconds, IEnumerable<int> non200SuccessCodes)
        {
            return Execute<T, TBody>(new Uri(url), method, body, headers, contentType, retryCount, delayInMilliseconds, non200SuccessCodes);
        }

        public WebResponse<T> Execute<T, TBody>(Uri url, HttpMethod method, TBody body, Dictionary<string, string> headers, string contentType, int retryCount, int delayInMilliseconds, IEnumerable<int> non200SuccessCodes)
        {
            var rawBody = JsonConvert.SerializeObject(body);
            return Execute<T>(url, method, rawBody, headers, contentType, retryCount, delayInMilliseconds, non200SuccessCodes);
        }

        public WebResponse<T> Execute<T>(string url, HttpMethod method, string body, Dictionary<string, string> headers, string contentType, int retryCount, int delayInMilliseconds, IEnumerable<int> non200SuccessCodes)
        {
            return Execute<T>(new Uri(url), method, body, headers, contentType, retryCount, delayInMilliseconds, non200SuccessCodes);
        }

        public WebResponse<T> Execute<T>(Uri url, HttpMethod method, string body, Dictionary<string, string> headers, string contentType, int retryCount, int delayInMilliseconds, IEnumerable<int> non200SuccessCodes)
        {
            return Execute(url, method, (resp, isError) => BuildWebResponse<T>(resp), body, headers, contentType, retryCount, delayInMilliseconds, non200SuccessCodes) as WebResponse<T>;
        }

        public WebResponse Execute<TBody>(string url, HttpMethod method, TBody body, Dictionary<string, string> headers, string contentType, int retryCount, int delayInMilliseconds, IEnumerable<int> non200SuccessCodes)
        {
            return Execute(new Uri(url), method, body, headers, contentType, retryCount, delayInMilliseconds, non200SuccessCodes);
        }

        public WebResponse Execute<TBody>(Uri url, HttpMethod method, TBody body, Dictionary<string, string> headers, string contentType, int retryCount, int delayInMilliseconds, IEnumerable<int> non200SuccessCodes)
        {
            var rawBody = JsonConvert.SerializeObject(body);
            return Execute(url, method, rawBody, headers, contentType, retryCount, delayInMilliseconds, non200SuccessCodes);
        }

        public WebResponse Execute(string url, HttpMethod method, string body, Dictionary<string, string> headers, string contentType, int retryCount, int delayInMilliseconds, IEnumerable<int> non200SuccessCodes)
        {
            return Execute(new Uri(url), method, body, headers, contentType, retryCount, delayInMilliseconds, non200SuccessCodes);
        }

        public WebResponse Execute(Uri url, HttpMethod method, string body, Dictionary<string, string> headers, string contentType, int retryCount, int delayInMilliseconds, IEnumerable<int> non200SuccessCodes)
        {
            return Execute(url, method, null, body, headers, contentType, retryCount, delayInMilliseconds, non200SuccessCodes);
        }

        private WebResponse Execute(Uri url, HttpMethod method, Func<HttpWebResponse, bool, WebResponse> callback, string body, Dictionary<string, string> headers, string contentType, int retryCount, int delayInMilliseconds, IEnumerable<int> non200SuccessCodes)
        {
            return _retryLogic.Execute(() =>
            {
                var req = WebRequest.Create(url) as HttpWebRequest;
                req.Method = method.ToString();
                req.ContentType = contentType;
                req.Accept = contentType;

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        req.Headers.Add(header.Key, header.Value);
                    }
                }

                // Encode the parameters as form data:
                if ((method == HttpMethod.POST || method == HttpMethod.PUT) && !string.IsNullOrWhiteSpace(body))
                {
                    byte[] formData = UTF8Encoding.UTF8.GetBytes(body);
                    req.ContentLength = formData.Length;

                    // Send the request:
                    using (Stream post = req.GetRequestStream())
                    {
                        post.Write(formData, 0, formData.Length);
                    }
                }

                var startTime = DateTime.UtcNow;
                WebResponse response;

                try
                {
                    using (var resp = req.GetResponse() as HttpWebResponse)
                    {
                        if (callback != null)
                            response = callback(resp, false);
                        else
                            response = BuildWebResponse(resp);
                    }
                }
                catch (WebException ex)
                {
                    using (var resp = ex.Response as HttpWebResponse)
                    {
                        if (callback != null)
                            response = callback(resp, true);
                        else
                            response = BuildWebResponse(resp);
                    }
                }

                var endTime = DateTime.UtcNow;

                LogExternalServiceCall(method.ToString(), url.OriginalString, body, response, startTime, endTime);

                return response;
            }, non200SuccessCodes, retryCount, delayInMilliseconds);
        }

        private WebResponse BuildWebResponse(HttpWebResponse resp)
        {
            if (resp == null)
                return new WebResponse(0, null, null);

            string respBody;
            using (var reader = new StreamReader(resp.GetResponseStream()))
            {
                respBody = reader.ReadToEnd();
            }

            var respHeaders = resp.Headers.AllKeys.Select(key => new HttpHeader() { Key = key, Value = resp.GetResponseHeader(key) }).ToList();
            return new WebResponse(resp.StatusCode, respHeaders, respBody);
        }

        private WebResponse<T> BuildWebResponse<T>(HttpWebResponse resp)
        {
            var baseReponse = BuildWebResponse(resp);
            T data = default(T);
            try
            {
                data = JsonConvert.DeserializeObject<T>(baseReponse.RawBody);
            }
            catch (JsonReaderException) { }
            return new WebResponse<T>(baseReponse, data);
        }

        private void LogExternalServiceCall(string http_verb, string uri, string request_details, WebResponse response, DateTime request_start_date_utc, DateTime request_end_date_utc)
        {
            if(_logger != null)
                _logger.Log(http_verb, uri, string.IsNullOrWhiteSpace(request_details) ? string.Empty : request_details, response.StatusCode, response.Status, response.Headers.ToArray(), response.RawBody, request_start_date_utc, request_end_date_utc, "SYSTEM_Web");
        }
    }

    public interface IWebRequestLogger
    {
        void Log(string httpVerb, string uri, string details, int statusCode, string status, HttpHeader[] headers, string rawResponseBody, DateTime requestStartDateUtc, DateTime requestEndDateUtc, string createdBy);
    }
}
