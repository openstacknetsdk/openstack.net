namespace OpenStackNetTests.Unit.Simulator.IdentityService.V3
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Infrastructure;
    using OpenStack.Services.Identity;

    public class IdentityV3AuthenticationHandler : AuthenticationHandler<IdentityV3AuthenticationOptions>
    {
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            if (IdentityController.TokenId == null || IdentityController.TokenExpires < DateTimeOffset.Now)
                return Task.FromResult(default(AuthenticationTicket));

            string[] values;
            if (!Request.Headers.TryGetValue("X-Auth-Token", out values) || values.Length != 1)
                return Task.FromResult(default(AuthenticationTicket));

            TokenId tokenId = new TokenId(values[0].Trim());
            if (tokenId != IdentityController.TokenId)
                return Task.FromResult(default(AuthenticationTicket));

            return Task.FromResult(new AuthenticationTicket(new ClaimsIdentity("OpenStackIdentityV3"), new AuthenticationProperties()));
        }
    }
}
