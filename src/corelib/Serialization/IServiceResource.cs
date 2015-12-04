using System;
using System.Runtime.CompilerServices;

namespace OpenStack.Serialization
{
    /// <summary>
    /// Resources which want to expose service operations directly (e.g. resource.Delete())
    /// should implement this interface and the service will use it to add a reference to itself.
    /// </summary>
    public interface IServiceResource
    {
        /// <summary>
        /// The service which originally constructed the resource. This instance will be used for further operations on the resource.
        /// </summary>
        object Owner { get; set; }
    }

    /// <summary />
    public static class ServiceResourceExtensions
    {
        /// <summary />
        public static T TryGetOwner<T>(this IServiceResource resource)
            where T : class
        {
            return resource.Owner as T;
        }

        /// <summary />
        public static T GetOwnerOrThrow<T>(this IServiceResource resource, [CallerMemberName]string callerName = "")
            where T : class
        {
            var owner = resource.TryGetOwner<T>();
            if (owner != null)
                return owner;

            var ownerName = typeof (T).Name;
            throw new InvalidOperationException(string.Format($"{callerName} can only be used on instances which were constructed by {ownerName}. Use {ownerName}.{callerName} instead."));
        }
    }
}