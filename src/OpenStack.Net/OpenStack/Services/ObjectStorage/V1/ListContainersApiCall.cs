namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to list containers in the Object Storage Service.
    /// </summary>
    /// <seealso cref="IObjectStorageService.PrepareListContainersAsync"/>
    /// <seealso cref="ObjectStorageServiceExtensions.ListContainersAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListContainersApiCall : DelegatingHttpApiCall<Tuple<AccountMetadata, ReadOnlyCollectionPage<Container>>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListContainersApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public ListContainersApiCall(IHttpApiCall<Tuple<AccountMetadata, ReadOnlyCollectionPage<Container>>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
