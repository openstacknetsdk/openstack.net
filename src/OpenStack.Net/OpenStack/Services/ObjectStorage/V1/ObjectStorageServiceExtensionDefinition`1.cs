namespace OpenStack.Services.ObjectStorage.V1
{
    /// <summary>
    /// This class serves as the base class for all service extension definitions for the
    /// <see cref="IObjectStorageService"/>.
    /// </summary>
    /// <typeparam name="TExtension">The service extension type.</typeparam>
    /// <seealso cref="IExtensibleService{TService}.GetServiceExtension{TExtension}"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public abstract class ObjectStorageServiceExtensionDefinition<TExtension> : ServiceExtensionDefinition<IObjectStorageService, TExtension>
    {
    }
}
