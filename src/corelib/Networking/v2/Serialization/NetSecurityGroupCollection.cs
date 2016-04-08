using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Serialization;
using OpenStack.Networking.v2.Layer3;

namespace OpenStack.Networking.v2.Serialization
{
    /// <summary>
    /// Represents a collection of security groups resources returned by the <see cref="NetworkingService"/>.
    /// <para>Intended for custom implementations and stubbing responses in unit tests.</para>
    /// </summary>
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "security_groups")]
     public class NetSecurityGroupCollection : List<SecurityGroup>
    {

        /// <summary>
        ///Initializes a new instance of the<see cref="SecurityGroup"/> class.
        /// </summary>
        public NetSecurityGroupCollection()
        {

        }

        /// <summary>
        ///Initializes a new instance of the<see cref="SecurityGroup"/> class.
        /// </summary>
        /// <param name="securityGroups"></param>
        public NetSecurityGroupCollection(IEnumerable<SecurityGroup> securityGroups) : base(securityGroups)
        {

        }
        
    }
}
