namespace OpenStack.Services.Identity
{
    /// <summary>
    /// This class serves as the base class for all service extension definitions for the
    /// <see cref="IBaseIdentityService"/>.
    /// </summary>
    /// <typeparam name="TExtension">The service extension type.</typeparam>
    /// <seealso cref="IBaseIdentityService.GetServiceExtension"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public abstract class BaseIdentityServiceExtensionDefinition<TExtension> : ServiceExtensionDefinition<IBaseIdentityService, TExtension>
    {
    }
}
