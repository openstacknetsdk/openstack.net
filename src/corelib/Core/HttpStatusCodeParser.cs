using System.Text.RegularExpressions;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public class HttpStatusCodeParser : IStatusParser
    {
        private const string RegEx = @"(?<StatusCode>\d*)\s*(?<Status>\w*)";
        public Status Parse(string value)
        {
            var regex = new Regex(RegEx);

            var match = regex.Match(value);
            if (!match.Success)
                return null;

            return new Status{Code = int.Parse(match.Groups["StatusCode"].Value), Description = match.Groups["Status"].Value};
        }
    }
}
