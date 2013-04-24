using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Exceptions.Response;

namespace net.openstack.Core
{
    /// <summary>
    /// Cloud Monitoring analyzes cloud services and dedicated infrastructure using a simple, yet powerful API. The API currently includes monitoring for external services
    /// </summary>
    public interface ICloudMonitoringProvider
    {
    #region Account
        #region Get Account

        #endregion

        #region Update Account

        #endregion

        #region Get Limits
        /// <summary>
        /// Returns account resource and rate limits.
        /// 
        /// Documentation URL: http://docs.rackspace.com/cm/api/v1.0/cm-devguide/content/service-account.html#d444e1296
        /// </summary>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <exception cref="UserAuthenticationException"></exception>
        /// <exception cref="UserNotAuthorizedException"></exception>
        /// Normal Response Code: 200
        /// 401 Unauthorized UserAuthenticationException
        /// 403 Forbidden UserAuthorizationException
        /// 500 Internal Server Error, 503 Service Unavailable
        /// <returns><see cref="Limit"></see></returns>
        Limit GetAccountLimits(string region = null, CloudIdentity identity = null);
        #endregion

        #region List Audits

        #endregion

    #endregion
    }
}
