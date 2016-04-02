using OpenStack.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.Authentication.V3.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "projects")]
    public class ProjectColletion : List<Project>
    {

    }
}
