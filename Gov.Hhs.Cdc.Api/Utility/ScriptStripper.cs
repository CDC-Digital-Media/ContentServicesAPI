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
using System.IO;

namespace Gov.Hhs.Cdc.Api
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ScriptStripper
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snippet"></param>
        /// <returns></returns>
        public String strip(String snippet)
        {
            String sReturn = String.Empty;
            if (!string.IsNullOrEmpty(snippet))
            {
                if (snippet.ToLower().Contains("<script"))
                {
                    sReturn = this.DoStrip(snippet);
                }
                else
                {
                    sReturn = snippet;
                }
            }
            return sReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inval"></param>
        /// <returns></returns>
        protected String DoStrip(String inval)
        {
            bool inscript = false;
            bool inliteral = false;
            bool escaped = false;
            String outval = String.Empty;
            String sBuffer = String.Empty;
            int chr;
            char lastchar = 'M'; // starting value is meaninless so making it 'M'
            char stringDefChar = 'M';// starting value is meaningless so making it 'M'
            StringReader oReader = new StringReader(inval);
            bool bDone = false;

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
                        oReader.ReadBlock(eatbuffer, 0, len);

                        // and set up to strip the next group of script
                        oldpos = pos;
                        offset = 0;
                        bDone = false;

                        while (!bDone)
                        {

                            chr = oReader.Read();

                            if (chr != -1)
                            {
                                offset++;

                                // we are not at the end of the stream
                                if (!inscript)
                                {
                                    sBuffer += (char)chr;
                                    if (!sBuffer.ToLower().StartsWith("<script"))
                                    {

                                        // we need to keep enough to check for the start tag
                                        if (sBuffer.Length > 7)
                                        {
                                            // move beginning character to output
                                            outval += sBuffer[0]; // sBuffer.Substring(0, 1);
                                            sBuffer = sBuffer.Substring(1);
                                        }
                                    }
                                    else
                                    {
                                        inscript = true;
                                        sBuffer = string.Empty;
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
                                        sBuffer += theChar;
                                        if (sBuffer.EndsWith("/>") || sBuffer.ToLower().EndsWith("</script>"))
                                        {
                                            inscript = false;
                                            sBuffer = string.Empty;
                                            bDone = true;  // go back so it can take off the next big block
                                        }
                                    }

                                    lastchar = theChar;
                                }
                            }
                            else
                            {
                                // we have read the end of the stream  need to place anything we have in the buffer on the end
                                if (!inscript && sBuffer != string.Empty)
                                {
                                    outval += sBuffer;
                                }

                                bDone = true;

                            }

                            current = oldpos + offset;

                        } // end while !bDone

                    } // end while contains <script

                    outval += oReader.ReadToEnd();
                }
                else
                {
                    outval = inval;
                }
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                // write it out if we are debugging otherwise
                System.Diagnostics.Debug.WriteLine(ex.ToString());

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return outval;
        }

    }    
}
