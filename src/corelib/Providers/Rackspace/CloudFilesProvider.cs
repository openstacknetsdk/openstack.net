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
    public class CloudFilesProvider : ProviderBase, ICloudFilesProvider
    {
        private readonly ICloudFilesHelper _cloudFilesHelper;
        #region Constructors

        public CloudFilesProvider()
            : this(null) { }

        public CloudFilesProvider(CloudIdentity defaultIdentity)
            : this(defaultIdentity, new CloudIdentityProvider(), new JsonRestServices(), new CloudFilesHelper()) { }

        public CloudFilesProvider(ICloudIdentityProvider cloudIdentityProvider, IRestService restService, ICloudFilesHelper cloudFilesHelper)
            : this(null, cloudIdentityProvider, restService, cloudFilesHelper) { }

        public CloudFilesProvider(CloudIdentity defaultIdentity, ICloudIdentityProvider cloudIdentityProvider, IRestService restService, ICloudFilesHelper cloudFilesHelper)
            : base(defaultIdentity, cloudIdentityProvider, restService)
        {
            _cloudFilesHelper = cloudFilesHelper;
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
            _cloudFilesHelper.ValidateContainerName(container);
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
            _cloudFilesHelper.ValidateContainerName(container);
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
            _cloudFilesHelper.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesHelper.ProcessMetadata(response.Headers);

            return processedHeaders[CloudFilesConstants.ProcessedHeadersHeaderKey];
        }

        public Dictionary<string, string> GetContainerMetaData(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.GET); // Should be HEAD

            var processedHeaders = _cloudFilesHelper.ProcessMetadata(response.Headers);

            return processedHeaders[CloudFilesConstants.ProcessedHeadersMetadataKey];
        }

        public ContainerCDN GetContainerCDNHeader(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var result = new ContainerCDN() { Name = container };

            foreach (var header in response.Headers)
            {
                if (header.Key.ToLower().Equals(CloudFilesConstants.CdnUri))
                {
                    result.CDNUri = header.Value;
                }
                if (header.Key.ToLower().Equals(CloudFilesConstants.CdnSslUri))
                {
                    result.CDNSslUri = header.Value;
                }
                if (header.Key.ToLower().Equals(CloudFilesConstants.CdnStreamingUri))
                {
                    result.CDNStreamingUri = header.Value;
                }
                if (header.Key.ToLower().Equals(CloudFilesConstants.CdnTTL))
                {
                    result.Ttl = long.Parse(header.Value);
                }
                if (header.Key.ToLower().Equals(CloudFilesConstants.CdnEnabled))
                {
                    result.CDNEnabled = bool.Parse(header.Value);
                }
                if (header.Key.ToLower().Equals(CloudFilesConstants.CdnLogRetention))
                {
                    result.LogRetention = bool.Parse(header.Value);
                }
                if (header.Key.ToLower().Equals(CloudFilesConstants.CdnIosUri))
                {
                    result.CDNIosUri = header.Value;
                }
            }

            return result;
        }

        public void AddContainerMetadata(string container, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);
            if (metadata.Equals(null))
            {
                throw new ArgumentNullException();
            }

            var headers = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> m in metadata)
            {
                if (m.Key.Contains(CloudFilesConstants.ContainerMetaDataPrefix))
                {
                    headers.Add(m.Key, m.Value);
                }
                else
                {
                    headers.Add(CloudFilesConstants.ContainerMetaDataPrefix + m.Key, m.Value);
                }
            }

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        public void AddContainerHeaders(string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);
            if (headers == null)
            {
                throw new ArgumentNullException();
            }

            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFiles(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
        }

        public void AddContainerCdnHeaders(string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);
            if (headers == null)
            {
                throw new ArgumentNullException();
            }

            if (!GetContainerCDNHeader(container, region, useInternalUrl, identity: identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));
                ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, headers: headers);
            }
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
            return EnableCDNOnContainer(container, 259200, logRetention, region, identity);
        }

        public Dictionary<string, string> EnableCDNOnContainer(string container, long ttl, bool logRetention, string region = null, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);
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
                 {CloudFilesConstants.CdnTTL, ttl.ToString(CultureInfo.InvariantCulture)},
                 {CloudFilesConstants.CdnLogRetention, logRetention.ToString(CultureInfo.InvariantCulture)},
                 {CloudFilesConstants.CdnEnabled, "true"}
                };
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, headers: headers);

            if (response == null)
                return null;

            return response.Headers.ToDictionary(header => header.Key, header => header.Value);
        }

        public Dictionary<string, string> DisableCDNOnContainer(string container, string region = null, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);


            var headers = new Dictionary<string, string>
                {
                {CloudFilesConstants.CdnEnabled, "false"}
                };
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpointCloudFilesCDN(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, headers: headers);

            if (response == null)
                return null;

            return response.Headers.ToDictionary(header => header.Key, header => header.Value);
        }

        public void EnableStaticWebOnContainer(string container, string index, string error, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);

            if (!GetContainerCDNHeader(container, region, useInternalUrl, identity: identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var headers = new Dictionary<string, string>
                                  {
                                      {CloudFilesConstants.WebIndex, index},
                                      {CloudFilesConstants.WebError, error},
                                      {CloudFilesConstants.WebListingsCSS, css},
                                      {CloudFilesConstants.WebListings, listing.ToString()}
                                  };
                AddContainerHeaders(container, headers, region, useInternalUrl, identity);
            }
        }

        public void EnableStaticWebOnContainer(string container, string index, string error, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);

            if (!GetContainerCDNHeader(container, region, useInternalUrl, identity: identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var headers = new Dictionary<string, string>
                                  {
                                      {CloudFilesConstants.WebIndex, index},
                                      {CloudFilesConstants.WebError, error},
                                      {CloudFilesConstants.WebListings, listing.ToString()}
                                  };
                AddContainerHeaders(container, headers, region, useInternalUrl, identity);
            }
        }

        public void EnableStaticWebOnContainer(string container, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);

            if (!GetContainerCDNHeader(container, region, useInternalUrl, identity: identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var headers = new Dictionary<string, string>
                                  {
                                      {CloudFilesConstants.WebListingsCSS, css},
                                      {CloudFilesConstants.WebListings, listing.ToString()}
                                  };
                AddContainerHeaders(container, headers, region, useInternalUrl, identity);
            }
        }

        public void EnableStaticWebOnContainer(string container, string index, string error, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);

            if (!GetContainerCDNHeader(container, region, useInternalUrl, identity: identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var headers = new Dictionary<string, string>
                                  {
                                      {CloudFilesConstants.WebIndex, index},
                                      {CloudFilesConstants.WebError, error}
                                  };
                AddContainerHeaders(container, headers, region, useInternalUrl, identity);
            }
        }

        public void DisableStaticWebOnContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);

            if (!GetContainerCDNHeader(container, region, useInternalUrl, identity: identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var headers = new Dictionary<string, string>
                                  {
                                      {CloudFilesConstants.WebIndex, string.Empty},
                                      {CloudFilesConstants.WebError, string.Empty},
                                      {CloudFilesConstants.WebListingsCSS, string.Empty},
                                      {CloudFilesConstants.WebListings, string.Empty}
                                  };
                AddContainerHeaders(container, headers, region, useInternalUrl, identity);
            }
        }

        #endregion

        #region Container Objects

        public IEnumerable<ContainerObject> GetObjects(string container, int? limit = null, string marker = null, string markerEnd = null, string format = "json", string region = null, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);
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
            _cloudFilesHelper.ValidateContainerName(container);
            _cloudFilesHelper.ValidateObjectName(objectName);
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region), container, objectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesHelper.ProcessMetadata(response.Headers);

            return processedHeaders[CloudFilesConstants.ProcessedHeadersHeaderKey];
        }

        public void CreateObjectFromFile(string container, string filePath, string objectName, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, CloudIdentity identity = null)
        {
            using (var stream = System.IO.File.OpenRead(filePath))
            {
                CreateObject(container, stream, objectName, chunkSize, headers, region, progressUpdated, identity);
            }
        }

        public void CreateObject(string container, Stream stream, string objectName, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, CloudIdentity identity = null)
        {
            if (stream == null)
                throw new ArgumentNullException();

            _cloudFilesHelper.ValidateContainerName(container);
            _cloudFilesHelper.ValidateObjectName(objectName);

            if (stream.Length > CloudFilesConstants.LargeFileBatchThreshold)
            {
                CreateObjectInSegments(container, stream, objectName, chunkSize, headers, region, progressUpdated, identity);
                return;
            }
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region), container, objectName));

            StreamRESTRequest(identity, urlPath, HttpMethod.PUT, stream, chunkSize, headers: headers, isRetry: true, progressUpdated: progressUpdated, requestSettings: new RequestSettings());
        }

        private void CreateObjectInSegments(string container, Stream stream, string objectName, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, CloudIdentity identity = null)
        {
            var totalLength = stream.Length;
            var segmentCount = Math.Ceiling((double)totalLength / (double)CloudFilesConstants.LargeFileBatchThreshold);

            long totalBytesWritten = 0;
            for (int i = 0; i < segmentCount; i++)
            {
                var remaining = (totalLength - CloudFilesConstants.LargeFileBatchThreshold * i);
                var length = (remaining < CloudFilesConstants.LargeFileBatchThreshold) ? remaining : CloudFilesConstants.LargeFileBatchThreshold;

                var urlPath = new Uri(string.Format("{0}/{1}/{2}.seg{3}", GetServiceEndpointCloudFiles(identity, region), container, objectName, i));
                long segmentBytesWritten = 0;
                StreamRESTRequest(identity, urlPath, HttpMethod.PUT, stream, chunkSize, length, headers: headers, isRetry: true, requestSettings: new RequestSettings(), progressUpdated: (
                    bytesWritten) =>
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
            var segmentUrlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region), container, objectName));

            if (headers == null)
                headers = new Dictionary<string, string>();

            headers.Add(CloudFilesConstants.ObjectManifestMetadataKey, string.Format("{0}/{1}", container, objectName));
            StreamRESTRequest(identity, segmentUrlPath, HttpMethod.PUT, new MemoryStream(new Byte[0]), chunkSize, headers: headers, isRetry: true, requestSettings: new RequestSettings(), progressUpdated:
                (bytesWritten) =>
                {
                    if (progressUpdated != null)
                    {
                        progressUpdated(totalBytesWritten);
                    }
                });
        }

        public void GetObject(string container, string objectName, Stream outputStream, int chunkSize = 4096, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, CloudIdentity identity = null)
        {
            _cloudFilesHelper.ValidateContainerName(container);
            _cloudFilesHelper.ValidateObjectName(objectName);

            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region), container, objectName));

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

                    var respHeaders = resp.Headers.AllKeys.Select(key => new HttpHeader() { Key = key, Value = resp.GetResponseHeader(key) }).ToList();

                    return new Response(resp.StatusCode, respHeaders, "[Binary]");
                }
                catch (Exception)
                {
                    return new Response(0, null, null);
                }
            }, headers: headers);

            if (verifyEtag && response.Headers.Any(h => h.Key.Equals(CloudFilesConstants.Etag, StringComparison.OrdinalIgnoreCase)))
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
                if (convertedMd5 != response.Headers.First(h => h.Key.Equals(CloudFilesConstants.Etag, StringComparison.OrdinalIgnoreCase)).Value.ToLower())
                {

                    throw new InvalidETagException();
                }
            }
        }

        public void GetObjectSaveToFile(string container, string saveDirectory, string objectName, string fileName = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, CloudIdentity identity = null)
        {
            if (String.IsNullOrEmpty(saveDirectory))
                throw new ArgumentNullException();

            var filePath = Path.Combine(saveDirectory, string.IsNullOrWhiteSpace(fileName) ? objectName : fileName);

            try
            {
                using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    GetObject(container, objectName, fileStream, chunkSize, headers, region, verifyEtag, progressUpdated, identity);
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
            _cloudFilesHelper.ValidateContainerName(container);
            _cloudFilesHelper.ValidateObjectName(objectName);

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
            _cloudFilesHelper.ValidateContainerName(sourceContainer);
            _cloudFilesHelper.ValidateObjectName(sourceObjectName);

            _cloudFilesHelper.ValidateContainerName(destinationContainer);
            _cloudFilesHelper.ValidateObjectName(destinationObjectName);

            if (headers != null)
            {
                if (string.IsNullOrWhiteSpace(headers.FirstOrDefault(x => x.Key.Equals(CloudFilesConstants.ContentLength, StringComparison.OrdinalIgnoreCase)).Value))
                {
                    var contentLength = GetObjectContentLength(identity, sourceContainer, sourceObjectName, region);
                    headers.Add(CloudFilesConstants.ContentLength, contentLength);
                }
            }
            else
            {
                headers = new Dictionary<string, string>();
                var contentLength = GetObjectContentLength(identity, sourceContainer, sourceObjectName, region);
                headers.Add(CloudFilesConstants.ContentLength, contentLength);

            }

            headers.Add(CloudFilesConstants.CopyFrom, string.Format("{0}/{1}", sourceContainer, sourceObjectName));

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
            _cloudFilesHelper.ValidateContainerName(container);
            _cloudFilesHelper.ValidateObjectName(objectName);
            var urlPath = new Uri(string.Format("{0}/{1}/{2}", GetServiceEndpointCloudFiles(identity, region), container, objectName));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesHelper.ProcessMetadata(response.Headers);

            return processedHeaders[CloudFilesConstants.ProcessedHeadersMetadataKey];
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
            _cloudFilesHelper.ValidateContainerName(container);
            _cloudFilesHelper.ValidateObjectName(objectName);

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException();
            }

            if (!GetContainerCDNHeader(container, region, identity: identity).CDNEnabled)
            {
                throw new CDNNotEnabledException();
            }
            else
            {
                var headers = new Dictionary<string, string>();
                if (email.Trim().Length > 0)
                {
                    headers[CloudFilesConstants.CdnPurgeEmail] = email;
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

            var processedHeaders = _cloudFilesHelper.ProcessMetadata(response.Headers);

            return processedHeaders[CloudFilesConstants.ProcessedHeadersHeaderKey];

        }

        public Dictionary<string, string> GetAccountMetaData(string region = null, bool useInternalUrl = false, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}", GetServiceEndpointCloudFiles(identity, region)));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.HEAD);

            var processedHeaders = _cloudFilesHelper.ProcessMetadata(response.Headers);

            return processedHeaders[CloudFilesConstants.ProcessedHeadersMetadataKey];
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
                if (m.Key.Contains(CloudFilesConstants.AccountMetaDataPrefix))
                {
                    headers.Add(m.Key, m.Value);
                }
                else
                {
                    headers.Add(CloudFilesConstants.AccountMetaDataPrefix + m.Key, m.Value);
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

        private string GetObjectContentLength(CloudIdentity identity, string sourceContainer, string sourceObjectName, string region)
        {
            var sourceHeaders = GetObjectHeaders(sourceContainer, sourceObjectName, null, region, identity);
            var contentLength = sourceHeaders.FirstOrDefault(x => x.Key.Equals(CloudFilesConstants.ContentLength, StringComparison.OrdinalIgnoreCase)).Value;
            return contentLength;
        }

        protected string GetServiceEndpointCloudFiles(CloudIdentity identity, string region = null)
        {
            return base.GetPublicServiceEndpoint(identity, "cloudFiles", region);
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
    }
}
