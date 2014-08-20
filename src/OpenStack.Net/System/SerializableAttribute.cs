#if !PORTABLE

using System;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(SerializableAttribute))]

#else

namespace System
{
    /// <summary>
    /// For internal compatibility use only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    internal sealed class SerializableAttribute : Attribute
    {
    }
}

#endif
