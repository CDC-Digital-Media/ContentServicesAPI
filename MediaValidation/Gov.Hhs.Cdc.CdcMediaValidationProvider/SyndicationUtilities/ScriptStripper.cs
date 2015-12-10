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
using System.Threading;
using System.Diagnostics;

namespace Gov.Hhs.Cdc.CdcMediaValidationProvider
{
    public class ScriptStripper
    {
        public string Strip(string snippet)
        {
            if (!string.IsNullOrEmpty(snippet))
            {
                if (snippet.ToLower().Contains("<script"))
                {
                    return DoStrip(snippet);
                }
                else
                {
                    return snippet;
                }
            }
            return string.Empty;
        }

        protected static string DoStrip(string inval)
        {
            bool inscript = false;
            bool inliteral = false;
            bool escaped = false;
            string outval = string.Empty;
            string buffer = string.Empty;
            int chr;
            char lastchar = 'M'; // starting value is meaninless so making it 'M'
            char stringDefChar = 'M';// starting value is meaningless so making it 'M'
            StringReader reader = new StringReader(inval);
            bool done = false;

            try
            {
                if (inval.ToLower().Contains("<script"))
                {
                    int oldpos = 0;
                    int offset = 0;
                    int current = 0;
                    char[] eatbuffer;

                    while (inval.ToLower().IndexOf("<script", current) > -1)
                    {



                        int pos = inval.ToLower().IndexOf("<script", current);

                        int len = pos - current;
                        outval += inval.Substring(current, len);

                        // update the current position to reflect the move
                        current += len;

                        // now pull the non-script stuff off the reader
                        eatbuffer = new char[len];
                        reader.ReadBlock(eatbuffer, 0, len);

                        // and set up to strip the next group of script
                        oldpos = pos;
                        offset = 0;
                        done = false;

                        while (!done)
                        {

                            chr = reader.Read();

                            if (chr != -1)
                            {
                                offset++;

                                // we are not at the end of the stream
                                if (!inscript)
                                {
                                    buffer += (char)chr;
                                    if (!buffer.ToLower().StartsWith("<script"))
                                    {

                                        // we need to keep enough to check for the start tag
                                        if (buffer.Length > 7)
                                        {
                                            // move beginning character to output
                                            outval += buffer[0]; // sBuffer.Substring(0, 1);
                                            buffer = buffer.Substring(1);
                                        }
                                    }
                                    else
                                    {
                                        inscript = true;
                                        buffer = string.Empty;
                                    }
                                }
                                else
                                {
                                    // note that since we are in a script we do not append this to the output at all
                                    // everything we are doing in here is to determine if this is at the end 
                                    // of the script (either in a self closing tag) or in an HTML friendly tag set

                                    char theChar = (char)chr;

                                    if (inliteral)
                                    {
                                        if (lastchar == '\\')
                                            escaped = true;
                                        else
                                            escaped = false;
                                    }
                                    else
                                    {
                                        escaped = false;
                                    }

                                    if ((theChar == '\"' || theChar == '\'') && !escaped)
                                    {
                                        if (!inliteral)
                                        {
                                            inliteral = true;
                                            stringDefChar = theChar;
                                        }
                                        else
                                        {
                                            if (theChar == stringDefChar && !escaped)
                                            {
                                                inliteral = false;
                                            }
                                        }
                                    }


                                    if (!inliteral)
                                    {
                                        buffer += theChar;
                                        if (buffer.EndsWith("/>") || buffer.ToLower().EndsWith("</script>"))
                                        {
                                            inscript = false;
                                            buffer = string.Empty;
                                            done = true;  // go back so it can take off the next big block
                                        }
                                    }

                                    lastchar = theChar;
                                }
                            }
                            else
                            {
                                // we have read the end of the stream  need to place anything we have in the buffer on the end
                                if (!inscript && buffer != string.Empty)
                                {
                                    outval += buffer;
                                }

                                done = true;

                            }

                            current = oldpos + offset;

                        } // end while !bDone

                    } // end while contains <script

                    outval += reader.ReadToEnd();
                }
                else
                {
                    outval = inval;
                }
            }
            catch (ThreadAbortException ex)
            {
                // write it out if we are debugging otherwise
                Debug.WriteLine(ex.ToString());

            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.ToString());
            }

            return outval;
        }

    }
}
