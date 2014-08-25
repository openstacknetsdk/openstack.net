namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using OpenStack.Net;
    using Stream = System.IO.Stream;

    /// <summary>
    /// This class represents an HTTP API call to get the content of and metadata associated with an object in the
    /// Object Storage Service.
    /// </summary>
    /// <seealso cref="IObjectStorageService.PrepareGetObjectAsync"/>
    /// <seealso cref="O:OpenStack.Services.ObjectStorage.V1.ObjectStorageServiceExtensions.GetObjectAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetObjectApiCall : DelegatingHttpApiCall<Tuple<ObjectMetadata, Stream>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetObjectApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public GetObjectApiCall(IHttpApiCall<Tuple<ObjectMetadata, Stream>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
