using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.Core
{
    public interface IEncodeDecodeProvider
    {
        string UrlEncode(string stringToEncode);
        string UrlDecode(string stringToDecode);
    }
}
