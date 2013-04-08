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
    public class ObjectStoreProvider : ProviderBase, IObjectStoreProvider
    {
        private readonly IObjectStoreHelper _objectStoreHelper;
        #region Constructors

        public ObjectStoreProvider()
            : this(null) { }

        public ObjectStoreProvider(CloudIdentity defaultIdentity)
            : this(defaultIdentity, new IdentityProvider(), new JsonRestServices(), new ObjectStoreHelper()) { }

        public ObjectStoreProvider(IIdentityProvider identityProvider, IRestService restService, IObjectStoreHelper objectStoreHelper)
            : this(null, identityProvider, restService, objectStoreHelper) { }

        public ObjectStoreProvider(CloudIdentity defaultIdentity, IIdentityProvider identityProvider, IRestService restService, IObjectStoreHelper objectStoreHelper)
            : base(defaultIdentity, identityProvider, restService)
        {
            _objectStoreHelper = objectStoreHelper;
        }


        #endregion

        #region Containers

        public IEnumerable<Container> ListContainers(int? limit = null, string marker = null, string markerEnd = null, string format = "json", string region = null, CloudIdentity identity = null)
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

        public ObjectStore CreateContainer(string container, string region = null, CloudIdentity identity = null)
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

        public ObjectStore DeleteContainer(string container, string region = null, CloudIdentity identity = null)
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

        public Dictionary<string, string> GetContainerHeader(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _objectStoreHelper.ProcessMetadata(response.Headers);

            return processedHeaders[ObjectStoreConstants.ProcessedHeadersHeaderKey];
        }

        public Dictionary<string, string> GetContainerMetaData(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.GET); // Should be HEAD

            var processedHeaders = _objectStoreHelper.ProcessMetadata(response.Headers);

            return processedHeaders[ObjectStoreConstants.ProcessedHeadersMetadataKey];
        }

        public Dictionary<string, string> GetContainerCDNHeader(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);


            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            return response.Headers.ToDictionary(header => header.Key, header => header.Value);
        }

        public void AddContainerMetadata(string container, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
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

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        public void AddContainerHeaders(string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            if (headers == null)
            {
                throw new ArgumentNullException();
            }

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        public void AddContainerCdnHeaders(string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            if (headers == null)
            {
                throw new ArgumentNullException();
            }

            if (!IsContainerCdnEnabled(container, region, useInternalUrl, identity))
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));
                ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
            }
        }

        private bool IsContainerCdnEnabled(string container, string region, bool useInternalUrl, CloudIdentity identity = null)
        {
            bool cdnEnabled = false;

            var cdnHeaders = GetContainerCDNHeader(container, region, useInternalUrl, identity: identity);
            if (cdnHeaders.ContainsKey(ObjectStoreConstants.CdnEnabled))
            {
                cdnEnabled =
                    bool.Parse(
                        cdnHeaders.FirstOrDefault(
                            x => x.Key.Equals(ObjectStoreConstants.CdnEnabled, StringComparison.InvariantCultureIgnoreCase)).
                            Value);
            }
            return cdnEnabled;
        }

        public IEnumerable<ContainerCDN> ListCDNContainers(int? limit = null, string marker = null, string markerEnd = null, bool cdnEnabled = false, string region = null, CloudIdentity identity = null)
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

        public Dictionary<string, string> EnableCDNOnContainer(string container, long ttl, string region = null, CloudIdentity identity = null)
        {
            return EnableCDNOnContainer(container, ttl, false, identity: identity);
        }

        public Dictionary<string, string> EnableCDNOnContainer(string container, bool logRetention, string region = null, CloudIdentity identity = null)
        {
            return EnableCDNOnContainer(container, 259200, logRetention, region,identity);
        }

        public Dictionary<string, string> EnableCDNOnContainer(string container, long ttl, bool logRetention, string region = null, CloudIdentity identity = null)
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

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, headers: headers);

            if (response == null)
                return null;

            return response.Headers.ToDictionary(header => header.Key, header => header.Value);
        }

        public Dictionary<string, string> DisableCDNOnContainer(string container, string region = null, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);


            var headers = new Dictionary<string, string>
                {
                {ObjectStoreConstants.CdnEnabled, "false"}
                };
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, headers: headers);

            if (response == null)
                return null;

            return response.Headers.ToDictionary(header => header.Key, header => header.Value);
        }

        public void EnableStaticWebOnContainer(string container, string index, string error, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);

            if (!IsContainerCdnEnabled(container, region, useInternalUrl, identity))
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var headers = new Dictionary<string, string>
                                  {
                                      {ObjectStoreConstants.WebIndex, index},
                                      {ObjectStoreConstants.WebError, error},
                                      {ObjectStoreConstants.WebListingsCSS, css},
                                      {ObjectStoreConstants.WebListings, listing.ToString()}
                                  };
                AddContainerHeaders(container, headers, region, useInternalUrl, identity);
            }
        }

        public void EnableStaticWebOnContainer(string container, string index, string error, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);

            if (!IsContainerCdnEnabled(container, region, useInternalUrl, identity))
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var headers = new Dictionary<string, string>
                                  {
                                      {ObjectStoreConstants.WebIndex, index},
                                      {ObjectStoreConstants.WebError, error},
                                      {ObjectStoreConstants.WebListings, listing.ToString()}
                                  };
                AddContainerHeaders(container, headers, region, useInternalUrl, identity);
            }
        }

        public void EnableStaticWebOnContainer(string container, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);

            if (!IsContainerCdnEnabled(container, region, useInternalUrl, identity))
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var headers = new Dictionary<string, string>
                                  {
                                      {ObjectStoreConstants.WebListingsCSS, css},
                                      {ObjectStoreConstants.WebListings, listing.ToString()}
                                  };
                AddContainerHeaders(container, headers, region, useInternalUrl, identity);
            }
        }

        public void EnableStaticWebOnContainer(string container, string index, string error, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);

            if (!IsContainerCdnEnabled(container, region, useInternalUrl, identity))
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var headers = new Dictionary<string, string>
                                  {
                                      {ObjectStoreConstants.WebIndex, index},
                                      {ObjectStoreConstants.WebError, error}
                                  };
                AddContainerHeaders(container, headers, region, useInternalUrl, identity);
            }
        }

        public void DisableStaticWebOnContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);

            if (!IsContainerCdnEnabled(container, region, useInternalUrl, identity))
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var headers = new Dictionary<string, string>
                                  {
                                      {ObjectStoreConstants.WebIndex, string.Empty},
                                      {ObjectStoreConstants.WebError, string.Empty},
                                      {ObjectStoreConstants.WebListingsCSS, string.Empty},
                                      {ObjectStoreConstants.WebListings, string.Empty}
                                  };
                AddContainerHeaders( container, headers, region, useInternalUrl, identity);
            }
        }

        #endregion

        #region Container Objects

        public IEnumerable<ContainerObject> GetObjects(string container, int? limit = null, string marker = null, string markerEnd = null, string format = "json", string region = null, CloudIdentity identity = null)
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

        public Dictionary<string, string> GetObjectHeaders(string container, string objectName, string format = "json", string region = null, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            _objectStoreHelper.ValidateObjectName(objectName);
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region), container, objectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _objectStoreHelper.ProcessMetadata(response.Headers);

            return processedHeaders[ObjectStoreConstants.ProcessedHeadersHeaderKey];
        }

        public void CreateObjectFromFile(string container, string filePath, string objectName, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, CloudIdentity identity = null)
        {
            using (var stream = System.IO.File.OpenRead(filePath))
            {
                CreateObjectFromStream(container, stream, objectName, chunkSize, headers, region, progressUpdated, identity);
            }
        }

        public void CreateObjectFromStream(string container, Stream stream, string objectName, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, CloudIdentity identity = null)
        {
            if (stream == null)
                throw new ArgumentNullException();

            _objectStoreHelper.ValidateContainerName(container);
            _objectStoreHelper.ValidateObjectName(objectName);

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region), container, objectName));

            var response = StreamRESTRequest(identity, urlPath, HttpMethod.PUT, stream, chunkSize, null, headers, true, null, progressUpdated);

        }

        public void GetObject(string container, string objectName, Stream outputStream, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            _objectStoreHelper.ValidateObjectName(objectName);

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region), container, objectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.GET, (resp, isError) =>
            {
                if (resp == null)
                    return new Response(0, null, null);

                try
                {
                    using (var respStream = resp.GetResponseStream())
                    {
                        CopyStream(respStream, outputStream, chunkSize);
                    }

                    var respHeaders = resp.Headers.AllKeys.Select(key => new HttpHeader() { Key = key, Value = resp.GetResponseHeader(key) }).ToList();

                    return new Response(resp.StatusCode, respHeaders, "[Binary]");
                }
                catch (Exception)
                {
                    return new Response(0, null, null);
                }
            }, headers: headers);

            if (verifyEtag && response.Headers.Any(h => h.Key.Equals(ObjectStoreConstants.Etag, StringComparison.OrdinalIgnoreCase)))
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
                if (convertedMd5 != response.Headers.First(h => h.Key.Equals(ObjectStoreConstants.Etag, StringComparison.OrdinalIgnoreCase)).Value.ToLower())
                {

                    throw new InvalidETagException();
                }
            }
        }

        public void GetObjectSaveToFile(string container, string saveDirectory, string objectName, string fileName = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, CloudIdentity identity = null)
        {
            if (String.IsNullOrEmpty(saveDirectory))
                throw new ArgumentNullException();

            var filePath = Path.Combine(saveDirectory, string.IsNullOrWhiteSpace(fileName) ? objectName : fileName);

            try
            {
                using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    GetObject(container, objectName, fileStream, chunkSize, headers, region, verifyEtag, identity);
                }
            }
            catch (InvalidETagException)
            {
                File.Delete(filePath);
                throw;
            }
        }

        public ObjectStore DeleteObject(string container, string objectName, Dictionary<string, string> headers = null, string region = null, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            _objectStoreHelper.ValidateObjectName(objectName);

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region), container, objectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE, headers);

            if (response.StatusCode == 204)
                return ObjectStore.ObjectDeleted;
            if (response.StatusCode == 404)
                return ObjectStore.ContainerNotFound;

            return ObjectStore.Unknown;

        }

        public ObjectStore CopyObject(string sourceContainer, string sourceObjectName, string destinationContainer, string destinationObjectName, Dictionary<string, string> headers = null, string region = null, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(sourceContainer);
            _objectStoreHelper.ValidateObjectName(sourceObjectName);

            _objectStoreHelper.ValidateContainerName(destinationContainer);
            _objectStoreHelper.ValidateObjectName(destinationObjectName);

            if (headers != null)
            {
                if (string.IsNullOrWhiteSpace(headers.FirstOrDefault(x => x.Key.Equals(ObjectStoreConstants.ContentLength, StringComparison.OrdinalIgnoreCase)).Value))
                {
                    var contentLength = GetObjectContentLength(identity, sourceContainer, sourceObjectName, region);
                    headers.Add(ObjectStoreConstants.ContentLength, contentLength);
                }
            }
            else
            {
                headers = new Dictionary<string, string>();
                var contentLength = GetObjectContentLength(identity, sourceContainer, sourceObjectName, region);
                headers.Add(ObjectStoreConstants.ContentLength, contentLength);

            }

            headers.Add(ObjectStoreConstants.CopyFrom, string.Format("{0}/{1}", sourceContainer, sourceObjectName));

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region), destinationContainer, destinationObjectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, headers);

            if (response.StatusCode == 201)
                return ObjectStore.ObjectCreated;
            if (response.StatusCode == 404)
                return ObjectStore.ContainerNotFound;

            return ObjectStore.Unknown;
        }

        public Dictionary<string, string> GetObjectMetaData(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            _objectStoreHelper.ValidateObjectName(objectName);
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region), container, objectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _objectStoreHelper.ProcessMetadata(response.Headers);

            return processedHeaders[ObjectStoreConstants.ProcessedHeadersMetadataKey];
        }

        public ObjectStore PurgeObjectFromCDN(string container, string objectName, string region = null, CloudIdentity identity = null)
        {
            return PurgeObjectFromCDN(container, objectName, " ", region, identity);
        }

        public ObjectStore PurgeObjectFromCDN(string container, string objectName, string[] emails, string region = null, CloudIdentity identity = null)
        {
            if (emails.Length < 0)
            {
                throw new ArgumentNullException();
            }

            return PurgeObjectFromCDN(container, objectName, string.Join(",", emails), region, identity);
        }

        public ObjectStore PurgeObjectFromCDN(string container, string objectName, string email, string region = null, CloudIdentity identity = null)
        {
            _objectStoreHelper.ValidateContainerName(container);
            _objectStoreHelper.ValidateObjectName(objectName);

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException();
            }

            if (!IsContainerCdnEnabled(container, region, true, identity))
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var headers = new Dictionary<string, string>();
                if (email.Trim().Length > 0)
                {
                    headers[ObjectStoreConstants.CdnPurgeEmail] = email;
                }
                var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFilesCDN(identity, region), container, objectName));
                var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE, headers: headers);
                if (response.StatusCode == 204)
                    return ObjectStore.ObjectPurged;

                return ObjectStore.Unknown;
            }

        }

        #endregion

        #region Accounts

        public Dictionary<string, string> GetAccountHeaders(string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _objectStoreHelper.ProcessMetadata(response.Headers);

            return processedHeaders[ObjectStoreConstants.ProcessedHeadersHeaderKey];

        }

        public Dictionary<string, string> GetAccountMetaData(string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _objectStoreHelper.ProcessMetadata(response.Headers);

            return processedHeaders[ObjectStoreConstants.ProcessedHeadersMetadataKey];
        }

        public void UpdateAccountMetadata(Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (metadata.Equals(null))
            {
                throw new ArgumentNullException();
            }

            var headers = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> m in metadata)
            {
                if (m.Key.Contains(ObjectStoreConstants.AccountMetaDataPrefix))
                {
                    headers.Add(m.Key, m.Value);
                }
                else
                {
                    headers.Add(ObjectStoreConstants.AccountMetaDataPrefix + m.Key, m.Value);
                }
            }

            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }
        
        public void UpdateAccountHeaders(Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            if (headers == null)
            {
                throw new ArgumentNullException();
            }

            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }


        #endregion


        #region Private methods

        private string GetObjectContentLength(CloudIdentity identity,string sourceContainer, string sourceObjectName, string region)
        {
            var sourceHeaders = GetObjectHeaders(sourceContainer, sourceObjectName, null, region, identity);
            var contentLength = sourceHeaders.FirstOrDefault(x => x.Key.Equals(ObjectStoreConstants.ContentLength, StringComparison.OrdinalIgnoreCase)).Value;
            return contentLength;
        }

        protected string GetServiceEndpointCloudFiles(CloudIdentity identity,string region = null)
        {
            return base.GetPublicServiceEndpoint(identity, "cloudFiles", region);
        }

        protected string GetServiceEndpointCloudFilesCDN(CloudIdentity identity,string region = null)
        {
            return base.GetPublicServiceEndpoint(identity, "cloudFilesCDN", region);
        }

        public static void CopyStream(Stream input, Stream output, int bufferSize)
        {
            var buffer = new byte[bufferSize];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        #endregion
    }
}
