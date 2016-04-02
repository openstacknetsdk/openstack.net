using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace OpenStack.Authentication.V3.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class LazyHttpClient
    {
        private Uri Url;
        private HttpWebRequest HttpWebRequest;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public LazyHttpClient(Uri url)
        {
            Url = url;
            HttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public HttpWebResponse LazyJsonPost(object obj)
        {
            HttpWebRequest.Method = "POST";
            HttpWebRequest.ContentType = "application/json";
            HttpWebRequest.Accept = "application/json";
            var jsonSerializerSetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            string content = JsonConvert.SerializeObject(obj, jsonSerializerSetting);
            var data = Encoding.ASCII.GetBytes(content);
            HttpWebRequest.ContentLength = data.Length;
            using (var stream = HttpWebRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var resp =  (HttpWebResponse)HttpWebRequest.GetResponse();
            return resp;
        }
    }
}
/// <summary>
/// 
/// </summary>
public static class HttpWebResponseExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpWebReponse"></param>
    /// <returns></returns>
    public static string GetContent(this HttpWebResponse httpWebReponse)
    {
            return new StreamReader(httpWebReponse.GetResponseStream()).ReadToEnd();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httWebResponse"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetHeaderValue(this HttpWebResponse httWebResponse, string key)
    {
        return httWebResponse.Headers.Get(key);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpWebReponse"></param>
    /// <returns></returns>
    public static T ConvertJsonContentToObject<T>(this HttpWebResponse httpWebReponse)
        where T : class
    {
        var content = httpWebReponse.GetContent();
        return JsonConvert.DeserializeObject<T>(content);
    }
}
