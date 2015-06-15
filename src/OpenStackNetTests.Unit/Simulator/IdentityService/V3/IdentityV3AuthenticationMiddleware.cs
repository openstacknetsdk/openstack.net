namespace OpenStackNetTests.Unit.Simulator.IdentityService.V3
{
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Infrastructure;
    using Owin;

    public class IdentityV3AuthenticationMiddleware : AuthenticationMiddleware<IdentityV3AuthenticationOptions>
    {
        public IdentityV3AuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, IdentityV3AuthenticationOptions options)
            : base(next, options)
        {
        }

        protected override AuthenticationHandler<IdentityV3AuthenticationOptions> CreateHandler()
        {
            return new IdentityV3AuthenticationHandler();
        }
    }
}
