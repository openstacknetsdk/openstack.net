namespace OpenStackNetTests.Unit.Simulator.IdentityService
{
    using System.Web.Http;
    using OpenStackNetTests.Unit.Simulator.IdentityService.V2;
    using Owin;

    public class BaseIdentityServiceConfiguration
    {
        protected virtual string ControllerName
        {
            get
            {
                return "baseIdentity";
            }
        }

        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration configuration = Configure();
            IdentityV2AuthenticationOptions options = new IdentityV2AuthenticationOptions();
            appBuilder.Use(typeof(IdentityV2AuthenticationMiddleware), appBuilder, options);
            appBuilder.UseWebApi(configuration);
        }

        protected virtual HttpConfiguration Configure()
        {
            HttpConfiguration configuration = new HttpConfiguration();
            configuration.Filters.Add(new AuthorizeAttribute());
            configuration.Routes.MapHttpRoute(name: "ListVersions", routeTemplate: string.Empty, defaults: new
            {
                controller = ControllerName,
                action = "ListVersions"
            });
            configuration.Routes.MapHttpRoute(name: "GetVersion", routeTemplate: "{versionId}", defaults: new
            {
                controller = ControllerName,
                action = "GetVersion"
            });

            return configuration;
        }
    }
}
