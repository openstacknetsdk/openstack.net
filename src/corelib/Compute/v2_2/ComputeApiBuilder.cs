using OpenStack.Authentication;

namespace OpenStack.Compute.v2_2
{
    /// <inheritdoc />
    /// <seealso href="https://github.com/openstack/nova/blob/master/nova/api/openstack/rest_api_version_history.rst#22">Compute Microversion 2.6</seealso>
    public class ComputeApiBuilder : v2_1.ComputeApiBuilder
    {
        /// <inheritdoc />
        public ComputeApiBuilder(IServiceType serviceType, IAuthenticationProvider authenticationProvider, string region)
            : base(serviceType, authenticationProvider, region, "2.2")
        { }
    }
}
