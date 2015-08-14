using System.Diagnostics;
using System.Reflection;

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
    }

}