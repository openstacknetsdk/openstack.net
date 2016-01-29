using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using OpenStack.Serialization;

namespace System.Extensions
{
    /// <summary>
    /// Useful System.Type extension methods for custom implementations.
    /// </summary>
    /// <exclude />
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the AssemblyFileVersion for the specified type.
        /// </summary>
        /// <param name="type">The type which resides in the desired assembly.</param>
        public static string GetAssemblyFileVersion(this Type type)
        {
            Assembly assembly = type.Assembly;
            try
            {
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fileVersionInfo.FileVersion;
            }
            catch
            {
                return assembly.GetName().Version.ToString();
            }
        }

        /// <summary />
        public static T Clone<T>(this T src)
            where T : new()
        {
            var dest = new T();
            src.CopyProperties(dest);
            return dest;
        }

        /// <summary />
        public static void CopyProperties<T>(this T src, T dest)
        {
            foreach (PropertyDescriptor srcProp in TypeDescriptor.GetProperties(src))
            {
                srcProp.SetValue(dest, srcProp.GetValue(src));
            }
        }

        /// <summary />
        public static void CopyProperties(this object src, object dest)
        {
            var destProps = TypeDescriptor.GetProperties(dest).Cast<PropertyDescriptor>();
            foreach (PropertyDescriptor srcProp in TypeDescriptor.GetProperties(src))
            {
                var destProp = destProps.FirstOrDefault(x => x.Name == srcProp.Name && x.PropertyType == srcProp.PropertyType);
                destProp?.SetValue(dest, srcProp.GetValue(src));
            }
        }

        /// <summary />
        public static void PropogateOwner(this IServiceResource resource, object owner)
        {
            if (resource == null)
                return;

            resource.Owner = owner;
            (resource as IEnumerable<IServiceResource>)?.PropogateOwner(owner);

            foreach (PropertyInfo prop in resource.GetType().GetProperties())
            {
                object propVal;
                try
                {
                    propVal = prop.GetValue(resource);
                }
                catch
                {
                    continue;
                }
                
                (propVal as IServiceResource)?.PropogateOwner(owner);
                (propVal as IEnumerable<IServiceResource>)?.PropogateOwner(owner);
            }
        }

        /// <summary />
        public static void PropogateOwner(this IEnumerable<IServiceResource> resources, object owner)
        {
            foreach (var resource in resources)
            {
                resource.PropogateOwner(owner);
            }
        }

        /// <summary />
        public static void SetParent(this IEnumerable<IChildResource> resources, string parentId)
        {
            foreach (var resource in resources)
            {
                resource.SetParent(parentId);
            }
        }
    }

}