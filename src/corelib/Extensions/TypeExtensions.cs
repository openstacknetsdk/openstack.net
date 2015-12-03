using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

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
            foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(src))
            {
                item.SetValue(dest, item.GetValue(src));
            }
        }
    }

}