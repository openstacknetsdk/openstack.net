namespace OpenStack.Services
{
    using System;

    /// <summary>
    /// This class serves as the base class for all service extension definitions. Service extension definitions are
    /// used to obtain instances of a particular service extension from a service client implementation.
    /// </summary>
    /// <typeparam name="TService">The primary service type.</typeparam>
    /// <typeparam name="TExtension">The service extension type.</typeparam>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public abstract class ServiceExtensionDefinition<TService, TExtension> : IEquatable<ServiceExtensionDefinition<TService, TExtension>>
        where TService : IHttpService
    {
        /// <summary>
        /// Gets the name of the service extension.
        /// </summary>
        /// <value>
        /// The name of the service extension.
        /// </value>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Creates a default instance of the extension.
        /// </summary>
        /// <remarks>
        /// Service client implementations are given the first opportunity to create an instance of
        /// <typeparamref name="TExtension"/> for a particular service extension definition. To improve flexibility in
        /// using vendor-specific extensions, service extension definitions support creating default instances of a
        /// service extension if the service client implementation does not provide its own implementation.
        /// </remarks>
        /// <param name="service">The service client instance.</param>
        /// <returns>
        /// A default instance of <typeparamref name="TExtension"/> for the specified service client.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="service"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the service extension definition does not provide a default implementation.
        /// </exception>
        public abstract TExtension CreateDefaultInstance(TService service);

        /// <inheritdoc/>
        /// <remarks>
        /// <para>By default, service extension definitions use reference equality.</para>
        /// </remarks>
        public bool Equals(ServiceExtensionDefinition<TService, TExtension> other)
        {
            return ReferenceEquals(this, other);
        }
    }
}
