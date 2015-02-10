#if PORTABLE

namespace System
{
    /// <summary>
    /// For internal compatibility use only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    internal sealed class NonSerializedAttribute : Attribute
    {
    }
}

#endif
