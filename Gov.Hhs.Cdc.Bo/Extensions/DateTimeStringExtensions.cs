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
using System.Globalization;
using System.Linq;
using System.Text;

namespace Gov.Hhs.Cdc.Bo
{
    public static class DateTimeStringExtensions
    {
        public static string[] DateTimeFormats = new string[]
        {
            //The first format is the one we will output for date format
            "yyyy-MM-dd'T'HH:mm:ss'Z'",
            "yyyy-MM-dd'T'HH:mm:ss",
            "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.fff",
            "yyyy-MM-dd'T'HH:mm:ss.fff zzz",
            "yyyy-MM-dd'T'HH:mm:ss.fffzzz",
            "yyyy-MM-dd'T'HH:mm:ss'Z'",            
            "yyyy-MM-dd'T'HH:mm:ss zzz",
            "yyyy-MM-dd'T'HH:mm:sszzz",
            "yyyy-MM-dd'T'HH:mm'Z'",
            "yyyy-MM-dd'T'HH:mm",
            "yyyy-MM-dd'T'HH:mm zzz",
            "yyyy-MM-dd'T'HH:mmzzz"
        };

        public static string FormatAsUtcString(this DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return "";
            }
            else
            {
                return ((DateTime)dateTime).FormatAsUtcString();
            }
        }

        public static string FormatAsUtcString(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime, new TimeSpan(0)).ToString(DateTimeFormats[0]);
        }

        public static DateTime? ParseUtcDateTime(this string strDate)
        {
            if (string.IsNullOrEmpty(strDate))
            {
                return null;
            }

            DateTime? result = DateTimeFormats.Select(f => strDate.ParseUtcDateTime(f)).Where(d => d != null).FirstOrDefault();
            return result;
        }


        public static DateTime? ParseUtcDateTime(this string strDate, string format)
        {
            DateTime parsedDateTime;
            if (DateTime.TryParseExact(strDate, format, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out parsedDateTime))
            {
                return parsedDateTime;
            }
            else
            {
                return null;
            }

        }

    }
}
