namespace OpenStackNetTests.Unit.Simulator.ObjectStorageService.V1
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Metadata;
    using System.Web.Http.Metadata.Providers;
    using OpenStackNetTests.Unit.Simulator.IdentityService.V2;
    using Owin;

    public class ObjectStorageServiceConfiguration
    {
        protected virtual string ControllerName
        {
            get
            {
                return "objectStorage";
            }
        }

        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration configuration = new HttpConfiguration();
            IdentityV2AuthenticationOptions options = new IdentityV2AuthenticationOptions();
            appBuilder.Use(typeof(IdentityV2AuthenticationMiddleware), appBuilder, options);
            configuration.Filters.Add(new AuthorizeAttribute());
            configuration.Routes.MapHttpRoute(name: "Info", routeTemplate: "info", defaults: new
            {
                controller = ControllerName,
                action = "Info"
            });
            configuration.Routes.MapHttpRoute(name: "Account", routeTemplate: "v1/{tenant}", defaults: new
            {
                controller = ControllerName,
                action = "Account"
            });
            configuration.Routes.MapHttpRoute(name: "Container", routeTemplate: "v1/{tenant}/{container}", defaults: new
            {
                controller = ControllerName,
                action = "Container"
            });
            configuration.Routes.MapHttpRoute(name: "Object", routeTemplate: "v1/{tenant}/{container}/{*object}", defaults: new
            {
                controller = ControllerName,
                action = "Object"
            });

            configuration.Services.Replace(typeof(ModelMetadataProvider), new EmptyStringAllowedModelMetadataProvider());

            appBuilder.UseWebApi(configuration);
        }

        private class EmptyStringAllowedModelMetadataProvider : DataAnnotationsModelMetadataProvider
        {
            protected override CachedDataAnnotationsModelMetadata CreateMetadataFromPrototype(CachedDataAnnotationsModelMetadata prototype, Func<object> modelAccessor)
            {
                var metadata = base.CreateMetadataFromPrototype(prototype, modelAccessor);
                metadata.ConvertEmptyStringToNull = false;
                return metadata;
            }

            protected override CachedDataAnnotationsModelMetadata CreateMetadataPrototype(IEnumerable<Attribute> attributes, Type containerType, Type modelType, string propertyName)
            {
                var metadata = base.CreateMetadataPrototype(attributes, containerType, modelType, propertyName);
                metadata.ConvertEmptyStringToNull = false;
                return metadata;
            }
        }
    }
}
