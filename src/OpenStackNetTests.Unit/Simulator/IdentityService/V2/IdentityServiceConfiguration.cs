namespace OpenStackNetTests.Unit.Simulator.IdentityService.V2
{
    using System.Web.Http;

    public class IdentityServiceConfiguration : BaseIdentityServiceConfiguration
    {
        protected override string ControllerName
        {
            get
            {
                return "identity";
            }
        }

        protected override HttpConfiguration Configure()
        {
            HttpConfiguration configuration = base.Configure();
            configuration.Routes.MapHttpRoute(name: "Authenticate", routeTemplate: "v2.0/tokens", defaults: new
            {
                controller = ControllerName,
                action = "Authenticate"
            });
            configuration.Routes.MapHttpRoute(name: "ListExtensions", routeTemplate: "v2.0/extensions", defaults: new
            {
                controller = ControllerName,
                action = "ListExtensions"
            });
            configuration.Routes.MapHttpRoute(name: "GetExtension", routeTemplate: "v2.0/extensions/{alias}", defaults: new
            {
                controller = ControllerName,
                action = "GetExtension"
            });
            configuration.Routes.MapHttpRoute(name: "ListTenants", routeTemplate: "v2.0/tenants", defaults: new
            {
                controller = ControllerName,
                action = "ListTenants"
            });

            return configuration;
        }
    }
}
