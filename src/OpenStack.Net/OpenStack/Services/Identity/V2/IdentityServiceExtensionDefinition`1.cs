namespace OpenStack.Services.Identity.V2
{
    /// <summary>
    /// This class serves as the base class for all service extension definitions for the
    /// <see cref="IIdentityService"/>.
    /// </summary>
    /// <typeparam name="TExtension">The service extension type.</typeparam>
    /// <seealso cref="IIdentityService.GetServiceExtension"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public abstract class IdentityServiceExtensionDefinition<TExtension> : ServiceExtensionDefinition<IIdentityService, TExtension>
    {
    }
}
