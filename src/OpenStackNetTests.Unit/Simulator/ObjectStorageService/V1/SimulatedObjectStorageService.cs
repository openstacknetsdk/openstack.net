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
    using ICSharpCode.SharpZipLib.BZip2;
    using ICSharpCode.SharpZipLib.GZip;
    using ICSharpCode.SharpZipLib.Tar;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Services.ObjectStorage.V1;
    using OpenStackNetTests.Unit.Simulator.IdentityService.V2;
    using Rackspace.Net;

    public class SimulatedObjectStorageService : SimulatedService
    {
        private static readonly UriTemplate _infoTemplate = new UriTemplate("info");
        private static readonly UriTemplate _accountRequestTemplate = new UriTemplate("v1/{tenant}");
        private static readonly UriTemplate _containerRequestTemplate = new UriTemplate("v1/{tenant}/{container}");
        private static readonly UriTemplate _objectRequestTemplate = new UriTemplate("v1/{tenant}/{container}/{+object}");

        private readonly SimulatedIdentityService _identityService;

        private static AccountMetadata _accountMetadata;

        private static readonly ConcurrentDictionary<ContainerName, Container> _containers =
            new ConcurrentDictionary<ContainerName, Container>();
        private static readonly ConditionalWeakTable<Container, ContainerMetadata> _containerMetadata =
            new ConditionalWeakTable<Container, ContainerMetadata>();
        private static readonly ConditionalWeakTable<Container, ConcurrentDictionary<ObjectName, ContainerObject>> _containerObjects =
            new ConditionalWeakTable<Container, ConcurrentDictionary<ObjectName, ContainerObject>>();

        private static readonly ConditionalWeakTable<ContainerObject, ObjectMetadata> _objectMetadata =
            new ConditionalWeakTable<ContainerObject, ObjectMetadata>();
        private static readonly ConditionalWeakTable<ContainerObject, byte[]> _objectData =
            new ConditionalWeakTable<ContainerObject, byte[]>();

        public SimulatedObjectStorageService(SimulatedIdentityService identityService)
            : base(8080)
        {
            _identityService = identityService;
        }

        protected override async Task<HttpResponseMessage> ProcessRequestImplAsync(HttpListenerContext context, Uri dispatchUri, CancellationToken cancellationToken)
        {
            UriTemplateMatch match = _infoTemplate.Match(dispatchUri);
            if (match != null)
            {
                return ProcessInfoRequest(context, cancellationToken);
            }

            match = _accountRequestTemplate.Match(dispatchUri);
            if (match != null)
            {
                return ProcessAccountRequest(context, cancellationToken);
            }

            match = _containerRequestTemplate.Match(dispatchUri);
            if (match != null)
            {
                ContainerName container = new ContainerName((string)match.Bindings["container"].Value);
                return ProcessContainerRequest(context, container, cancellationToken);
            }

            match = _objectRequestTemplate.Match(dispatchUri);
            if (match != null)
            {
                ContainerName container = new ContainerName((string)match.Bindings["container"].Value);
                ObjectName @object = new ObjectName((string)match.Bindings["object"].Value);
                return ProcessObjectRequest(context, container, @object, cancellationToken);
            }

            return await base.ProcessRequestImplAsync(context, dispatchUri, cancellationToken);
        }

        private HttpResponseMessage ProcessInfoRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {
            ValidateAuthenticatedRequest(context.Request);

            switch (context.Request.HttpMethod.ToUpperInvariant())
            {
            case "GET":
                return ProcessGetInfoRequest(context, cancellationToken);

            default:
                return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
            }
        }

        private HttpResponseMessage ProcessAccountRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {
            ValidateAuthenticatedRequest(context.Request);

            switch (context.Request.HttpMethod.ToUpperInvariant())
            {
            case "GET":
                return ProcessListContainersRequest(context, cancellationToken);

            case "HEAD":
                return ProcessGetAccountMetadataRequest(context, cancellationToken);

            case "PUT":
                return ProcessPutAccountRequest(context, cancellationToken);

            case "POST":
                return ProcessUpdateAccountMetadataRequest(context, cancellationToken);

            default:
                return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
            }
        }

        private HttpResponseMessage ProcessContainerRequest(HttpListenerContext context, ContainerName containerName, CancellationToken cancellationToken)
        {
            ValidateAuthenticatedRequest(context.Request);

            switch (context.Request.HttpMethod.ToUpperInvariant())
            {
            case "GET":
                return ProcessListObjectsRequest(context, containerName, cancellationToken);

            case "HEAD":
                return ProcessGetContainerMetadataRequest(context, containerName, cancellationToken);

            case "PUT":
                return ProcessCreateContainerRequest(context, containerName, cancellationToken);

            case "POST":
                return ProcessUpdateContainerMetadataRequest(context, containerName, cancellationToken);

            case "DELETE":
                return ProcessDeleteContainerRequest(context, containerName, cancellationToken);

            default:
                return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
            }
        }

        private HttpResponseMessage ProcessObjectRequest(HttpListenerContext context, ContainerName containerName, ObjectName objectName, CancellationToken cancellationToken)
        {
            ValidateAuthenticatedRequest(context.Request);

            switch (context.Request.HttpMethod.ToUpperInvariant())
            {
            case "GET":
                return ProcessGetObjectDataRequest(context, containerName, objectName, cancellationToken);

            case "HEAD":
                return ProcessGetObjectMetadataRequest(context, containerName, objectName, cancellationToken);

            case "PUT":
                return ProcessCreateObjectRequest(context, containerName, objectName, cancellationToken);

            case "POST":
                return ProcessSetObjectMetadataRequest(context, containerName, objectName, cancellationToken);

            case "COPY":
                return ProcessCopyObjectRequest(context, containerName, objectName, cancellationToken);

            case "DELETE":
                return ProcessDeleteObjectRequest(context, containerName, objectName, cancellationToken);

            default:
                return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
            }
        }

        private HttpResponseMessage ProcessGetInfoRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(GetQueryString(context)))
                throw new NotImplementedException();

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(ObjectStorageResources.GetInfoResponse, Encoding.UTF8, "application/json");
            return response;
        }

        private HttpResponseMessage ProcessListContainersRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {
            UpdateContainerStatistics();

            int limit = 5000;
            ContainerName marker = null;
            if (!string.IsNullOrEmpty(GetQueryString(context)))
            {
                UriTemplate queryTemplate = new UriTemplate("{?limit,marker,params*}");
                UriTemplateMatch match = queryTemplate.Match(new Uri(GetQueryString(context), UriKind.Relative));
                if (match == null)
                    throw new InvalidOperationException();

                if (match.Bindings.ContainsKey("params"))
                    throw new NotImplementedException("The query contains unhandled query parameters.");

                KeyValuePair<VariableReference, object> limitValue;
                if (match.Bindings.TryGetValue("limit", out limitValue))
                {
                    if (!int.TryParse(limitValue.Value.ToString(), out limit) || limit <= 0)
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    limit = Math.Min(5000, limit);
                }

                KeyValuePair<VariableReference, object> markerValue;
                if (match.Bindings.TryGetValue("marker", out markerValue))
                {
                    string markerString = markerValue.Value as string;
                    if (string.IsNullOrEmpty(markerString) || markerString.IndexOf('/') >= 0)
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    marker = new ContainerName(markerString);
                }
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            IEnumerable<Container> filtered = _containers.Values.OrderBy(i => i.Name.Value, StringComparer.Ordinal);
            if (marker != null)
                filtered = filtered.Where(i => string.CompareOrdinal(i.Name.Value, marker.Value) > 0);

            Container[] containers = filtered.Take(limit).ToArray();
            string body = JsonConvert.SerializeObject(containers);
            result.Content = new StringContent(body, Encoding.UTF8, "application/json");

            AccountMetadata accountMetadata = GetAccountMetadata();
            ApplyMetadata(result, accountMetadata);
            return result;
        }

        private HttpResponseMessage ProcessGetAccountMetadataRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(GetQueryString(context)))
                throw new NotImplementedException();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.NoContent);

            AccountMetadata accountMetadata = GetAccountMetadata();
            ApplyMetadata(result, accountMetadata);
            return result;
        }

        private HttpResponseMessage ProcessPutAccountRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {
            ArchiveFormat archiveFormat = null;

            if (!string.IsNullOrEmpty(GetQueryString(context)))
            {
                UriTemplate queryTemplate = new UriTemplate("{?params*}");

                UriTemplateMatch match = queryTemplate.Match(new Uri(GetQueryString(context), UriKind.Relative));
                if (match == null)
                    throw new InvalidOperationException();

                KeyValuePair<VariableReference, object> paramsValue;
                if (match.Bindings.TryGetValue("params", out paramsValue))
                {
                    IDictionary<string, string> additionalParameters = paramsValue.Value as IDictionary<string, string>;
                    if (additionalParameters != null)
                    {
                        string extractArchiveValue;
                        if (additionalParameters.TryGetValue("extract-archive", out extractArchiveValue))
                        {
                            archiveFormat = ArchiveFormat.FromName(extractArchiveValue);
                            if (additionalParameters.Count != 1)
                                throw new NotImplementedException();
                        }
                        else
                        {
                            if (additionalParameters.Count != 0)
                                throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
            }

            if (archiveFormat != null)
            {
                return ExtractArchiveImpl(context.Request.InputStream, null, null, archiveFormat, cancellationToken);
            }

            return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
        }

        private HttpResponseMessage ProcessUpdateAccountMetadataRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(GetQueryString(context)))
                throw new NotImplementedException();

            IDictionary<string, string> headers;
            IDictionary<string, string> metadata;
            ExtractMetadata(context.Request, AccountMetadata.AccountMetadataPrefix, out headers, out metadata);

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

        private HttpResponseMessage ProcessListObjectsRequest(HttpListenerContext context, ContainerName containerName, CancellationToken cancellationToken)
        {
            UpdateContainerStatistics();

            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            int limit = 5000;
            ObjectName marker = null;
            if (!string.IsNullOrEmpty(GetQueryString(context)))
            {
                UriTemplate queryTemplate = new UriTemplate("{?limit,marker,params*}");
                UriTemplateMatch match = queryTemplate.Match(new Uri(GetQueryString(context), UriKind.Relative));
                if (match == null)
                    throw new InvalidOperationException();

                if (match.Bindings.ContainsKey("params"))
                    throw new NotImplementedException("The query contains unhandled query parameters.");

                KeyValuePair<VariableReference, object> limitValue;
                if (match.Bindings.TryGetValue("limit", out limitValue))
                {
                    if (!int.TryParse(limitValue.Value.ToString(), out limit) || limit <= 0)
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);

                    limit = Math.Min(5000, limit);
                }

                KeyValuePair<VariableReference, object> markerValue;
                if (match.Bindings.TryGetValue("marker", out markerValue))
                {
                    string markerString = markerValue.Value as string;
                    marker = new ObjectName(markerString);
                }
            }

            ConcurrentDictionary<ObjectName, ContainerObject> containerObjects;
            if (!_containerObjects.TryGetValue(container, out containerObjects) || containerObjects == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            IEnumerable<ContainerObject> filtered = containerObjects.Values.OrderBy(i => i.Name.Value, StringComparer.Ordinal);
            if (marker != null)
                filtered = filtered.Where(i => string.CompareOrdinal(i.Name.Value, marker.Value) > 0);

            ContainerObject[] objects = filtered.Take(limit).ToArray();
            string body = JsonConvert.SerializeObject(objects);
            result.Content = new StringContent(body, Encoding.UTF8, "application/json");

            ContainerMetadata containerMetadata;
            if (!_containerMetadata.TryGetValue(container, out containerMetadata))
                containerMetadata = ContainerMetadata.Empty;

            ApplyMetadata(result, containerMetadata);
            return result;
        }

        private HttpResponseMessage ProcessGetContainerMetadataRequest(HttpListenerContext context, ContainerName containerName, CancellationToken cancellationToken)
        {
            UpdateContainerStatistics();

            if (!string.IsNullOrEmpty(GetQueryString(context)))
                throw new NotImplementedException();

            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.NoContent);
            result.Content = new ByteArrayContent(new byte[0]);

            ContainerMetadata containerMetadata;
            if (!_containerMetadata.TryGetValue(container, out containerMetadata))
                containerMetadata = ContainerMetadata.Empty;

            ApplyMetadata(result, containerMetadata);
            return result;
        }

        private HttpResponseMessage ProcessCreateContainerRequest(HttpListenerContext context, ContainerName containerName, CancellationToken cancellationToken)
        {
            ArchiveFormat archiveFormat = null;

            if (!string.IsNullOrEmpty(GetQueryString(context)))
            {
                UriTemplate queryTemplate = new UriTemplate("{?params*}");

                UriTemplateMatch match = queryTemplate.Match(new Uri(GetQueryString(context), UriKind.Relative));
                if (match == null)
                    throw new InvalidOperationException();

                KeyValuePair<VariableReference, object> paramsValue;
                if (match.Bindings.TryGetValue("params", out paramsValue))
                {
                    IDictionary<string, string> additionalParameters = paramsValue.Value as IDictionary<string, string>;
                    if (additionalParameters != null)
                    {
                        string extractArchiveValue;
                        if (additionalParameters.TryGetValue("extract-archive", out extractArchiveValue))
                        {
                            archiveFormat = ArchiveFormat.FromName(extractArchiveValue);
                            if (additionalParameters.Count != 1)
                                throw new NotImplementedException();
                        }
                        else
                        {
                            if (additionalParameters.Count != 0)
                                throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
            }

            IDictionary<string, string> headers;
            IDictionary<string, string> metadata;
            ExtractMetadata(context.Request, ContainerMetadata.ContainerMetadataPrefix, out headers, out metadata);
            ContainerMetadata containerMetadata = new ContainerMetadata(headers, metadata);

            HttpResponseMessage response = CreateContainerImpl(containerName, containerMetadata, cancellationToken);

            if (archiveFormat != null)
            {
                // create the container before extracting items...
                return ExtractArchiveImpl(context.Request.InputStream, containerName, null, archiveFormat, cancellationToken);
            }

            return response;
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
            _containerMetadata.Add(container, new ContainerMetadata(headers, metadata));
            return container;
        }

        private HttpResponseMessage ProcessUpdateContainerMetadataRequest(HttpListenerContext context, ContainerName containerName, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(GetQueryString(context)))
                throw new NotImplementedException();

            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            IDictionary<string, string> headers;
            IDictionary<string, string> metadata;
            ExtractMetadata(context.Request, ContainerMetadata.ContainerMetadataPrefix, out headers, out metadata);

            ContainerMetadata previous;
            if (!_containerMetadata.TryGetValue(container, out previous))
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
            // TODO: atomic update
            _containerMetadata.Remove(container);
            _containerMetadata.Add(container, containerMetadata);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        private HttpResponseMessage ProcessDeleteContainerRequest(HttpListenerContext context, ContainerName containerName, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(GetQueryString(context)))
                throw new NotImplementedException();

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

        private HttpResponseMessage ProcessGetObjectDataRequest(HttpListenerContext context, ContainerName containerName, ObjectName objectName, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(GetQueryString(context)))
                throw new NotImplementedException();

            Tuple<Container, ContainerObject> containerObject = TryGetObject(containerName, objectName);
            if (containerObject == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            byte[] data;
            if (!_objectData.TryGetValue(containerObject.Item2, out data))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(new MemoryStream(data));

            ObjectMetadata objectMetadata;
            if (!_objectMetadata.TryGetValue(containerObject.Item2, out objectMetadata))
                objectMetadata = ObjectMetadata.Empty;

            ApplyMetadata(result, objectMetadata);
            return result;
        }

        private HttpResponseMessage ProcessGetObjectMetadataRequest(HttpListenerContext context, ContainerName containerName, ObjectName objectName, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(GetQueryString(context)))
                throw new NotImplementedException();

            Tuple<Container, ContainerObject> containerObject = TryGetObject(containerName, objectName);
            if (containerObject == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            byte[] data;
            if (!_objectData.TryGetValue(containerObject.Item2, out data))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.NoContent);
            result.Content = new ByteArrayContent(new byte[0]);
            result.Content.Headers.ContentLength = data.Length;

            ObjectMetadata objectMetadata;
            if (!_objectMetadata.TryGetValue(containerObject.Item2, out objectMetadata))
                objectMetadata = ObjectMetadata.Empty;

            ApplyMetadata(result, objectMetadata);
            return result;
        }

        private HttpResponseMessage ProcessCreateObjectRequest(HttpListenerContext context, ContainerName containerName, ObjectName objectName, CancellationToken cancellationToken)
        {
            ArchiveFormat archiveFormat = null;

            if (!string.IsNullOrEmpty(GetQueryString(context)))
            {
                UriTemplate queryTemplate = new UriTemplate("{?params*}");

                UriTemplateMatch match = queryTemplate.Match(new Uri(GetQueryString(context), UriKind.Relative));
                if (match == null)
                    throw new InvalidOperationException();

                KeyValuePair<VariableReference, object> paramsValue;
                if (match.Bindings.TryGetValue("params", out paramsValue))
                {
                    IDictionary<string, string> additionalParameters = paramsValue.Value as IDictionary<string, string>;
                    if (additionalParameters != null)
                    {
                        string extractArchiveValue;
                        if (additionalParameters.TryGetValue("extract-archive", out extractArchiveValue))
                        {
                            archiveFormat = ArchiveFormat.FromName(extractArchiveValue);
                            if (additionalParameters.Count != 1)
                                throw new NotImplementedException();
                        }
                        else
                        {
                            if (additionalParameters.Count != 0)
                                throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
            }

            if (archiveFormat != null)
            {
                return ExtractArchiveImpl(context.Request.InputStream, containerName, objectName, archiveFormat, cancellationToken);
            }

            string ifNoneMatch = context.Request.Headers["If-None-Match"];
            if (ifNoneMatch != null)
            {
                if (ifNoneMatch != "\"*\"" && ifNoneMatch != "*")
                    throw new NotSupportedException();

                Container container = TryGetContainer(containerName);
                if (container == null)
                    return new HttpResponseMessage(HttpStatusCode.NotFound);

                ContainerMetadata containerMetadata;
                if (!_containerMetadata.TryGetValue(container, out containerMetadata))
                    containerMetadata = ContainerMetadata.Empty;

                ConcurrentDictionary<ObjectName, ContainerObject> containerObjects;
                if (!_containerObjects.TryGetValue(container, out containerObjects) || containerObjects == null)
                    return new HttpResponseMessage(HttpStatusCode.NotFound);

                if (containerObjects.ContainsKey(objectName))
                    return new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
            }

            if (context.Request.Headers["X-Copy-From"] != null)
                throw new NotImplementedException();

            IDictionary<string, string> headers;
            IDictionary<string, string> metadata;
            ExtractMetadata(context.Request, ObjectMetadata.ObjectMetadataPrefix, out headers, out metadata);

            string contentType = "application/octet-stream";
            if (context.Request.ContentType != null)
            {
                headers["Content-Type"] = context.Request.ContentType;

                var contentTypeHeader = MediaTypeHeaderValue.Parse(context.Request.ContentType);
                contentType = contentTypeHeader.MediaType;
            }

            MemoryStream buffer = new MemoryStream();
            context.Request.InputStream.CopyTo(buffer);
            byte[] data = buffer.ToArray();

            return CreateObjectImpl(data, containerName, objectName, new ObjectMetadata(headers, metadata), cancellationToken);
        }

        private HttpResponseMessage CreateObjectImpl(byte[] data, ContainerName containerName, ObjectName objectName, ObjectMetadata objectMetadata, CancellationToken cancellationToken)
        {
            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            ContainerMetadata containerMetadata;
            if (!_containerMetadata.TryGetValue(container, out containerMetadata))
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
                    _objectMetadata.Add(containerObject, metadata);
                    return containerObject;
                };

            Func<ObjectName, ContainerObject, ContainerObject> updateValueFactory =
                (key, value) => addValue(key);

            containerObjects.AddOrUpdate(objectName, addValue, updateValueFactory);

            _updatedContainers.TryAdd(containerName, true);

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        private HttpResponseMessage ProcessSetObjectMetadataRequest(HttpListenerContext context, ContainerName containerName, ObjectName objectName, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(GetQueryString(context)))
                throw new NotImplementedException();

            Tuple<Container, ContainerObject> containerObject = TryGetObject(containerName, objectName);
            if (containerObject == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            IDictionary<string, string> headers;
            IDictionary<string, string> metadata;
            ExtractMetadata(context.Request, ObjectMetadata.ObjectMetadataPrefix, out headers, out metadata);
            ObjectMetadata objectMetadata = new ObjectMetadata(headers, metadata);
            // TODO: atomic update
            _objectMetadata.Remove(containerObject.Item2);
            _objectMetadata.Add(containerObject.Item2, objectMetadata);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        private HttpResponseMessage ProcessCopyObjectRequest(HttpListenerContext context, ContainerName containerName, ObjectName objectName, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(GetQueryString(context)))
                throw new NotImplementedException();

            string destination = context.Request.Headers["Destination"];
            if (destination == null)
                throw new NotImplementedException();

            if (destination.StartsWith("/"))
                destination = destination.Substring(1);

            string[] destinationContainerAndObject = destination.Split(new[] { '/' }, 2);
            ContainerName destinationContainerName = new ContainerName(destinationContainerAndObject[0]);
            ObjectName destinationObjectName = new ObjectName(destinationContainerAndObject[1]);

            IDictionary<string, string> headers;
            IDictionary<string, string> metadata;
            ExtractMetadata(context.Request, ObjectMetadata.ObjectMetadataPrefix, out headers, out metadata);

            return CopyObjectImpl(containerName, objectName, destinationContainerName, destinationObjectName, new ObjectMetadata(headers, metadata), cancellationToken);
        }

        private HttpResponseMessage CopyObjectImpl(ContainerName sourceContainerName, ObjectName sourceObjectName, ContainerName destinationContainerName, ObjectName destinationObjectName, ObjectMetadata updatedMetadata, CancellationToken cancellationToken)
        {
            Tuple<Container, ContainerObject> sourceContainerObject = TryGetObject(sourceContainerName, sourceObjectName);
            if (sourceContainerObject == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            byte[] objectData;
            if (!_objectData.TryGetValue(sourceContainerObject.Item2, out objectData) || objectData == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            ObjectMetadata objectMetadata;
            if (!_objectMetadata.TryGetValue(sourceContainerObject.Item2, out objectMetadata))
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
                    _objectMetadata.Add(copiedObject, objectMetadata);
                    return copiedObject;
                };

            Func<ObjectName, ContainerObject, ContainerObject> updateValueFactory =
                (key, value) => addValue(key);

            destinationContainerObjects.AddOrUpdate(destinationObjectName, addValue, updateValueFactory);

            _updatedContainers.TryAdd(destinationContainerName, true);

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        private HttpResponseMessage ProcessDeleteObjectRequest(HttpListenerContext context, ContainerName containerName, ObjectName objectName, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(GetQueryString(context)))
                throw new NotImplementedException();

            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            ContainerMetadata containerMetadata;
            if (!_containerMetadata.TryGetValue(container, out containerMetadata))
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
                        HttpResponseMessage response = CopyObjectImpl(versionsLocation, lastBackup, containerName, objectName, ObjectMetadata.Empty, cancellationToken);
                        if (!response.IsSuccessStatusCode)
                            return response;

                        // then delete the backup
                        return DeleteObjectImpl(versionsLocation, lastBackup, cancellationToken);
                    }
                }
            }

            return DeleteObjectImpl(containerName, objectName, cancellationToken);
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

        private readonly ConcurrentDictionary<ContainerName, bool> _updatedContainers =
            new ConcurrentDictionary<ContainerName, bool>();

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

                    ContainerMetadata containerMetadata;
                    if (!_containerMetadata.TryGetValue(existing, out containerMetadata))
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

        private static void ExtractMetadata(HttpListenerRequest request, string metadataPrefix, out IDictionary<string, string> headers, out IDictionary<string, string> metadata)
        {
            headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var key in request.Headers.AllKeys)
            {
                if (key.IndexOf('\'') >= 0)
                {
                    throw new NotSupportedException("The code for sending metadata back to the client does not support the single quote character.");
                }

                if (key.StartsWith(metadataPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    metadata.Add(key.Substring(metadataPrefix.Length), request.Headers.Get(key));
                }
                else
                {
                    switch (key.ToLowerInvariant())
                    {
                    case "content-type":
                        headers.Add(key, request.Headers.Get(key));
                        break;

                    case "accept":
                    case "connection":
                    case "content-length":
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
                            headers.Add(key, request.Headers.Get(key));
                        }

                        break;

                    default:
                        throw new NotImplementedException(string.Format("The '{0}' header is not yet supported.", key));
                    }
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

        private static string GetQueryString(HttpListenerContext context)
        {
            if (!context.Request.RawUrl.StartsWith("/"))
                throw new NotSupportedException();

            Uri uri = new Uri(context.Request.Url.GetLeftPart(UriPartial.Authority) + context.Request.RawUrl);
            return uri.Query;
        }

        private void ValidateAuthenticatedRequest(HttpListenerRequest request)
        {
            _identityService.ValidateAuthenticatedRequest(request);
            // TODO: validate tenant ID
        }
    }
}
