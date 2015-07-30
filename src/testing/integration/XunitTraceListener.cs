using System.Diagnostics;
using Xunit.Abstractions;

namespace OpenStack
{
    public class XunitTraceListener : TraceListener
    {
        private readonly ITestOutputHelper _testLog;

        public XunitTraceListener(ITestOutputHelper testLog)
        {
            _testLog = testLog;
        }

        public override void Write(string message)
        {
            if (message.StartsWith(OpenStackNet.Tracing.Http.Name))
                return;

            _testLog.WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            _testLog.WriteLine(message);
        }
    }
}
