namespace OpenStack.Services
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class provides a base class for service extensions which are initialized with a service client instance.
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
        /// Initializes a new instance of the <see cref="ServiceExtension{TService}"/> class using the specified service
        /// client.
        /// </summary>
        /// <param name="service">The service instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// </exception>
        protected ServiceExtension(TService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            _service = service;
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
    }
}
