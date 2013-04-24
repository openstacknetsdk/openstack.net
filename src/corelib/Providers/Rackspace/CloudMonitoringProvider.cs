using System;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// Rackspace Cloud Monitoring analyzes cloud services and dedicated infrastructure using a simple, yet powerful API. 
    /// The API currently includes monitoring for external services. 
    ///
    /// The key benefits you receive from using this API include the following:
    /// Use of Domain Specific Language (DSL)
    /// - The Rackspace Cloud Monitoring API uses a DSL, which makes it a powerful tool for configuring advanced monitoring features. 
    ///   For example, typically complex tasks, such as defining triggers on thresholds for metrics or performing an inverse string match 
    ///   become much easier with a concise, special purpose language created for defining alarms. 
    /// 
    /// Monitoring from Multiple Datacenters
    /// - Rackspace Cloud Monitoring allows you to simultaneously monitor the performance of different resources from multiple datacenters 
    ///   and provides a clear picture of overall system health. It includes tunable parameters to interpret mixed results which help you 
    ///   to create deliberate and accurate alerting policies.
    /// 
    /// Alarms and Notifications
    /// - When an alarm occurs on a monitored resource, Rackspace Cloud Monitoring sends you a notification so that you can take the 
    ///   appropriate action to either prevent an adverse situation from occurring or rectify a situation that has already occurred. 
    ///   These notifications are sent based on the severity of the alert as defined in the notification plan.
    /// 
    /// Documentation URL: http://docs.rackspace.com/cm/api/v1.0/cm-devguide/content/overview.html
    /// </summary>
    public class CloudMonitoringProvider : ProviderBase, ICloudMonitoringProvider
    {

        private readonly int[] _validResponseCode = new[] { 200 };

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudMonitoringProvider"/> class.
        /// </summary>
        public CloudMonitoringProvider() : this(null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudMonitoringProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object.<remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        public CloudMonitoringProvider(CloudIdentity identity) : this(identity, new CloudIdentityProvider(), new JsonRestServices()) { }

        internal CloudMonitoringProvider(ICloudIdentityProvider identityProvider, IRestService restService)
            : this(null, identityProvider, restService) { }

        internal CloudMonitoringProvider(CloudIdentity identity, ICloudIdentityProvider identityProvider, IRestService restService)
            : base(identity, identityProvider, restService){ }

    #region Account

        #region Get Account

        #endregion
    
        #region Update Account

        #endregion

        #region Get Limits

        #endregion
        
        #region List Audits
        public Audits ListAudits(string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/audits", GetServiceEndpoint(identity, region)));
            var response = ExecuteRESTRequest<Audits>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data;
        }
        #endregion

    #endregion

        #region Private methods

        protected string GetServiceEndpoint(CloudIdentity identity = null, string region = null)
        {
            return base.GetPublicServiceEndpoint(identity, "cloudMonitoring", region);
        }

        #endregion

    }
}
