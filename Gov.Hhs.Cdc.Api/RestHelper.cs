// Copyright [2015] [Centers for Disease Control and Prevention] 
// Licensed under the CDC Custom Open Source License 1 (the 'License'); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
// 
//   http://t.cdc.gov/O4O
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an 'AS IS' BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Cryptography;

namespace Gov.Hhs.Cdc.Api
{
    public sealed class RestHelper
    {
        /*
         * Percent-encode (URL Encode) according to RFC 3986
         * 
         * This is necessary because .NET's HttpUtility.UrlEncode does not encode
         * according to the above standard. Also, .NET returns lower-case encoding
         * by default (upper-case encoding is required).
         */
        private static string PercentEncodeRfc3986(string str)
        {
            str = HttpUtility.UrlEncode(str, System.Text.Encoding.UTF8);
            str = str.Replace("'", "%27").Replace("(", "%28").Replace(")", "%29").Replace("*", "%2A").Replace("!", "%21").Replace("%7e", "~").Replace("+", "%20");

            StringBuilder sbuilder = new StringBuilder(str);
            for (int i = 0; i < sbuilder.Length; i++)
            {
                if (sbuilder[i] == '%')
                {
                    if (char.IsLetter(sbuilder[i + 1]) || char.IsLetter(sbuilder[i + 2]))
                    {
                        sbuilder[i + 1] = char.ToUpper(sbuilder[i + 1]);
                        sbuilder[i + 2] = char.ToUpper(sbuilder[i + 2]);
                    }
                }
            }
            return sbuilder.ToString();
        }

        /*
         * Convert a query string to corresponding dictionary of name-value pairs.
         */
        public static IDictionary<string, string> CreateDictionary(string queryString)
        {
            //Convert the KeyedByTypeCollection to lower case here
            //Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> map = queryString.Split('&')
                .Select(p => Decode(ToKeyValuePair(p)))
                .Where(p => !string.IsNullOrEmpty(p.Key))
                .ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);

            return map;
        }

        /*
         * Convert a query string to corresponding dictionary of name-value pairs.
         */
        public static IDictionary<string, string> CreateUnfilteredDictionary(string queryString)
        {
            //Convert the KeyedByTypeCollection to lower case here
            //Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> map = queryString.Split('&')
                .Select(p => DecodeUnfiltered(ToKeyValuePair(p)))
                .Where(p => !string.IsNullOrEmpty(p.Key))
                .ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);

            return map;
        }

        /// <summary>
        /// Decodes and strips a key value pair
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> Decode(KeyValuePair<string, string> parameter)
        {
            string key = parameter.Key ?? "";
            key = Util.Strip(HttpUtility.UrlDecode(key, Encoding.UTF8)).ToLower();
            bool allowSlash = (key == "urlcontains" || key == "sourceurlcontains" || key == "xpath");
            string value = allowSlash ? HttpUtility.UrlDecode(parameter.Value, Encoding.UTF8) : HttpUtility.UrlDecode(parameter.Value, Encoding.UTF8);
            //bool allowWeirdChars = (key == "xpath" || key == "title" || key == "q" || key == "urlcontains" || key == "topic");
            //value = allowWeirdChars ? Util.StripForXpath(value) : Util.Strip(value);

            return new KeyValuePair<string, string>(key, value);
        }

        /// <summary>
        /// Decodes a key value pair
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> DecodeUnfiltered(KeyValuePair<string, string> parameter)
        {
            string key = parameter.Key ?? "";
            key = Util.Strip(HttpUtility.UrlDecode(key, Encoding.UTF8)).ToLower();

            string value = HttpUtility.UrlDecode(parameter.Value, Encoding.UTF8).Trim('/');

            return new KeyValuePair<string, string>(key, value);
        }

        /// <summary>
        /// Returns a key value pair of a=b, with guaranteed empty strings for the key and value if
        /// either is null
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static KeyValuePair<string, string> ToKeyValuePair(string parameter)
        {
            char[] keywordSeperator = { '=' };
            var keyAndValue = parameter.Split(keywordSeperator, 2);
            return new KeyValuePair<string, string>(
                parameter.Length < 1 ? "" : keyAndValue[0],
                parameter.Length < 2 ? "" : keyAndValue[1]
                );
        }


        /*
         * Construct the canonical query string from the sorted parameter map.
         */
        internal static string ConstructCanonicalQueryStringFromDictionary(IDictionary<string, string> dictionary)
        {
            // Use a SortedDictionary to get the parameters in naturual byte order
            ParamComparer pc = new ParamComparer();
            SortedDictionary<string, string> sortedParamMap = new SortedDictionary<string, string>(dictionary, pc);

            return ConstructQueryStringFromDictionary(sortedParamMap);
        }

        internal static string ConstructQueryStringFromDictionary(IDictionary<string, string> dict)
        {
            StringBuilder builder = new StringBuilder();

            if (dict.Count == 0)
            {
                builder.Append(" ");
            }
            else
            {
                foreach (KeyValuePair<string, string> kvp in dict)
                {
                    builder.Append(PercentEncodeRfc3986(kvp.Key));
                    builder.Append("=");
                    builder.Append(PercentEncodeRfc3986(kvp.Value));
                    builder.Append("&");
                }
            }
            
            string queryString = builder.ToString().ToLower();

            return queryString.Substring(0, queryString.Length - 1);
        }

    }
}
