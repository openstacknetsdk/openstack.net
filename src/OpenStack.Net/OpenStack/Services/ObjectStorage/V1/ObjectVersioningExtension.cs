namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;
    using Rackspace.Threading;

    /// <summary>
    /// This class provides the default implementation of the Object Versioning extension to the OpenStack Object
    /// Storage Service V1.
    /// </summary>
    /// <remarks>
    /// <note type="note">
    /// <para>This class should not be instantiated directly. It should be obtained by passing
    /// <see cref="PredefinedObjectStorageExtensions.ExtractArchive"/> to
    /// <see cref="IExtensibleService{TService}.GetServiceExtension{TExtension}"/>.</para>
    /// </note>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ObjectVersioningExtension : ServiceExtension<IObjectStorageService>, IObjectVersioningExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectVersioningExtension"/> class using the specified Object
        /// Storage Service client and HTTP API call factory.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="httpApiCallFactory">The factory to use for creating new HTTP API calls for the
        /// extension.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="httpApiCallFactory"/> is <see langword="null"/>.</para>
        /// </exception>
        public ObjectVersioningExtension(IObjectStorageService service, IHttpApiCallFactory httpApiCallFactory)
            : base(service, httpApiCallFactory)
        {
        }

        /// <inheritdoc/>
        public virtual Task<CreateVersionedContainerApiCall> PrepareCreateVersionedContainerAsync(ContainerName container, ContainerName versionsLocation, CancellationToken cancellationToken)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (versionsLocation == null)
                throw new ArgumentNullException("versionsLocation");

            return Service.PrepareCreateContainerAsync(container, cancellationToken)
                .WithVersionsLocation(versionsLocation)
                .Select(task => new CreateVersionedContainerApiCall(task.Result));
        }

        /// <inheritdoc/>
        public virtual Task<GetVersionsLocationApiCall> PrepareGetVersionsLocationAsync(ContainerName container, CancellationToken cancellationToken)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            return Service.PrepareGetContainerMetadataAsync(container, cancellationToken)
                .Select(task => new GetVersionsLocationApiCall(task.Result));
        }

        /// <inheritdoc/>
        public virtual Task<SetVersionsLocationApiCall> PrepareSetVersionsLocationAsync(ContainerName container, ContainerName versionsLocation, CancellationToken cancellationToken)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            return Service.PrepareUpdateContainerMetadataAsync(container, ContainerMetadata.Empty, cancellationToken)
                .WithVersionsLocation(versionsLocation)
                .Select(task => new SetVersionsLocationApiCall(task.Result));
        }
    }
}
