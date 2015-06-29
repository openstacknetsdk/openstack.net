using System.Threading;
using System.Threading.Tasks;

namespace OpenStack.Authentication
{
    /// <summary>
    /// TODO: doc
    /// </summary>
    public interface IAuthenticationProvider
    {
        Task<string> GetEndpoint(ServiceType serviceType, string region, bool useInternalUrl, CancellationToken cancellationToken);
        Task<string> GetToken(CancellationToken cancellationToken);
    }
}
