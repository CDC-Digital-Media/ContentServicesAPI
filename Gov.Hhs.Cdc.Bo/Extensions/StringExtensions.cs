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
using System.Text.RegularExpressions;

namespace Gov.Hhs.Cdc.Bo
{
    public static class StringExtension
    {
        public static byte[] ToBytes(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return Convert.FromBase64String(str);
            }
                return null;
        }

        public static string GetLast(this string source, int length)
        {
            if (length >= source.Length)
            {
                return source;
            }
            return source.Substring(source.Length - length);
        }

        public static string GetAllButLast(this string source, int length)
        {
            if (length <= 0)
            {
                return source;
            }
            if (length >= source.Length)
            {
                return "";
            }
            return source.Substring(0, source.Length - length);
        }

        public static string urlRegExPattern = @"^(|http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?$";
        public static string SafeUrlContainsSearch(this string url)
        {
            return url;
            //string str = string.Empty;
            //Regex urlPattern = new Regex(urlRegExPattern);

            //if (!string.IsNullOrEmpty(url))
            //    str = url.Trim().ToLower();

            //str = urlPattern.Replace(str, string.Empty);
            //return str;
        }

        public static bool IsUrl(this string url)
        {
            var pattern = new Regex(urlRegExPattern);
            return pattern.IsMatch(url);
        }

        public static int? ToNullableInt32(this string s)
        {
            int i;
            return Int32.TryParse(s, out i) ? (int?)i : null;
        }

        /// <summary>
        /// Remove the endString from the end of source, if it exists
        /// </summary>
        /// <param name="source"></param>
        /// <param name="endString"></param>
        /// <returns></returns>
        public static string TrimEnd(this string source, string endString)
        {
            return source.EndsWith(endString) ? source.Substring(0, source.Length - endString.Length) : source;
        }

        public static Criterion Is(this string key, string value)
        {
            return new Criterion(key, value);
        }

        public static Criterion Is(this string key, DateTime value)
        {
            return new Criterion(key, value);
        }

        public static Criterion Is(this string key, int value)
        {
            return new Criterion(key, value);
        }

        public static Criterion Is(this string key, params int[] value)
        {
            return new Criterion(key, value.ToList());
        }

        public static Criterion Is(this string key, List<int> value)
        {
            return new Criterion(key, value);
        }

        public static Criterion Is(this string key, List<long> value)
        {
            return new Criterion(key, value);
        }

        public static Criterion Is(this string key, List<string> value)
        {
            return new Criterion(key, value);
        }

        public static Criterion Is(this string key, params string[] values)
        {
            return new Criterion(key, values.ToList());
        }

        public static Criterion Is(this string key, bool value)
        {
            return new Criterion(key, value);
        }

        public static SortColumn Direction(this string key, SortOrderType orderType)
        {
            return new SortColumn(key, orderType);
        }

        public static string ToXmlSearchString(this List<SortColumn> key)
        {
            string forXml = string.Join(",", key.Select(a => a.Column + "|" + a.SortOrder.ToString()).ToList());
            return forXml;
        }

        public static IEnumerable<string> Split(this string str, Func<char, bool> controller)
        {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }

        public static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) &&
                (input[0] == quote) && (input[input.Length - 1] == quote))
            {
                return input.Substring(1, input.Length - 2);
            }

            return input;
        }

        public static bool ContainsIgnoreCase(this string source, string searchString)
        {
            if (source == null)
            {
                return false;
            }
            return source.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool ContainsAnyIgnoreCase(this string source, IEnumerable<string> searchStrings)
        {
            if (source == null)
            {
                return false;
            }
            return searchStrings.Where(s => source.ContainsIgnoreCase(s)).Any();
        }

    }
}
