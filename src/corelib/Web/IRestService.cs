using System;
using System.Collections.Generic;

namespace net.openstack.corelib.Web
{
    public interface IRestService
    {
        WebResponse<T> Execute<T, TBody>(String url, HttpMethod method, TBody body, Dictionary<string, string> headers = null, string contentType = "application/json", int retryCount = 0, int delayInMilliseconds = 0, IEnumerable<int> non200SuccessCodes = null);
        WebResponse<T> Execute<T, TBody>(Uri url, HttpMethod method, TBody body, Dictionary<string, string> headers = null, string contentType = "application/json", int retryCount = 0, int delayInMilliseconds = 0, IEnumerable<int> non200SuccessCodes = null);
        WebResponse<T> Execute<T>(String url, HttpMethod method, string body = null, Dictionary<string, string> headers = null, string contentType = "application/json", int retryCount = 0, int delayInMilliseconds = 0, IEnumerable<int> non200SuccessCodes = null);
        WebResponse<T> Execute<T>(Uri url, HttpMethod method, string body = null, Dictionary<string, string> headers = null, string contentType = "application/json", int retryCount = 0, int delayInMilliseconds = 0, IEnumerable<int> non200SuccessCodes = null);
        WebResponse Execute<TBody>(String url, HttpMethod method, TBody body, Dictionary<string, string> headers = null, string contentType = "application/json", int retryCount = 0, int delayInMilliseconds = 0, IEnumerable<int> non200SuccessCodes = null);
        WebResponse Execute<TBody>(Uri url, HttpMethod method, TBody body, Dictionary<string, string> headers = null, string contentType = "application/json", int retryCount = 0, int delayInMilliseconds = 0, IEnumerable<int> non200SuccessCodes = null);
        WebResponse Execute(String url, HttpMethod method, string body = null, Dictionary<string, string> headers = null, string contentType = "application/json", int retryCount = 0, int delayInMilliseconds = 0, IEnumerable<int> non200SuccessCodes = null);
        WebResponse Execute(Uri url, HttpMethod method, string body = null, Dictionary<string, string> headers = null, string contentType = "application/json", int retryCount = 0, int delayInMilliseconds = 0, IEnumerable<int> non200SuccessCodes = null);
    }
}