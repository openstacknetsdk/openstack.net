namespace OpenStackNetTests.Unit.Simulator.IdentityService
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Web.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Net;
    using OpenStack.Services.Identity;

    public class BaseIdentityController : ApiController
    {
        [AllowAnonymous]
        [ActionName("ListVersions")]
        public HttpResponseMessage GetAllVersions()
        {
            ValidateRequest(Request);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.MultipleChoices);
            response.Content = new StringContent(IdentityServiceResources.ListApiVersionsResponse, Encoding.UTF8, "application/json");
            return response;
        }

        [AllowAnonymous]
        [ActionName("GetVersion")]
        public HttpResponseMessage GetVersion([FromUri(Name = "versionId")] string versionIdString)
        {
            ValidateRequest(Request);

            ApiVersionId versionId = new ApiVersionId(versionIdString);

            JObject allVersions = JsonConvert.DeserializeObject<JObject>(IdentityServiceResources.ListApiVersionsResponse);
            ApiVersion[] versions = allVersions["versions"]["values"].ToObject<ApiVersion[]>();
            ApiVersion version = versions.FirstOrDefault(i => i.Id.Equals(versionId));

            if (version == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            else
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

                JObject responseObject = new JObject(new JProperty("version", JObject.FromObject(version)));
                response.Content = new StringContent(responseObject.ToString(Formatting.None), Encoding.UTF8, "application/json");

                return response;
            }
        }

        internal virtual void ValidateRequest(HttpRequestMessage request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Headers.Accept != null && request.Headers.Accept.Count > 0)
            {
                MediaTypeHeaderValue[] acceptTypes = request.Headers.Accept.ToArray();
                MediaTypeHeaderValue contentType = new MediaTypeHeaderValue("application/json");
                if (!acceptTypes.Any(acceptType => HttpApiCall.IsAcceptable(acceptType, contentType)))
                    throw new NotSupportedException();
            }
        }
    }
}
