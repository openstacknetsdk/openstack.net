using System.Collections.Generic;

namespace OpenStack.Serialization
{
    /// <summary />
    public interface IQueryStringBuilder
    {
        /// <summary />
        IDictionary<string, object> Build();
    }
}