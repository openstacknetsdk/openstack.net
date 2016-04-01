using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenStack.Authentication.V3
{
    /// <summary>
    /// represent authorization scope.
    /// </summary>
    public class ProjectScope 
    {

        /// <summary>
        ///
        /// </summary>
        [JsonProperty("id")]
        public string Id { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        public ProjectScope(string projectId)
        {
           this.Id = projectId;
        }

    }
}
