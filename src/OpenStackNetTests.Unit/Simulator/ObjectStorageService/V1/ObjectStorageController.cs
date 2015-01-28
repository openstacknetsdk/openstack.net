namespace OpenStackNetTests.Unit.Simulator.ObjectStorageService.V1
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using ICSharpCode.SharpZipLib.BZip2;
    using ICSharpCode.SharpZipLib.GZip;
    using ICSharpCode.SharpZipLib.Tar;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Services.ObjectStorage.V1;
    using OpenStackNetTests.Unit.Simulator.IdentityService.V2;

    public class ObjectStorageController : ApiController
    {
        private IdentityController _identityService = new IdentityController();

        private static AccountMetadata _accountMetadata;

        private static readonly ConcurrentDictionary<ContainerName, Container> _containers =
            new ConcurrentDictionary<ContainerName, Container>();
        private static readonly ConditionalWeakTable<Container, StrongBox<ContainerMetadata>> _containerMetadata =
            new ConditionalWeakTable<Container, StrongBox<ContainerMetadata>>();
        private static readonly ConditionalWeakTable<Container, ConcurrentDictionary<ObjectName, ContainerObject>> _containerObjects =
            new ConditionalWeakTable<Container, ConcurrentDictionary<ObjectName, ContainerObject>>();

        private static readonly ConditionalWeakTable<ContainerObject, StrongBox<ObjectMetadata>> _objectMetadata =
            new ConditionalWeakTable<ContainerObject, StrongBox<ObjectMetadata>>();
        private static readonly ConditionalWeakTable<ContainerObject, byte[]> _objectData =
            new ConditionalWeakTable<ContainerObject, byte[]>();

        private static readonly ConcurrentDictionary<ContainerName, bool> _updatedContainers =
            new ConcurrentDictionary<ContainerName, bool>();

        [ActionName("Info")]
        public HttpResponseMessage GetInfo()
        {
            ValidateRequest(Request);
            if (Request.GetQueryNameValuePairs().Any())
                throw new NotImplementedException();

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(ObjectStorageResources.GetInfoResponse, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpHead]
        [ActionName("Account")]
        public HttpResponseMessage GetAccountMetadata([FromUri(Name = "tenant")] string tenantString)
        {
            ValidateRequest(Request);
            if (Request.GetQueryNameValuePairs().Any())
                throw new NotImplementedException();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.NoContent);

            AccountMetadata accountMetadata = GetAccountMetadata();
            ApplyMetadata(result, accountMetadata);
            return result;
        }

        [HttpGet]
        [ActionName("Account")]
        public HttpResponseMessage ListContainers([FromUri(Name = "tenant")] string tenantString)
        {
            ValidateRequest(Request);
            UpdateContainerStatistics();

            int? limit = null;
            ContainerName marker = null;
            if (Request.GetQueryNameValuePairs().Any())
            {
                foreach (var pair in Request.GetQueryNameValuePairs())
                {
                    switch (pair.Key)
                    {
                    case "limit":
                        if (limit != null)
                            throw new NotImplementedException("The limit is already set.");

                        int limitValue;
                        if (!int.TryParse(pair.Value, out limitValue) || limitValue <= 0)
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);

                        limit = Math.Min(5000, limitValue);
                        break;

                    case "marker":
                        if (marker != null)
                            throw new InvalidOperationException("The marker is already set.");

                        if (string.IsNullOrEmpty(pair.Value) || pair.Value.IndexOf('/') >= 0)
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);

                        marker = new ContainerName(pair.Value);
                        break;

                    default:
                        throw new NotImplementedException("The query contains unhandled query parameters.");
                    }
                }
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            IEnumerable<Container> filtered = _containers.Values.OrderBy(i => i.Name.Value, StringComparer.Ordinal);
            if (marker != null)
                filtered = filtered.Where(i => string.CompareOrdinal(i.Name.Value, marker.Value) > 0);

            Container[] containers = filtered.Take(limit ?? 5000).ToArray();
            string body = JsonConvert.SerializeObject(containers);
            result.Content = new StringContent(body, Encoding.UTF8, "application/json");

            AccountMetadata accountMetadata = GetAccountMetadata();
            ApplyMetadata(result, accountMetadata);
            return result;
        }

        [HttpPut]
        [ActionName("Account")]
        public async Task<HttpResponseMessage> PutAccount([FromUri(Name = "tenant")] string tenantString)
        {
            ValidateRequest(Request);
            ArchiveFormat archiveFormat = null;

            if (Request.GetQueryNameValuePairs().Any())
            {
                foreach (var pair in Request.GetQueryNameValuePairs())
                {
                    switch (pair.Key)
                    {
                    case "extract-archive":
                        if (archiveFormat != null)
                            throw new InvalidOperationException("The archive format is already set.");

                        if (string.IsNullOrEmpty(pair.Value))
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);

                        archiveFormat = ArchiveFormat.FromName(pair.Value);
                        break;

                    default:
                        throw new NotImplementedException("The query contains unhandled query parameters.");
                    }
                }
            }

            if (archiveFormat != null)
            {
                return ExtractArchiveImpl(await Request.Content.ReadAsStreamAsync(), null, null, archiveFormat, CancellationToken.None);
            }

            return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
        }

        [HttpPost]
        [ActionName("Account")]
        public HttpResponseMessage UpdateAccountMetadata([FromUri(Name = "tenant")] string tenantString)
        {
            ValidateRequest(Request);
            if (Request.GetQueryNameValuePairs().Any())
                throw new NotImplementedException();

            IDictionary<string, string> headers;
            IDictionary<string, string> metadata;
            ExtractMetadata(Request, AccountMetadata.AccountMetadataPrefix, out headers, out metadata);

            AccountMetadata previous = _accountMetadata ?? AccountMetadata.Empty;

            foreach (var pair in previous.Headers)
            {
                if (headers.ContainsKey(pair.Key))
                    continue;

                headers.Add(pair.Key, pair.Value);
            }

            foreach (var pair in previous.Metadata)
            {
                if (metadata.ContainsKey(pair.Key))
                    continue;

                metadata.Add(pair.Key, pair.Value);
            }

            foreach (var pair in headers.ToArray())
            {
                if (string.IsNullOrEmpty(pair.Value))
                    headers.Remove(pair.Key);
            }

            foreach (var pair in metadata.ToArray())
            {
                if (string.IsNullOrEmpty(pair.Value))
                    metadata.Remove(pair.Key);
            }

            _accountMetadata = new AccountMetadata(headers, metadata);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [HttpHead]
        [ActionName("Container")]
        public HttpResponseMessage GetContainerMetadata([FromUri(Name = "tenant")] string tenantString, [FromUri(Name = "container")] string containerString)
        {
            ValidateRequest(Request);
            UpdateContainerStatistics();

            if (Request.GetQueryNameValuePairs().Any())
                throw new NotImplementedException();

            ContainerName containerName = new ContainerName(containerString);

            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.NoContent);
            result.Content = new ByteArrayContent(new byte[0]);

            StrongBox<ContainerMetadata> containerMetadataBox;
            ContainerMetadata containerMetadata;
            if (_containerMetadata.TryGetValue(container, out containerMetadataBox))
                containerMetadata = containerMetadataBox.Value ?? ContainerMetadata.Empty;
            else
                containerMetadata = ContainerMetadata.Empty;

            ApplyMetadata(result, containerMetadata);
            return result;
        }

        [HttpGet]
        [ActionName("Container")]
        public HttpResponseMessage ListObjects([FromUri(Name = "tenant")] string tenantString, [FromUri(Name = "container")] string containerString)
        {
            ValidateRequest(Request);
            UpdateContainerStatistics();

            ContainerName containerName = new ContainerName(containerString);

            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            int? limit = null;
            ObjectName marker = null;
            if (Request.GetQueryNameValuePairs().Any())
            {
                foreach (var pair in Request.GetQueryNameValuePairs())
                {
                    switch (pair.Key)
                    {
                    case "limit":
                        if (limit != null)
                            throw new NotImplementedException("The limit is already set.");

                        int limitValue;
                        if (!int.TryParse(pair.Value, out limitValue) || limitValue <= 0)
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);

                        limit = Math.Min(5000, limitValue);
                        break;

                    case "marker":
                        if (marker != null)
                            throw new InvalidOperationException("The marker is already set.");

                        if (string.IsNullOrEmpty(pair.Value) || pair.Value.IndexOf('/') >= 0)
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);

                        marker = new ObjectName(pair.Value);
                        break;

                    default:
                        throw new NotImplementedException("The query contains unhandled query parameters.");
                    }
                }
            }

            ConcurrentDictionary<ObjectName, ContainerObject> containerObjects;
            if (!_containerObjects.TryGetValue(container, out containerObjects) || containerObjects == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            IEnumerable<ContainerObject> filtered = containerObjects.Values.OrderBy(i => i.Name.Value, StringComparer.Ordinal);
            if (marker != null)
                filtered = filtered.Where(i => string.CompareOrdinal(i.Name.Value, marker.Value) > 0);

            ContainerObject[] objects = filtered.Take(limit ?? 5000).ToArray();
            string body = JsonConvert.SerializeObject(objects);
            result.Content = new StringContent(body, Encoding.UTF8, "application/json");

            StrongBox<ContainerMetadata> containerMetadataBox;
            ContainerMetadata containerMetadata;
            if (_containerMetadata.TryGetValue(container, out containerMetadataBox))
                containerMetadata = containerMetadataBox.Value ?? ContainerMetadata.Empty;
            else
                containerMetadata = ContainerMetadata.Empty;

            ApplyMetadata(result, containerMetadata);
            return result;
        }

        [HttpPut]
        [ActionName("Container")]
        public async Task<HttpResponseMessage> CreateContainer([FromUri(Name = "tenant")] string tenantString, [FromUri(Name = "container")] string containerString)
        {
            ValidateRequest(Request);
            ArchiveFormat archiveFormat = null;

            if (Request.GetQueryNameValuePairs().Any())
            {
                foreach (var pair in Request.GetQueryNameValuePairs())
                {
                    switch (pair.Key)
                    {
                    case "extract-archive":
                        if (archiveFormat != null)
                            throw new InvalidOperationException("The archive format is already set.");

                        if (string.IsNullOrEmpty(pair.Value))
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);

                        archiveFormat = ArchiveFormat.FromName(pair.Value);
                        break;

                    default:
                        throw new NotImplementedException("The query contains unhandled query parameters.");
                    }
                }
            }

            ContainerName containerName = new ContainerName(containerString);

            IDictionary<string, string> headers;
            IDictionary<string, string> metadata;
            ExtractMetadata(Request, ContainerMetadata.ContainerMetadataPrefix, out headers, out metadata);
            ContainerMetadata containerMetadata = new ContainerMetadata(headers, metadata);

            HttpResponseMessage response = CreateContainerImpl(containerName, containerMetadata, CancellationToken.None);

            if (archiveFormat != null)
            {
                // create the container before extracting items...
                return ExtractArchiveImpl(await Request.Content.ReadAsStreamAsync(), containerName, null, archiveFormat, CancellationToken.None);
            }

            return response;
        }

        [HttpPost]
        [ActionName("Container")]
        public HttpResponseMessage UpdateContainerMetadata([FromUri(Name = "tenant")] string tenantString, [FromUri(Name = "container")] string containerString)
        {
            ValidateRequest(Request);
            if (Request.GetQueryNameValuePairs().Any())
                throw new NotImplementedException();

            ContainerName containerName = new ContainerName(containerString);

            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            IDictionary<string, string> headers;
            IDictionary<string, string> metadata;
            ExtractMetadata(Request, ContainerMetadata.ContainerMetadataPrefix, out headers, out metadata);

            StrongBox<ContainerMetadata> previousBox;
            ContainerMetadata previous;
            if (_containerMetadata.TryGetValue(container, out previousBox))
                previous = previousBox.Value ?? ContainerMetadata.Empty;
            else
                previous = ContainerMetadata.Empty;

            foreach (var pair in previous.Headers)
            {
                if (headers.ContainsKey(pair.Key))
                    continue;

                headers.Add(pair.Key, pair.Value);
            }

            foreach (var pair in previous.Metadata)
            {
                if (metadata.ContainsKey(pair.Key))
                    continue;

                metadata.Add(pair.Key, pair.Value);
            }

            foreach (var pair in headers.ToArray())
            {
                if (string.IsNullOrEmpty(pair.Value))
                    headers.Remove(pair.Key);
            }

            foreach (var pair in metadata.ToArray())
            {
                if (string.IsNullOrEmpty(pair.Value))
                    metadata.Remove(pair.Key);
            }

            ContainerMetadata containerMetadata = new ContainerMetadata(headers, metadata);
            _containerMetadata.GetOrCreateValue(container).Value = containerMetadata;

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [ActionName("Container")]
        public HttpResponseMessage DeleteContainer([FromUri(Name = "tenant")] string tenantString, [FromUri(Name = "container")] string containerString)
        {
            ValidateRequest(Request);
            if (Request.GetQueryNameValuePairs().Any())
                throw new NotImplementedException();

            ContainerName containerName = new ContainerName(containerString);

            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            ConcurrentDictionary<ObjectName, ContainerObject> containerObjects;
            if (_containerObjects.TryGetValue(container, out containerObjects) && containerObjects != null)
            {
                if (containerObjects.Count > 0)
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
            }

            if (!_containers.TryRemove(containerName, out container))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [HttpHead]
        [ActionName("Object")]
        public HttpResponseMessage GetObjectMetadata([FromUri(Name = "tenant")] string tenantString, [FromUri(Name = "container")] string containerString, [FromUri(Name = "object")] string objectString)
        {
            ValidateRequest(Request);
            if (Request.GetQueryNameValuePairs().Any())
                throw new NotImplementedException();

            ContainerName containerName = new ContainerName(containerString);
            ObjectName objectName = new ObjectName(objectString);

            Tuple<Container, ContainerObject> containerObject = TryGetObject(containerName, objectName);
            if (containerObject == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            byte[] data;
            if (!_objectData.TryGetValue(containerObject.Item2, out data))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.NoContent);
            result.Content = new ByteArrayContent(new byte[0]);
            result.Content.Headers.ContentLength = data.Length;

            StrongBox<ObjectMetadata> objectMetadataBox;
            ObjectMetadata objectMetadata;
            if (_objectMetadata.TryGetValue(containerObject.Item2, out objectMetadataBox))
                objectMetadata = objectMetadataBox.Value ?? ObjectMetadata.Empty;
            else
                objectMetadata = ObjectMetadata.Empty;

            ApplyMetadata(result, objectMetadata);
            return result;
        }

        [HttpGet]
        [ActionName("Object")]
        public HttpResponseMessage GetObjectData([FromUri(Name = "tenant")] string tenantString, [FromUri(Name = "container")] string containerString, [FromUri(Name = "object")] string objectString)
        {
            ValidateRequest(Request);
            if (Request.GetQueryNameValuePairs().Any())
                throw new NotImplementedException();

            ContainerName containerName = new ContainerName(containerString);
            ObjectName objectName = new ObjectName(objectString);

            Tuple<Container, ContainerObject> containerObject = TryGetObject(containerName, objectName);
            if (containerObject == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            byte[] data;
            if (!_objectData.TryGetValue(containerObject.Item2, out data))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(new MemoryStream(data));

            StrongBox<ObjectMetadata> objectMetadataBox;
            ObjectMetadata objectMetadata;
            if (_objectMetadata.TryGetValue(containerObject.Item2, out objectMetadataBox))
                objectMetadata = objectMetadataBox.Value ?? ObjectMetadata.Empty;
            else
                objectMetadata = ObjectMetadata.Empty;

            ApplyMetadata(result, objectMetadata);
            return result;
        }

        [HttpPut]
        [ActionName("Object")]
        public async Task<HttpResponseMessage> CreateObject([FromUri(Name = "tenant")] string tenantString, [FromUri(Name = "container")] string containerString, [FromUri(Name = "object")] string objectString)
        {
            ValidateRequest(Request);
            ArchiveFormat archiveFormat = null;

            if (Request.GetQueryNameValuePairs().Any())
            {
                foreach (var pair in Request.GetQueryNameValuePairs())
                {
                    switch (pair.Key)
                    {
                    case "extract-archive":
                        if (archiveFormat != null)
                            throw new InvalidOperationException("The archive format is already set.");

                        if (string.IsNullOrEmpty(pair.Value))
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);

                        archiveFormat = ArchiveFormat.FromName(pair.Value);
                        break;

                    default:
                        throw new NotImplementedException("The query contains unhandled query parameters.");
                    }
                }
            }

            ContainerName containerName = new ContainerName(containerString);
            ObjectName objectName = new ObjectName(objectString);

            if (archiveFormat != null)
            {
                return ExtractArchiveImpl(await Request.Content.ReadAsStreamAsync(), containerName, objectName, archiveFormat, CancellationToken.None);
            }

            EntityTagHeaderValue ifNoneMatch = Request.Headers.IfNoneMatch.SingleOrDefault();
            if (ifNoneMatch != null)
            {
                if (ifNoneMatch.Tag != "\"*\"" && ifNoneMatch.Tag != "*")
                    throw new NotSupportedException();

                Container container = TryGetContainer(containerName);
                if (container == null)
                    return new HttpResponseMessage(HttpStatusCode.NotFound);

                StrongBox<ContainerMetadata> containerMetadataBox;
                ContainerMetadata containerMetadata;
                if (_containerMetadata.TryGetValue(container, out containerMetadataBox))
                    containerMetadata = containerMetadataBox.Value ?? ContainerMetadata.Empty;
                else
                    containerMetadata = ContainerMetadata.Empty;

                ConcurrentDictionary<ObjectName, ContainerObject> containerObjects;
                if (!_containerObjects.TryGetValue(container, out containerObjects) || containerObjects == null)
                    return new HttpResponseMessage(HttpStatusCode.NotFound);

                if (containerObjects.ContainsKey(objectName))
                    return new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
            }

            if (Request.Headers.Contains("X-Copy-From"))
                throw new NotImplementedException();

            IDictionary<string, string> headers;
            IDictionary<string, string> metadata;
            ExtractMetadata(Request, ObjectMetadata.ObjectMetadataPrefix, out headers, out metadata);

            string contentType = "application/octet-stream";
            if (Request.Content.Headers.ContentType != null)
            {
                headers["Content-Type"] = Request.Content.Headers.ContentType.ToString();
                contentType = Request.Content.Headers.ContentType.MediaType;
            }

            MemoryStream buffer = new MemoryStream();
            await (await Request.Content.ReadAsStreamAsync()).CopyToAsync(buffer);
            byte[] data = buffer.ToArray();

            return CreateObjectImpl(data, containerName, objectName, new ObjectMetadata(headers, metadata), CancellationToken.None);
        }

        [HttpPost]
        [ActionName("Object")]
        public HttpResponseMessage SetObjectMetadata([FromUri(Name = "tenant")] string tenantString, [FromUri(Name = "container")] string containerString, [FromUri(Name = "object")] string objectString)
        {
            ValidateRequest(Request);
            if (Request.GetQueryNameValuePairs().Any())
                throw new NotImplementedException();

            ContainerName containerName = new ContainerName(containerString);
            ObjectName objectName = new ObjectName(objectString);

            Tuple<Container, ContainerObject> containerObject = TryGetObject(containerName, objectName);
            if (containerObject == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            IDictionary<string, string> headers;
            IDictionary<string, string> metadata;
            ExtractMetadata(Request, ObjectMetadata.ObjectMetadataPrefix, out headers, out metadata);
            ObjectMetadata objectMetadata = new ObjectMetadata(headers, metadata);
            _objectMetadata.GetOrCreateValue(containerObject.Item2).Value = objectMetadata;
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [AcceptVerbs("COPY")]
        [ActionName("Object")]
        public HttpResponseMessage CopyObject([FromUri(Name = "tenant")] string tenantString, [FromUri(Name = "container")] string containerString, [FromUri(Name = "object")] string objectString)
        {
            ValidateRequest(Request);
            if (Request.GetQueryNameValuePairs().Any())
                throw new NotImplementedException();

            string destination = Request.Headers.GetValues("Destination").Single();
            if (destination == null)
                throw new NotImplementedException();

            if (destination.StartsWith("/"))
                destination = destination.Substring(1);

            string[] destinationContainerAndObject = destination.Split(new[] { '/' }, 2);
            ContainerName destinationContainerName = new ContainerName(destinationContainerAndObject[0]);
            ObjectName destinationObjectName = new ObjectName(destinationContainerAndObject[1]);

            ContainerName containerName = new ContainerName(containerString);
            ObjectName objectName = new ObjectName(objectString);

            IDictionary<string, string> headers;
            IDictionary<string, string> metadata;
            ExtractMetadata(Request, ObjectMetadata.ObjectMetadataPrefix, out headers, out metadata);

            return CopyObjectImpl(containerName, objectName, destinationContainerName, destinationObjectName, new ObjectMetadata(headers, metadata), CancellationToken.None);
        }

        [HttpDelete]
        [ActionName("Object")]
        public HttpResponseMessage DeleteObject([FromUri(Name = "tenant")] string tenantString, [FromUri(Name = "container")] string containerString, [FromUri(Name = "object")] string objectString)
        {
            ValidateRequest(Request);
            if (Request.GetQueryNameValuePairs().Any())
                throw new NotImplementedException();

            ContainerName containerName = new ContainerName(containerString);
            ObjectName objectName = new ObjectName(objectString);

            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            StrongBox<ContainerMetadata> containerMetadataBox;
            ContainerMetadata containerMetadata;
            if (_containerMetadata.TryGetValue(container, out containerMetadataBox))
                containerMetadata = containerMetadataBox.Value ?? ContainerMetadata.Empty;
            else
                containerMetadata = ContainerMetadata.Empty;

            ContainerName versionsLocation = containerMetadata.GetVersionsLocation();
            if (versionsLocation != null)
            {
                Container versionsContainer = TryGetContainer(versionsLocation);
                if (versionsContainer != null)
                {
                    IEnumerable<ObjectName> backupCopies = Enumerable.Empty<ObjectName>();
                    ConcurrentDictionary<ObjectName, ContainerObject> backupContainerObjects;
                    if (_containerObjects.TryGetValue(versionsContainer, out backupContainerObjects))
                    {
                        string prefix = string.Format("{0:XXX}{1}/", objectName.Value.Length, objectName);
                        backupCopies = backupContainerObjects.Keys.Where(i => i.Value.StartsWith(prefix, StringComparison.Ordinal))
                            .OrderBy(i => long.Parse(i.Value.Substring(prefix.Length)));
                    }

                    ObjectName lastBackup = backupCopies.LastOrDefault();
                    if (lastBackup != null)
                    {
                        // copy the original back instead of deleting it
                        HttpResponseMessage response = CopyObjectImpl(versionsLocation, lastBackup, containerName, objectName, ObjectMetadata.Empty, CancellationToken.None);
                        if (!response.IsSuccessStatusCode)
                            return response;

                        // then delete the backup
                        return DeleteObjectImpl(versionsLocation, lastBackup, CancellationToken.None);
                    }
                }
            }

            return DeleteObjectImpl(containerName, objectName, CancellationToken.None);
        }

        private HttpResponseMessage CreateContainerImpl(ContainerName containerName, ContainerMetadata containerMetadata, CancellationToken cancellationToken)
        {
            bool created = false;
            Container container = _containers.GetOrAdd(containerName,
                i =>
                {
                    created = true;
                    return CreateContainer(i, containerMetadata.Headers, containerMetadata.Metadata, 0, 0);
                });

            _containerObjects.GetOrCreateValue(container);

            return new HttpResponseMessage(created ? HttpStatusCode.Created : HttpStatusCode.Accepted);
        }

        private HttpResponseMessage ExtractArchiveImpl(Stream inputStream, ContainerName containerName, ObjectName objectPrefix, ArchiveFormat format, CancellationToken cancellationToken)
        {
            Stream decompressedStream;
            if (format == ArchiveFormat.Tar)
            {
                decompressedStream = inputStream;
            }
            else if (format == ArchiveFormat.TarBz2)
            {
                decompressedStream = new BZip2InputStream(inputStream);
            }
            else if (format == ArchiveFormat.TarGz)
            {
                decompressedStream = new GZipInputStream(inputStream);
            }
            else
            {
                throw new NotSupportedException(string.Format("The specified archive format '{0}' is not supported.", format));
            }

            int totalObjects = 0;
            JArray errors = new JArray();

            TarInputStream tarInputStream = new TarInputStream(decompressedStream);
            while (true)
            {
                TarEntry entry = tarInputStream.GetNextEntry();
                if (entry == null)
                    break;

                if (entry.IsDirectory)
                    continue;

                string entryName = entry.Name;
                if (entryName.IndexOf('/') == 0)
                    entryName = entryName.Substring(1);

                ContainerName entryContainer = containerName;
                if (entryContainer == null)
                {
                    int slash = entryName.IndexOf('/');
                    if (slash < 0)
                        continue;

                    entryContainer = new ContainerName(entryName.Substring(0, slash));
                    entryName = entryName.Substring(slash + 1);
                    if (string.IsNullOrEmpty(entryName))
                        continue;

                    // make sure the container exists
                    CreateContainerImpl(entryContainer, ContainerMetadata.Empty, cancellationToken);
                }

                if (objectPrefix != null)
                    entryName = objectPrefix.Value + entryName;

                ObjectName objectName = new ObjectName(entryName);

                MemoryStream stream = new MemoryStream();
                tarInputStream.CopyEntryContents(stream);
                byte[] objectData = stream.ToArray();

                HttpResponseMessage createResponse = CreateObjectImpl(objectData, entryContainer, objectName, ObjectMetadata.Empty, cancellationToken);
                if (createResponse.IsSuccessStatusCode)
                {
                    totalObjects++;
                }
                else
                {
                    string[] errorData =
                        {
                            string.Format("/v1/.../{0}/{1}", entryContainer, objectName),
                            string.Format("{0} {1}", (int)createResponse.StatusCode, createResponse.ReasonPhrase)
                        };
                    errors.Add(JToken.FromObject(errorData));
                }
            }

            JObject jsonBody =
                new JObject(
                    new JProperty("Number Files Created", JToken.FromObject(totalObjects)),
                    new JProperty("Errors", errors));

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            string body = JsonConvert.SerializeObject(jsonBody);
            response.Content = new StringContent(body, Encoding.UTF8, "application/json");
            return response;
        }

        private static Container CreateContainer(ContainerName containerName, IDictionary<string, string> headers, IDictionary<string, string> metadata, long objectCount, long objectSize)
        {
            JObject jsonObject =
                new JObject(
                    new JProperty("count", objectCount),
                    new JProperty("bytes", objectSize),
                    new JProperty("name", JValue.CreateString(containerName.Value)));

            Container container = jsonObject.ToObject<Container>();
            _containerMetadata.Add(container, new StrongBox<ContainerMetadata>(new ContainerMetadata(headers, metadata)));
            return container;
        }

        private HttpResponseMessage CreateObjectImpl(byte[] data, ContainerName containerName, ObjectName objectName, ObjectMetadata objectMetadata, CancellationToken cancellationToken)
        {
            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            StrongBox<ContainerMetadata> containerMetadataBox;
            ContainerMetadata containerMetadata;
            if (_containerMetadata.TryGetValue(container, out containerMetadataBox))
                containerMetadata = containerMetadataBox.Value ?? ContainerMetadata.Empty;
            else
                containerMetadata = ContainerMetadata.Empty;

            ConcurrentDictionary<ObjectName, ContainerObject> containerObjects;
            if (!_containerObjects.TryGetValue(container, out containerObjects) || containerObjects == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            ContainerName versionsLocation = containerMetadata.GetVersionsLocation();
            ContainerObject existingObject;
            if (versionsLocation != null && containerObjects.TryGetValue(objectName, out existingObject))
            {
                DateTimeOffset lastModified = existingObject.LastModified ?? DateTimeOffset.Now;
                ObjectName backupObjectName = new ObjectName(string.Format("{0:XXX}{1}/{2}", objectName.Value.Length, objectName, lastModified.UtcTicks));
                HttpResponseMessage backup = CopyObjectImpl(containerName, objectName, versionsLocation, backupObjectName, ObjectMetadata.Empty, cancellationToken);
                if (!backup.IsSuccessStatusCode)
                    return backup;
            }

            Dictionary<string, string> headers = new Dictionary<string, string>(objectMetadata.Headers, StringComparer.OrdinalIgnoreCase);

            string contentType;
            if (!headers.TryGetValue("Content-Type", out contentType))
            {
                contentType = "application/octet-stream";
                headers["Content-Type"] = contentType;
            }

            string hashText;
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(data);

                var sbuilder = new StringBuilder();
                foreach (var b in hash)
                {
                    sbuilder.Append(b.ToString("x2"));
                }

                hashText = sbuilder.ToString();
            }

            headers["ETag"] = "\"" + hashText + "\"";

            ObjectMetadata metadata = new ObjectMetadata(headers, objectMetadata.Metadata);

            Func<ObjectName, ContainerObject> addValue =
                key =>
                {
                    JObject jsonObject =
                        new JObject(
                            new JProperty("hash", hashText),
                            new JProperty("last_modified", JToken.FromObject(DateTimeOffset.Now)),
                            new JProperty("bytes", data.Length),
                            new JProperty("name", JToken.FromObject(objectName)),
                            new JProperty("content_type", contentType));

                    ContainerObject containerObject = jsonObject.ToObject<ContainerObject>();
                    _objectData.Add(containerObject, data);
                    _objectMetadata.Add(containerObject, new StrongBox<ObjectMetadata>(metadata));
                    return containerObject;
                };

            Func<ObjectName, ContainerObject, ContainerObject> updateValueFactory =
                (key, value) => addValue(key);

            containerObjects.AddOrUpdate(objectName, addValue, updateValueFactory);

            _updatedContainers.TryAdd(containerName, true);

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        private HttpResponseMessage CopyObjectImpl(ContainerName sourceContainerName, ObjectName sourceObjectName, ContainerName destinationContainerName, ObjectName destinationObjectName, ObjectMetadata updatedMetadata, CancellationToken cancellationToken)
        {
            Tuple<Container, ContainerObject> sourceContainerObject = TryGetObject(sourceContainerName, sourceObjectName);
            if (sourceContainerObject == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            byte[] objectData;
            if (!_objectData.TryGetValue(sourceContainerObject.Item2, out objectData) || objectData == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            StrongBox<ObjectMetadata> objectMetadataBox;
            if (!_objectMetadata.TryGetValue(sourceContainerObject.Item2, out objectMetadataBox))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            ObjectMetadata objectMetadata = objectMetadataBox.Value;
            if (objectMetadata == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            Container destinationContainer = TryGetContainer(destinationContainerName);
            if (destinationContainer == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            ConcurrentDictionary<ObjectName, ContainerObject> destinationContainerObjects;
            if (!_containerObjects.TryGetValue(destinationContainer, out destinationContainerObjects) || destinationContainerObjects == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            Dictionary<string, string> headers = new Dictionary<string, string>(objectMetadata.Headers, StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> metadata = new Dictionary<string, string>(objectMetadata.Metadata, StringComparer.OrdinalIgnoreCase);

            IDictionary<string, string> newHeaders = updatedMetadata.Headers;
            IDictionary<string, string> newMetadata = updatedMetadata.Metadata;

            foreach (var pair in newHeaders)
                headers[pair.Key] = pair.Value;
            foreach (var pair in newMetadata)
                metadata[pair.Key] = pair.Value;

            objectMetadata = new ObjectMetadata(headers, metadata);

            Func<ObjectName, ContainerObject> addValue =
                key =>
                {
                    JObject jsonContainerObject = JObject.FromObject(sourceContainerObject.Item2);
                    jsonContainerObject["last_modified"] = JToken.FromObject(DateTimeOffset.Now);
                    jsonContainerObject["name"] = JToken.FromObject(destinationObjectName);

                    ContainerObject copiedObject = jsonContainerObject.ToObject<ContainerObject>();
                    _objectData.Add(copiedObject, objectData);
                    _objectMetadata.Add(copiedObject, new StrongBox<ObjectMetadata>(objectMetadata));
                    return copiedObject;
                };

            Func<ObjectName, ContainerObject, ContainerObject> updateValueFactory =
                (key, value) => addValue(key);

            destinationContainerObjects.AddOrUpdate(destinationObjectName, addValue, updateValueFactory);

            _updatedContainers.TryAdd(destinationContainerName, true);

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        private HttpResponseMessage DeleteObjectImpl(ContainerName containerName, ObjectName objectName, CancellationToken cancellationToken)
        {
            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            ConcurrentDictionary<ObjectName, ContainerObject> containerObjects;
            if (!_containerObjects.TryGetValue(container, out containerObjects) || containerObjects == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            ContainerObject @object;
            if (!containerObjects.TryRemove(objectName, out @object))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            _updatedContainers.TryAdd(containerName, true);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        private void UpdateContainerStatistics()
        {
            foreach (var pair in _updatedContainers)
            {
                bool dummy;
                if (!_updatedContainers.TryRemove(pair.Key, out dummy))
                    continue;

                Container existing;
                Container updated;
                do
                {
                    existing = TryGetContainer(pair.Key);
                    if (existing == null)
                        break;

                    ConcurrentDictionary<ObjectName, ContainerObject> containerObjects;
                    if (!_containerObjects.TryGetValue(existing, out containerObjects))
                        break;

                    StrongBox<ContainerMetadata> containerMetadataBox;
                    ContainerMetadata containerMetadata;
                    if (_containerMetadata.TryGetValue(existing, out containerMetadataBox))
                        containerMetadata = containerMetadataBox.Value ?? ContainerMetadata.Empty;
                    else
                        containerMetadata = ContainerMetadata.Empty;

                    long objectCount = containerObjects.Count;
                    long objectSize = containerObjects.Sum(i => i.Value.Size ?? 0);
                    updated = CreateContainer(pair.Key, containerMetadata.Headers, containerMetadata.Metadata, objectCount, objectSize);
                    _containerObjects.Add(updated, containerObjects);
                } while (!_containers.TryUpdate(pair.Key, updated, existing));
            }
        }

        private AccountMetadata GetAccountMetadata()
        {
            AccountMetadata accountMetadata = _accountMetadata ?? AccountMetadata.Empty;

            UpdateContainerStatistics();

            Container[] containers = _containers.Values.ToArray();
            long containerCount = containers.Length;
            long objectCount = containers.Sum(i => i.ObjectCount ?? 0);
            long containerSize = containers.Sum(i => i.Size ?? 0);

            IDictionary<string, string> headers = new Dictionary<string, string>(accountMetadata.Headers, StringComparer.OrdinalIgnoreCase);
            headers.Add(AccountMetadataExtensions.AccountContainerCount, containerCount.ToString());
            headers.Add(AccountMetadataExtensions.AccountObjectCount, objectCount.ToString());
            headers.Add(AccountMetadataExtensions.AccountBytesUsed, containerSize.ToString());

            IDictionary<string, string> metadata = accountMetadata.Metadata;

            return new AccountMetadata(headers, metadata);
        }

        private static void ExtractMetadata(HttpRequestMessage request, string metadataPrefix, out IDictionary<string, string> headers, out IDictionary<string, string> metadata)
        {
            headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var pair in request.Headers)
            {
                string key = pair.Key;

                if (key.StartsWith(metadataPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    metadata.Add(key.Substring(metadataPrefix.Length), string.Join(", ", pair.Value));
                }
                else
                {
                    switch (key.ToLowerInvariant())
                    {
                    case "accept":
                    case "connection":
                    case "destination":
                    case "expect":
                    case "if-none-match":
                    case "host":
                    case "server":
                    case "user-agent":
                    case "x-auth-token":
                        // these headers are simply ignored
                        break;

                    case "x-versions-location":
                        if (ContainerMetadata.ContainerMetadataPrefix.Equals(metadataPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            headers.Add(key, string.Join(", ", pair.Value));
                        }

                        break;

                    default:
                        throw new NotImplementedException(string.Format("The '{0}' header is not yet supported.", key));
                    }
                }
            }

            foreach (var pair in request.Content.Headers)
            {
                string key = pair.Key;
                switch (key.ToLowerInvariant())
                {
                case "content-type":
                    headers.Add(key, string.Join(", ", pair.Value));
                    break;

                case "content-length":
                    // these headers are simply ignored
                    break;

                default:
                    throw new NotImplementedException(string.Format("The '{0}' header is not yet supported.", key));
                }
            }
        }

        private static void ApplyMetadata(HttpResponseMessage response, StorageMetadata metadata)
        {
            foreach (var headerItem in metadata.Headers)
            {
                switch (headerItem.Key.ToLowerInvariant())
                {
                case "content-type":
                    response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(headerItem.Value);
                    break;

                case "etag":
                    response.Headers.ETag = EntityTagHeaderValue.Parse(headerItem.Value);
                    break;

                default:
                    if (headerItem.Key.StartsWith("X-", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Headers.Add(headerItem.Key, headerItem.Value);
                        break;
                    }

                    throw new NotImplementedException();
                }
            }

            foreach (var metadataItem in metadata.Metadata)
            {
                response.Headers.Add(metadata.MetadataPrefix + metadataItem.Key, metadataItem.Value);
            }
        }

        private static Container TryGetContainer(ContainerName containerName)
        {
            Container container;
            if (!_containers.TryGetValue(containerName, out container))
                return null;

            return container;
        }

        private static Tuple<Container, ContainerObject> TryGetObject(ContainerName containerName, ObjectName objectName)
        {
            Container container = TryGetContainer(containerName);
            if (container == null)
                return null;

            ConcurrentDictionary<ObjectName, ContainerObject> containerObjects;
            if (!_containerObjects.TryGetValue(container, out containerObjects) || containerObjects == null)
                return null;

            ContainerObject @object;
            if (!containerObjects.TryGetValue(objectName, out @object))
                return null;

            return Tuple.Create(container, @object);
        }

        private void ValidateRequest(HttpRequestMessage request)
        {
            _identityService.ValidateRequest(request);
        }
    }
}
