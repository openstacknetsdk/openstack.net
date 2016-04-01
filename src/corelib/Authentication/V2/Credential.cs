using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.Authentication.V3
{
    /// <summary>
    /// 
    /// </summary>
    public class Credential
    {
        /// <summary>
        /// 
        /// </summary>
        public ProjectScope ProjectScope { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        public User User { get;  set;  }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="scope"></param>
        public Credential(User user, ProjectScope scope)
        {
            this.User = user;
            this.ProjectScope = scope;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        public Credential(string userid, string password)
        {
            this.User = new User(userid, password);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <param name="projectId"></param>
        public Credential(string userid, string password, string projectId)
        {
            this.User = new User(userid, password);
            this.ProjectScope = new ProjectScope(projectId);
        }
 
    }
}
