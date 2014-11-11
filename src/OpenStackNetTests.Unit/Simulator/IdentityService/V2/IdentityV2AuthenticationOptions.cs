namespace OpenStackNetTests.Unit.Simulator.IdentityService.V2
{
    using Microsoft.Owin.Security;

    public class IdentityV2AuthenticationOptions : AuthenticationOptions
    {
        public IdentityV2AuthenticationOptions()
            : base("OpenStackIdentityV2")
        {
        }
    }
}
