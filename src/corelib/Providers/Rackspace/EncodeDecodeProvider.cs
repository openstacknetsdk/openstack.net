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
        public string UrlEncode(string stringToEncode)
        {
            return HttpUtility.UrlEncode(stringToEncode).Replace("+","%20");
        }

        public string UrlDecode(string stringToDecode)
        {
            return HttpUtility.UrlDecode(stringToDecode);
        }
    }
}
