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

namespace Gov.Hhs.Cdc.DataProvider
{
    public class DataUtil
    {

        /*
         * Percent-encode (URL Encode) according to RFC 3986
         * 
         * This is necessary because .NET's HttpUtility.UrlEncode does not encode
         * according to the above standard. Also, .NET returns lower-case encoding
         * by default (upper-case encoding is required).
         */
        public static string PercentEncodeRfc3986(string str)
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

        public static string PercentDecodeRfc3986(string str)
        {
            str = HttpUtility.UrlDecode(str, System.Text.Encoding.UTF8);
            str = str.Replace("%27", "'").Replace("%28", "(").Replace("%29", ")").Replace("%2A", "*").Replace("%21", "!").Replace("~", "%7e").Replace("%20", "+");

            return str;
        }
    }
}
