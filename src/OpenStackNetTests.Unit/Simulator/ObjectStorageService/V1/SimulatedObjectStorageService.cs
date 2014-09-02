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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Services.ObjectStorage.V1;
    using OpenStackNetTests.Unit.Simulator.IdentityService.V2;
    using Rackspace.Net;

    public class SimulatedObjectStorageService : SimulatedService
    {
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
            UriTemplateMatch match = _accountRequestTemplate.Match(dispatchUri);
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

        private HttpResponseMessage ProcessAccountRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {
            ValidateAuthenticatedRequest(context.Request);

            switch (context.Request.HttpMethod.ToUpperInvariant())
            {
            case "GET":
                return ProcessListContainersRequest(context, cancellationToken);

            case "HEAD":
                return ProcessGetAccountMetadataRequest(context, cancellationToken);

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

        private HttpResponseMessage ProcessListContainersRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {
            int limit = 5000;
            ContainerName marker = null;
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
            {
                UriTemplate queryTemplate = new UriTemplate("{?limit,marker,params*}");
                UriTemplateMatch match = queryTemplate.Match(new Uri(context.Request.Url.Query, UriKind.Relative));
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
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
                throw new NotImplementedException();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.NoContent);

            AccountMetadata accountMetadata = GetAccountMetadata();
            ApplyMetadata(result, accountMetadata);
            return result;
        }

        private HttpResponseMessage ProcessUpdateAccountMetadataRequest(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
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
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
            {
                UriTemplate queryTemplate = new UriTemplate("{?limit,marker,params*}");
                UriTemplateMatch match = queryTemplate.Match(new Uri(context.Request.Url.Query, UriKind.Relative));
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

            if (!string.IsNullOrEmpty(context.Request.Url.Query))
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
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
                throw new NotImplementedException();

            bool created = false;
            Container container = _containers.GetOrAdd(containerName,
                i =>
                {
                    created = true;
                    return CreateContainer(i, 0, 0);
                });

            _containerObjects.GetOrCreateValue(container);

            return new HttpResponseMessage(created ? HttpStatusCode.Created : HttpStatusCode.Accepted);
        }

        private static Container CreateContainer(ContainerName containerName, long objectCount, long objectSize)
        {
            JObject jsonObject =
                new JObject(
                    new JProperty("count", objectCount),
                    new JProperty("bytes", objectSize),
                    new JProperty("name", JValue.CreateString(containerName.Value)));

            return jsonObject.ToObject<Container>();
        }

        private HttpResponseMessage ProcessUpdateContainerMetadataRequest(HttpListenerContext context, ContainerName containerName, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
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
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
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
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
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
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
                throw new NotImplementedException();

            Tuple<Container, ContainerObject> containerObject = TryGetObject(containerName, objectName);
            if (containerObject == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.NoContent);
            result.Content = new ByteArrayContent(new byte[0]);

            ObjectMetadata objectMetadata;
            if (!_objectMetadata.TryGetValue(containerObject.Item2, out objectMetadata))
                objectMetadata = ObjectMetadata.Empty;

            ApplyMetadata(result, objectMetadata);
            return result;
        }

        private HttpResponseMessage ProcessCreateObjectRequest(HttpListenerContext context, ContainerName containerName, ObjectName objectName, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
                throw new NotImplementedException();

            Container container = TryGetContainer(containerName);
            if (container == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            ConcurrentDictionary<ObjectName, ContainerObject> containerObjects;
            if (!_containerObjects.TryGetValue(container, out containerObjects) || containerObjects == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

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

            ObjectMetadata objectMetadata = new ObjectMetadata(headers, metadata);

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
                    _objectMetadata.Add(containerObject, objectMetadata);
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
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
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
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
                throw new NotImplementedException();

            Tuple<Container, ContainerObject> containerObject = TryGetObject(containerName, objectName);
            if (containerObject == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);



            throw new NotImplementedException();
        }

        private HttpResponseMessage ProcessDeleteObjectRequest(HttpListenerContext context, ContainerName containerName, ObjectName objectName, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(context.Request.Url.Query))
                throw new NotImplementedException();

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

                    long objectCount = containerObjects.Count;
                    long objectSize = containerObjects.Sum(i => i.Value.Size ?? 0);
                    updated = CreateContainer(pair.Key, objectCount, objectSize);
                    _containerObjects.Add(updated, containerObjects);

                    ContainerMetadata containerMetadata;
                    if (!_containerMetadata.TryGetValue(existing, out containerMetadata))
                        containerMetadata = ContainerMetadata.Empty;

                    _containerMetadata.Add(updated, containerMetadata);
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
            headers.Add("X-Account-Container-Count", containerCount.ToString());
            headers.Add("X-Account-Object-Count", objectCount.ToString());
            headers.Add("X-Account-Bytes-Used", containerSize.ToString());

            IDictionary<string, string> metadata = accountMetadata.Metadata;

            return new AccountMetadata(headers, metadata);
        }

        private static void ExtractMetadata(HttpListenerRequest request, string metadataPrefix, out IDictionary<string, string> headers, out IDictionary<string, string> metadata)
        {
            headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var key in request.Headers.AllKeys)
            {
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
                    case "expect":
                    case "host":
                    case "server":
                    case "user-agent":
                    case "x-auth-token":
                        // these headers are simply ignored
                        break;

                    default:
                        throw new NotImplementedException();
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

        private void ValidateAuthenticatedRequest(HttpListenerRequest request)
        {
            _identityService.ValidateAuthenticatedRequest(request);
            // TODO: validate tenant ID
        }
    }
}
