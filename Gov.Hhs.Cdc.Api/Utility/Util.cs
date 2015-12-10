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

using System.Text.RegularExpressions;
using System.Web;

namespace Gov.Hhs.Cdc.Api
{
    public sealed class Util
    {
        /// <summary>
        /// Function to strip out disallowed characters from parameters
        /// and ensure a maximum length is not exceeded.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        public static string StripTrim(string param, int maxLen)
        {
            if (param != null)
            {
                // The following regular expression is used to remove the disallowed
                // characters <>"%;)(&+ from URL/form parameter values.
                Regex re = new Regex("\\<|\\>|\\\"|\\%|\\;|\\)|\\(|\\+|\\`|\\'|\\~|\\+");
                param = re.Replace(param, "");
                param = param.Trim();
                if (param.Length > maxLen)
                    param = param.Substring(0, maxLen);
            }
            return param;
        }
                
        /// <summary>
        /// Function to strip out disallowed characters from parameters.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Strip(string param)
        {
            if (param != null)
            {
                // The following regular expression is used to remove the disallowed
                // characters <>"%;)(&+ from URL/form parameter values.
                //Regex re = new Regex("\\<|\\>|\\\"|\\%|\\;|\\)|\\(|\\+|\\`|\\'|\\~|\\+");
                //Do not remove quotes
                Regex re = new Regex("\\<|\\>|\\%|\\;|\\)|\\(|\\+|\\`|\\'|\\~|\\+");
                param = re.Replace(param, "");
                param = param.Trim();
            }
            return param;
        }

        public static string StripForXpath(string param)
        {
            if (param != null)
            {
                // The following regular expression is used to remove the disallowed
                // characters <>"%;)(&+ from URL/form parameter values.
                Regex re = new Regex("\\<|\\>|\\\"|\\%|\\;|\\)|\\(|\\+|\\`|\\~|\\+");
                param = re.Replace(param, "");
                param = param.Trim();
            }
            return param;
        }


        public static string HtmlEncodeOutput(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = HttpUtility.HtmlDecode(str);
                str = HttpUtility.HtmlEncode(str);
            }

            return str;
        }

        public static string HtmlDecodeOutput(string str)
        {
            if (!string.IsNullOrEmpty(str))
                str = HttpUtility.HtmlDecode(str);
            
            return str;
        }

    }
}
