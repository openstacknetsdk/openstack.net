using System.Collections.Generic;
using System.Net;

namespace net.openstack.corelib.Web
{
    public class WebResponse
    {
        public int StatusCode { get; private set; }

        public string Status { get; private set; }

        public IList<HttpHeader> Headers { get; private set; }

        public string RawBody { get; private set; }

        public WebResponse(int responseCode, string status, IList<HttpHeader> headers, string rawBody)
        {
            StatusCode = responseCode;
            Status = status;
            Headers = headers;
            RawBody = rawBody;
        }

        public WebResponse(HttpStatusCode statusCode, IList<HttpHeader> headers, string rawBody)
            : this((int)statusCode, statusCode.ToString(), headers, rawBody)
        {
        }
    }

    public class WebResponse<T> : WebResponse
    {
        public T Data { get; private set; }

        public WebResponse(int responseCode, string status, T data, IList<HttpHeader> headers, string rawBody)
            : base(responseCode, status, headers, rawBody)
        {
            Data = data;
        }

        public WebResponse(HttpStatusCode statusCode, T data, IList<HttpHeader> headers, string rawBody)
            : this((int)statusCode, statusCode.ToString(), data, headers, rawBody)
        {
        }

        public WebResponse(WebResponse baseResponse, T data)
            : this((baseResponse == null) ? default(int) : baseResponse.StatusCode,
                (baseResponse == null) ? null : baseResponse.Status, data,
                (baseResponse == null) ? null : baseResponse.Headers,
                (baseResponse == null) ? null : baseResponse.RawBody) { }
    }

    public class HttpHeader
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }  
}
