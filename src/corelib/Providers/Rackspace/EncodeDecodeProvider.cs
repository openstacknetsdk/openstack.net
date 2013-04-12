using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net.openstack.Core;

namespace net.openstack.Providers.Rackspace
{
    public class EncodeDecodeProvider : IEncodeDecodeProvider
    {
        public string HtmlEncode(string stringToEncode)
        {
            return HttpUtility.UrlEncode(stringToEncode);
        }

        public string HtmlDecode(string stringToDecode)
        {
            return HttpUtility.HtmlDecode(stringToDecode);
        }
    }
}
