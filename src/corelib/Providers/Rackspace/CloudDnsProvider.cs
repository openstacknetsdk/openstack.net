using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Domain.Mapping;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Providers;
using net.openstack.Core.Validators;
using net.openstack.Providers.Rackspace.Exceptions;
using net.openstack.Providers.Rackspace.Objects;
using net.openstack.Providers.Rackspace.Objects.Mapping;
using net.openstack.Providers.Rackspace.Objects.Response;
using net.openstack.Providers.Rackspace.Validators;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// <para>Rackspace Cloud DNS is a Domain Name System (DNS) available to Rackspace Cloud customers.</para>
    /// <para>Documentation URL: http://docs.rackspace.com/cdns/api/v1.0/cdns-devguide/content/overview.html</para>
    /// </summary>
    /// <see cref="IDnsProvider"/>
    /// <inheritdoc />
    public class CloudDnsProvider : ProviderBase<IDnsProvider>, IDnsProvider
    {
        private readonly IDnsValidator _cloudDnsValidator;

        #region Constructors

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudDnsProvider"/> class.
        /// </summary>
        public CloudDnsProvider()
            : this(null, null, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudDnsProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        public CloudDnsProvider(CloudIdentity identity)
            : this(identity, null, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudDnsProvider"/> class.
        /// </summary>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudDnsProvider(IRestService restService)
            : this(null, null, restService) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudDnsProvider"/> class.
        /// </summary>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        public CloudDnsProvider(IIdentityProvider identityProvider)
            : this(null, identityProvider, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudDnsProvider"/> class.
        /// </summary>
        /// /<param name="identity">An instance of a <see cref="Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        public CloudDnsProvider(CloudIdentity identity, IIdentityProvider identityProvider)
            : this(identity, identityProvider, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudDnsProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudDnsProvider(CloudIdentity identity, IRestService restService)
            : this(identity, null, restService) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudDnsProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudDnsProvider(CloudIdentity identity, IIdentityProvider identityProvider, IRestService restService)
            : this(identity, identityProvider, restService, CloudDnsValidator.Default) { }

        internal CloudDnsProvider(CloudIdentity identity, IIdentityProvider identityProvider, IRestService restService, IDnsValidator cloudDnsValidator)
            : base(identity, identityProvider, restService) 
        {
            _cloudDnsValidator = cloudDnsValidator;
        }

        #endregion

        #region Domains

        /// <inheritdoc />
        public IEnumerable<Core.Domain.Domain> ListDomains(int? limit = null, int? offset = null, string name = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/domains", GetServiceEndpointCloudDns(identity)));

            var queryStringParameter = new Dictionary<string, string>();

            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (offset != null)
                queryStringParameter.Add("offset", offset.ToString());

            if (!string.IsNullOrWhiteSpace(name))
                queryStringParameter.Add("name", name);

            var response = ExecuteRESTRequest<ListDomainsResponse>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Domains;
        }

        /// <inheritdoc />
        public Core.Domain.Domain ListDomainDetails(int domainId, int? limit = null, int? offset = null, bool showRecords = true, bool showSubdomains = true, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/domains/{1}", GetServiceEndpointCloudDns(identity), domainId));

            var queryStringParameter = new Dictionary<string, string>();
            
            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (offset != null)
                queryStringParameter.Add("offset", offset.ToString());

            if (showRecords)
                queryStringParameter.Add("showRecords", showRecords.ToString());

            if (showSubdomains)
                queryStringParameter.Add("showSubdomains", showRecords.ToString());

            var response = ExecuteRESTRequest<Core.Domain.Domain>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            if (response == null || response.Data == null)
                return null;

            return response.Data;
        }

        /// <inheritdoc />
        public void CreateDomains(IEnumerable<Core.Domain.Domain> domains, CloudIdentity identity = null)
        {
            foreach (var domain in domains)
            {
                _cloudDnsValidator.ValidateTTL(domain.Ttl);
                if (domain.RecordsList != null && domain.RecordsList.Records != null)
                {
                    foreach (var record in domain.RecordsList.Records)
                    {
                        _cloudDnsValidator.ValidateRecordType(record.Type);
                        _cloudDnsValidator.ValidateTTL(record.Ttl);
                    }
                }
            }
            var urlPath = new Uri(string.Format("{0}/domains", GetServiceEndpointCloudDns(identity)));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, new { domains = domains }, null);
        }

        /// <inheritdoc />
        public void ModifyDomains(IEnumerable<Core.Domain.Domain> domains, CloudIdentity identity = null)
        {
            foreach (var domain in domains)
                _cloudDnsValidator.ValidateTTL(domain.Ttl);
            var urlPath = new Uri(string.Format("{0}/domains", GetServiceEndpointCloudDns(identity)));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, new { domains = domains }, null);
        }

        /// <inheritdoc />
        public void RemoveDomains(IEnumerable<int> domainIds, bool deleteSubdomains = false, CloudIdentity identity = null)
        {
            if (domainIds == null || domainIds.Count() <= 0)
                throw new InvalidArgumentException("ERROR: At least 1 domainId is required.");
            var urlPath = new Uri(string.Format("{0}/domains/?id={1}", GetServiceEndpointCloudDns(identity), string.Join("&id=", domainIds)));

            var queryStringParameter = new Dictionary<string, string>();

            if (deleteSubdomains)
                queryStringParameter.Add("deleteSubdomains", deleteSubdomains.ToString());

            ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE, null, queryStringParameter, null, false, null);
        }

        #endregion

        #region Subdomains

        /// <inheritdoc />
        public IEnumerable<Core.Domain.Domain> ListSubdomains(int domainId, int? limit = null, int? offset = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/domains/{1}/subdomains", GetServiceEndpointCloudDns(identity), domainId));

            var queryStringParameter = new Dictionary<string, string>();

            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (offset != null)
                queryStringParameter.Add("offset", offset.ToString());
            
            var response = ExecuteRESTRequest<ListSubdomainsResponse>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Domains;
        }

        #endregion

        #region Records

        /// <inheritdoc />
        public IEnumerable<Record> ListRecords(int domainId, int? limit = null, int? offset = null, string type = null, string name = null, string data = null, CloudIdentity identity = null)
        {
            if (!string.IsNullOrEmpty(type))
                _cloudDnsValidator.ValidateRecordType(type);

            var urlPath = new Uri(string.Format("{0}/domains/{1}/records", GetServiceEndpointCloudDns(identity), domainId));

            var queryStringParameter = new Dictionary<string, string>();

            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (offset != null)
                queryStringParameter.Add("offset", offset.ToString());

            if (!string.IsNullOrEmpty(type))
                queryStringParameter.Add("type", type);

            if (!string.IsNullOrEmpty(name))
                queryStringParameter.Add("name", name);

            if (!string.IsNullOrEmpty(data))
                queryStringParameter.Add("data", data);

            var response = ExecuteRESTRequest<RecordsList>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Records;
        }

        /// <inheritdoc />
        public void AddRecords(int domainId, IEnumerable<Record> records, CloudIdentity identity = null)
        {
            foreach (var record in records)
            {
                _cloudDnsValidator.ValidateRecordType(record.Type);
                _cloudDnsValidator.ValidateTTL(record.Ttl);
            }
            var urlPath = new Uri(string.Format("{0}/domains/{1}/records", GetServiceEndpointCloudDns(identity), domainId));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, new { records = records }, null);
        }

        /// <inheritdoc />
        public void ModifyRecords(int domainId, IEnumerable<Record> records, CloudIdentity identity = null)
        {
            foreach (var record in records)
            {
                _cloudDnsValidator.ValidateRecordType(record.Type);
                _cloudDnsValidator.ValidateTTL(record.Ttl);
            }
            var urlPath = new Uri(string.Format("{0}/domains/{1}/records", GetServiceEndpointCloudDns(identity), domainId));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, new { records = records }, null);
        }

        /// <inheritdoc />
        public void RemoveRecords(int domainId, IEnumerable<string> recordIds, CloudIdentity identity = null)
        {
            if (recordIds == null || recordIds.Count() <= 0)
                throw new InvalidArgumentException("ERROR: At least 1 recordId is required.");
            var urlPath = new Uri(string.Format("{0}/domains/{1}/records?id={2}", GetServiceEndpointCloudDns(identity), domainId, string.Join("&id=", recordIds)));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);
        }

        #endregion

        #region Private methods

        /// <inheritdoc />
        protected string GetServiceEndpointCloudDns(CloudIdentity identity)
        {
            return base.GetPublicServiceEndpoint(identity, "cloudDNS", null);
        }

        /// <inheritdoc />
        protected override IDnsProvider BuildProvider(CloudIdentity identity)
        {
            return new CloudDnsProvider(identity, IdentityProvider, RestService);
        }

        #endregion
    }
}
