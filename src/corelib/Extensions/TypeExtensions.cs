using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using net.openstack.Providers.Rackspace.Objects.Response;

namespace System.Extensions
{
    /// <summary>
    /// Useful System.Type extension methods for custom implementations.
    /// </summary>
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
    }

}