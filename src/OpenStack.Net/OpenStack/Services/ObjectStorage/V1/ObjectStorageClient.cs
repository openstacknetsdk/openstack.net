// Intentionally placed in the outer scope so IProgress<T> resolves to System.IProgress<T> instead of
// Rackspace.Threading.IProgress<T> in the .NET 4.0 build.
using Rackspace.Threading;

namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Collections;
    using OpenStack.IO;
    using OpenStack.Net;
    using OpenStack.Security.Authentication;
    using Rackspace.Net;
    using Stream = System.IO.Stream;

    /// <summary>
    /// This class provides a default implementation of <see cref="IObjectStorageService"/> suitable for connecting to
    /// OpenStack-compatible installations of the Object Storage Service V1.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ObjectStorageClient : ServiceClient, IObjectStorageService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectStorageClient"/> class with the specified authentication
        /// service, default region, and value indicating whether an internal or public endpoint should be used for
        /// communicating with the service.
        /// </summary>
        /// <param name="authenticationService">The authentication service to use for authenticating requests made to
        /// this service.</param>
        /// <param name="defaultRegion">The preferred region for the service. Unless otherwise specified for a specific
        /// client, derived service clients will not use a default region if this value is <see langword="null"/> (i.e.
        /// only region-less or "global" service endpoints will be considered acceptable).</param>
        /// <param name="internalUrl"><see langword="true"/> to access the service over a local network; otherwise,
        /// <see langword="false"/> to access the service over a public network (the Internet).</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="authenticationService"/> is <see langword="null"/>.
        /// </exception>
        public ObjectStorageClient(IAuthenticationService authenticationService, string defaultRegion, bool internalUrl)
            : base(authenticationService, defaultRegion, internalUrl)
        {
        }

        #region IObjectStorageService Members

        /// <inheritdoc/>
        public virtual Task<GetObjectStorageInfoApiCall> PrepareGetObjectStorageInfoAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("/info");
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetObjectStorageInfoApiCall(CreateJsonApiCall<ReadOnlyDictionary<string, JToken>>(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<ListContainersApiCall> PrepareListContainersAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate(string.Empty);
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<Tuple<AccountMetadata, ReadOnlyCollectionPage<Container>>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    Uri originalUri = responseMessage.RequestMessage.RequestUri;

                    AccountMetadata metadata = new AccountMetadata(responseMessage);
                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            innerTask =>
                            {
                                IList<Container> list = JsonConvert.DeserializeObject<Container[]>(innerTask.Result);
                                Func<CancellationToken, Task<ReadOnlyCollectionPage<Container>>> getNextPageAsync;
                                if (list.Count > 0)
                                {
                                    getNextPageAsync =
                                        innerCancellationToken2 =>
                                        {
                                            ContainerName marker = list.Last().Name;
                                            return
                                                TaskBlocks.Using(
                                                    () => PrepareListContainersAsync(innerCancellationToken2)
                                                        .WithUri(originalUri)
                                                        .WithMarker(marker),
                                                    _ => _.Result.SendAsync(innerCancellationToken2))
                                                .Select(_ => _.Result.Item2.Item2);
                                        };
                                }
                                else
                                {
                                    getNextPageAsync = null;
                                }

                                ReadOnlyCollectionPage<Container> results = new BasicReadOnlyCollectionPage<Container>(list, getNextPageAsync);
                                return Tuple.Create(metadata, results);
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListContainersApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<GetAccountMetadataApiCall> PrepareGetAccountMetadataAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate(string.Empty);
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<AccountMetadata>> deserializeResult =
                (responseMessage, innerCancellationToken) => CompletedTask.FromResult(new AccountMetadata(responseMessage));

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Head, template, parameters, cancellationToken))
                .Select(task => new GetAccountMetadataApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<UpdateAccountMetadataApiCall> PrepareUpdateAccountMetadataAsync(AccountMetadata metadata, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate(string.Empty);
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, cancellationToken))
                .Select(
                    task =>
                    {
                        var call = CreateBasicApiCall(task.Result);
                        var requestHeaders = call.RequestMessage.Headers;
                        foreach (var pair in metadata.Headers)
                        {
                            requestHeaders.Remove(pair.Key);
                            requestHeaders.Add(pair.Key, StorageMetadata.EncodeHeaderValue(pair.Value));
                            ValidateAccountMetadataValue(true, pair.Key, pair.Value);
                        }

                        foreach (var pair in metadata.Metadata)
                        {
                            requestHeaders.Remove(AccountMetadata.AccountMetadataPrefix + pair.Key);
                            requestHeaders.Add(AccountMetadata.AccountMetadataPrefix + pair.Key, StorageMetadata.EncodeHeaderValue(pair.Value));
                            ValidateAccountMetadataValue(false, pair.Key, pair.Value);
                        }

                        return new UpdateAccountMetadataApiCall(call);
                    });
        }

        /// <inheritdoc/>
        public virtual Task<UpdateAccountMetadataApiCall> PrepareRemoveAccountMetadataAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            if (keys == null)
                throw new ArgumentNullException("keys");

            Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in keys)
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentException("keys cannot contain any null or empty values");

                metadata.Add(key, string.Empty);
            }

            return PrepareUpdateAccountMetadataAsync(new AccountMetadata(new Dictionary<string, string>(), metadata), cancellationToken);
        }

        /// <inheritdoc/>
        public virtual Task<CreateContainerApiCall> PrepareCreateContainerAsync(ContainerName container, CancellationToken cancellationToken)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            UriTemplate template = new UriTemplate("{container}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "container", container.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, cancellationToken))
                .Select(task => new CreateContainerApiCall(CreateBasicApiCall(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<RemoveContainerApiCall> PrepareRemoveContainerAsync(ContainerName container, CancellationToken cancellationToken)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            UriTemplate template = new UriTemplate("{container}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "container", container.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveContainerApiCall(CreateBasicApiCall(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<ListObjectsApiCall> PrepareListObjectsAsync(ContainerName container, CancellationToken cancellationToken)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            UriTemplate template = new UriTemplate("{container}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "container", container.Value } };

            Func<HttpResponseMessage, CancellationToken, Task<Tuple<ContainerMetadata, ReadOnlyCollectionPage<ContainerObject>>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    Uri originalUri = responseMessage.RequestMessage.RequestUri;

                    ContainerMetadata metadata = new ContainerMetadata(responseMessage);
                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            innerTask =>
                            {
                                IList<ContainerObject> list = JsonConvert.DeserializeObject<ContainerObject[]>(innerTask.Result);
                                Func<CancellationToken, Task<ReadOnlyCollectionPage<ContainerObject>>> getNextPageAsync;
                                if (list.Count > 0)
                                {
                                    getNextPageAsync =
                                        innerCancellationToken2 =>
                                        {
                                            ObjectName marker = list.Last().Name;
                                            return
                                                TaskBlocks.Using(
                                                    () => PrepareListObjectsAsync(container, innerCancellationToken2)
                                                        .WithUri(originalUri)
                                                        .WithMarker(marker),
                                                    _ => _.Result.SendAsync(innerCancellationToken2))
                                                .Select(_ => _.Result.Item2.Item2);
                                        };
                                }
                                else
                                {
                                    getNextPageAsync = null;
                                }

                                ReadOnlyCollectionPage<ContainerObject> results = new BasicReadOnlyCollectionPage<ContainerObject>(list, getNextPageAsync);
                                return Tuple.Create(metadata, results);
                            });
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListObjectsApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<GetContainerMetadataApiCall> PrepareGetContainerMetadataAsync(ContainerName container, CancellationToken cancellationToken)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            UriTemplate template = new UriTemplate("{container}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "container", container.Value } };

            Func<HttpResponseMessage, CancellationToken, Task<ContainerMetadata>> deserializeResult =
                (responseMessage, innerCancellationToken) => CompletedTask.FromResult(new ContainerMetadata(responseMessage));

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Head, template, parameters, cancellationToken))
                .Select(task => new GetContainerMetadataApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<UpdateContainerMetadataApiCall> PrepareUpdateContainerMetadataAsync(ContainerName container, ContainerMetadata metadata, CancellationToken cancellationToken)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            UriTemplate template = new UriTemplate("{container}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "container", container.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, cancellationToken))
                .Select(
                    task =>
                    {
                        var call = CreateBasicApiCall(task.Result);
                        var requestHeaders = call.RequestMessage.Headers;
                        foreach (var pair in metadata.Headers)
                        {
                            requestHeaders.Remove(pair.Key);
                            requestHeaders.Add(pair.Key, StorageMetadata.EncodeHeaderValue(pair.Value));
                            ValidateContainerMetadataValue(true, pair.Key, pair.Value);
                        }

                        foreach (var pair in metadata.Metadata)
                        {
                            requestHeaders.Remove(ContainerMetadata.ContainerMetadataPrefix + pair.Key);
                            requestHeaders.Add(ContainerMetadata.ContainerMetadataPrefix + pair.Key, StorageMetadata.EncodeHeaderValue(pair.Value));
                            ValidateContainerMetadataValue(false, pair.Key, pair.Value);
                        }

                        return new UpdateContainerMetadataApiCall(call);
                    });
        }

        /// <inheritdoc/>
        public virtual Task<UpdateContainerMetadataApiCall> PrepareRemoveContainerMetadataAsync(ContainerName container, IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            if (keys == null)
                throw new ArgumentNullException("keys");

            Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in keys)
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentException("keys cannot contain any null or empty values");

                metadata.Add(key, string.Empty);
            }

            return PrepareUpdateContainerMetadataAsync(container, new ContainerMetadata(new Dictionary<string, string>(), metadata), cancellationToken);
        }

        /// <inheritdoc/>
        public virtual Task<CreateObjectApiCall> PrepareCreateObjectAsync(ContainerName container, ObjectName @object, Stream stream, CancellationToken cancellationToken, IProgress<long> progress)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (@object == null)
                throw new ArgumentNullException("object");
            if (stream == null)
                throw new ArgumentNullException("stream");

            UriTemplate template = new UriTemplate("{container}/{object}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "container", container.Value }, { "object", @object.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, cancellationToken))
                .Select(
                    task =>
                    {
                        var call = CreateBasicApiCall(task.Result);
                        Stream contentStream = stream;
                        if (progress != null)
                            contentStream = new ProgressStream(contentStream, progress, false);
                        else
                            contentStream = new DelegatingStream(contentStream, false);

                        call.RequestMessage.Content = new StreamContent(contentStream);
                        return new CreateObjectApiCall(call);
                    });
        }

        /// <inheritdoc/>
        public virtual Task<CopyObjectApiCall> PrepareCopyObjectAsync(ContainerName sourceContainer, ObjectName sourceObject, ContainerName destinationContainer, ObjectName destinationObject, CancellationToken cancellationToken)
        {
            if (sourceContainer == null)
                throw new ArgumentNullException("sourceContainer");
            if (sourceObject == null)
                throw new ArgumentNullException("sourceObject");
            if (destinationContainer == null)
                throw new ArgumentNullException("destinationContainer");
            if (destinationObject == null)
                throw new ArgumentNullException("destinationObject");

            UriTemplate template = new UriTemplate("{container}/{object}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "container", sourceContainer.Value }, { "object", sourceObject.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(new HttpMethod("COPY"), template, parameters, cancellationToken))
                .Select(
                    task =>
                    {
                        var call = CreateBasicApiCall(task.Result);
                        var requestHeaders = call.RequestMessage.Headers;

                        UriTemplate destinationTemplate = new UriTemplate("/{container}/{object}");
                        Dictionary<string, string> destinationParameters = new Dictionary<string, string>
                        {
                            { "container", destinationContainer.Value },
                            { "object", destinationObject.Value }
                        };

                        requestHeaders.Add("Destination", destinationTemplate.BindByName(destinationParameters).OriginalString);
                        return new CopyObjectApiCall(call);
                    });
        }

        /// <inheritdoc/>
        public virtual Task<RemoveObjectApiCall> PrepareRemoveObjectAsync(ContainerName container, ObjectName @object, CancellationToken cancellationToken)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (@object == null)
                throw new ArgumentNullException("object");

            UriTemplate template = new UriTemplate("{container}/{object}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "container", container.Value }, { "object", @object.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveObjectApiCall(CreateBasicApiCall(task.Result)));
        }

        /// <inheritdoc/>
        public virtual Task<GetObjectApiCall> PrepareGetObjectAsync(ContainerName container, ObjectName @object, CancellationToken cancellationToken)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (@object == null)
                throw new ArgumentNullException("object");

            UriTemplate template = new UriTemplate("{container}/{object}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "container", container.Value }, { "object", @object.Value } };

            Func<HttpResponseMessage, CancellationToken, Task<Tuple<ObjectMetadata, Stream>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    return responseMessage.Content.ReadAsStreamAsync()
                        .Select(innerTask => Tuple.Create(new ObjectMetadata(responseMessage), innerTask.Result));
                };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetObjectApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseHeadersRead, deserializeResult)));
        }

        /// <inheritdoc/>
        public virtual Task<GetObjectMetadataApiCall> PrepareGetObjectMetadataAsync(ContainerName container, ObjectName @object, CancellationToken cancellationToken)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (@object == null)
                throw new ArgumentNullException("object");

            UriTemplate template = new UriTemplate("{container}/{object}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "container", container.Value }, { "object", @object.Value } };

            Func<HttpResponseMessage, CancellationToken, Task<ObjectMetadata>> deserializeResult =
                (responseMessage, innerCancellationToken) => CompletedTask.FromResult(new ObjectMetadata(responseMessage));

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Head, template, parameters, cancellationToken))
                .Select(task => new GetObjectMetadataApiCall(CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>This implementation avoids sending HTTP headers present in the <see cref="StorageMetadata.Headers"/>
        /// property of <paramref name="metadata"/> which the Object Storage service is known to reject. This simplifies
        /// the use of this method in combination with <see cref="PrepareGetObjectMetadataAsync"/> to update, as opposed
        /// to replace, the metadata associated with an object.</para>
        /// </remarks>
        public virtual Task<SetObjectMetadataApiCall> PrepareSetObjectMetadataAsync(ContainerName container, ObjectName @object, ObjectMetadata metadata, CancellationToken cancellationToken)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (@object == null)
                throw new ArgumentNullException("object");
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            UriTemplate template = new UriTemplate("{container}/{object}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "container", container.Value }, { "object", @object.Value } };

            return GetBaseUriAsync(cancellationToken)
                .Then(PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, cancellationToken))
                .Select(
                    task =>
                    {
                        var call = CreateBasicApiCall(task.Result);
                        if (call.RequestMessage.Content == null)
                            call.RequestMessage.Content = new ByteArrayContent(new byte[0]);

                        var requestHeaders = call.RequestMessage.Headers;
                        var contentHeaders = call.RequestMessage.Content.Headers;
                        foreach (var pair in metadata.Headers)
                        {
                            switch (pair.Key.ToLowerInvariant())
                            {
                            case "content-length":
                            case "etag":
                            case "accept-ranges":
                            //case "x-trans-id":
                            case "x-timestamp":
                            case "date":
                                continue;

                            case "allow":
                            case "last-modified":
                                //contentHeaders.Remove(pair.Key);
                                //contentHeaders.Add(pair.Key, pair.Value);
                                break;

                            default:
                                if (pair.Key.ToLowerInvariant().StartsWith("content-"))
                                {
                                    contentHeaders.Remove(pair.Key);
                                    if (!string.IsNullOrEmpty(pair.Value))
                                    {
                                        contentHeaders.Add(pair.Key, StorageMetadata.EncodeHeaderValue(pair.Value));
                                        ValidateObjectMetadataValue(true, pair.Key, pair.Value);
                                    }
                                }
                                else
                                {
                                    requestHeaders.Remove(pair.Key);
                                    if (!string.IsNullOrEmpty(pair.Value))
                                    {
                                        requestHeaders.Add(pair.Key, StorageMetadata.EncodeHeaderValue(pair.Value));
                                        ValidateObjectMetadataValue(true, pair.Key, pair.Value);
                                    }
                                }

                                break;
                            }
                        }

                        foreach (var pair in metadata.Metadata)
                        {
                            string prefix = ObjectMetadata.ObjectMetadataPrefix;
                            string key = prefix + pair.Key;
                            string value = pair.Value;
                            requestHeaders.Remove(key);
                            if (!string.IsNullOrEmpty(value))
                            {
                                requestHeaders.Add(key, StorageMetadata.EncodeHeaderValue(value));
                                ValidateObjectMetadataValue(false, pair.Key, value);
                            }
                        }

                        return new SetObjectMetadataApiCall(call);
                    });
        }

        /// <inheritdoc/>
        public virtual TExtension GetServiceExtension<TExtension>(ServiceExtensionDefinition<IObjectStorageService, TExtension> definition)
        {
            if (definition == null)
                throw new ArgumentNullException("definition");

            return definition.CreateDefaultInstance(this, this);
        }

        #endregion

        /// <summary>
        /// Validate a header or metadata item associated with an account in the Object Storage Service.
        /// </summary>
        /// <remarks>
        /// <note type="implement">
        /// <para>This method is provided in order to change the specific behavior for accounts in the Object Storage
        /// Service. To change the common behavior for all resources, override <see cref="ValidateMetadataValue"/>
        /// instead.</para>
        /// </note>
        /// <para>The default implementation simply calls <see cref="ValidateMetadataValue"/>.</para>
        /// </remarks>
        /// <param name="isHeader">
        /// <para><see langword="true"/> if the value is associated with a non-metadata HTTP header.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> if the value is associated with a metadata HTTP header.</para>
        /// </param>
        /// <param name="key">
        /// The header or metadata key. If <paramref name="isHeader"/> is <see langword="false"/>, this value does not
        /// contain the <see cref="AccountMetadata.AccountMetadataPrefix"/>.
        /// </param>
        /// <param name="value">The header or metadata value.</param>
        /// <exception cref="NotSupportedException">
        /// <para>If the specified header or metadata item is not supported by the Object Storage Service.</para>
        /// </exception>
        protected virtual void ValidateAccountMetadataValue(bool isHeader, string key, string value)
        {
            ValidateMetadataValue(isHeader, key, value);
        }

        /// <summary>
        /// Validate a header or metadata item associated with a container in the Object Storage Service.
        /// </summary>
        /// <remarks>
        /// <note type="implement">
        /// <para>This method is provided in order to change the specific behavior for containers in the Object Storage
        /// Service. To change the common behavior for all resources, override <see cref="ValidateMetadataValue"/>
        /// instead.</para>
        /// </note>
        /// <para>The default implementation simply calls <see cref="ValidateMetadataValue"/>.</para>
        /// </remarks>
        /// <param name="isHeader">
        /// <para><see langword="true"/> if the value is associated with a non-metadata HTTP header.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> if the value is associated with a metadata HTTP header.</para>
        /// </param>
        /// <param name="key">
        /// The name of the header or metadata item. If <paramref name="isHeader"/> is <see langword="false"/>, this
        /// value does not contain the <see cref="ContainerMetadata.ContainerMetadataPrefix"/>.
        /// </param>
        /// <param name="value">The header or metadata value.</param>
        /// <exception cref="NotSupportedException">
        /// <para>If the specified header or metadata item is not supported by the Object Storage Service.</para>
        /// </exception>
        protected virtual void ValidateContainerMetadataValue(bool isHeader, string key, string value)
        {
            ValidateMetadataValue(isHeader, key, value);
        }

        /// <summary>
        /// Validate a header or metadata item associated with an object in the Object Storage Service.
        /// </summary>
        /// <remarks>
        /// <note type="implement">
        /// <para>This method is provided in order to change the specific behavior for objects in the Object Storage
        /// Service. To change the common behavior for all resources, override <see cref="ValidateMetadataValue"/>
        /// instead.</para>
        /// </note>
        /// <para>The default implementation simply calls <see cref="ValidateMetadataValue"/>.</para>
        /// </remarks>
        /// <param name="isHeader">
        /// <para><see langword="true"/> if the value is associated with a non-metadata HTTP header.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> if the value is associated with a metadata HTTP header.</para>
        /// </param>
        /// <param name="key">
        /// The name of the header or metadata item. If <paramref name="isHeader"/> is <see langword="false"/>, this
        /// value does not contain the <see cref="ObjectMetadata.ObjectMetadataPrefix"/>.
        /// </param>
        /// <param name="value">The header or metadata value.</param>
        /// <exception cref="NotSupportedException">
        /// <para>If the specified header or metadata item is not supported by the Object Storage Service.</para>
        /// </exception>
        protected virtual void ValidateObjectMetadataValue(bool isHeader, string key, string value)
        {
            ValidateMetadataValue(isHeader, key, value);
        }

        /// <summary>
        /// Validate a header or metadata item associated with a resource in the Object Storage Service.
        /// </summary>
        /// <remarks>
        /// <para>This method is called <em>after</em> verifying that the metadata key and value are valid according to
        /// the HTTP/1.1 specification. It provides support for additional restrictions which are imposed by a
        /// particular vendor's deployment of the Object Storage Service. The default implementation returns
        /// <see langword="false"/> for any <paramref name="key"/> which contains an underscore (<c>_</c>), since the
        /// default OpenStack Object Storage Service does not allow a header or metadata key to contain that
        /// character.</para>
        /// <note type="implement">
        /// <para>This method is called by the default implementations of <see cref="ValidateAccountMetadataValue"/>,
        /// <see cref="ValidateContainerMetadataValue"/>, and <see cref="ValidateObjectMetadataValue"/>. Override this
        /// method to change the common behavior shared by of each of those methods.</para>
        /// </note>
        /// </remarks>
        /// <param name="isHeader">
        /// <para><see langword="true"/> if the value is associated with a non-metadata HTTP header.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> if the value is associated with a metadata HTTP header.</para>
        /// </param>
        /// <param name="key">
        /// The header or metadata key. If <paramref name="isHeader"/> is <see langword="false"/>, this value does not
        /// contain the metadata prefix for the associated resource.
        /// </param>
        /// <param name="value">The header or metadata value for an account, container, or object in the Object Storage
        /// Service.</param>
        /// <exception cref="NotSupportedException">
        /// <para>If the specified header or metadata item is not supported by the Object Storage Service.</para>
        /// </exception>
        protected virtual void ValidateMetadataValue(bool isHeader, string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (key.IndexOf('_') >= 0)
                throw new NotSupportedException("Metadata keys in the Object Storage Service cannot contain an underscore");
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method calls <see cref="IAuthenticationService.GetBaseAddressAsync"/> to obtain a URI for the type
        /// <c>object-store</c>. The preferred name is not specified.
        /// </remarks>
        protected override Task<Uri> GetBaseUriAsyncImpl(CancellationToken cancellationToken)
        {
            const string serviceType = "object-store";
            const string serviceName = null;
            return AuthenticationService.GetBaseAddressAsync(serviceType, serviceName, DefaultRegion, InternalUrl, cancellationToken);
        }
    }
}
