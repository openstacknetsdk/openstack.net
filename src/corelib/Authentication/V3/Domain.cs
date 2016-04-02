using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Serialization;
using Newtonsoft.Json;
namespace OpenStack.Authentication.V3
{

    /// <summary>
    /// 
    /// </summary>
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "port")]
    public class Domain
    {
    }
}
