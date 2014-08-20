#if !NET35 && !PORTABLE

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[assembly: TypeForwardedTo(typeof(ISafeSerializationData))]

#else

namespace System.Runtime.Serialization
{
    /// <summary>
    /// For internal compatibility use only.
    /// </summary>
    internal interface ISafeSerializationData
    {
        /// <summary>
        /// For internal compatibility use only.
        /// </summary>
        void CompleteDeserialization(object deserialized);
    }
}

#endif
