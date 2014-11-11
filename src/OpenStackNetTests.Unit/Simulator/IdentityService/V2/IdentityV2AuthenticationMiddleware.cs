namespace OpenStackNetTests.Unit.Simulator.IdentityService.V2
{
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Infrastructure;
    using Owin;

    public class IdentityV2AuthenticationMiddleware : AuthenticationMiddleware<IdentityV2AuthenticationOptions>
    {
        public IdentityV2AuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, IdentityV2AuthenticationOptions options)
            : base(next, options)
        {
        }

        protected override AuthenticationHandler<IdentityV2AuthenticationOptions> CreateHandler()
        {
            return new IdentityV2AuthenticationHandler();
        }
    }
}
