namespace OpenStackNetTests.Live
{
    using System;
    using System.Diagnostics;
    using OpenStack.Security.Authentication;
    using OpenStack.Services.Identity.V3;

    partial class IdentityV3Tests
    {
        protected Uri BaseAddress
        {
            get
            {
                TestCredentials credentials = Credentials;
                if (credentials == null)
                    return null;

                return credentials.BaseAddress;
            }
        }

        protected TestProxy Proxy
        {
            get
            {
                TestCredentials credentials = Credentials;
                if (credentials == null)
                    return null;

                return credentials.Proxy;
            }
        }

        protected string Vendor
        {
            get
            {
                TestCredentials credentials = Credentials;
                if (credentials == null)
                    return "OpenStack";

                return credentials.Vendor;
            }
        }

        protected TimeSpan TestTimeout(TimeSpan timeSpan)
        {
            if (Debugger.IsAttached)
                return TimeSpan.FromDays(6);

            return timeSpan;
        }

        internal static IIdentityService CreateService(TestCredentials credentials)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");

            IdentityClient client;
            switch (credentials.Vendor)
            {
            case "HP":
                // currently HP does not have a vendor-specific IIdentityService
                goto default;

            case "Rackspace":
                // currently Rackspace does not support this service
                goto default;

            case "OpenStack":
            default:
                client = new IdentityClient(credentials.BaseAddress);
                break;
            }

            TestProxy.ConfigureService(client, credentials.Proxy);
            client.BeforeAsyncWebRequest += TestHelpers.HandleBeforeAsyncWebRequest;
            client.AfterAsyncWebResponse += TestHelpers.HandleAfterAsyncWebResponse;

            return client;
        }

        internal static IAuthenticationService CreateAuthenticationService(TestCredentials credentials)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");

            IIdentityService identityService = CreateService(credentials);
            IAuthenticationService authenticationService;
            switch (credentials.Vendor)
            {
            case "HP":
                // currently HP does not have a vendor-specific IIdentityService
                goto default;

            case "Rackspace":
                // currently Rackspace does not support this service
                goto default;

            case "OpenStack":
            default:
                authenticationService = new IdentityV3AuthenticationService(identityService, credentials.AuthenticateRequestV3);
                break;
            }

            return authenticationService;
        }

        protected IIdentityService CreateService()
        {
            return CreateService(Credentials);
        }

        protected IIdentityService CreateService(IAuthenticationService authenticationService)
        {
            IdentityClient client;
            switch (Vendor)
            {
            case "HP":
                // currently HP does not have a vendor-specific IIdentityService
                goto default;

            case "Rackspace":
                // currently Rackspace does not support this service
                goto default;

            case "OpenStack":
            default:
                client = new IdentityClient(authenticationService, BaseAddress);
                break;
            }

            TestProxy.ConfigureService(client, Proxy);
            client.BeforeAsyncWebRequest += TestHelpers.HandleBeforeAsyncWebRequest;
            client.AfterAsyncWebResponse += TestHelpers.HandleAfterAsyncWebResponse;

            return client;
        }
    }
}
