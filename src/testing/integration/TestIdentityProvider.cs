using System;
using net.openstack.Core.Domain;
using net.openstack.Core.Providers;

namespace OpenStack
{
    public class TestIdentityProvider
    {
        private static readonly string EnvironmentVariablesNotFoundErrorMessage = 
            "No identity environemnt variables found. Make sure the following environment variables exist: " + Environment.NewLine +
                            "OPENSTACKNET_IDENTITY_URL" + Environment.NewLine +
                            "OPENSTACKNET_USER" + Environment.NewLine +
                            "OPENSTACKNET_PASSWORD" + Environment.NewLine +
                            "OPENSTACKNET_PROJECT";

        public static IIdentityProvider GetIdentityProvider()
        {
            var identity = GetIdentityFromEnvironment();
            var identityEndpoint = GetIdentityEndpointFromEnvironment();
            return new OpenStackIdentityProvider(identityEndpoint, identity)
            {
                ApplicationUserAgent = "CI-BOT"
            };
        }

        public static Uri GetIdentityEndpointFromEnvironment()
        {
            var identityUrl = Environment.GetEnvironmentVariable("OPENSTACKNET_IDENTITY_URL");
            if(!string.IsNullOrEmpty(identityUrl))
                return new Uri(identityUrl);

            identityUrl = Environment.GetEnvironmentVariable("BAMBOO_OPENSTACKNET_IDENTITY_URL");
            if (!string.IsNullOrEmpty(identityUrl))
                return new Uri(identityUrl);

            throw new Exception(EnvironmentVariablesNotFoundErrorMessage);
        }

        public static CloudIdentity GetIdentityFromEnvironment()
        {
            var user = Environment.GetEnvironmentVariable("OPENSTACKNET_USER");
            if (!string.IsNullOrEmpty(user))
            {
                var password = Environment.GetEnvironmentVariable("OPENSTACKNET_PASSWORD");
                var projectName = Environment.GetEnvironmentVariable("OPENSTACKNET_PROJECT");

                if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(projectName))
                {
                    return new CloudIdentityWithProject
                    {
                        Username = user,
                        Password = password,
                        ProjectName = projectName
                    };
                }
            }

            user = Environment.GetEnvironmentVariable("BAMBOO_OPENSTACKNET_USER");
            if (!string.IsNullOrEmpty(user))
            {
                var password = Environment.GetEnvironmentVariable("BAMBOO_OPENSTACKNET_PASSWORD");
                var projectName = Environment.GetEnvironmentVariable("BAMBOO_OPENSTACKNET_PROJECT");

                if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(projectName))
                {
                    return new CloudIdentityWithProject
                    {
                        Username = user,
                        Password = password,
                        ProjectName = projectName
                    };
                }
            }

            throw new Exception(EnvironmentVariablesNotFoundErrorMessage);
        }
    }
}