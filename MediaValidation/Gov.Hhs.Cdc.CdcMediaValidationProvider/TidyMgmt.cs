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
using System.Text;
using System.Text.RegularExpressions;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaValidatonProvider;
using TidyNet;

namespace Gov.Hhs.Cdc.CdcMediaValidationProvider
{
    public class TidyResults
    {
        public ValidationMessages Messages {get; set;}
        public string Data { get; set; }
    }

    public static class TidyMgmt
    {
        public static TidyResults TidyContent(string source, string renderingType)
        {
            
            string data = source;
            bool isCharacterSetUtf8 = GetMetaContentAttribute(data).ToLower().Contains("utf-8");
            if( !isCharacterSetUtf8)
                data = ConvertSpecialCharactersToEscapedCharacters(data);

            try
            {
                byte[] tidyOutput;
                TidyMessageCollection tidyMessages = CallTidy(Encoding.UTF8.GetBytes(data), out tidyOutput);
                return new TidyResults()
                {
                    Data = tidyOutput.Length == 0 ? "" : 
                        UTF8Encoding.UTF8.GetString(tidyOutput, 0, tidyOutput.Length).TrimEnd("\0".ToCharArray()),
                    Messages = TransformMessages(tidyMessages)
                };

            }
            catch (Exception ex)
            {
                ValidationMessages messages = new ValidationMessages();
                messages.AddError("Tidy Failure", "Tidy Failure: " + ex.Message);
                return new TidyResults()
                {
                    Data = "",
                    Messages = messages
                };
            }
        }

        /// <summary>
        /// Call Tidy on HTML content and return a collection of messages and output data
        /// </summary>
        /// <param name="inputBuffer"></param>
        /// <param name="outputBuffer"></param>
        /// <returns></returns>
        private static TidyMessageCollection CallTidy(byte[] inputBuffer, out byte[] outputBuffer)
        {
            var tidyMessages = new TidyMessageCollection();
            using (var outputStream = new System.IO.MemoryStream())
            {
                using (var inputStream = new System.IO.MemoryStream())
                {
                    inputStream.Write(inputBuffer, 0, inputBuffer.Length);
                    inputStream.Seek(0, SeekOrigin.Begin);

                    Tidy tidy = CreateNewTidyManager();
                    tidy.Parse(inputStream, outputStream, tidyMessages);

                    outputBuffer = outputStream.GetBuffer();
                    inputStream.Close();
                }
                outputStream.Close();
            }
            return tidyMessages;
        }

        private static ValidationMessages TransformMessages(TidyMessageCollection tidyMessages)
        {
            var messages = new ValidationMessages();
            if (tidyMessages.Errors > 0 || tidyMessages.Warnings > 0)
            {
                foreach (TidyMessage theMessage in tidyMessages)
                {
                    string level = string.Empty;
                    string tidyMessage = Encoder.EncodeAsHtml(theMessage.Message);
                    switch (theMessage.Level)
                    {
                        case MessageLevel.Error:
                            messages.AddTidyError(tidyMessage, theMessage.Line, theMessage.Column);
                            break;
                        case MessageLevel.Warning:
                            messages.AddTidyWarning(tidyMessage, theMessage.Line, theMessage.Column);
                            break;
                        case MessageLevel.Info:
                            messages.AddTidyInformation(tidyMessage, theMessage.Line, theMessage.Column);
                            break;
                        default:
                            messages.AddTidyInformation(tidyMessage, theMessage.Line, theMessage.Column);
                            break;
                    }
                }
            }
            return messages;
        }



        private static Tidy CreateNewTidyManager()
        {
            Tidy tidy = new Tidy();
            tidy.Options.Xhtml = true;
            tidy.Options.CharEncoding = CharEncoding.UTF8;
            tidy.Options.DropEmptyParas = true;
            tidy.Options.LogicalEmphasis = false;
            tidy.Options.TidyMark = false;
            tidy.Options.NumEntities = false;
            tidy.Options.RawOut = true;
            tidy.Options.XmlPi = true;
            tidy.Options.Word2000 = true;
            tidy.Options.WrapLen = 0;
            return tidy;
        }

        private static string GetMetaContentAttribute(string data)
        {
            Regex findContentTypeRegEx = new Regex("<meta(.*?)http-equiv=\"Content-Type\"(.*?)content=\"(?<contentAttribute>(.*?))\">", 
                RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match contentTypeMatch = findContentTypeRegEx.Match(data);
            if (contentTypeMatch.Success)
                return contentTypeMatch.Groups["contentAttribute"].Value ?? "";
            return "";

        }

        private static string ConvertSpecialCharactersToEscapedCharacters(string data)
        {
            data = data.Replace("\u2013", "&ndash;");
            data = data.Replace("\u2014", "&mdash;");
            data = data.Replace("\u2018", "&lsquo;");
            data = data.Replace("\u2019", "&rsquo;");
            data = data.Replace("\u201A", "&sbquo;");
            data = data.Replace("\u201C", "&ldquo;");
            data = data.Replace("\u201D", "&rdquo;");
            data = data.Replace("\u201E", "&bdquo;");
            return data;
        }

    }
}
