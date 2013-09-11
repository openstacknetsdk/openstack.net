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
using net.openstack.Core.Exceptions.Response;
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
    /// Provides an implementation of <see cref="IObjectStorageProvider"/>
    /// for operating with Rackspace's Cloud Files product.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/">OpenStack Object Storage API v1 Reference</seealso>
    /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/Overview-d1e70.html">Rackspace Cloud Files Developer Guide - API v1</seealso>
    public class CloudFilesProvider : ProviderBase<IObjectStorageProvider>, IObjectStorageProvider
    {
        /// <summary>
        /// The <see cref="IObjectStorageValidator"/> to use for this provider. This is
        /// typically set to <see cref="CloudFilesValidator.Default"/>.
        /// </summary>
        private readonly IObjectStorageValidator _cloudFilesValidator;

        /// <summary>
        /// The <see cref="IObjectStorageMetadataProcessor"/> to use for this provider. This is
        /// typically set to <see cref="CloudFilesMetadataProcessor.Default"/>.
        /// </summary>
        private readonly IObjectStorageMetadataProcessor _cloudFilesMetadataProcessor;

        /// <summary>
        /// The <see cref="IEncodeDecodeProvider"/> to use for this provider. This is
        /// typically set to <see cref="EncodeDecodeProvider.Default"/>.
        /// </summary>
        private readonly IEncodeDecodeProvider _encodeDecodeProvider;

        /// <summary>
        /// The <see cref="IStatusParser"/> to use for this provider. This is
        /// typically set to <see cref="HttpStatusCodeParser.Default"/>.
        /// </summary>
        private readonly IStatusParser _statusParser;

        /// <summary>
        /// The <see cref="IObjectMapper{BulkDeleteResponse, BulkDeletionResults}"/> to use for
        /// this provider. This is typically set to a new instance of <see cref="BulkDeletionResultMapper"/>.
        /// </summary>
        private readonly IObjectMapper<BulkDeleteResponse, BulkDeletionResults> _bulkDeletionResultMapper;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudFilesProvider"/> class with
        /// no default identity, and the default identity provider and REST service implementation.
        /// </summary>
        public CloudFilesProvider()
            : this(null, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudFilesProvider"/> class with
        /// the specified default identity, and the default identity provider and REST service
        /// implementation.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        public CloudFilesProvider(CloudIdentity defaultIdentity)
            : this(defaultIdentity, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudFilesProvider"/> class with
        /// no default identity, the default identity provider, and the specified REST service
        /// implementation.
        /// </summary>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        public CloudFilesProvider(IRestService restService)
            : this(null, null, restService) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudFilesProvider"/> class with
        /// no default identity, the specified identity provider, and the default REST service
        /// implementation.
        /// </summary>
        /// <param name="identityProvider">The identity provider to use for authenticating requests to this provider. If this value is <c>null</c>, a new instance of <see cref="CloudIdentityProvider"/> is created with no default identity.</param>
        public CloudFilesProvider(IIdentityProvider identityProvider)
            : this(null, identityProvider, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudFilesProvider"/> class with
        /// the specified default identity and identity provider, and the default REST service
        /// implementation.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        /// <param name="identityProvider">The identity provider to use for authenticating requests to this provider. If this value is <c>null</c>, a new instance of <see cref="CloudIdentityProvider"/> is created using <paramref name="defaultIdentity"/> as the default identity.</param>
        public CloudFilesProvider(CloudIdentity defaultIdentity, IIdentityProvider identityProvider)
            : this(defaultIdentity, identityProvider, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudFilesProvider"/> class with
        /// the specified default identity and REST service implementation, and the default
        /// identity provider.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        public CloudFilesProvider(CloudIdentity defaultIdentity, IRestService restService)
            : this(defaultIdentity, null, restService) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudFilesProvider"/> class with
        /// the specified default identity, identity provider, and REST service implementation,
        /// and the default Rackspace-Cloud-Files-specific implementations of the object storage
        /// validator, metadata processor, encoder, status parser, and bulk delete results mapper.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        /// <param name="identityProvider">The identity provider to use for authenticating requests to this provider. If this value is <c>null</c>, a new instance of <see cref="CloudIdentityProvider"/> is created using <paramref name="defaultIdentity"/> as the default identity.</param>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        public CloudFilesProvider(CloudIdentity defaultIdentity, IIdentityProvider identityProvider, IRestService restService)
            : this(defaultIdentity, identityProvider, restService, CloudFilesValidator.Default, CloudFilesMetadataProcessor.Default, EncodeDecodeProvider.Default, HttpStatusCodeParser.Default, new BulkDeletionResultMapper(HttpStatusCodeParser.Default)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudFilesProvider"/> class with
        /// no default identity, and the default identity provider, REST service implementation,
        /// validator, metadata processor, encoder, status parser, and bulk delete results
        /// mapper.
        /// </summary>
        /// <param name="identityProvider">The identity provider to use for authenticating requests to this provider. If this value is <c>null</c>, a new instance of <see cref="CloudIdentityProvider"/> is created with no default identity.</param>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        /// <param name="cloudFilesValidator">The <see cref="IObjectStorageValidator"/> to use for validating requests to this service.</param>
        /// <param name="cloudFilesMetadataProcessor">The <see cref="IObjectStorageMetadataProcessor"/> to use for processing metadata returned in HTTP headers.</param>
        /// <param name="encodeDecodeProvider">The <see cref="IEncodeDecodeProvider"/> to use for encoding data in URI query strings.</param>
        /// <param name="statusParser">The <see cref="IStatusParser"/> to use for parsing HTTP status codes.</param>
        /// <param name="mapper">The object mapper to use for mapping <see cref="BulkDeleteResponse"/> objects to <see cref="BulkDeletionResults"/> objects.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="cloudFilesValidator"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="cloudFilesMetadataProcessor"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="encodeDecodeProvider"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="statusParser"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="mapper"/> is <c>null</c>.</para>
        /// </exception>
        internal CloudFilesProvider(IIdentityProvider identityProvider, IRestService restService, IObjectStorageValidator cloudFilesValidator, IObjectStorageMetadataProcessor cloudFilesMetadataProcessor, IEncodeDecodeProvider encodeDecodeProvider, IStatusParser statusParser, IObjectMapper<BulkDeleteResponse, BulkDeletionResults> mapper)
            : this(null, identityProvider, restService, cloudFilesValidator, cloudFilesMetadataProcessor, encodeDecodeProvider, statusParser, mapper) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudFilesProvider"/> class with
        /// the specified default identity, identity provider, REST service implementation,
        /// validator, metadata processor, encoder, status parser, and bulk delete results
        /// mapper.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        /// <param name="identityProvider">The identity provider to use for authenticating requests to this provider. If this value is <c>null</c>, a new instance of <see cref="CloudIdentityProvider"/> is created using <paramref name="defaultIdentity"/> as the default identity.</param>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        /// <param name="cloudFilesValidator">The <see cref="IObjectStorageValidator"/> to use for validating requests to this service.</param>
        /// <param name="cloudFilesMetadataProcessor">The <see cref="IObjectStorageMetadataProcessor"/> to use for processing metadata returned in HTTP headers.</param>
        /// <param name="encodeDecodeProvider">The <see cref="IEncodeDecodeProvider"/> to use for encoding data in URI query strings.</param>
        /// <param name="statusParser">The <see cref="IStatusParser"/> to use for parsing HTTP status codes.</param>
        /// <param name="bulkDeletionResultMapper">The object mapper to use for mapping <see cref="BulkDeleteResponse"/> objects to <see cref="BulkDeletionResults"/> objects.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="cloudFilesValidator"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="cloudFilesMetadataProcessor"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="encodeDecodeProvider"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="statusParser"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="bulkDeletionResultMapper"/> is <c>null</c>.</para>
        /// </exception>
        internal CloudFilesProvider(CloudIdentity defaultIdentity, IIdentityProvider identityProvider, IRestService restService, IObjectStorageValidator cloudFilesValidator, IObjectStorageMetadataProcessor cloudFilesMetadataProcessor, IEncodeDecodeProvider encodeDecodeProvider, IStatusParser statusParser, IObjectMapper<BulkDeleteResponse, BulkDeletionResults> bulkDeletionResultMapper)
            : base(defaultIdentity, identityProvider, restService)
        {
            if (cloudFilesValidator == null)
                throw new ArgumentNullException("cloudFilesValidator");
            if (cloudFilesMetadataProcessor == null)
                throw new ArgumentNullException("cloudFilesMetadataProcessor");
            if (encodeDecodeProvider == null)
                throw new ArgumentNullException("encodeDecodeProvider");
            if (statusParser == null)
                throw new ArgumentNullException("statusParser");
            if (bulkDeletionResultMapper == null)
                throw new ArgumentNullException("bulkDeletionResultMapper");

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
            if (limit < 0)
                throw new ArgumentOutOfRangeException("limit");
            CheckIdentity(identity);

            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));

            var queryStringParameter = new Dictionary<string, string>();

            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (!string.IsNullOrEmpty(marker))
                queryStringParameter.Add("marker", marker);

            if (!string.IsNullOrEmpty(markerEnd))
                queryStringParameter.Add("end_marker", markerEnd);

            var response = ExecuteRESTRequest<Container[]>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            if (response == null || response.Data == null)
                return null;

            return response.Data;

        }

        /// <inheritdoc />
        public ObjectStore CreateContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT);

            switch (response.StatusCode)
            {
            case HttpStatusCode.Created:
                return ObjectStore.ContainerCreated;

            case HttpStatusCode.Accepted:
                return ObjectStore.ContainerExists;

            default:
                throw new ResponseException(string.Format("Unexpected status {0} returned by Create Container.", response.StatusCode), response);
            }
        }

        /// <inheritdoc />
        public void DeleteContainer(string container, bool deleteObjects = false, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            CheckIdentity(identity);

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

            try
            {
                ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);
            }
            catch (ServiceConflictException ex)
            {
                throw new ContainerNotEmptyException(null, ex);
            }
        }

        /// <inheritdoc />
        public Dictionary<string, string> GetContainerHeader(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersHeaderKey];
        }

        /// <inheritdoc />
        public Dictionary<string, string> GetContainerMetaData(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.GET); // Should be HEAD

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersMetadataKey];
        }

        /// <inheritdoc />
        public ContainerCDN GetContainerCDNHeader(string container, string region = null, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), _encodeDecodeProvider.UrlEncode(container)));
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            string name = container;
            string uri = null;
            string streamingUri = null;
            string sslUri = null;
            string iosUri = null;
            bool enabled = false;
            long ttl = 0;
            bool logRetention = false;

            foreach (var header in response.Headers)
            {
                if (header.Key.Equals(CdnUri, StringComparison.OrdinalIgnoreCase))
                {
                    uri = header.Value;
                }
                else if (header.Key.Equals(CdnSslUri, StringComparison.OrdinalIgnoreCase))
                {
                    sslUri = header.Value;
                }
                else if (header.Key.Equals(CdnStreamingUri, StringComparison.OrdinalIgnoreCase))
                {
                    streamingUri = header.Value;
                }
                else if (header.Key.Equals(CdnTTL, StringComparison.OrdinalIgnoreCase))
                {
                    ttl = long.Parse(header.Value);
                }
                else if (header.Key.Equals(CdnEnabled, StringComparison.OrdinalIgnoreCase))
                {
                    enabled = bool.Parse(header.Value);
                }
                else if (header.Key.Equals(CdnLogRetention, StringComparison.OrdinalIgnoreCase))
                {
                    logRetention = bool.Parse(header.Value);
                }
                else if (header.Key.Equals(CdnIosUri, StringComparison.OrdinalIgnoreCase))
                {
                    iosUri = header.Value;
                }
            }

            ContainerCDN result = new ContainerCDN(name, uri, streamingUri, sslUri, iosUri, enabled, ttl, logRetention);
            return result;
        }

        /// <inheritdoc />
        public IEnumerable<ContainerCDN> ListCDNContainers(int? limit = null, string marker = null, string markerEnd = null, bool cdnEnabled = false, string region = null, CloudIdentity identity = null)
        {
            if (limit < 0)
                throw new ArgumentOutOfRangeException("limit");
            CheckIdentity(identity);

            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFilesCDN(identity, region)));

            var queryStringParameter = new Dictionary<string, string>
                {
                    {"format", "json"},
                    {"enabled_only", cdnEnabled.ToString().ToLower()}
                };

            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (!string.IsNullOrEmpty(marker))
                queryStringParameter.Add("marker", marker);

            if (!string.IsNullOrEmpty(markerEnd))
                queryStringParameter.Add("end_marker", markerEnd);

            var response = ExecuteRESTRequest<ContainerCDN[]>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            if (response == null || response.Data == null)
                return null;

            return response.Data;
        }

        /// <inheritdoc />
        public Dictionary<string, string> EnableCDNOnContainer(string container, long timeToLive, string region = null, CloudIdentity identity = null)
        {
            return EnableCDNOnContainer(container, timeToLive, false, identity: identity);
        }

        /// <inheritdoc />
        public Dictionary<string, string> EnableCDNOnContainer(string container, bool logRetention, string region = null, CloudIdentity identity = null)
        {
            return EnableCDNOnContainer(container, 259200, logRetention, region, identity);
        }

        /// <inheritdoc />
        public Dictionary<string, string> EnableCDNOnContainer(string container, long timeToLive, bool logRetention, string region = null, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (timeToLive < 0)
                throw new ArgumentOutOfRangeException("timeToLive");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);

            if (timeToLive > 1577836800 || timeToLive < 900)
            {
                throw new TTLLengthException("TTL range must be 900 to 1577836800 seconds TTL: " + timeToLive.ToString(CultureInfo.InvariantCulture));
            }

            var headers = new Dictionary<string, string>
                {
                 {CdnTTL, timeToLive.ToString(CultureInfo.InvariantCulture)},
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
            if (container == null)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            CheckIdentity(identity);

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
            if (container == null)
                throw new ArgumentNullException("container");
            if (metadata == null)
                throw new ArgumentNullException("metadata");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, string> m in metadata)
            {
                if (string.IsNullOrEmpty(m.Key))
                    throw new ArgumentException("metadata keys cannot be null or empty");
                if (m.Key.Contains('_'))
                    throw new NotSupportedException("This provider does not support metadata keys containing an underscore.");
                if (m.Key.Contains('\''))
                    throw new NotSupportedException("This provider does not support metadata keys containing an apostrophe.");

                headers.Add(ContainerMetaDataPrefix + m.Key, EncodeUnicodeValue(m.Value));
            }

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        /// <inheritdoc />
        public void DeleteContainerMetadata(string container, IEnumerable<string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (metadata == null)
                throw new ArgumentNullException("metadata");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            CheckIdentity(identity);

            Dictionary<string, string> headers = metadata.ToDictionary(i => i, i => default(string), StringComparer.OrdinalIgnoreCase);
            UpdateContainerMetadata(container, headers, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public void DeleteContainerMetadata(string container, string key, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty");

            DeleteContainerMetadata(container, new[] { key }, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public void UpdateContainerCdnHeaders(string container, Dictionary<string, string> headers, string region = null, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (headers == null)
                throw new ArgumentNullException("headers");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            VerifyContainerIsCDNEnabled(container, region, identity);

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), _encodeDecodeProvider.UrlEncode(container)));
            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        /// <inheritdoc />
        public void EnableStaticWebOnContainer(string container, string index, string error, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (index == null)
                throw new ArgumentNullException("index");
            if (error == null)
                throw new ArgumentNullException("error");
            if (css == null)
                throw new ArgumentNullException("css");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(index))
                throw new ArgumentException("index cannot be empty");
            if (string.IsNullOrEmpty(error))
                throw new ArgumentException("error cannot be empty");
            if (string.IsNullOrEmpty(css))
                throw new ArgumentException("css cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(index);
            _cloudFilesValidator.ValidateObjectName(error);
            _cloudFilesValidator.ValidateObjectName(css);
            VerifyContainerIsCDNEnabled(container, region, identity);

            var metadata = new Dictionary<string, string>
                                {
                                    {WebIndex, index},
                                    {WebError, error},
                                    {WebListingsCSS, css},
                                    {WebListings, listing.ToString()}
                                };
            UpdateContainerMetadata(container, metadata, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public void EnableStaticWebOnContainer(string container, string index, string error, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (index == null)
                throw new ArgumentNullException("index");
            if (error == null)
                throw new ArgumentNullException("error");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(index))
                throw new ArgumentException("index cannot be empty");
            if (string.IsNullOrEmpty(error))
                throw new ArgumentException("error cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(index);
            _cloudFilesValidator.ValidateObjectName(error);
            VerifyContainerIsCDNEnabled(container, region, identity);

            var headers = new Dictionary<string, string>
                                  {
                                      {WebIndex, index},
                                      {WebError, error},
                                      {WebListings, listing.ToString()}
                                  };
            UpdateContainerMetadata(container, headers, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public void EnableStaticWebOnContainer(string container, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (css == null)
                throw new ArgumentNullException("css");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(css))
                throw new ArgumentException("css cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(css);
            VerifyContainerIsCDNEnabled(container, region, identity);

            var headers = new Dictionary<string, string>
                                {
                                    {WebListingsCSS, css},
                                    {WebListings, listing.ToString()}
                                };
            UpdateContainerMetadata(container, headers, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public void EnableStaticWebOnContainer(string container, string index, string error, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (index == null)
                throw new ArgumentNullException("index");
            if (error == null)
                throw new ArgumentNullException("error");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(index))
                throw new ArgumentException("index cannot be empty");
            if (string.IsNullOrEmpty(error))
                throw new ArgumentException("error cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(index);
            _cloudFilesValidator.ValidateObjectName(error);
            VerifyContainerIsCDNEnabled(container, region, identity);

            var headers = new Dictionary<string, string>
                                  {
                                      {WebIndex, index},
                                      {WebError, error}
                                  };
            UpdateContainerMetadata(container, headers, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public void DisableStaticWebOnContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            VerifyContainerIsCDNEnabled(container, region, identity);

            var headers = new Dictionary<string, string>
                                {
                                    {WebIndex, string.Empty},
                                    {WebError, string.Empty},
                                    {WebListingsCSS, string.Empty},
                                    {WebListings, string.Empty}
                                };
            UpdateContainerMetadata(container, headers, region, useInternalUrl, identity);
        }

        #endregion

        #region Container Objects

        /// <inheritdoc />
        public Dictionary<string, string> GetObjectHeaders(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (objectName == null)
                throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("objectName cannot be empty");
            CheckIdentity(identity);

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
            if (container == null)
                throw new ArgumentNullException("container");
            if (objectName == null)
                throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("objectName cannot be empty");
            CheckIdentity(identity);

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
            if (container == null)
                throw new ArgumentNullException("container");
            if (objectName == null)
                throw new ArgumentNullException("objectName");
            if (metadata == null)
                throw new ArgumentNullException("metadata");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("objectName cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, string> m in metadata)
            {
                if (string.IsNullOrEmpty(m.Key))
                    throw new ArgumentException("metadata cannot contain any null or empty keys");

                headers.Add(ObjectMetaDataPrefix + m.Key, EncodeUnicodeValue(m.Value));
            }

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container), _encodeDecodeProvider.UrlEncode(objectName)));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        /// <inheritdoc />
        public void DeleteObjectMetadata(string container, string objectName, IEnumerable<string> keys, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (objectName == null)
                throw new ArgumentNullException("objectName");
            if (keys == null)
                throw new ArgumentNullException("keys");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("objectName cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);

            var headers = new Dictionary<string, string>(GetObjectMetaData(container, objectName, region, useInternalUrl, identity), StringComparer.OrdinalIgnoreCase);
            foreach (string key in keys)
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentException("keys cannot contain any null or empty values");

                headers.Remove(key);
            }

            UpdateObjectMetadata(container, objectName, headers, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public void DeleteObjectMetadata(string container, string objectName, string key, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty");

            DeleteObjectMetadata(container, objectName, new[] { key }, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public IEnumerable<ContainerObject> ListObjects(string container, int? limit = null, string marker = null, string markerEnd = null,  string prefix = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (limit < 0)
                throw new ArgumentOutOfRangeException("limit");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container)));

            var queryStringParameter = new Dictionary<string, string>();

            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (!string.IsNullOrEmpty(marker))
                queryStringParameter.Add("marker", marker);

            if (!string.IsNullOrEmpty(markerEnd))
                queryStringParameter.Add("end_marker", markerEnd);

            if (!string.IsNullOrEmpty(prefix))
                queryStringParameter.Add("prefix", prefix);

            var response = ExecuteRESTRequest<ContainerObject[]>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            if (response == null || response.Data == null)
                return null;

            return response.Data;
        }

        /// <inheritdoc />
        public void CreateObjectFromFile(string container, string filePath, string objectName = null, string contentType = null, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (filePath == null)
                throw new ArgumentNullException("filePath");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("filePath cannot be empty");
            if (chunkSize < 0)
                throw new ArgumentOutOfRangeException("chunkSize");
            CheckIdentity(identity);

            if (string.IsNullOrEmpty(objectName))
                objectName = Path.GetFileName(filePath);

            using (var stream = File.OpenRead(filePath))
            {
                CreateObject(container, stream, objectName, contentType, chunkSize, headers, region, progressUpdated, useInternalUrl, identity);
            }
        }

        /// <inheritdoc />
        public void CreateObject(string container, Stream stream, string objectName, string contentType = null, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (objectName == null)
                throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("objectName cannot be empty");
            if (chunkSize < 0)
                throw new ArgumentOutOfRangeException("chunkSize");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);

            if (stream.Length > LargeFileBatchThreshold)
            {
                CreateObjectInSegments(_encodeDecodeProvider.UrlEncode(container), stream, _encodeDecodeProvider.UrlEncode(objectName), contentType, chunkSize, headers, region, progressUpdated, useInternalUrl, identity);
                return;
            }
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(container), _encodeDecodeProvider.UrlEncode(objectName)));

            RequestSettings settings = BuildDefaultRequestSettings();
            settings.ChunkRequest = true;
            settings.ContentType = contentType;

            StreamRESTRequest(identity, urlPath, HttpMethod.PUT, stream, chunkSize, headers: headers, progressUpdated: progressUpdated, requestSettings: settings);
        }

        /// <inheritdoc />
        public void GetObject(string container, string objectName, Stream outputStream, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (objectName == null)
                throw new ArgumentNullException("objectName");
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("objectName cannot be empty");
            if (chunkSize < 0)
                throw new ArgumentOutOfRangeException("chunkSize");
            CheckIdentity(identity);

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

            string etag;
            if (verifyEtag && response.TryGetHeader(Etag, out etag))
            {
                outputStream.Flush(); // flush the contents of the stream to the output device
                outputStream.Position = 0;  // reset the head of the stream to the beginning

                using (var md5 = MD5.Create())
                {
                    md5.ComputeHash(outputStream);

                    var sbuilder = new StringBuilder();
                    var hash = md5.Hash;
                    foreach (var b in hash)
                    {
                        sbuilder.Append(b.ToString("x2").ToLower());
                    }
                    var convertedMd5 = sbuilder.ToString();
                    if (!string.Equals(convertedMd5, etag, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidETagException();
                    }
                }
            }
        }

        /// <inheritdoc />
        public void GetObjectSaveToFile(string container, string saveDirectory, string objectName, string fileName = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (saveDirectory == null)
                throw new ArgumentNullException("saveDirectory");
            if (objectName == null)
                throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(saveDirectory))
                throw new ArgumentException("saveDirectory cannot be empty");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("objectName cannot be empty");
            if (chunkSize < 0)
                throw new ArgumentOutOfRangeException("chunkSize");
            CheckIdentity(identity);

            if (string.IsNullOrEmpty(fileName))
                fileName = objectName;

            var filePath = Path.Combine(saveDirectory, string.IsNullOrEmpty(fileName) ? objectName : fileName);

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
        public void CopyObject(string sourceContainer, string sourceObjectName, string destinationContainer, string destinationObjectName, string destinationContentType = null, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (sourceContainer == null)
                throw new ArgumentNullException("sourceContainer");
            if (sourceObjectName == null)
                throw new ArgumentNullException("sourceObjectName");
            if (destinationContainer == null)
                throw new ArgumentNullException("destinationContainer");
            if (destinationObjectName == null)
                throw new ArgumentNullException("destinationObjectName");
            if (string.IsNullOrEmpty(sourceContainer))
                throw new ArgumentException("sourceContainer cannot be empty");
            if (string.IsNullOrEmpty(sourceObjectName))
                throw new ArgumentException("sourceObjectName cannot be empty");
            if (string.IsNullOrEmpty(destinationContainer))
                throw new ArgumentException("destinationContainer cannot be empty");
            if (string.IsNullOrEmpty(destinationObjectName))
                throw new ArgumentException("destinationObjectName cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(sourceContainer);
            _cloudFilesValidator.ValidateObjectName(sourceObjectName);

            _cloudFilesValidator.ValidateContainerName(destinationContainer);
            _cloudFilesValidator.ValidateObjectName(destinationObjectName);

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), _encodeDecodeProvider.UrlEncode(sourceContainer), _encodeDecodeProvider.UrlEncode(sourceObjectName)));

            if (headers == null)
                headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            headers.Add(Destination, string.Format("{0}/{1}", destinationContainer, destinationObjectName));

            RequestSettings settings = BuildDefaultRequestSettings();
            if (destinationContentType != null)
            {
                settings.ContentType = destinationContentType;
            }
            else
            {
                // make sure to preserve the content type during the copy operation
                Dictionary<string, string> sourceHeaders = GetObjectHeaders(sourceContainer, sourceObjectName, region, useInternalUrl, identity);
                string contentType;
                if (sourceHeaders.TryGetValue("Content-Type", out contentType))
                    settings.ContentType = contentType;
            }

            ExecuteRESTRequest(identity, urlPath, HttpMethod.COPY, headers: headers, settings: settings);
        }

        /// <inheritdoc />
        public void DeleteObject(string container, string objectName, Dictionary<string, string> headers = null, bool deleteSegments = true, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (objectName == null)
                throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("objectName cannot be empty");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);

            Dictionary<string, string> objectHeader = null;
            if(deleteSegments)
                objectHeader = GetObjectHeaders(container, objectName, region, useInternalUrl, identity);

            if (deleteSegments && objectHeader != null && objectHeader.Any(h => h.Key.Equals(ObjectManifest, StringComparison.OrdinalIgnoreCase)))
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
        }

        /// <summary>
        /// Deletes a collection of objects from a container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objects">A names of objects to delete.</param>
        /// <param name="headers">A collection of custom HTTP headers to include with the request.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="objects"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="objects"/> contains any null or empty values.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objects"/> contains an item that is not a valid object name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/Bulk_Delete-d1e2338.html">Bulk Delete (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public void DeleteObjects(string container, IEnumerable<string> objects, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (objects == null)
                throw new ArgumentNullException("objects");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");

            BulkDelete(objects.Select(o => new KeyValuePair<string, string>(container, o)), headers, region, useInternalUrl, identity);
        }

        /// <summary>
        /// Deletes a collection of objects stored in object storage.
        /// </summary>
        /// <param name="items">The collection of items to delete. The keys of each pair specifies the container name, and the value specifies the object name.</param>
        /// <param name="headers">A collection of custom HTTP headers to include with the request.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="items"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="items"/> contains any values with null or empty keys or values.
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="items"/> contains a pair where the key is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="items"/> contains a pair where the value is not a valid object name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/Bulk_Delete-d1e2338.html">Bulk Delete (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public void BulkDelete(IEnumerable<KeyValuePair<string, string>> items, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/?bulk-delete", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));

            var encoded = items.Select(
                pair =>
                {
                    if (string.IsNullOrEmpty(pair.Key))
                        throw new ArgumentException("items", "items cannot contain any entries with a null or empty key (container name)");
                    if (string.IsNullOrEmpty(pair.Value))
                        throw new ArgumentException("items", "items cannot contain any entries with a null or empty value (object name)");
                    _cloudFilesValidator.ValidateContainerName(pair.Key);
                    _cloudFilesValidator.ValidateObjectName(pair.Value);

                    return string.Format("/{0}/{1}", _encodeDecodeProvider.UrlEncode(pair.Key), _encodeDecodeProvider.UrlEncode(pair.Value));
                });
            var body = string.Join("\n", encoded);

            var response = ExecuteRESTRequest<BulkDeleteResponse>(identity, urlPath, HttpMethod.DELETE, body: body, headers: headers, settings: new JsonRequestSettings { ContentType = "text/plain" });

            Status status;
            if (_statusParser.TryParse(response.Data.Status, out status))
            {
                if (status.Code != 200 && !response.Data.Errors.Any())
                {
                    response.Data.AllItems = encoded;
                    throw new BulkDeletionException(response.Data.Status, _bulkDeletionResultMapper.Map(response.Data));
                }
            }
        }

        /// <inheritdoc />
        public void MoveObject(string sourceContainer, string sourceObjectName, string destinationContainer, string destinationObjectName, string destinationContentType = null, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            CopyObject(sourceContainer, sourceObjectName, destinationContainer, destinationObjectName, destinationContentType, headers, region, useInternalUrl, identity);
            DeleteObject(sourceContainer, sourceObjectName, headers, true, region, useInternalUrl, identity);
        }

        /// <inheritdoc />
        public void PurgeObjectFromCDN(string container, string objectName, IEnumerable<string> emails = null, string region = null, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (objectName == null)
                throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("objectName cannot be empty");
            if (emails != null && emails.Any(string.IsNullOrEmpty))
                throw new ArgumentException("emails cannot contain any null or empty values");

            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);
            VerifyContainerIsCDNEnabled(container, region, identity);

            string email = emails != null ? string.Join(",", emails) : null;
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(email))
            {
                headers[CdnPurgeEmail] = email;
            }

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFilesCDN(identity, region), _encodeDecodeProvider.UrlEncode(container), _encodeDecodeProvider.UrlEncode(objectName)));
            ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE, headers: headers);
        }

        #endregion

        #region Accounts

        /// <inheritdoc />
        public Dictionary<string, string> GetAccountHeaders(string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            CheckIdentity(identity);

            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersHeaderKey];

        }

        /// <inheritdoc />
        public Dictionary<string, string> GetAccountMetaData(string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            CheckIdentity(identity);

            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesMetadataProcessor.ProcessMetadata(response.Headers);

            return processedHeaders[ProcessedHeadersMetadataKey];
        }

        /// <inheritdoc />
        public void UpdateAccountMetadata(Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");
            CheckIdentity(identity);

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, string> m in metadata)
            {
                if (string.IsNullOrEmpty(m.Key))
                    throw new ArgumentException("metadata keys cannot be null or empty");
                if (m.Key.Contains('_'))
                    throw new NotSupportedException("This provider does not support metadata keys containing an underscore.");
                if (m.Key.Contains('\''))
                    throw new NotSupportedException("This provider does not support metadata keys containing an apostrophe.");

                headers.Add(AccountMetaDataPrefix + m.Key, EncodeUnicodeValue(m.Value));
            }

            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl)));
            ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        private static string EncodeUnicodeValue(string value)
        {
            if (value == null)
                return null;

            return Encoding.GetEncoding("ISO-8859-1").GetString(Encoding.UTF8.GetBytes(value));
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the public or internal service endpoint to use for Cloud Files requests for the specified identity and region.
        /// </summary>
        /// <remarks>
        /// This method uses <c>object-store</c> for the service type, and <c>cloudFiles</c> for the preferred service name.
        /// </remarks>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="region">The preferred region for the service. If this value is <c>null</c>, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the internal service endpoint; otherwise <c>false</c> to use the public service endpoint.</param>
        /// <returns>The URL for the requested Cloud Files endpoint.</returns>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="NoDefaultRegionSetException">If <paramref name="region"/> is <c>null</c> and no default region is available for the identity or provider.</exception>
        /// <exception cref="UserAuthenticationException">If no service catalog is available for the user.</exception>
        /// <exception cref="UserAuthorizationException">If no endpoint is available for the requested service.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected string GetServiceEndpointCloudFiles(CloudIdentity identity, string region = null, bool useInternalUrl = false)
        {
            return useInternalUrl ? base.GetInternalServiceEndpoint(identity, "object-store", "cloudFiles", region) : base.GetPublicServiceEndpoint(identity, "object-store", "cloudFiles", region);
        }

        /// <summary>
        /// Gets the public service endpoint to use for Cloud Files CDN requests for the specified identity and region.
        /// </summary>
        /// <remarks>
        /// This method uses <c>rax:object-cdn</c> for the service type, and <c>cloudFilesCDN</c> for the preferred service name.
        /// </remarks>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <param name="region">The preferred region for the service. If this value is <c>null</c>, the user's default region will be used.</param>
        /// <returns>The public URL for the requested Cloud Files CDN endpoint.</returns>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// </exception>
        /// <exception cref="NoDefaultRegionSetException">If <paramref name="region"/> is <c>null</c> and no default region is available for the identity or provider.</exception>
        /// <exception cref="UserAuthenticationException">If no service catalog is available for the user.</exception>
        /// <exception cref="UserAuthorizationException">If no endpoint is available for the requested service.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected string GetServiceEndpointCloudFilesCDN(CloudIdentity identity, string region = null)
        {
            return base.GetPublicServiceEndpoint(identity, "rax:object-cdn", "cloudFilesCDN", region);
        }

        /// <summary>
        /// Copy data from an input stream to an output stream.
        /// </summary>
        /// <remarks>
        /// The argument to the callback method is the total number of bytes written to the output stream thus far.
        /// Note that <see cref="Stream.Flush()"/> is not called on <paramref name="output"/> prior to reporting a
        /// progress update, so data may remain in the stream's buffer.
        /// </remarks>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="bufferSize">The size of the buffer to use for copying data.</param>
        /// <param name="progressUpdated">A callback for progress updates. If the value is <c>null</c>, no progress updates are reported.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="input"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="output"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="bufferSize"/> is less than or equal to 0.</exception>
        public static void CopyStream(Stream input, Stream output, int bufferSize, Action<long> progressUpdated)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (output == null)
                throw new ArgumentNullException("output");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize");

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
        /// Creates an object consisting of multiple segments, each no larger than
        /// <see cref="LargeFileBatchThreshold"/>, using data from a <see cref="Stream"/>.
        /// If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <remarks>
        /// In addition to the individual segments containing file data, this method creates
        /// the manifest required for treating the segments as a single object in future GET
        /// requests.
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="stream">A <see cref="Stream"/> providing the data for the file.</param>
        /// <param name="objectName">The destination object name. Example <localUri>image_name.jpeg</localUri></param>
        /// <param name="contentType">The content type of the created object. If the value is <c>null</c> or empty, the content type of the created object is unspecified.</param>
        /// <param name="chunkSize">The buffer size to use for copying streaming data.</param>
        /// <param name="headers">A collection of custom HTTP headers to associate with the object (see <see cref="GetObjectHeaders"/>).</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="progressUpdated">A callback for progress updates. If the value is <c>null</c>, no progress updates are reported.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="stream"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objectName"/> is not a valid object name.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="chunkSize"/> is less than 0.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/create-update-object.html">Create or Update Object (OpenStack Object Storage API v1 Reference)</seealso>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/large-object-creation.html">Create Large Objects (OpenStack Object Storage API v1 Reference)</seealso>
        private void CreateObjectInSegments(string container, Stream stream, string objectName, string contentType = null, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (objectName == null)
                throw new ArgumentNullException("objectName");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("objectName cannot be empty");
            if (chunkSize < 0)
                throw new ArgumentOutOfRangeException("chunkSize");
            CheckIdentity(identity);

            _cloudFilesValidator.ValidateContainerName(container);
            _cloudFilesValidator.ValidateObjectName(objectName);

            long totalLength = stream.Length - stream.Position;
            long segmentCount = (totalLength / LargeFileBatchThreshold) + (((totalLength % LargeFileBatchThreshold) != 0) ? 1 : 0);

            long totalBytesWritten = 0;
            for (int i = 0; i < segmentCount; i++)
            {
                // the total amount of data left to write
                long remaining = (totalLength - LargeFileBatchThreshold * i);
                // the size of the current segment
                long length = Math.Min(remaining, LargeFileBatchThreshold);

                Uri urlPath = new Uri(string.Format("{0}/{1}/{2}.seg{3}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container, objectName, i.ToString("0000")));
                long segmentBytesWritten = 0;

                RequestSettings settings = BuildDefaultRequestSettings();
                settings.ChunkRequest = true;
                settings.ContentType = contentType;

                StreamRESTRequest(identity, urlPath, HttpMethod.PUT, stream, chunkSize, length, headers: headers, requestSettings: settings, progressUpdated:
                    bytesWritten =>
                    {
                        if (progressUpdated != null)
                        {
                            segmentBytesWritten = bytesWritten;
                            progressUpdated(totalBytesWritten + segmentBytesWritten);
                        }
                    });

                totalBytesWritten += length;
            }

            // upload the manifest file
            Uri segmentUrlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region, useInternalUrl), container, objectName));

            if (headers == null)
                headers = new Dictionary<string, string>();

            headers.Add(ObjectManifest, string.Format("{0}/{1}", container, objectName));

            RequestSettings requestSettings = BuildDefaultRequestSettings();
            requestSettings.ChunkRequest = true;
            requestSettings.ContentType = contentType;

            StreamRESTRequest(identity, segmentUrlPath, HttpMethod.PUT, new MemoryStream(new Byte[0]), chunkSize, headers: headers, requestSettings: requestSettings, progressUpdated:
                (bytesWritten) =>
                {
                    if (progressUpdated != null)
                    {
                        progressUpdated(totalBytesWritten);
                    }
                });
        }

        /// <summary>
        /// Verifies that a particular container is CDN-enabled.
        /// </summary>
        /// <remarks>
        /// Normally, the <see cref="ContainerCDN.CDNEnabled"/> property is used to check if a container is
        /// CDN-enabled. However, if a container has <em>never</em> been CDN-enabled, the
        /// <see cref="GetContainerCDNHeader"/> method throws a misleading <see cref="ItemNotFoundException"/>.
        /// This method uses <see cref="GetContainerHeader"/> to distinguish between these cases, ensuring
        /// that a <see cref="CDNNotEnabledException"/> gets thrown whenever a container exists but is not
        /// CDN-enabled.
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="container"/> is empty.</exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="CDNNotEnabledException">If the container does not have a CDN header, or if the <see cref="ContainerCDN.CDNEnabled"/> property is <c>false</c>.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        protected void VerifyContainerIsCDNEnabled(string container, string region, CloudIdentity identity)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (string.IsNullOrEmpty(container))
                throw new ArgumentException("container cannot be empty");
            _cloudFilesValidator.ValidateContainerName(container);
            CheckIdentity(identity);

            try
            {
                // If the container is currently CDN enabled, or was CDN enabled at some
                // point in the past, GetContainerCDNHeader returns non-null and the CDNEnabled
                // property determines whether or not the container is currently CDN enabled.
                if (!GetContainerCDNHeader(container, region, identity).CDNEnabled)
                {
                    throw new CDNNotEnabledException("The specified container is not CDN-enabled.");
                }
            }
            catch (ItemNotFoundException ex)
            {
                // In response to an ItemNotFoundException, the GetContainerHeader method is used
                // to distinguish between cases where the container does not exist (or is not
                // accessible), and cases where the container exists but has never been CDN enabled.
                GetContainerHeader(container, region, false, identity);

                // If we get to this line, we know the container exists but has never been CDN enabled.
                throw new CDNNotEnabledException("The specified container is not CDN-enabled.", ex);
            }
        }

        #endregion

        #region constants

        #region Headers

        #region Auth Constants

        /// <summary>
        /// The X-Auth-Token header, which specifies the token to use for authenticated requests.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/authentication-object-dev-guide.html">Authentication (OpenStack Object Storage API v1 Reference)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/Authentication-d1e639.html">Authentication (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string AuthToken = "x-auth-token";

        /// <summary>
        /// The X-Cdn-Management-Url header.
        /// <note type="warning">The value of this header is not defined. Do not use.</note>
        /// </summary>
        public const string CdnManagementUrl = "x-cdn-management-url";

        /// <summary>
        /// The X-Storage-Url header, which specifies the base URI for all object storage requests.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/authentication-object-dev-guide.html">Authentication (OpenStack Object Storage API v1 Reference)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/Authentication-d1e639.html">Authentication (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string StorageUrl = "x-storage-url";

        #endregion

        #region Account Constants

        /// <summary>
        /// The X-Account-Meta- header prefix, which specifies the HTTP header prefix for metadata keys associated with an account.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/create-update-account-metadata.html">Create or Update Account Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        public const string AccountMetaDataPrefix = "x-account-meta-";

        /// <summary>
        /// The X-Account-Bytes-Used header, which specifies total storage used by an account in bytes.
        /// <note type="warning">The value of this property is not defined by OpenStack, and may not be consistent across vendors.</note>
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/retrieve-account-metadata.html">Get Account Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/View_Account_Details-d1e108.html">View Account Details (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string AccountBytesUsed = "x-account-bytes-used";

        /// <summary>
        /// The X-Account-Container-Count header, which specifies the number of containers associated with an account.
        /// <note type="warning">The value of this property is not defined by OpenStack, and may not be consistent across vendors.</note>
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/retrieve-account-metadata.html">Get Account Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/View_Account_Details-d1e108.html">View Account Details (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string AccountContainerCount = "x-account-container-count";

        /// <summary>
        /// The X-Account-Object-Count header, which specifies the number of objects associated with an account.
        /// <note type="warning">The value of this property is not defined by OpenStack, and may not be consistent across vendors.</note>
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/listing-and-creating-storage-containers.html">Listing and Creating Containers (OpenStack Object Storage API v1 Reference)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/View_Account_Details-d1e108.html">View Account Details (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string AccountObjectCount = "x-account-object-count";

        #endregion

        #region Container Constants

        /// <summary>
        /// The X-Container-Meta- header prefix, which specifies the HTTP header prefix for metadata keys associated with a container.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Update_Container_Metadata-d1e1900.html">Create or Update Container Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        public const string ContainerMetaDataPrefix = "x-container-meta-";

        /// <summary>
        /// The X-Remove-Container-Meta- header prefix, which specifies the HTTP header prefix for removing metadata keys from a container.
        /// </summary>
        /// <remarks>
        /// This value is not required in the .NET SDK, since a shorter way to remove metadata is to simply assign an empty string as the value for a metadata key.
        /// </remarks>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/delete-container-metadata.html">Delete Container Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        public const string ContainerRemoveMetaDataPrefix = "x-remove-container-meta-";

        /// <summary>
        /// The X-Container-Bytes-Used header, which specifies the total size of all objects stored in a container.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/retrieve-container-metadata.html">Get Container Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        public const string ContainerBytesUsed = "x-container-bytes-used";

        /// <summary>
        /// The X-Container-Object-Count header, which specifies the total number of objects stored in a container.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/retrieve-container-metadata.html">Get Container Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        public const string ContainerObjectCount = "x-container-object-count";

        /// <summary>
        /// The Web-Index metadata key, which specifies the index page for every pseudo-directory in a website.
        /// </summary>
        /// <remarks>
        /// If your pseudo-directory does not have a file with the same name as your index file, visits to the sub-directory return a 404 error.
        /// </remarks>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Create_Static_Website-dle4000.html">Create Static Website (OpenStack Object Storage API v1 Reference - API v1)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/Create_Static_Website-dle4000.html">Create Static Website (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string WebIndex = "web-index";

        /// <summary>
        /// The Web-Error metadata key, which specifies the suffix for error pages displayed for a website.
        /// </summary>
        /// <remarks>
        /// You may create and set custom error pages for visitors to your website; currently, only
        /// 401 (Unauthorized) and 404 (Not Found) errors are supported. To do this, set the metadata
        /// value <see cref="WebError"/>.
        ///
        /// <para>
        /// Error pages are served with the &lt;status&gt; code prepended to the name of the error
        /// page you set. For instance, if you set <see cref="WebError"/> to <fictitiousUri>error.html</fictitiousUri>,
        /// 401 errors will display the page <fictitiousUri>401error.html</fictitiousUri>. Similarly, 404
        /// errors will display <fictitiousUri>404error.html</fictitiousUri>. You must have both of these
        /// pages created in your container when you set the <see cref="WebError"/> metadata, or your site
        /// will display generic error pages.
        /// </para>
        ///
        /// <para>
        /// You need only set the <see cref="WebError"/> metadata once for your entire static website.
        /// </para>
        /// </remarks>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Set_Error_Pages_for_Static_Website-dle4005.html">Set Error Pages for Static Website (OpenStack Object Storage API v1 Reference - API v1)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/Set_Error_Pages_for_Static_Website-dle4005.html">Set Error Pages for Static Website (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string WebError = "web-error";

        /// <summary>
        /// The Web-Listings metadata key, which specifies whether or not pseudo-directories should
        /// display a list of files instead of returning a 404 error when the pseudo-directory does
        /// not contain an index file.
        /// </summary>
        /// <remarks>
        /// To display a list of files in pseudo-directories instead of an index, set the
        /// <see cref="WebListings"/> metadata value to <c>"TRUE"</c> for a container.
        /// </remarks>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Create_Static_Website-dle4000.html">Create Static Website (OpenStack Object Storage API v1 Reference - API v1)</seealso>
        public const string WebListings = "web-listings";

        /// <summary>
        /// The Web-Listings-CSS metadata key, which specifies the stylesheet to use for file listings
        /// when <see cref="WebListings"/> is <c>true</c> and a pseudo-directory does not contain an
        /// index file.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Create_Static_Website-dle4000.html">Create Static Website (OpenStack Object Storage API v1 Reference - API v1)</seealso>
        public const string WebListingsCSS = "web-listings-css";

        /// <summary>
        /// The X-Versions-Location header, which specifies the name of the container where previous
        /// versions of objects are stored for a container.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Object_Versioning-e1e3230.html">Object Versioning (OpenStack Object Storage API v1 Reference - API v1)</seealso>
        public const string VersionsLocation = "x-versions-location";

        #endregion

        #region CDN Container Constants

        /// <summary>
        /// The X-Cdn-Uri header, which specifies the publicly-available URL
        /// for a CDN-enabled container.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This header is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// </remarks>
        /// <seealso cref="ContainerCDN.CDNUri"/>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN_Container_Services-d1e2632.html">CDN Container Services (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string CdnUri = "x-cdn-uri";

        /// <summary>
        /// The X-Cdn-Ssl-Uri header, which specifies the publicly-available
        /// URL for SSL access to a CDN-enabled container.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This header is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// </remarks>
        /// <seealso cref="ContainerCDN.CDNSslUri"/>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN_Container_Services-d1e2632.html">CDN Container Services (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string CdnSslUri = "x-cdn-ssl-uri";

        /// <summary>
        /// The X-Cdn-Streaming-Uri header, which specifies the publicly-available
        /// URL for streaming access to a CDN-enabled container.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This header is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// </remarks>
        /// <seealso cref="ContainerCDN.CDNStreamingUri"/>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN_Container_Services-d1e2632.html">CDN Container Services (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string CdnStreamingUri = "x-cdn-streaming-uri";

        /// <summary>
        /// The X-Ttl header, which specifies the Time To Live (TTL) in seconds for a CDN-enabled container.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This header is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// </remarks>
        /// <seealso cref="ContainerCDN.Ttl"/>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN-Enable_a_Container-d1e2665.html">CDN-Enable a Container (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string CdnTTL = "x-ttl";

        /// <summary>
        /// The X-Log-Retention header, which specifies whether or not log retention is enabled for a CDN-enabled container.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This header is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// </remarks>
        /// <seealso cref="ContainerCDN.LogRetention"/>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/List_CDN-Enabled_Container_Metadata-d1e2711.html">List a CDN-Enabled Container's Metadata (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string CdnLogRetention = "x-log-retention";

        /// <summary>
        /// The X-Cdn-Enabled header, which specifies whether or not a container is CDN-enabled.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This header is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// </remarks>
        /// <seealso cref="ContainerCDN.CDNEnabled"/>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN-Enable_a_Container-d1e2665.html">CDN-Enable a Container (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string CdnEnabled = "x-cdn-enabled";

        /// <summary>
        /// The X-Cdn-Ios-Uri header, which specifies the publicly-available URL for
        /// iOS streaming access to a CDN-enabled container.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This header is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// </remarks>
        /// <seealso cref="ContainerCDN.CDNIosUri"/>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/iOS-Streaming-d1f3725.html">iOS Streaming (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string CdnIosUri = "x-cdn-ios-uri";

        #endregion

        #region Object Constants

        /// <summary>
        /// The X-Object-Meta- header prefix, which specifies the HTTP header prefix for metadata keys associated with an object.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/update-object-metadata.html">Update Object Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        public const string ObjectMetaDataPrefix = "x-object-meta-";

        /// <summary>
        /// The X-Delete-After header, which specifies the relative time (in seconds
        /// from "now") after which an object should expire, not be served, and be
        /// deleted completely from the storage system.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Expiring_Objects-e1e3228.html">Expiring Objects with the X-Delete-After and X-Delete-At Headers (OpenStack Object Storage API v1 Reference)</seealso>
        public const string ObjectDeleteAfter = "x-delete-after";

        /// <summary>
        /// The X-Delete-At header, which specifies the absolute time (in Unix Epoch
        /// format) after which an object should expire, not be served, and be deleted
        /// completely from the storage system.
        /// </summary>
        /// <remarks>
        /// Unix time is specified as the number of seconds elapsed since 00:00:00 UTC,
        /// 1 January 1970, not counting leap seconds.
        /// </remarks>
        /// <seealso href="http://en.wikipedia.org/wiki/Unix_time">Unix time (Wikipedia)</seealso>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Expiring_Objects-e1e3228.html">Expiring Objects with the X-Delete-After and X-Delete-At Headers (OpenStack Object Storage API v1 Reference)</seealso>
        public const string ObjectDeleteAt = "x-delete-at";

        /// <summary>
        /// The ETag header, which specifies the MD5 checksum of the data in an object stored in Object Storage.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/create-update-object.html">Create or Update Object (OpenStack Object Storage API v1 Reference)</seealso>
        public const string Etag = "etag";

        /// <summary>
        /// The Destination header, which specifies the destination container and object
        /// name for a Copy Object operation.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/copy-object.html">Copy Object (OpenStack Object Storage API v1 Reference)</seealso>
        public const string Destination = "destination";

        /// <summary>
        /// The X-Object-Manifest header, which specifies the container and prefix for the segments of a
        /// large object.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/large-object-creation.html">Create Large Objects (OpenStack Object Storage API v1 Reference)</seealso>
        public const string ObjectManifest = "x-object-manifest";

        #endregion

        #region CDN Object Constants

        /// <summary>
        /// The X-Purge-Email header, which specifies the comma-separated list of email addresses to notify when a CDN purge request completes.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This header is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// </remarks>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/Purge_CDN-Enabled_Objects-d1e3858.html">Purge CDN-Enabled Objects (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public const string CdnPurgeEmail = "x-purge-email";

        #endregion

        #endregion

        /// <summary>
        /// The maximum value of <see cref="LargeFileBatchThreshold"/> supported by this provider.
        /// This value is set to the minimum value for which creation of a single object larger than
        /// the value may result in the server closing the TCP/IP connection and purging the object's
        /// data.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/large-object-creation.html">Create Large Objects (OpenStack Object Storage API v1 Reference)</seealso>
        public static readonly long MaxLargeFileBatchThreshold = 5368709120; // 5GB

        /// <summary>
        /// This is the backing field for <see cref="LargeFileBatchThreshold"/>. The
        /// default value is <see cref="MaxLargeFileBatchThreshold"/>.
        /// </summary>
        private long _largeFileBatchThreshold = MaxLargeFileBatchThreshold;

        /// <summary>
        /// Gets or sets the maximum allowable size of a single object stored in this provider.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">If <paramref name="value"/> exceeds <see cref="MaxLargeFileBatchThreshold"/>.</exception>
        public long LargeFileBatchThreshold
        {
            get
            {
                return _largeFileBatchThreshold;
            }

            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                if (value > MaxLargeFileBatchThreshold)
                    throw new ArgumentException(string.Format("The large file threshold cannot exceed the provider's maximum value {0}", MaxLargeFileBatchThreshold), "value");

                _largeFileBatchThreshold = value;
            }
        }

        /// <summary>
        /// This value is used as the key for storing metadata information in the dictionary
        /// returned by <see cref="CloudFilesMetadataProcessor.ProcessMetadata"/>.
        /// </summary>
        /// <seealso cref="CloudFilesMetadataProcessor"/>
        public const string ProcessedHeadersMetadataKey = "metadata";

        /// <summary>
        /// This value is used as the key for storing non-metadata header information in the
        /// dictionary returned by <see cref="CloudFilesMetadataProcessor.ProcessMetadata"/>.
        /// </summary>
        /// <seealso cref="CloudFilesMetadataProcessor"/>
        public const string ProcessedHeadersHeaderKey = "headers";

        #endregion
 
    }
}
