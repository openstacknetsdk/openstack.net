namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to list objects in a container in the Object Storage Service.
    /// </summary>
    /// <seealso cref="IObjectStorageService.PrepareListObjectsAsync"/>
    /// <seealso cref="ObjectStorageServiceExtensions.ListObjectsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListObjectsApiCall : DelegatingHttpApiCall<Tuple<ContainerMetadata, ReadOnlyCollectionPage<ContainerObject>>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListObjectsApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public ListObjectsApiCall(IHttpApiCall<Tuple<ContainerMetadata, ReadOnlyCollectionPage<ContainerObject>>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
