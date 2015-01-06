namespace OpenStackNetTests.Unit
{
    using System;
    using Microsoft.Owin.Host.HttpListener;

    internal static class ExplicitOwinReference
    {
        // Make sure the unit test assembly keeps a reference to the Microsoft.Owin.Host.HttpListener assembly, or it
        // will not be deployed and all tests will fail.
        private static readonly Type DummyType = typeof(OwinHttpListener);
    }
}
