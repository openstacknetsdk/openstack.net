namespace OpenStackNetTests.Unit.Simulator.IdentityService.V3
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
            return configuration;
        }
    }
}
