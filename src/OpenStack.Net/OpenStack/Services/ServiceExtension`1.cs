namespace OpenStack.Services
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class provides a base class for service extensions which are initialized with a service client instance and
    /// an HTTP API call factory.
    /// </summary>
    /// <typeparam name="TService">The service type which is extended by this extension.</typeparam>
    /// <seealso cref="IExtensibleService{TService}"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public abstract class ServiceExtension<TService>
        where TService : IHttpService
    {
        /// <summary>
        /// This is the backing field for the <see cref="Service"/> property.
        /// </summary>
        private readonly TService _service;

        /// <summary>
        /// This is the backing field for the <see cref="HttpApiCallFactory"/> property.
        /// </summary>
        private readonly IHttpApiCallFactory _httpApiCallFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceExtension{TService}"/> class using the specified service
        /// client and HTTP API call factory.
        /// </summary>
        /// <param name="service">The service instance.</param>
        /// <param name="httpApiCallFactory">The factory to use for creating new HTTP API calls for the
        /// extension.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="httpApiCallFactory"/> is <see langword="null"/>.</para>
        /// </exception>
        protected ServiceExtension(TService service, IHttpApiCallFactory httpApiCallFactory)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (httpApiCallFactory == null)
                throw new ArgumentNullException("httpApiCallFactory");

            _service = service;
            _httpApiCallFactory = httpApiCallFactory;
        }

        /// <summary>
        /// Gets the service instance the extension was created from.
        /// </summary>
        /// <value>
        /// The service instance the extension was created from.
        /// </value>
        protected TService Service
        {
            get
            {
                return _service;
            }
        }

        /// <summary>
        /// Gets the HTTP API call factory to use for creating new HTTP API calls for the extension.
        /// </summary>
        /// <value>
        /// The <see cref="IHttpApiCallFactory"/> to use for creating new HTTP API calls for the extension.
        /// </value>
        protected IHttpApiCallFactory HttpApiCallFactory
        {
            get
            {
                return _httpApiCallFactory;
            }
        }
    }
}
