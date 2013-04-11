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
using net.openstack.Core.Exceptions;
using net.openstack.Providers.Rackspace.Exceptions;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// The Cloud Files Provider contains methods required to interact with Cloud Files.
    /// For generic information: http://docs.rackspace.com/ check Cloud Files section.
    /// </summary>
    public class CloudFilesProvider : ProviderBase, ICloudFilesProvider
    {
        private readonly ICloudFilesValidator _cloudFilesValidator;
        private readonly ICloudFilesMetadataProcessor _cloudFilesMetadataProcessor;

        #region Constructors

        public CloudFilesProvider()
            : this(null) { }

        public CloudFilesProvider(CloudIdentity defaultIdentity)
            : this(defaultIdentity, new CloudIdentityProvider(), new JsonRestServices(), new CloudFilesValidator(), new CloudFilesMetadataProcessor()) { }

        public CloudFilesProvider(ICloudIdentityProvider cloudIdentityProvider, IRestService restService, ICloudFilesValidator cloudFilesValidator, ICloudFilesMetadataProcessor cloudFilesMetadataProcessor)
            : this(null, cloudIdentityProvider, restService, cloudFilesValidator, cloudFilesMetadataProcessor) { }

        public CloudFilesProvider(CloudIdentity defaultIdentity, ICloudIdentityProvider cloudIdentityProvider, IRestService restService, ICloudFilesValidator cloudFilesValidator, ICloudFilesMetadataProcessor cloudFilesMetadataProcessor)
            : base(defaultIdentity, cloudIdentityProvider, restService)
        {
            _cloudFilesValidator = cloudFilesValidator;
            _cloudFilesMetadataProcessor = cloudFilesMetadataProcessor;
        }


        #endregion

        #region Containers

        /// <summary>
        /// Lists the containers.
        /// </summary>
        /// <param name="limit">The limit.<remarks>[Optional]</remarks></param>
        /// <param name="marker">The marker.<remarks>[Optional]</remarks></param>
        /// <param name="markerEnd">The marker end.<remarks>[Optional]</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        ///<param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns>IEnumerable of <see cref="net.openstack.Core.Domain.Container"/></returns>
        public IEnumerable<Container> ListContainers(int? limit = null, string marker = null, string markerEnd = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));

            var queryStringParameter = new Dictionary<string, string>();

            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (!string.IsNullOrWhiteSpace(marker))
                queryStringParameter.Add("marker", marker);

            if (!string.IsNullOrWhiteSpace(markerEnd))
                queryStringParameter.Add("end_marker", markerEnd);

            var response = ExecuteRESTRequest<Container[]>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            if (response == null || response.Data == null)
                return null;

            return response.Data;

        }

        /// <summary>
        /// Creates the container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        ///<param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        public ObjectStore CreateContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT);

            if (response.StatusCode == 201)
                return ObjectStore.ContainerCreated;
            if (response.StatusCode == 202)
                return ObjectStore.ContainerExists;

            return ObjectStore.Unknown;
        }

        /// <summary>
        /// Deletes the container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        ///<param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        public ObjectStore DeleteContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            if (response.StatusCode == 204)
                return ObjectStore.ContainerDeleted;
            if (response.StatusCode == 404)
                return ObjectStore.ContainerNotFound;
            if (response.StatusCode == 409)
                return ObjectStore.ContainerNotEmpty;

            return ObjectStore.Unknown;
        }

        /// <summary>
        /// Gets the container header.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt;</returns>
        public Dictionary<string, string> GetContainerHeader(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersHeaderKey];
        }

        /// <summary>
        /// Gets the container meta data.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt;</returns>
        public Dictionary<string, string> GetContainerMetaData(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.GET); // Should be HEAD

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersMetadataKey];
        }

        /// <summary>
        /// Gets the container CDN header.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns><see cref="ContainerCDN"/></returns>
        public ContainerCDN GetContainerCDNHeader(string container, string region = null, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var result = new ContainerCDN { Name = container };

            foreach (var header in response.Headers)
            {
                if (header.Key.ToLower().Equals(CdnUri))
                {
                    result.CDNUri = header.Value;
                }
                if (header.Key.ToLower().Equals(CdnSslUri))
                {
                    result.CDNSslUri = header.Value;
                }
                if (header.Key.ToLower().Equals(CdnStreamingUri))
                {
                    result.CDNStreamingUri = header.Value;
                }
                if (header.Key.ToLower().Equals(CdnTTL))
                {
                    result.Ttl = long.Parse(header.Value);
                }
                if (header.Key.ToLower().Equals(CdnEnabled))
                {
                    result.CDNEnabled = bool.Parse(header.Value);
                }
                if (header.Key.ToLower().Equals(CdnLogRetention))
                {
                    result.LogRetention = bool.Parse(header.Value);
                }
                if (header.Key.ToLower().Equals(CdnIosUri))
                {
                    result.CDNIosUri = header.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the container metadata.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="metadata">The metadata. <remarks>Dictionary&lt;string,string&gt;</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void UpdateContainerMetadata(string container, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            if (metadata.Equals(null))
            {
                throw new ArgumentNullException();
            }

            var headers = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> m in metadata)
            {
                if (m.Key.Contains(ContainerMetaDataPrefix))
                {
                    headers.Add(m.Key, m.Value);
                }
                else
                {
                    headers.Add(ContainerMetaDataPrefix + m.Key, m.Value);
                }
            }

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        /// <summary>
        /// Adds the container headers.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="headers">The headers.<remarks>Dictionary&lt;string,string&gt;</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void AddContainerHeaders(string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            if (headers == null)
            {
                throw new ArgumentNullException();
            }

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        /// <summary>
        /// Updates the container CDN headers.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="headers">The headers.<remarks>Dictionary&lt;string,string&gt;</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="CDNNotEnabledException"></exception>
        public void UpdateContainerCdnHeaders(string container, Dictionary<string, string> headers, string region = null, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            if (headers == null)
            {
                throw new ArgumentNullException();
            }

            if (!GetContainerCDNHeader(container, region, identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));
            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        /// <summary>
        /// Lists the CDN containers.
        /// </summary>
        /// <param name="limit">The limit.<remarks>[Optional]</remarks></param>
        /// <param name="marker">The marker.<remarks>[Optional]</remarks></param>
        /// <param name="markerEnd">The marker end.<remarks>[Optional]</remarks></param>
        /// <param name="cdnEnabled">if set to <c>true</c> lists CDN enabled containers.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns>IEnumerable of <see cref="net.openstack.Core.Domain.ContainerCDN"/></returns>
        public IEnumerable<ContainerCDN> ListCDNContainers(int? limit = null, string marker = null, string markerEnd = null, bool cdnEnabled = false, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFilesCDN(identity, region)));

            var queryStringParameter = new Dictionary<string, string>
                {
                    {"format", "json"},
                    {"enabled_only", cdnEnabled.ToString().ToLower()}
                };

            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (!string.IsNullOrWhiteSpace(marker))
                queryStringParameter.Add("marker", marker);

            if (!string.IsNullOrWhiteSpace(markerEnd))
                queryStringParameter.Add("end_marker", markerEnd);

            var response = ExecuteRESTRequest<ContainerCDN[]>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            if (response == null || response.Data == null)
                return null;

            return response.Data;
        }

        /// <summary>
        /// Enables the CDN on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="ttl">The TTL in seconds.<c>[Range 900 to 1577836800]</c></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="TTLLengthException">TTL range must be 900 to 1577836800 seconds TTL:  + ttl.ToString(CultureInfo.InvariantCulture)</exception>
        /// <returns>Dictionary&lt;string,string&gt; of CDN Headers</returns>
        public Dictionary<string, string> EnableCDNOnContainer(string container, long ttl, string region = null, CloudIdentity identity = null)
        {
            return EnableCDNOnContainer(container, ttl, false, identity: identity);
        }

        /// <summary>
        /// Enables the CDN on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="logRetention">if set to <c>true</c> enables log retention on container.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <returns>Dictionary&lt;string,string&gt; of CDN Headers</returns>
        public Dictionary<string, string> EnableCDNOnContainer(string container, bool logRetention, string region = null, CloudIdentity identity = null)
        {
            return EnableCDNOnContainer(container, 259200, logRetention, region, identity);
        }

        /// <summary>
        /// Enables the CDN on container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="ttl">The TTL in seconds.<c>[Range 900 to 1577836800]</c></param>
        /// <param name="logRetention">if set to <c>true</c> enables log retention on container.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of CDN Headers</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="TTLLengthException">TTL range must be 900 to 1577836800 seconds TTL:  + ttl.ToString(CultureInfo.InvariantCulture)</exception>
        public Dictionary<string, string> EnableCDNOnContainer(string container, long ttl, bool logRetention, string region = null, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            if (ttl.Equals(null) || logRetention.Equals(null))
            {
                throw new ArgumentNullException();
            }

            if (ttl > 1577836800 || ttl < 900)
            {
                throw new TTLLengthException("TTL range must be 900 to 1577836800 seconds TTL: " + ttl.ToString(CultureInfo.InvariantCulture));
            }

            var headers = new Dictionary<string, string>
                {
                 {CdnTTL, ttl.ToString(CultureInfo.InvariantCulture)},
                 {CdnLogRetention, logRetention.ToString(CultureInfo.InvariantCulture)},
                 {CdnEnabled, "true"}
                };
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, headers: headers);

            if (response == null)
                return null;

            return response.Headers.ToDictionary(header => header.Key, header => header.Value);
        }

        /// <summary>
        /// Disables the CDN on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region. <remarks>[Nullable]</remarks></param>
        /// <param name="identity">The identity <remarks>[Nullable]</remarks></param>
        /// <returns>Dictionary&lt;string,string&gt; of CDN Headers</returns>
        public Dictionary<string, string> DisableCDNOnContainer(string container, string region = null, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);

            var headers = new Dictionary<string, string>
                {
                {CdnEnabled, "false"}
                };
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, headers: headers);

            if (response == null)
                return null;

            return response.Headers.ToDictionary(header => header.Key, header => header.Value);
        }

        /// <summary>
        /// Enables the static web on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="index">Value for <c>x-container-meta-web-index</c>.</param>
        /// <param name="error">Value for <c>x-container-meta-web-error</c>.</param>
        /// <param name="css">Value for <c>x-container-meta-web-listings-css</c>.</param>
        /// <param name="listing">Value for <c>x-container-meta-web-listings</c></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="CDNNotEnabledException"></exception>
        public void EnableStaticWebOnContainer(string container, string index, string error, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);

            if (!GetContainerCDNHeader(container, region, identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }

            var headers = new Dictionary<string, string>
                                {
                                    {WebIndex, index},
                                    {WebError, error},
                                    {WebListingsCSS, css},
                                    {WebListings, listing.ToString()}
                                };
            AddContainerHeaders(container, headers, region, useInternalUrl, identity);

        }

        /// <summary>
        /// Enables the static web on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="index">Value for <c>x-container-meta-web-index</c>.</param>
        /// <param name="error">Value for <c>x-container-meta-web-error</c>.</param>
        /// <param name="listing">Value for <c>x-container-meta-web-listings</c></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="CDNNotEnabledException"></exception>
        public void EnableStaticWebOnContainer(string container, string index, string error, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);

            if (!GetContainerCDNHeader(container, region, identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }

            var headers = new Dictionary<string, string>
                                  {
                                      {WebIndex, index},
                                      {WebError, error},
                                      {WebListings, listing.ToString()}
                                  };
            AddContainerHeaders(container, headers, region, useInternalUrl, identity);
        }

        /// <summary>
        /// Enables the static web on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="css">Value for <c>x-container-meta-web-listings-css</c>.</param>
        /// <param name="listing">Value for <c>x-container-meta-web-listings</c></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="CDNNotEnabledException"></exception>
        public void EnableStaticWebOnContainer(string container, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);

            if (!GetContainerCDNHeader(container, region, identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }

            var headers = new Dictionary<string, string>
                                {
                                    {WebListingsCSS, css},
                                    {WebListings, listing.ToString()}
                                };
            AddContainerHeaders(container, headers, region, useInternalUrl, identity);
        }

        /// <summary>
        /// Enables the static web on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="index">Value for <c>x-container-meta-web-index</c>.</param>
        /// <param name="error">Value for <c>x-container-meta-web-error</c>.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="CDNNotEnabledException"></exception>
        public void EnableStaticWebOnContainer(string container, string index, string error, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);

            if (!GetContainerCDNHeader(container, region, identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }

            var headers = new Dictionary<string, string>
                                  {
                                      {WebIndex, index},
                                      {WebError, error}
                                  };
            AddContainerHeaders(container, headers, region, useInternalUrl, identity);
        }

        /// <summary>
        /// Disables the static web on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="CDNNotEnabledException"></exception>
        public void DisableStaticWebOnContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);

            if (!GetContainerCDNHeader(container, region, identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }

            var headers = new Dictionary<string, string>
                                {
                                    {WebIndex, string.Empty},
                                    {WebError, string.Empty},
                                    {WebListingsCSS, string.Empty},
                                    {WebListings, string.Empty}
                                };
            AddContainerHeaders(container, headers, region, useInternalUrl, identity);
        }

        #endregion

        #region Container Objects

        /// <summary>
        /// Lists the objects.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="limit">The limit.<remarks>[Optional]</remarks></param>
        /// <param name="marker">The marker.<remarks>[Optional]</remarks></param>
        /// <param name="markerEnd">The marker end.<remarks>[Optional]</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns>IEnumerable of <see cref="net.openstack.Core.Domain.ContainerObject"/></returns>
        public IEnumerable<ContainerObject> ListObjects(string container, int? limit = null, string marker = null, string markerEnd = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container));

            var queryStringParameter = new Dictionary<string, string>();

            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (!string.IsNullOrWhiteSpace(marker))
                queryStringParameter.Add("marker", marker);

            if (!string.IsNullOrWhiteSpace(markerEnd))
                queryStringParameter.Add("end_marker", markerEnd);

            var response = ExecuteRESTRequest<ContainerObject[]>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            if (response == null || response.Data == null)
                return null;

            return response.Data;
        }

        /// <summary>
        /// Gets the object headers.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of CDN Headers</returns>
        public Dictionary<string, string> GetObjectHeaders(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container, objectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersHeaderKey];
        }

        /// <summary>
        /// Creates the object from file.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="filePath">The file path.<remarks>Example c:\folder1\folder2\image_name.jpeg</remarks></param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        public void CreateObjectFromFile(string container, string filePath, string objectName, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null,bool useInternalUrl = false, CloudIdentity identity = null)
        {
            using (var stream = File.OpenRead(filePath))
            {
                CreateObject(container, stream, objectName, chunkSize, headers, region, progressUpdated,useInternalUrl, identity);
            }
        }

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="stream">The stream. <see cref="Stream"/></param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        public void CreateObject(string container, Stream stream, string objectName, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (stream == null)
                throw new ArgumentNullException();

            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);

            if (stream.Length > LargeFileBatchThreshold)
            {
                CreateObjectInSegments(container, stream, objectName, chunkSize, headers, region, progressUpdated, useInternalUrl, identity);
                return;
            }
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container, objectName));

            StreamRESTRequest(identity, urlPath, HttpMethod.PUT, stream, chunkSize, headers: headers, isRetry: true, progressUpdated: progressUpdated, requestSettings: new RequestSettings());
        }

        /// <summary>
        /// Creates the object in segments.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="stream">The stream. <see cref="Stream"/></param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        private void CreateObjectInSegments(string container, Stream stream, string objectName, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var totalLength = stream.Length;
            var segmentCount = Math.Ceiling((double)totalLength / (double)LargeFileBatchThreshold);

            long totalBytesWritten = 0;
            for (int i = 0; i < segmentCount; i++)
            {
                var remaining = (totalLength - LargeFileBatchThreshold * i);
                var length = (remaining < LargeFileBatchThreshold) ? remaining : LargeFileBatchThreshold;

                var urlPath = new Uri(string.Format("{0}/{1}/{2}.seg{3}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container, objectName, i));
                long segmentBytesWritten = 0;
                StreamRESTRequest(identity, urlPath, HttpMethod.PUT, stream, chunkSize, length, headers: headers, isRetry: true, requestSettings: new RequestSettings(), progressUpdated:
                    bytesWritten =>
                    {
                        if (progressUpdated != null)
                        {
                            segmentBytesWritten = bytesWritten;
                            progressUpdated(totalBytesWritten + segmentBytesWritten);
                        }
                    });

                totalBytesWritten += segmentBytesWritten;
            }

            // upload the manifest file
            var segmentUrlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container, objectName));

            if (headers == null)
                headers = new Dictionary<string, string>();

            headers.Add(ObjectManifestMetadataKey, string.Format("{0}/{1}", container, objectName));
            StreamRESTRequest(identity, segmentUrlPath, HttpMethod.PUT, new MemoryStream(new Byte[0]), chunkSize, headers: headers, isRetry: true, requestSettings: new RequestSettings(), progressUpdated:
                (bytesWritten) =>
                {
                    if (progressUpdated != null)
                    {
                        progressUpdated(totalBytesWritten);
                    }
                });
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="outputStream">The output stream.<see cref="Stream"/></param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="verifyEtag">if set to <c>true</c> will verify etag.</param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        ///<param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="InvalidETagException"></exception>
        public void GetObject(string container, string objectName, Stream outputStream, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container, objectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.GET, (resp, isError) =>
            {
                if (resp == null)
                    return new Response(0, null, null);

                try
                {
                    using (var respStream = resp.GetResponseStream())
                    {
                        CopyStream(respStream, outputStream, chunkSize, progressUpdated);
                    }

                    var respHeaders = resp.Headers.AllKeys.Select(key => new HttpHeader { Key = key, Value = resp.GetResponseHeader(key) }).ToList();

                    return new Response(resp.StatusCode, respHeaders, "[Binary]");
                }
                catch (Exception)
                {
                    return new Response(0, null, null);
                }
            }, headers: headers);

            if (verifyEtag && response.Headers.Any(h => h.Key.Equals(Etag, StringComparison.OrdinalIgnoreCase)))
            {
                outputStream.Flush(); // flush the contents of the stream to the output device
                outputStream.Position = 0;  // reset the head of the stream to the beginning

                var md5 = MD5.Create();
                md5.ComputeHash(outputStream);

                var sbuilder = new StringBuilder();
                var hash = md5.Hash;
                foreach (var b in hash)
                {
                    sbuilder.Append(b.ToString("x2").ToLower());
                }
                var convertedMd5 = sbuilder.ToString();
                if (convertedMd5 != response.Headers.First(h => h.Key.Equals(Etag, StringComparison.OrdinalIgnoreCase)).Value.ToLower())
                {

                    throw new InvalidETagException();
                }
            }
        }

        /// <summary>
        /// Gets the object save to file.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="saveDirectory">The save directory path. <remarks>Example c:\user\</remarks></param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="fileName">Name of the file.<remarks>Example image_name1.jpeg</remarks></param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="verifyEtag">if set to <c>true</c> will verify etag.</param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        ///<param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        public void GetObjectSaveToFile(string container, string saveDirectory, string objectName, string fileName = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null,bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (String.IsNullOrEmpty(saveDirectory))
                throw new ArgumentNullException();

            var filePath = Path.Combine(saveDirectory, string.IsNullOrWhiteSpace(fileName) ? objectName : fileName);

            try
            {
                using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    GetObject(container, objectName, fileStream, chunkSize, headers, region, verifyEtag, progressUpdated, useInternalUrl, identity);
                }
            }
            catch (InvalidETagException)
            {
                File.Delete(filePath);
                throw;
            }
        }

        /// <summary>
        /// Deletes the object.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        ///<param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        public ObjectStore DeleteObject(string container, string objectName, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container, objectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE, headers);

            if (response.StatusCode == 204)
                return ObjectStore.ObjectDeleted;
            if (response.StatusCode == 404)
                return ObjectStore.ContainerNotFound;

            return ObjectStore.Unknown;

        }

        /// <summary>
        /// Copies the object.
        /// </summary>
        /// <param name="sourceContainer">The source container name.</param>
        /// <param name="sourceObjectName">Name of the source object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="destinationContainer">The destination container name.</param>
        /// <param name="destinationObjectName">Name of the destination object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        ///<param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        public ObjectStore CopyObject(string sourceContainer, string sourceObjectName, string destinationContainer, string destinationObjectName, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(sourceContainer);
            _cloudFilesValidator.ValidateObjectName(sourceObjectName);

            _cloudFilesValidator.ValidateContainerName(destinationContainer);
            _cloudFilesValidator.ValidateObjectName(destinationObjectName);

            if (headers != null)
            {
                if (string.IsNullOrWhiteSpace(headers.FirstOrDefault(x => x.Key.Equals(ContentLength, StringComparison.OrdinalIgnoreCase)).Value))
                {
                    var contentLength = GetObjectContentLength(identity, sourceContainer, sourceObjectName, region, useInternalUrl);
                    headers.Add(ContentLength, contentLength);
                }
            }
            else
            {
                headers = new Dictionary<string, string>();
                var contentLength = GetObjectContentLength(identity, sourceContainer, sourceObjectName, region, useInternalUrl);
                headers.Add(ContentLength, contentLength);

            }

            headers.Add(CopyFrom, string.Format("{0}/{1}", sourceContainer, sourceObjectName));

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), destinationContainer, destinationObjectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, headers);

            if (response.StatusCode == 201)
                return ObjectStore.ObjectCreated;
            if (response.StatusCode == 404)
                return ObjectStore.ContainerNotFound;

            return ObjectStore.Unknown;
        }

        /// <summary>
        /// Gets the object meta data.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        ///<param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of Meta data</returns>
        public Dictionary<string, string> GetObjectMetaData(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container, objectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersMetadataKey];
        }

        /// <summary>
        /// Purges the object from CDN.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        /// <exception cref="CDNNotEnabledException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public ObjectStore PurgeObjectFromCDN(string container, string objectName, string region = null, CloudIdentity identity = null)
        {
            return PurgeObjectFromCDN(container, objectName, " ", region, identity);
        }

        /// <summary>
        /// Purges the object from CDN.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="emails">string[] of email address.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        /// <exception cref="CDNNotEnabledException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public ObjectStore PurgeObjectFromCDN(string container, string objectName, string[] emails, string region = null, CloudIdentity identity = null)
        {
            if (emails.Length < 0)
            {
                throw new ArgumentNullException();
            }

            return PurgeObjectFromCDN(container, objectName, string.Join(",", emails), region, identity);
        }

        /// <summary>
        /// Purges the object from CDN.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="email">Email Address.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="CDNNotEnabledException"></exception>
        public ObjectStore PurgeObjectFromCDN(string container, string objectName, string email, string region = null, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException();
            }

            if (!GetContainerCDNHeader(container, region, identity: identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }

            var headers = new Dictionary<string, string>();
            if (email.Trim().Length > 0)
            {
                headers[CdnPurgeEmail] = email;
            }
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFilesCDN(identity, region), container, objectName));
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE, headers: headers);
            if (response.StatusCode == 204)
                return ObjectStore.ObjectPurged;

            return ObjectStore.Unknown;

        }

        #endregion

        #region Accounts

        /// <summary>
        /// Gets the account headers.
        /// </summary>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of headers</returns>
        public Dictionary<string, string> GetAccountHeaders(string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersHeaderKey];

        }

        /// <summary>
        /// Gets the account meta data.
        /// </summary>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of meta data</returns>
        public Dictionary<string, string> GetAccountMetaData(string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersMetadataKey];
        }

        /// <summary>
        /// Updates the account metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void UpdateAccountMetadata(Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (metadata.Equals(null))
            {
                throw new ArgumentNullException();
            }

            var headers = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> m in metadata)
            {
                if (m.Key.Contains(AccountMetaDataPrefix))
                {
                    headers.Add(m.Key, m.Value);
                }
                else
                {
                    headers.Add(AccountMetaDataPrefix + m.Key, m.Value);
                }
            }

            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        /// <summary>
        /// Updates the account headers.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="region">The region.<remarks>[Optional]</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/> <remarks>[Optional]</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void UpdateAccountHeaders(Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (headers == null)
            {
                throw new ArgumentNullException();
            }

            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }


        #endregion


        #region Private methods

        private string GetObjectContentLength(CloudIdentity identity, string sourceContainer, string sourceObjectName, string region, bool useInternalUrl)
        {
            var sourceHeaders = GetObjectHeaders(sourceContainer, sourceObjectName, region,useInternalUrl, identity);
            var contentLength = sourceHeaders.FirstOrDefault(x => x.Key.Equals(ContentLength, StringComparison.OrdinalIgnoreCase)).Value;
            return contentLength;
        }

        protected string GetServiceEndpointCloudFiles(CloudIdentity identity, string region = null, bool useInternalUrl = false)
        {
            return useInternalUrl ? base.GetInternalServiceEndpoint(identity, "cloudFiles", region) : base.GetPublicServiceEndpoint(identity, "cloudFiles", region);
        }

        protected string GetServiceEndpointCloudFilesCDN(CloudIdentity identity, string region = null)
        {
            return base.GetPublicServiceEndpoint(identity, "cloudFilesCDN", region);
        }

        public static void CopyStream(Stream input, Stream output, int bufferSize, Action<long> progressUpdated)
        {
            var buffer = new byte[bufferSize];
            int len;
            long bytesWritten = 0;

            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
                bytesWritten += len;

                if (progressUpdated != null)
                    progressUpdated(bytesWritten);
            }
        }

        #endregion

        #region constants

        #region Headers
        //Auth Constants
        public const string AuthToken = "x-auth-token";
        public const string CdnManagementUrl = "x-cdn-management-url";
        public const string StorageUrl = "x-storage-url";
        //Account Constants
        public const string AccountMetaDataPrefix = "x-account-meta-";
        public const string AccountBytesUsed = "x-account-bytes-used";
        public const string AccountContainerCount = "x-account-container-count";
        public const string AccountObjectCount = "x-account-object-count";
        //Container Constants
        public const string ContainerMetaDataPrefix = "x-container-meta-";
        public const string ContainerBytesUsed = "x-container-bytes-used";
        public const string ContainerObjectCount = "x-container-object-count";
        public const string WebIndex = "x-container-meta-web-index";
        public const string WebError = "x-container-meta-web-error";
        public const string WebListings = "x-container-meta-web-listings";
        public const string WebListingsCSS = "x-container-meta-web-listings-css";
        public const string VersionsLocation = "x-versions-location";
        //CDN Container Constants
        public const string CdnUri = "x-cdn-uri";
        public const string CdnSslUri = "x-cdn-ssl-uri";
        public const string CdnStreamingUri = "x-cdn-streaming-uri";
        public const string CdnTTL = "x-ttl";
        public const string CdnLogRetention = "x-log-retention";
        public const string CdnEnabled = "x-cdn-enabled";
        public const string CdnIosUri = "x-cdn-ios-uri";
        //Object Constants
        public const string ObjectMetaDataPrefix = "x-object-meta-";
        public const string ObjectDeleteAfter = "x-delete-after";
        public const string ObjectDeleteAt = "x-delete-at";
        public const string Etag = "etag";
        public const string ContentType = "content-type";
        public const string ContentLength = "content-length";
        public const string CopyFrom = "x-copy-from";
        public const string ObjectManifestMetadataKey = "X-Object-Manifest";
        //Cdn Object Constants
        public const string CdnPurgeEmail = "x-purge-email";

        #endregion

        public const long LargeFileBatchThreshold = 5368709120; // 5GB
        public const string ProcessedHeadersMetadataKey = "metadata";
        public const string ProcessedHeadersHeaderKey = "headers";

        #endregion
    }
}
