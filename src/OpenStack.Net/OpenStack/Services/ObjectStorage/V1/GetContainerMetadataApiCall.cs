namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to get the metadata associated with a container in the Object Storage
    /// Service.
    /// </summary>
    /// <seealso cref="IObjectStorageService.PrepareGetContainerMetadataAsync"/>
    /// <seealso cref="ObjectStorageServiceExtensions.GetContainerMetadataAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetContainerMetadataApiCall : DelegatingHttpApiCall<ContainerMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetContainerMetadataApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public GetContainerMetadataApiCall(IHttpApiCall<ContainerMetadata> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
