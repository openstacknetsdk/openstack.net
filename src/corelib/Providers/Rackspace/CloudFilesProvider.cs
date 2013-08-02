﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
    /// <para>The Cloud Files Provider enable simple access to the Rackspace Cloud Files Services.
    /// Rackspace Cloud Files™ is an affordable, redundant, scalable, and dynamic storage service offering. 
    /// The core storage system is designed to provide a safe, secure, automatically re-sizing and network-accessible way to store data. 
    /// You can store an unlimited quantity of files and each one can be as large as 5 gigabytes. 
    /// Users can store as much as they want and pay only for storage space they actually use.</para>
    /// <para />
    /// <para>Additionally, Cloud Files provides a simple yet powerful way to publish and distribute content behind the industry-leading Akamai Content Distribution Network (CDN). 
    /// Cloud Files users get access to this network automatically without having to worry about contracts, additional costs, or technical hurdles.</para>
    /// <para />
    /// <para>Cloud Files allows users to store/retrieve files and CDN-enable content via a simple ReST (Representational State Transfer) web service interface. 
    /// There are also language-specific APIs that utilize the ReST API to make it much easier for developers to integrate into their applications.</para>
    /// <para />
    /// <para>Documentation URL: http://docs.rackspace.com/files/api/v1/cf-intro/content/Introduction-d1e82.html</para>
    /// </summary>
    /// <see cref="IObjectStorageProvider"/>
    /// <inheritdoc />
    public class CloudFilesProvider : ProviderBase<IObjectStorageProvider>, IObjectStorageProvider
    {
        private readonly IObjectStorageValidator _cloudFilesValidator;
        private readonly IObjectStorageMetadataProcessor _cloudFilesMetadataProcessor;
        private readonly IEncodeDecodeProvider _encodeDecodeProvider;
        private readonly IStatusParser _statusParser;
        private readonly IObjectMapper<BulkDeleteResponse, BulkDeletionResults> _bulkDeletionResultMapper;

        #region Constructors

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="CloudFilesProvider"/> class.
        /// </summary>
        public CloudFilesProvider()
            : this(null, null, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="CloudFilesProvider"/> class.
        /// </summary>
        /// <param name="defaultIdentity">The default identity. An instance of <see cref="net.openstack.Core.Domain.CloudIdentity"/></param>
        public CloudFilesProvider(CloudIdentity defaultIdentity)
            : this(defaultIdentity, null, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="CloudFilesProvider"/> class.
        /// </summary>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudFilesProvider(IRestService restService)
            : this(null, null, restService) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudFilesProvider"/> class.
        /// </summary>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        public CloudFilesProvider(IIdentityProvider identityProvider)
            : this(null, identityProvider, null) { }

                /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudFilesProvider"/> class.
        /// </summary>
        /// /<param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        public CloudFilesProvider(CloudIdentity identity, IIdentityProvider identityProvider)
            : this(identity, identityProvider, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="CloudFilesProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudFilesProvider(CloudIdentity identity, IRestService restService)
            : this(identity, null, restService) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudFilesProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudFilesProvider(CloudIdentity identity, IIdentityProvider identityProvider, IRestService restService)
            : this(identity, identityProvider, restService, CloudFilesValidator.Default, CloudFilesMetadataProcessor.Default, EncodeDecodeProvider.Default, HttpStatusCodeParser.Default, new BulkDeletionResultMapper(HttpStatusCodeParser.Default)) { }

        internal CloudFilesProvider(IIdentityProvider cloudIdentityProvider, IRestService restService, IObjectStorageValidator cloudFilesValidator, IObjectStorageMetadataProcessor cloudFilesMetadataProcessor, IEncodeDecodeProvider encodeDecodeProvider, IStatusParser statusParser, IObjectMapper<BulkDeleteResponse, BulkDeletionResults> mapper)
            : this(null, cloudIdentityProvider, restService, cloudFilesValidator, cloudFilesMetadataProcessor, encodeDecodeProvider, statusParser, mapper) { }

        internal CloudFilesProvider(CloudIdentity defaultIdentity, IIdentityProvider cloudIdentityProvider, IRestService restService, IObjectStorageValidator cloudFilesValidator, IObjectStorageMetadataProcessor cloudFilesMetadataProcessor, IEncodeDecodeProvider encodeDecodeProvider, IStatusParser statusParser, IObjectMapper<BulkDeleteResponse, BulkDeletionResults> bulkDeletionResultMapper)
            : base(defaultIdentity, cloudIdentityProvider, restService)
        {
            _cloudFilesValidator = cloudFilesValidator;
            _cloudFilesMetadataProcessor = cloudFilesMetadataProcessor;
            _encodeDecodeProvider = encodeDecodeProvider;
            _statusParser = statusParser;
            _bulkDeletionResultMapper = bulkDeletionResultMapper;
        }


        #endregion

        #region Containers

        /// <inheritdoc />
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

        /// <inheritdoc />
        public ObjectStore CreateContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT);

            if (response.StatusCode == HttpStatusCode.Created)
                return ObjectStore.ContainerCreated;
            if (response.StatusCode == HttpStatusCode.Accepted)
                return ObjectStore.ContainerExists;

            return ObjectStore.Unknown;
        }

        /// <inheritdoc />
        public ObjectStore DeleteContainer(string container, bool deleteObjects = true, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);

            if (deleteObjects)
            {
                var headers = GetContainerHeader(container, region, useInternalUrl, identity);
                var countHeader = headers.FirstOrDefault(h => h.Key.Equals(ContainerObjectCount, StringComparison.OrdinalIgnoreCase));
                if (!EqualityComparer<KeyValuePair<string, string>>.Default.Equals(countHeader, default(KeyValuePair<string, string>)))
                {
                    int count;
                    if (!int.TryParse(countHeader.Value, out count))
                        throw new Exception(string.Format("Unable to parse the object count header for container: {0}.  Actual count value: {1}", container, countHeader.Value));

                    if (count > 0)
                    {
                        var objects = ListObjects(container, count, region: region, useInternalUrl: useInternalUrl, identity: identity);

                        if(objects.Any())
                            DeleteObjects(container, objects.Select(o => o.Name), region: region, useInternalUrl: useInternalUrl, identity: identity);
                    }
                }
            }
            
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            return ObjectStore.ContainerDeleted;
        }

        /// <inheritdoc />
        public Dictionary<string, string> GetContainerHeader(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersHeaderKey];
        }

        /// <inheritdoc />
        public Dictionary<string, string> GetContainerMetaData(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.GET); // Should be HEAD

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersMetadataKey];
        }

        /// <inheritdoc />
        public ContainerCDN GetContainerCDNHeader(string container, string region = null, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), _encodeDecodeProvider.UrlEncode(container)));
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public Dictionary<string, string> EnableCDNOnContainer(string container, long ttl, string region = null, CloudIdentity identity = null)
        {
            return EnableCDNOnContainer(container, ttl, false, identity: identity);
        }

        /// <inheritdoc />
        public Dictionary<string, string> EnableCDNOnContainer(string container, bool logRetention, string region = null, CloudIdentity identity = null)
        {
            return EnableCDNOnContainer(container, 259200, logRetention, region, identity);
        }

        /// <inheritdoc />
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
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), _encodeDecodeProvider.UrlEncode(container)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, headers: headers);

            if (response == null)
                return null;

            return response.Headers.ToDictionary(header => header.Key, header => header.Value);
        }

        /// <inheritdoc />
        public Dictionary<string, string> DisableCDNOnContainer(string container, string region = null, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);

            var headers = new Dictionary<string, string>
                {
                {CdnEnabled, "false"}
                };
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), _encodeDecodeProvider.UrlEncode(container)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, headers: headers);

            if (response == null)
                return null;

            return response.Headers.ToDictionary(header => header.Key, header => header.Value);
        }

        /// <inheritdoc />
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
                if (m.Key.StartsWith(ContainerMetaDataPrefix))
                {
                    headers.Add(m.Key, m.Value);
                }
                else
                {
                    headers.Add(ContainerMetaDataPrefix + m.Key, m.Value);
                }
            }

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        /// <inheritdoc />
        public void DeleteContainerMetadata(string container, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            if (metadata.Equals(null))
            {
                throw new ArgumentNullException();
            }

            var headers = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> m in metadata)
            {
                if (m.Key.StartsWith(ContainerRemoveMetaDataPrefix))
                {
                    headers.Add(m.Key, m.Value);
                }
                else
                {
                    headers.Add(ContainerRemoveMetaDataPrefix + m.Key, m.Value);
                }
            }

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        /// <inheritdoc />
        public void DeleteContainerMetadata(string container, string key, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException();
            }

            var headers = new Dictionary<string, string>();
            headers.Add(key, "xxxx"); // when deleting metadata, we must still provide a value for the key

            DeleteContainerMetadata(container, headers, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public void AddContainerHeaders(string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            if (headers == null)
            {
                throw new ArgumentNullException();
            }

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        /// <inheritdoc />
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

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), _encodeDecodeProvider.UrlEncode(container)));
            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public Dictionary<string, string> GetObjectHeaders(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container), _encodeDecodeProvider.UrlEncode(objectName)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersHeaderKey];
        }

        /// <inheritdoc />
        public Dictionary<string, string> GetObjectMetaData(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container), _encodeDecodeProvider.UrlEncode(objectName)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersMetadataKey];
        }

        /// <inheritdoc />
        public void UpdateObjectMetadata(string container, string objectName, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);
            if (metadata.Equals(null))
            {
                throw new ArgumentNullException();
            }

            var headers = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> m in metadata)
            {
                if (m.Key.StartsWith(ObjectMetaDataPrefix) || m.Key.StartsWith(ObjectRemoveMetaDataPrefix))
                {
                    headers.Add(m.Key, m.Value);
                }
                else
                {
                    headers.Add(ObjectMetaDataPrefix + m.Key, m.Value);
                }
            }

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container), _encodeDecodeProvider.UrlEncode(objectName)));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        /// <inheritdoc />
        public void DeleteObjectMetadata(string container, string objectName, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);
            if (metadata.Equals(null))
            {
                throw new ArgumentNullException();
            }

            var headers = GetObjectMetaData(container, objectName, region, useInternalUrl, identity);
            foreach (KeyValuePair<string, string> m in metadata)
            {
                if (headers.ContainsKey(m.Key))
                {
                    headers.Remove(m.Key);
                    headers.Add(ObjectRemoveMetaDataPrefix + m.Key, m.Value);
                }
            }

            UpdateObjectMetadata(container, objectName, headers, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public void DeleteObjectMetadata(string container, string objectName, string key, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException();
            }

            var headers = new Dictionary<string, string>();
            headers.Add(key, "xxxx"); // when deleting metadata, we must still provide a value for the key

            DeleteObjectMetadata(container, objectName, headers, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public IEnumerable<ContainerObject> ListObjects(string container, int? limit = null, string marker = null, string markerEnd = null,  string prefix = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            var queryStringParameter = new Dictionary<string, string>();

            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (!string.IsNullOrWhiteSpace(marker))
                queryStringParameter.Add("marker", marker);

            if (!string.IsNullOrWhiteSpace(markerEnd))
                queryStringParameter.Add("end_marker", markerEnd);

            if (!string.IsNullOrWhiteSpace(prefix))
                queryStringParameter.Add("prefix", prefix);

            var response = ExecuteRESTRequest<ContainerObject[]>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            if (response == null || response.Data == null)
                return null;

            return response.Data;
        }

        /// <inheritdoc />
        public void CreateObjectFromFile(string container, string filePath, string objectName = null, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                var info = new FileInfo(filePath);
                objectName = info.Name;
            }

            using (var stream = File.OpenRead(filePath))
            {
                CreateObject(container, stream, objectName, chunkSize, headers, region, progressUpdated, useInternalUrl, identity);
            }
        }

        /// <inheritdoc />
        public void CreateObject(string container, Stream stream, string objectName, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (stream == null)
                throw new ArgumentNullException();

            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);

            if (stream.Length > LargeFileBatchThreshold)
            {
                CreateObjectInSegments(_encodeDecodeProvider.UrlEncode(container), stream, _encodeDecodeProvider.UrlEncode(objectName), chunkSize, headers, region, progressUpdated, useInternalUrl, identity);
                return;
            }
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container), _encodeDecodeProvider.UrlEncode(objectName)));

            StreamRESTRequest(identity, urlPath, HttpMethod.PUT, stream, chunkSize, headers: headers, progressUpdated: progressUpdated, requestSettings: new RequestSettings {ChunkRequest = true});
        }

        /// <inheritdoc />
        public void GetObject(string container, string objectName, Stream outputStream, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container), _encodeDecodeProvider.UrlEncode(objectName)));

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

                    var respHeaders = resp.Headers.AllKeys.Select(key => new HttpHeader(key, resp.GetResponseHeader(key))).ToList();

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

        /// <inheritdoc />
        public void GetObjectSaveToFile(string container, string saveDirectory, string objectName, string fileName = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
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

        /// <inheritdoc />
        public ObjectStore CopyObject(string sourceContainer, string sourceObjectName, string destinationContainer, string destinationObjectName, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(sourceContainer);
            _cloudFilesValidator.ValidateObjectName(sourceObjectName);

            _cloudFilesValidator.ValidateContainerName(destinationContainer);
            _cloudFilesValidator.ValidateObjectName(destinationObjectName);

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(sourceContainer), _encodeDecodeProvider.UrlEncode(sourceObjectName)));
            
            if(headers == null)
                headers = new Dictionary<string, string>();

            headers.Add(DestinationMetadataKey, string.Format("{0}/{1}", destinationContainer, destinationObjectName));
            ExecuteRESTRequest(identity, urlPath, HttpMethod.COPY, headers: headers);

            return ObjectStore.ObjectCreated;
        }

        /// <inheritdoc />
        public ObjectStore DeleteObject(string container, string objectName, Dictionary<string, string> headers = null, bool deleteSegments = true, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);

            Dictionary<string, string> objectHeader = null;
            if(deleteSegments)
                objectHeader = GetObjectHeaders(container, objectName, region, useInternalUrl, identity);

            if (deleteSegments && objectHeader != null && objectHeader.Any(h => h.Key.Equals(ObjectManifestMetadataKey, StringComparison.OrdinalIgnoreCase)))
            {
                var objects = ListObjects(container, region: region, useInternalUrl: useInternalUrl,
                                               identity: identity);

                if (objects != null && objects.Any())
                {
                    var segments = objects.Where(f => f.Name.StartsWith(string.Format("{0}.seg", objectName)));
                    var delObjects = new List<string> { objectName };
                    if (segments.Any())
                        delObjects.AddRange(segments.Select(s => s.Name));

                    DeleteObjects(container, delObjects, headers, region, useInternalUrl, identity);
                }
                else
                    throw new Exception(string.Format("Object \"{0}\" in container \"{1}\" does not exist.", objectName, container));
            }
            else
            {
                var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container), _encodeDecodeProvider.UrlEncode(objectName)));

                ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE, headers: headers);
            }
            
            return ObjectStore.ObjectDeleted;
        }

        /// <summary>
        /// Deletes container objects.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objects">List of container object names for the objects that should be deleted.</param>
        /// <param name="headers">A list of HTTP headers to send to the service. </param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        public ObjectStore DeleteObjects(string container, IEnumerable<string> objects, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesValidator.ValidateContainerName(container);
            foreach (var objectName in objects)
            {
                _cloudFilesValidator.ValidateObjectName(objectName);
            }

            return BulkDelete(objects.Select(o => string.Format("{0}/{1}", container, o)), headers, region, useInternalUrl, identity);
        }

        /// <summary>
        /// Deletes a list of items.  Items can be containers or objects.
        /// </summary>
        /// <param name="items">List of items (containers or objects) that should be deleted. <remarks>Should be in the form of: Container = \"/container_name\"  Object = \"container_name/my_image.jpg\"</remarks></param>
        /// <param name="headers">A list of HTTP headers to send to the service. </param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        public ObjectStore BulkDelete(IEnumerable<string> items, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/?bulk-delete", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));

            var body = string.Join('\n'.ToString(), items.Select(o => _encodeDecodeProvider.UrlEncode(o)));

            var response = ExecuteRESTRequest<BulkDeleteResponse>(identity, urlPath, HttpMethod.DELETE, body: body, headers: headers, settings: new JsonRequestSettings { ContentType = "text/plain" });

            var status = _statusParser.Parse(response.Data.Status);
 
            if (status.Code != 200 && !response.Data.Errors.Any())
            {
                response.Data.AllItems = items;
                throw new BulkDeletionException(response.Data.Status, _bulkDeletionResultMapper.Map(response.Data));    
            }

            return ObjectStore.ObjectDeleted;
        }

        /// <inheritdoc />
        public ObjectStore MoveObject(string sourceContainer, string sourceObjectName, string destinationContainer, string destinationObjectName, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var result = CopyObject(sourceContainer, sourceObjectName, destinationContainer, destinationObjectName, headers, region, useInternalUrl, identity);

            if (result != ObjectStore.ObjectCreated)
                return result;

            return DeleteObject(sourceContainer, sourceObjectName, headers, true, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public ObjectStore PurgeObjectFromCDN(string container, string objectName, string region = null, CloudIdentity identity = null)
        {
            return PurgeObjectFromCDN(container, objectName, " ", region, identity);
        }

        /// <inheritdoc />
        public ObjectStore PurgeObjectFromCDN(string container, string objectName, string[] emails, string region = null, CloudIdentity identity = null)
        {
            if (emails.Length < 0)
            {
                throw new ArgumentNullException();
            }

            return PurgeObjectFromCDN(container, objectName, string.Join(",", emails), region, identity);
        }

        /// <inheritdoc />
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
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFilesCDN(identity, region), _encodeDecodeProvider.UrlEncode(container), _encodeDecodeProvider.UrlEncode(objectName)));
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE, headers: headers);
            if (response.StatusCode == HttpStatusCode.NoContent)
                return ObjectStore.ObjectPurged;

            return ObjectStore.Unknown;

        }

        #endregion

        #region Accounts

        /// <inheritdoc />
        public Dictionary<string, string> GetAccountHeaders(string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersHeaderKey];

        }

        /// <inheritdoc />
        public Dictionary<string, string> GetAccountMetaData(string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersMetadataKey];
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <summary>
        /// Creates the object in segments.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="stream">The stream. <see cref="Stream"/></param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. </param>
        /// <param name="region">The region.</param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The identity. <see cref="CloudIdentity"/>  </param>
        private void CreateObjectInSegments(string container, Stream stream, string objectName, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var totalLength = stream.Length;
            var segmentCount = Math.Ceiling((double)totalLength / (double)LargeFileBatchThreshold);

            long totalBytesWritten = 0;
            for (int i = 0; i < segmentCount; i++)
            {
                var remaining = (totalLength - LargeFileBatchThreshold * i);
                var length = (remaining < LargeFileBatchThreshold) ? remaining : LargeFileBatchThreshold;

                var urlPath = new Uri(string.Format("{0}/{1}/{2}.seg{3}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container, objectName, i.ToString("0000")));
                long segmentBytesWritten = 0;
                StreamRESTRequest(identity, urlPath, HttpMethod.PUT, stream, chunkSize, length, headers: headers, requestSettings: new RequestSettings {ChunkRequest = true}, progressUpdated:
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
            StreamRESTRequest(identity, segmentUrlPath, HttpMethod.PUT, new MemoryStream(new Byte[0]), chunkSize, headers: headers, requestSettings: new RequestSettings {ChunkRequest = true}, progressUpdated:
                (bytesWritten) =>
                {
                    if (progressUpdated != null)
                    {
                        progressUpdated(totalBytesWritten);
                    }
                });
        }

        protected override IObjectStorageProvider BuildProvider(CloudIdentity identity)
        {
            return new CloudFilesProvider(identity, IdentityProvider, RestService, _cloudFilesValidator, _cloudFilesMetadataProcessor, _encodeDecodeProvider, _statusParser, _bulkDeletionResultMapper);
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
        public const string ContainerRemoveMetaDataPrefix = "x-remove-container-meta-";
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
        public const string ObjectRemoveMetaDataPrefix = "x-remove-object-meta-";
        public const string ObjectDeleteAfter = "x-delete-after";
        public const string ObjectDeleteAt = "x-delete-at";
        public const string Etag = "etag";
        public const string DestinationMetadataKey = "destination";
        public const string ObjectManifestMetadataKey = "x-object-manifest";

        //Cdn Object Constants
        public const string CdnPurgeEmail = "x-purge-email";

        #endregion

        private static long _largeFileBatchThreshold = 5368709120; // 5GB

        public static long LargeFileBatchThreshold
        {
            get { return _largeFileBatchThreshold; }
            internal set { _largeFileBatchThreshold = value; }
        }
        public const string ProcessedHeadersMetadataKey = "metadata";
        public const string ProcessedHeadersHeaderKey = "headers";
        #endregion
 
    }
}
