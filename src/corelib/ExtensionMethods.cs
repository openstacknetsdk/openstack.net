using System.Collections.Generic;

namespace net.openstack
{
    internal static class ExtensionMethods
    {
        internal static string Format(this string input, Dictionary<string, string> replaceValues)
        {
            var output = input;
            foreach (var replaceValue in replaceValues)
            {
                var key = replaceValue.Key;
                if (!key.StartsWith("{"))
                    key = "{" + key;
                if (!key.EndsWith("}"))
                    key = key + "}";

                output = output.Replace(key, replaceValue.Value);
            }

            return output;
        }
    }
}
