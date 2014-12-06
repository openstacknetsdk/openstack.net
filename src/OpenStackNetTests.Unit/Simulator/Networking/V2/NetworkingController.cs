namespace OpenStackNetTests.Unit.Simulator.Networking.V2
{
    using System.Net.Http;
    using System.Web.Http;
    using OpenStackNetTests.Unit.Simulator.IdentityService.V2;

    public class NetworkingController : ApiController
    {
        private IdentityController _identityService = new IdentityController();

        private void ValidateRequest(HttpRequestMessage request)
        {
            _identityService.ValidateRequest(request);
        }
    }
}
