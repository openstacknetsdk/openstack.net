namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to copy an object in the Object Storage Service.
    /// </summary>
    /// <seealso cref="IObjectStorageService.PrepareCopyObjectAsync"/>
    /// <seealso cref="ObjectStorageServiceExtensions.CopyObjectAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class CopyObjectApiCall : DelegatingHttpApiCall<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyObjectApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public CopyObjectApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
