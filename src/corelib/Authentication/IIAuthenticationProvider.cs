using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace OpenStack.Authentication
{
    /// <summary>
    ///this Authentication provider is used for Openstack Identity service 
    /// </summary>
    public interface IIAuthenticationProvider : IAuthenticationProvider
    {
        /// <summary>
        /// Get uuid for user that owns the token 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GetUserId(CancellationToken cancellationToken);

    }
}
