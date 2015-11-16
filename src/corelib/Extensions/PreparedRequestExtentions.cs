using Flurl.Extensions;
using Flurl.Http;
using OpenStack.Compute.v2_1;
using OpenStack.Serialization;

namespace OpenStack.Extensions
{
    internal static class PreparedRequestExtentions
    {
        public static PreparedRequest SetMicroversion(this PreparedRequest request, ISupportMicroversions api)
        {
            if (!string.IsNullOrEmpty(api.Microversion))
                request.WithHeader(api.MicroversionHeader, api.Microversion);

            return request;
        }
    }
}