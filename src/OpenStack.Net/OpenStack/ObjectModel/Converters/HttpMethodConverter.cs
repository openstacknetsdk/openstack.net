namespace OpenStack.ObjectModel.Converters
{
    using System.Net.Http;
    using Newtonsoft.Json;

    /// <summary>
    /// This method provides a <see cref="JsonConverter"/> for deserializing
    /// <see cref="HttpMethod"/> instances.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class HttpMethodConverter : SimpleStringJsonConverter<HttpMethod>
    {
        /// <inheritdoc/>
        protected override HttpMethod ConvertToObject(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            switch (str.ToUpperInvariant())
            {
            case "DELETE":
                return HttpMethod.Delete;
            case "GET":
                return HttpMethod.Get;
            case "HEAD":
                return HttpMethod.Head;
            case "OPTIONS":
                return HttpMethod.Options;
            case "POST":
                return HttpMethod.Post;
            case "PUT":
                return HttpMethod.Put;
            case "TRACE":
                return HttpMethod.Trace;
            default:
                return new HttpMethod(str.ToUpperInvariant());
            }
        }
    }
}
