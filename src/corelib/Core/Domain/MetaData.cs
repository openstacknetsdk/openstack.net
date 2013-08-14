using System;
using System.Collections.Generic;

namespace net.openstack.Core.Domain
{
    /// <summary>
    /// Represents metadata for servers and images in the Compute Service.
    /// </summary>
    /// <remarks>
    /// The metadata keys for the compute provider are case-sensitive.
    /// </remarks>
    [Serializable]
    public class Metadata : Dictionary<string, string>
    {
    }
}
