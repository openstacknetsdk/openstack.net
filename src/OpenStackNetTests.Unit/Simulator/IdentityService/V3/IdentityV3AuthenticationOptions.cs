namespace OpenStackNetTests.Unit.Simulator.IdentityService.V3
{
    using Microsoft.Owin.Security;

    public class IdentityV3AuthenticationOptions : AuthenticationOptions
    {
        public IdentityV3AuthenticationOptions()
            : base("OpenStackIdentityV3")
        {
        }
    }
}
