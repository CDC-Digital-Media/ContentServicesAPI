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

namespace Gov.Hhs.Cdc.CdcMediaValidationProvider
{
    public static class Encoder
    {
        public static string Encode(string html, string outputFormat, string outputCharacterEncoding)
        {
            if (outputFormat == "xml")
                return EncodeAsHtml(EncodeCharacters("<?xml version=\"1.0\"?>" + html, outputCharacterEncoding));
            if (string.IsNullOrEmpty(html))
                return "";

            //return EncodeAsHtml(EncodeCharacters(html, outputCharacterEncoding));
            return EncodeCharacters(html, outputCharacterEncoding);
        }

        private static string EncodeCharacters(string data, string outputCharacterEncoding)
        {
            // now we need to see if we need to convert the output
            // encoding to somthing other than UTF-8
            Encoding encoding = Encoding.GetEncoding(outputCharacterEncoding);
            if (encoding == null || encoding == Encoding.UTF8)
                return data;

            // convert from whatever we got to UTF8 because that is what WE serve
            byte[] cbuffer = Encoding.UTF8.GetBytes(data);
            byte[] ebuffer = Encoding.Convert(Encoding.UTF8, encoding, cbuffer, 0, cbuffer.Length);
            return encoding.GetString(ebuffer);
            //// and handle any metrics stuff
            //if (!request.SuppressMetrics)
            //{
            //    strOut = ApplyMetrics(request, strOut);
            //}

            //// The the request was for javascript then wrap each line in
            //// a document.write() call.
            //if (request.GenerateJavascript)
            //{
            //    if (request.OutputJavascriptFunction)
            //    {
            //        strOut = "function GetContentBlock() {return '" + strOut.Replace("'", "\\'").Replace(Environment.NewLine, " ") + "';}";
            //    }
            //    else
            //    {
            //        String newResult = "";
            //        String line = null;
            //        System.IO.StringReader sr = new StringReader(strOut);
            //        while ((line = sr.ReadLine()) != null)
            //        {
            //            line = line.Replace("'", "\\'");
            //            newResult += "document.write('" + line + " ');\n";
            //        }
            //        strOut = newResult;
            //    }
            //}

        }

        public static string EncodeAsHtml(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                string decodedData = HttpUtility.HtmlDecode(data);
                return HttpUtility.HtmlEncode(decodedData);
            }
            else
                return data;
        }
    }
}
