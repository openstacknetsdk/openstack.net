namespace OpenStackNet.Testing.Unit
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Core;
    using System;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    /// <preliminary/>
    [TestClass]
    public class AsyncWebRequestTests
    {
        [TestMethod]
        public void TestAsyncWebRequest()
        {
            Uri uri = new Uri("http://google.com");
            WebRequest request = HttpWebRequest.Create(uri);
            Task<WebResponse> response = request.GetResponseAsync();
            response.Wait();
        }

        [TestMethod]
        public void TestAsyncWebRequestTimeout()
        {
            Uri uri = new Uri("http://google.com");
            WebRequest request = HttpWebRequest.Create(uri);
            request.Timeout = 0;
            Task<WebResponse> response = request.GetResponseAsync();
            try
            {
                response.Wait();
                Assert.Fail("Expected an exception");
            }
            catch (AggregateException exception)
            {
                Assert.AreEqual(TaskStatus.Faulted, response.Status);

                ReadOnlyCollection<Exception> exceptions = exception.InnerExceptions;
                Assert.AreEqual(1, exceptions.Count);
                Assert.IsInstanceOfType(exceptions[0], typeof(WebException));

                WebException webException = (WebException)exceptions[0];
                Assert.AreEqual(WebExceptionStatus.Timeout, webException.Status);
            }
        }

        [TestMethod]
        public void TestAsyncWebRequestCancellation()
        {
            Uri uri = new Uri("http://google.com");
            WebRequest request = HttpWebRequest.Create(uri);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Task<WebResponse> response = request.GetResponseAsync(cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();
            try
            {
                response.Wait();
                Assert.Fail("Expected an exception");
            }
            catch (AggregateException exception)
            {
                Assert.AreEqual(TaskStatus.Canceled, response.Status);

                ReadOnlyCollection<Exception> exceptions = exception.InnerExceptions;
                Assert.AreEqual(1, exceptions.Count);
                Assert.IsInstanceOfType(exceptions[0], typeof(OperationCanceledException));
            }
        }

        [TestMethod]
        public void TestAsyncWebRequestError()
        {
            Uri uri = new Uri("http://google.com/fail");
            WebRequest request = HttpWebRequest.Create(uri);
            Task<WebResponse> response = request.GetResponseAsync();
            try
            {
                response.Wait();
                Assert.Fail("Expected an exception");
            }
            catch (AggregateException exception)
            {
                Assert.AreEqual(TaskStatus.Faulted, response.Status);

                ReadOnlyCollection<Exception> exceptions = exception.InnerExceptions;
                Assert.AreEqual(1, exceptions.Count);
                Assert.IsInstanceOfType(exceptions[0], typeof(WebException));

                WebException webException = (WebException)exceptions[0];
                Assert.AreEqual(HttpStatusCode.NotFound, ((HttpWebResponse)webException.Response).StatusCode);
            }
        }
    }
}
