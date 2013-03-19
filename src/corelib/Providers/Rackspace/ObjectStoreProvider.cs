using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;

namespace net.openstack.Providers.Rackspace
{
    public class ObjectStoreProvider : ProviderBase, IObjectStoreProvider
    {
        private readonly IObjectStoreHelper _objectStoreHelper;
        #region Constructors

        public ObjectStoreProvider()
            : this(new IdentityProvider(), new JsonRestServices(), new ObjectStoreHelper()) { }

        public ObjectStoreProvider(IIdentityProvider identityProvider, IRestService restService, IObjectStoreHelper objectStoreValidator)
            : base(identityProvider, restService)
        {
            _objectStoreHelper = objectStoreValidator;
        }


        #endregion

        #region Containers

        public IEnumerable<Container> ListContainers(CloudIdentity identity, int? limit = null, string marker = null, string markerEnd = null, string format = "json", string region = null)
        {
            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region)));

            var queryStringParameter = new Dictionary<string, string>();
            queryStringParameter.Add("format", format);

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

        public ObjectStore CreateContainer(CloudIdentity identity, string container, string region = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT);

            if (response.StatusCode == 201)
                return ObjectStore.ContainerCreated;
            if (response.StatusCode == 202)
                return ObjectStore.ContainerExists;

            return ObjectStore.Unknown;
        }

        public ObjectStore DeleteContainer(CloudIdentity identity, string container, string region = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            if (response.StatusCode == 204)
                return ObjectStore.ContainerDeleted;
            if (response.StatusCode == 404)
                return ObjectStore.ContainerNotFound;
            if (response.StatusCode == 409)
                return ObjectStore.ContainerNotEmpty;

            return ObjectStore.Unknown;
        }


        public Dictionary<string, string> GetHeaderForContainer(CloudIdentity identity, string container, string region = null, bool useInternalUrl = false)
        {
            _objectStoreHelper.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.GET); // Should be HEAD

            var processedHeaders = _objectStoreHelper.ProcessMetadata(response.Headers);

            return processedHeaders[ObjectStoreConstants.ProcessedHeadersHeaderKey];
        }

        public Dictionary<string, string> GetMetaDataForContainer(CloudIdentity identity, string container, string region = null, bool useInternalUrl = false)
        {
            _objectStoreHelper.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.GET); // Should be HEAD

            var processedHeaders = _objectStoreHelper.ProcessMetadata(response.Headers);

            return processedHeaders[ObjectStoreConstants.ProcessedHeadersMetadataKey];
        }

        public Dictionary<string, string> GetCDNHeaderForContainer(CloudIdentity identity, string container, string region = null, bool useInternalUrl = false)
        {
            _objectStoreHelper.ValidateContainerName(container);


            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            return response.Headers.ToDictionary(header => header.Key, header => header.Value);
        }

        public void AddContainerMetadata(CloudIdentity identity, string container, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false)
        {
            _objectStoreHelper.ValidateContainerName(container);
            if (metadata.Equals(null))
            {
                throw new ArgumentNullException();
            }

            var headers = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> m in metadata)
            {
                if (m.Key.Contains(ObjectStoreConstants.ContainerMetaDataPrefix))
                {
                    headers.Add(m.Key, m.Value);
                }
                else
                {
                    headers.Add(ObjectStoreConstants.ContainerMetaDataPrefix + m.Key, m.Value);
                }
            }

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, null, null, headers);
        }

        public void AddContainerHeaders(CloudIdentity identity, string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false)
        {
            _objectStoreHelper.ValidateContainerName(container);
            if (headers == null)
            {
                throw new ArgumentNullException();
            }

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, null, null, headers);
        }

        public void AddContainerCdnHeaders(CloudIdentity identity, string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false)
        {
            _objectStoreHelper.ValidateContainerName(container);
            if (headers == null)
            {
                throw new ArgumentNullException();
            }

            bool cdnEnabled = false;

            var cdnHeaders = GetCDNHeaderForContainer(identity, container);
            if (cdnHeaders.ContainsKey(ObjectStoreConstants.CdnEnabled))
            {
                cdnEnabled = bool.Parse(cdnHeaders.FirstOrDefault(x => x.Key.Equals(ObjectStoreConstants.CdnEnabled, StringComparison.InvariantCultureIgnoreCase)).Value);
            }

            if (!cdnEnabled)
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));
                ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, null, null, headers);
            }
        }

        public IEnumerable<ContainerCDN> ListCDNContainers(CloudIdentity identity, int? limit = null, string marker = null, string markerEnd = null, bool cdnEnabled = false, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFilesCDN(identity, region)));

            var queryStringParameter = new Dictionary<string, string>();
            queryStringParameter.Add("format", "json");
            queryStringParameter.Add("enabled_only", cdnEnabled.ToString().ToLower());

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

        public Dictionary<string, string> EnableCDNOnContainer(CloudIdentity identity, string container, long ttl, string region = null)
        {
            return EnableCDNOnContainer(identity, container, ttl, false);
        }

        public Dictionary<string, string> EnableCDNOnContainer(CloudIdentity identity, string container, bool logRetention, string region = null)
        {
            return EnableCDNOnContainer(identity, container, 259200, logRetention, region);
        }

        public Dictionary<string, string> EnableCDNOnContainer(CloudIdentity identity, string container, long ttl, bool logRetention, string region = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
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
                 {ObjectStoreConstants.CdnTTL, ttl.ToString(CultureInfo.InvariantCulture)},
                 {ObjectStoreConstants.CdnLogRetention, logRetention.ToString(CultureInfo.InvariantCulture)},
                 {ObjectStoreConstants.CdnEnabled, "true"}
                };
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, null, null, headers);

            if (response == null)
                return null;

            return response.Headers.ToDictionary(header => header.Key, header => header.Value);
        }

        public Dictionary<string, string> DisableCDNOnContainer(CloudIdentity identity, string container, string region = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
           

            var headers = new Dictionary<string, string>
                {
                {ObjectStoreConstants.CdnEnabled, "false"}
                };
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, null, null, headers);

            if (response == null)
                return null;

            return response.Headers.ToDictionary(header => header.Key, header => header.Value);
        }


        #endregion

        #region Container Objects

        public IEnumerable<ContainerObject> GetObjects(CloudIdentity identity, string container, int? limit = null, string marker = null, string markerEnd = null, string format = "json", string region = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var queryStringParameter = new Dictionary<string, string>();
            queryStringParameter.Add("format", format);

            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (!string.IsNullOrWhiteSpace(marker))
                queryStringParameter.Add("marker", marker);

            if (!string.IsNullOrWhiteSpace(markerEnd))
                queryStringParameter.Add("end_marker", markerEnd);

            var response = ExecuteRESTRequest<ContainerObject[]>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            //if (response.StatusCode == 204)


            if (response == null || response.Data == null)
                return null;

            return response.Data;
        }

        public IEnumerable<HttpHeader> GetObjectHeaders(CloudIdentity identity, string container, string objectName, string format = "json", string region = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            _objectStoreHelper.ValidateObjectName(objectName);
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region), container, objectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.GET);
            if (response == null)
                return null;

            return response.Headers;
        }

        #endregion

        #region Private methods

        protected string GetServiceEndpointCloudFiles(CloudIdentity identity, string region = null)
        {
            return base.GetServiceEndpoint(identity, "cloudFiles", region);
        }

        protected string GetServiceEndpointCloudFilesCDN(CloudIdentity identity, string region = null)
        {
            return base.GetServiceEndpoint(identity, "cloudFilesCDN", region);
        }

        #endregion
    }
}
