using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class BulkDeleteResponse
    {
        [DataMember(Name = "Number Not Found")]
        public int NumberNotFound { get; set; }

        [DataMember(Name = "Response Status")]
        public string Status { get; set; }

        [DataMember(Name = "Errors")]
        public IEnumerable<IEnumerable<string>> Errors { get; set; }

        [DataMember(Name = "Number Deleted")]
        public int NumberDeleted { get; set; }

        [DataMember(Name = "Response Body")]
        public string ResponseBody { get; set; }

        public IEnumerable<string> AllItems { get; set; }  

        public bool IsItemError(string s)
        {
            return Errors.Any(e => e.Any(e2 => e2.Equals(s)));
        }
    }
}
