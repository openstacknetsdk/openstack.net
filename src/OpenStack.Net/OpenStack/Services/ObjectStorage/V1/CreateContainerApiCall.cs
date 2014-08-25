namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to create a container in the Object Storage Service.
    /// </summary>
    /// <seealso cref="IObjectStorageService.PrepareCreateContainerAsync"/>
    /// <seealso cref="ObjectStorageServiceExtensions.CreateContainerAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class CreateContainerApiCall : DelegatingHttpApiCall<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateContainerApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public CreateContainerApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
