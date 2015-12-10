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
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Gov.Hhs.Cdc.CdcMediaValidationProvider
{
    public class ResponseStream : IDisposable
    {
        public MemoryStream theStream;
        private HttpWebResponse response;
        bool streamHasBeenLoaded = false;

        public ResponseStream(HttpWebResponse response)
        {
            this.response = response;
        }

        private void LoadStream()
        {
            theStream = new MemoryStream();
            byte[] buffer = new byte[4092];
            int readBytes;
            Stream ContentStream = response.GetResponseStream();
            while ((readBytes = ContentStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                theStream.Write(buffer, 0, readBytes);
            }
            response.Close();
            streamHasBeenLoaded = true;
        }

        public string Read(Encoding requestedEncoding)
        {
            if (!streamHasBeenLoaded)
                LoadStream();
            
            //Assume the encoding is what we want
            theStream.Position = 0;
            StreamReader reader = new StreamReader(theStream, requestedEncoding);
            string data = reader.ReadToEnd();

            System.Text.Encoding actualEncoding = GetCharacterEncodingFromData(response.CharacterSet, data);
            if (actualEncoding != requestedEncoding)
            {
                theStream.Position = 0;
                reader = new StreamReader(theStream, actualEncoding);
                data = ConvertStringEncoding(reader.ReadToEnd(), actualEncoding, requestedEncoding);
            }
            return data;
        }

        private static Encoding GetCharacterEncodingFromData(string charSet, string content)
        {
            if (charSet == "ISO-8859-1")
            {
                return Encoding.GetEncoding(GetCharacterSetFromContent(content));
            }
            else
            {
                return Encoding.GetEncoding(charSet);
            }
        }

        private static string GetCharacterSetFromContent(string content)
        {
            Regex characterSet = new Regex("meta(.*?)charset=(\"|)(.*?)(?<set>(.*?))\"(.*?)>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match match = characterSet.Match(content.ToLower());
            if (match.Success && !string.IsNullOrEmpty(match.Groups["set"].Value))
            {
                return match.Groups["set"].Value;
            }
            else
            {
                return "ISO-8859-1";
            }
        }

        private static string ConvertStringEncoding(string data, System.Text.Encoding fromEncoding, System.Text.Encoding toEncoding)
        {
            // convert from whatever we got to UTF8 because that is what WE serve
            byte[] cbuffer = fromEncoding.GetBytes(data);
            byte[] ebuffer = Encoding.Convert(fromEncoding, toEncoding, cbuffer, 0, cbuffer.Length);
            return toEncoding.GetString(ebuffer);
        }

        public void Dispose()
        {
            theStream.Dispose();
        }
    }
}
