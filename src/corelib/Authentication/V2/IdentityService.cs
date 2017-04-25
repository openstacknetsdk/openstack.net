using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenStack.Authentication.v3
{
    class IdentityService : IAuthenticationProvider
    {
        readonly private string UserId;
        readonly private string Password;

        public IdentityService(string userId, string password){
            this.UserId = userId;
            this.Password = password;
        }

        public Task<string> GetEndpoint(IServiceType serviceType, string region, bool useInternalUrl, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetToken(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
