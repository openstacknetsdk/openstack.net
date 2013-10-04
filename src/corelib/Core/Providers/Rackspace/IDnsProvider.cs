using System.Collections.Generic;
using net.openstack.Core.Domain;
using net.openstack.Core.Domain.Rackspace;

namespace net.openstack.Core.Providers.Rackspace
{
    /// <summary>
    /// Provides simple access to the Rackspace Cloud DNS Services.
    /// </summary>
    public interface IDnsProvider
    {
        #region Domains

        /// <summary>
        /// List all domains manageable by the account specified. Display IDs and names only.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The limit offset.</param>
        /// <param name="name">Specifies the name for the domain or subdomain.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>IEnumerable of <see cref="Domain"/></returns>
        IEnumerable<Domain.Rackspace.Domain> ListDomains(int? limit = null, int? offset = null, string name = null, CloudIdentity identity = null);

        /// <summary>
        /// This call provides the detailed output for a specific domain configured and associated with an account. This call is not capable of returning details for a domain that has been deleted.
        /// </summary>
        /// <param name="domainId">The Domain Id.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The limit offset.</param>
        /// <param name="showRecords">If this parameter is set to true, then information about records is returned; if this parameter is set to false, then information about records is not returned.</param>
        /// <param name="showSubdomains">If this parameter is set to true, then information about subdomains is returned; if this parameter is set to false, then information about subdomains is not returned.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="Domain"/></returns>
        Domain.Rackspace.Domain ListDomainDetails(int domainId, int? limit = null, int? offset = null, bool showRecords = true, bool showSubdomains = true, CloudIdentity identity = null);

        /// <summary>
        /// Creates new domains with the configuration defined by the request.
        /// </summary>
        /// <param name="domains">IEnumerable of <see cref="Domain"/> to create.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        void CreateDomains(IEnumerable<Domain.Rackspace.Domain> domains, CloudIdentity identity = null);

        /// <summary>
        /// This call modifies DNS domain(s) attributes only. Records cannot be added, modified, or removed. Only the TTL, email address and comment attributes of a domain can be modified.
        /// </summary>
        /// <param name="domains">IEnumerable of <see cref="Domain"/> to modify.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        void ModifyDomains(IEnumerable<Domain.Rackspace.Domain> domains, CloudIdentity identity = null);

        /// <summary>
        /// This call removes one or more specified domains from the account; when a domain is deleted, its immediate resource records are also deleted from the account.
        /// </summary>
        /// <param name="domainIds">DomainIds to remove.</param>
        /// <param name="deleteSubdomains">If false and a deleted domain had subdomains, each subdomain becomes a root domain and is not deleted. If true subdomains for a domain are also deleted.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        void RemoveDomains(IEnumerable<int> domainIds, bool deleteSubdomains = false, CloudIdentity identity = null);

        #endregion

        #region Subdomains

        /// <summary>
        /// List domains that are subdomains of the specified domain.
        /// </summary>
        /// <param name="domainId">The Domain Id.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The limit offset.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>IEnumerable of <see cref="Domain"/></returns>
        IEnumerable<Domain.Rackspace.Domain> ListSubdomains(int domainId, int? limit = null, int? offset = null, CloudIdentity identity = null);

        #endregion

        #region Records

        /// <summary>
        /// This call lists all records configured for the specified domain.
        /// </summary>
        /// <param name="domainId">The Domain Id.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The limit offset.</param>
        /// <param name="type">Specifies the type of record to search for.</param>
        /// <param name="name">Specifies the name of record to search for.</param>
        /// <param name="data">Specifies the data of record to search for.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>IEnumerable of <see cref="Record"/></returns>
        IEnumerable<Record> ListRecords(int domainId, int? limit = null, int? offset = null, string type = null, string name = null, string data = null, CloudIdentity identity = null);

        /// <summary>
        /// Add the configuration of records in the domain.
        /// </summary>
        /// <param name="domainId">The Domain Id.</param>
        /// <param name="records">IEnumerable of <see cref="Domain"/> to add.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        void AddRecords(int domainId, IEnumerable<Record> records, CloudIdentity identity = null);

        /// <summary>
        /// Modify the configuration of records in the domain.
        /// </summary>
        /// <param name="domainId">The Domain Id.</param>
        /// <param name="records">IEnumerable of <see cref="Domain"/> to modify.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        void ModifyRecords(int domainId, IEnumerable<Record> records, CloudIdentity identity = null);

        /// <summary>
        /// Remove the configuration of records in the domain.
        /// </summary>
        /// <param name="domainId">The Domain Id.</param>
        /// <param name="recordIds">RecordIds to remove.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        void RemoveRecords(int domainId, IEnumerable<string> recordIds, CloudIdentity identity = null);

        #endregion
    }
}
