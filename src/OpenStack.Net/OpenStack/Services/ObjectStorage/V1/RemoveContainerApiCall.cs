namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to remove a container in the Object Storage Service.
    /// </summary>
    /// <seealso cref="IObjectStorageService.PrepareRemoveContainerAsync"/>
    /// <seealso cref="ObjectStorageServiceExtensions.RemoveContainerAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class RemoveContainerApiCall : DelegatingHttpApiCall<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveContainerApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public RemoveContainerApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
