namespace OpenStack.Services
{
    using System;

    /// <summary>
    /// This interface represents a service which can be extended by defining service extension definitions.
    /// </summary>
    /// <typeparam name="TService">The service type which supports extensions.</typeparam>
    /// <preliminary/>
    public interface IExtensibleService<TService>
        where TService : IHttpService
    {
        /// <summary>
        /// Gets an implementation of an extension to the service.
        /// </summary>
        /// <typeparam name="TExtension">The service extension type.</typeparam>
        /// <param name="definition">The service extension definition.</param>
        /// <returns>
        /// An instance of <typeparamref name="TExtension"/> providing the implementation of the service extension.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="definition"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the service client implementation does not provide its own implementation of the specified service
        /// extension, and the service extension definition does not provide a default implementation.
        /// </exception>
        TExtension GetServiceExtension<TExtension>(ServiceExtensionDefinition<TService, TExtension> definition);
    }
}
