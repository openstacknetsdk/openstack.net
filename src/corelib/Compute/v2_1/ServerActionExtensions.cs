using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;


// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary />
    public static class ServerActionExtensions_v2_1
    {
        /// <inheritdoc cref="ServerActionSummary.GetActionAsync"/>
        public static ServerAction GetAction(this ServerActionSummary action)
        {
            return action.GetActionAsync().ForceSynchronous();
        }
    }
}
