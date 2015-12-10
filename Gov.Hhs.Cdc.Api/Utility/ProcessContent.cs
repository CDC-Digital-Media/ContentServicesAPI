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
using System.Text.RegularExpressions;
using System.IO;
using TidyNet;
using HtmlAgilityPack;
using Gov.Hhs.Cdc.Media.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public sealed class ProcessContent
    {
        //protected static Regex titleExpression = new Regex("<title>(?<title>.*)</title>",
        //    RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        protected static Regex commentExpression = new Regex("<!--(.*?)-->",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        //protected static Regex scriptExpression = new Regex("<script(.*?)>(?<script>(.*?))</script>",
        //    RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        //protected static Regex getDivExpression = new Regex("<div class='syndicate'(.*?)>(?<div>(.*?))</div>",
        //    RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        //protected static Regex anchorExpression = new Regex("<a\\s(.*?)>(?<anchor>(.*?))</a>",
        //    RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        //protected static Regex imageExpression = new Regex("<img\\s(.*?)/>",
        //    RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        #region "Public Methods"

        public static Encoding ParseEncoding(String CharacterSet, String Content)
        {
            Encoding oReturn = null;
            if (CharacterSet != "ISO-8859-1")
            {
                oReturn = Encoding.GetEncoding(CharacterSet);
            }
            else
            {
                Regex charset = new Regex("meta(.*?)content=\"(.*?)charset=(.*?)(?<set>(.*?))\"(.*?)>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                Match m = charset.Match(Content.ToLower());
                if (m.Success)
                {
                    String c = m.Groups["set"].Value;
                    if (!String.IsNullOrEmpty(c))
                    {
                        oReturn = Encoding.GetEncoding(c);
                    }
                    else
                    {
                        oReturn = Encoding.GetEncoding("ISO-8859-1");
                    }
                }
                else
                {
                    oReturn = Encoding.GetEncoding("ISO-8859-1");
                }
            }

            return oReturn;
        }

        public static string TidyContent(string result)
        {
            // Adding a replace run here because we found that TIDY.NET does not 
            // handle upper case closing tags well.  Checked other tags and they seem fine
            // so just going with this here.
            result = result.Replace("</SCRIPT", "</script");

            // now that all that is out of the way we can run some special character level replacements
            // and the real deal tidy
            try
            {
                Boolean runReplacements = true;
                Regex re = new Regex("<meta(.*?)http-equiv=\"Content-Type\"(.*?)content=\"(?<contentType>(.*?))\">", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                Match m = re.Match(result);
                if (m.Success)
                {
                    string c = m.Groups["contentType"].Value;
                    if (c.ToLower().IndexOf("utf-8") > -1)
                    {
                        runReplacements = false;
                    }
                }

                if (runReplacements)
                {
                    if (!runReplacements)    //request.IsWML)
                    {
                        // WML doesn't understand the HTML entities for these characters so just 
                        // use the WML entities for the UTF-8 values.
                        result = result.Replace("\u2013", "&#8211;");
                        result = result.Replace("\u2014", "&#8212;");
                        result = result.Replace("\u2018", "&#8216;");
                        result = result.Replace("\u2019", "&#8217;");
                        result = result.Replace("\u201A", "&#8218;");
                        result = result.Replace("\u201C", "&#8220;");
                        result = result.Replace("\u201D", "&#8221;");
                        result = result.Replace("\u201E", "&#8222;");
                    }
                    else
                    {
                        result = result.Replace("\u2013", "&ndash;");
                        result = result.Replace("\u2014", "&mdash;");
                        result = result.Replace("\u2018", "&lsquo;");
                        result = result.Replace("\u2019", "&rsquo;");
                        result = result.Replace("\u201A", "&sbquo;");
                        result = result.Replace("\u201C", "&ldquo;");
                        result = result.Replace("\u201D", "&rdquo;");
                        result = result.Replace("\u201E", "&bdquo;");
                    }
                }

                System.IO.MemoryStream inputStream = new System.IO.MemoryStream();
                System.IO.MemoryStream outputStream = new System.IO.MemoryStream();

                byte[] inbuffer = Encoding.UTF8.GetBytes(result);

                inputStream.Write(inbuffer, 0, inbuffer.Length);
                inputStream.Seek(0, SeekOrigin.Begin);

                Tidy t = new Tidy();
                TidyMessageCollection tmc = new TidyMessageCollection();
                t.Options.Xhtml = true;
                t.Options.CharEncoding = CharEncoding.UTF8;
                t.Options.DropEmptyParas = false;
                t.Options.LogicalEmphasis = false;
                t.Options.TidyMark = false;
                t.Options.NumEntities = false;
                t.Options.RawOut = true;
                t.Options.XmlPi = true;
                t.Options.Word2000 = true;
                t.Parse(inputStream, outputStream, tmc);
                byte[] buffer = outputStream.GetBuffer();
                inputStream.Close();
                inputStream.Dispose();
                outputStream.Close();
                outputStream.Dispose();
                if (tmc.Errors > 0 || tmc.Warnings > 0)
                {
                    //this.validationResponse.ReportItems.Add("***Begin messages and warnings from Tidy.NET***");
                    //foreach (TidyMessage tm in tmc)
                    //{
                    //    this.validationResponse.ReportItems.Add("Tidy message [level " + tm.Level.ToString() + "] [line " + tm.Line.ToString() + "] [column " + tm.Column.ToString() + "]: " + tm.Message);
                    //    System.Diagnostics.Debug.WriteLine(tm.Message);
                    //    logger.Info(tm.Message);
                    //}
                    //this.validationResponse.ReportItems.Add("***End messages and warnings from Tidy.NET***");
                }


                // if there were tidy errors that will prevent us from moving forward, get the last known good state (post tidy)
                if (tmc.Errors > 0)
                {
                    //if (_store != null && _store.Valid)
                    //{
                    //    if (_store.Contains(request.ContentCacheKey))
                    //    {
                    //        ContentCacheItem item = (ContentCacheItem)_store[request.ContentCacheKey];
                    //        this.FinalUrl = item.FinalUrl;
                    //        result = item.Content;
                    //        this.SourceEncoding = item.ResponseEncoding;
                    //    }
                    //    else
                    //    {
                    //        result = null;
                    //        this.IsValid = false;
                    //    }
                    //}
                }
                else
                {
                    result = UTF8Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    result = result.TrimEnd("\0".ToCharArray());

                    //// since everything was OK, let's store this as the last known good state (post tidy)
                    //if (_store != null && _store.Valid)
                    //{
                    //    ContentCacheItem item = new ContentCacheItem();
                    //    item.Content = result;
                    //    item.ContentSize = result.Length;
                    //    item.ResponseEncoding = this.SourceEncoding;
                    //    _store.Add(request.ContentCacheKey, item);
                    //}
                }

            }
            catch (Exception ex)
            {
                throw;
                //this.validationResponse.ReportItems.Add("Tidy failure :" + ex.ToString());
                //logger.Error(ex.ToString());
                //result = String.Empty;
                //this.IsValid = false;
                //if (_store != null && _store.Valid)
                //{
                //    if (_store.Contains(request.ContentCacheKey))
                //    {
                //        result = (String)_store[request.ContentCacheKey];
                //        this.IsValid = true;
                //    }
                //}
            }

            return result;
        }

        public static string StripComments(string result)
        {
            if (string.IsNullOrEmpty(result))
                throw new ArgumentNullException("result", "result cannot be null or empty");

            Match commentMatch = commentExpression.Match(result);
            if (commentMatch.Success)
            {
                result = commentExpression.Replace(result, "");
            }

            return result;
        }

        public static string StripScripts(string result)
        {
            if (string.IsNullOrEmpty(result))
                throw new ArgumentNullException("result", "result cannot be null or empty");

            ScriptStripper oStripper = new ScriptStripper();
            result = oStripper.strip(result);

            return result;
        }

        public static string FilterXmlDocument(Uri sourceUrl, string xmlString, HtmlExtractionCriteria extractionCriteria)
        {
            HtmlDocument document = new HtmlDocument();

            StringBuilder sbHTML = new StringBuilder();

            // Strip all comments (may break/remove javascript).
            // Doing this replacement here because it appears the people don't understand
            // how to use HTML comments and this was causing TIDY.NET to "break".           
            if (extractionCriteria.ShallStripComments)
                xmlString = ProcessContent.StripComments(xmlString);

            xmlString = ProcessContent.TidyContent(xmlString);

            // remove tabs and newline
            xmlString = xmlString.Replace("\t", " ").Replace("\n", " ").Replace("\r", " ");

            if (extractionCriteria.ShallStripScripts && xmlString.ToLower().Contains("<script"))
                xmlString = ProcessContent.StripScripts(xmlString);

            //  load xml document
            document.LoadHtml(xmlString);

            HtmlNodeCollection elemCollection = (extractionCriteria.IncludedElements != null) ?
                GetIncludedElements(document, extractionCriteria) : null;

            if (extractionCriteria.ExcludedElements != null)
            {
                // remove element by class
                if (extractionCriteria.ExcludedElements.UseElementIds)
                    foreach (string str in extractionCriteria.ExcludedElements.ElementIds)
                        ProcessContent.StripElementByAttribute(elemCollection, "id", str);
                else if (extractionCriteria.ExcludedElements.UseClassNames)
                    foreach (string str in extractionCriteria.ExcludedElements.ClassNames)
                        ProcessContent.StripElementByAttribute(elemCollection, "class", str);
            }
            //----------------------------------------------------

            // set namespace
            ProcessContent.AppendNamespaceToAttribute(elemCollection, "id", ValidateNamespace(extractionCriteria.ContentNamespace));

            // anchor new window
            if (extractionCriteria.ShallOpenInNewWindow)
                ProcessContent.AddAttributeToElement(elemCollection, "a", "target",
                    extractionCriteria.ShallOpenInNewWindow ? "_blank" : "_self");

            if (extractionCriteria.ShallStripStyle)
                ProcessContent.StripAttribute(elemCollection, "style");

            if (extractionCriteria.ShallStripImage)
                ProcessContent.StripElementByType(elemCollection, "img", false);

            if (extractionCriteria.ShallStripAnchor)
                ProcessContent.StripElementByType(elemCollection, "a", true);

            if (extractionCriteria.ShallAlignImages)
                ProcessContent.AddAttributeToElement(elemCollection, "img", "style", "float:" + extractionCriteria.ImageAlign + ";");

            foreach (HtmlNode elem in elemCollection)
            {
                // update img src to absolute path
                ProcessContent.SetAbsoluteURL(elem, sourceUrl, "img");

                // update anchor href to absolute path
                ProcessContent.SetAbsoluteURL(elem, sourceUrl, "a");

                // update area href to absolute path
                ProcessContent.SetAbsoluteURL(elem, sourceUrl, "area");

                // update area href to absolute path
                ProcessContent.SetAbsoluteURL(elem, sourceUrl, "input");

                // build div html string
                sbHTML.AppendLine(elem.OuterHtml);
            }

            if (extractionCriteria.OutputFormat.ToLower() == "xml")
                return OutputEncode("<?xml version=\"1.0\"?>" + sbHTML.ToString(), extractionCriteria.OutputEncoding);
            else
                return OutputEncode(sbHTML.ToString(), extractionCriteria.OutputEncoding);
        }

        private static HtmlNodeCollection GetIncludedElements(HtmlDocument htmlDoc, HtmlExtractionCriteria extractionCriteria)
        {

            if (extractionCriteria.IncludedElements.UseXPath)
            {
                StringBuilder sbSelector = new StringBuilder();
                sbSelector.Clear();
                sbSelector.AppendLine(extractionCriteria.IncludedElements.XPath);

                return htmlDoc.DocumentNode.SelectNodes("//" + sbSelector.ToString());
            }
            else
            {
                StringBuilder sbSelector = new StringBuilder();
                if (extractionCriteria.IncludedElements.UseElementIds)
                {

                    //_reqOption.xElementIDs = ClassElementFilter(extractionCriteria.IncludedElements.ElementIds, true);
                    //_reqOption.ElementIDs = string.Join(",", ClassElementFilter(_reqOption.ElementIDs, false));

                    sbSelector.AppendLine(ProcessContent.BuildXPathAttributeSelector(
                        string.Join(",", extractionCriteria.IncludedElements.ElementIds), "id"));
                }
                else if (extractionCriteria.IncludedElements.UseClassNames)
                {
                    //_reqOption.xClassIDs = ClassElementFilter(_reqOption.ClassIDs, true);
                    //_reqOption.ClassIDs = string.Join(",", ClassElementFilter(_reqOption.ClassIDs, false));

                    sbSelector.AppendLine(ProcessContent.BuildXPathAttributeSelector(
                        string.Join(",", extractionCriteria.IncludedElements.ClassNames), "class"));
                }
                else
                {
                    sbSelector.AppendLine(ProcessContent.BuildXPathAttributeSelector(Param.DEFAULT_CLASS_EXTRACTOR, "class"));
                }

                return htmlDoc.DocumentNode.SelectNodes("//html/body/descendant::*[" + sbSelector.ToString() + "]");
            }
        }

        #endregion

        #region "Private Methods"

        private static void AddAttributeToElement(HtmlNodeCollection nodes, string tagName, string attributeName, string attributeValue)
        {
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    //exclude email and #bookmark links
                    HtmlNodeCollection elements = node.SelectNodes(".//" + tagName + "[not(contains(@href, 'mailto') or contains(@href, '#') or contains(@href, 'javascript'))]");
                    if (elements != null)
                        AddAttributeToElement(elements, attributeName, attributeValue);
                }
            }
        }

        private static void AddAttributeToElement(HtmlNodeCollection nodes, string attributeName, string attributeValue)
        {
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    if (node.Attributes != null &&
                        node.Attributes.Count > 0 &&
                        node.Attributes[attributeName] != null)
                    {
                        switch (attributeName)
                        {
                            case "style":
                                node.Attributes[attributeName].Value += ";" + attributeValue;
                                break;
                            default:
                                node.Attributes[attributeName].Value = attributeValue;
                                break;
                        }
                    }
                    else if (node.Attributes != null &&
                        node.Attributes[attributeName] == null)
                    {
                        node.Attributes.Add(attributeName, attributeValue);
                    }
                }
            }
        }

        private static void AppendNamespaceToAttribute(HtmlNodeCollection nodes, string attributeName, string spaceName)
        {
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    if (node.Attributes != null &&
                        node.Attributes.Count > 0 &&
                        node.Attributes[attributeName] != null)
                    {
                        node.Attributes[attributeName].Value = spaceName + "_" + node.Attributes[attributeName].Value.Trim();
                    }
                    // Recurse to the children nodes.
                    if (node.HasChildNodes)
                        AppendNamespaceToAttribute(node.ChildNodes, attributeName, spaceName);
                }
            }
        }

        /// <summary>
        /// A helper method that "walks" the DOM and removes any style attributes from the nodes.
        /// </summary>
        /// <param name="nodes"></param>
        private static void StripAttribute(HtmlNodeCollection nodes, string attributeName)
        {
            foreach (HtmlNode node in nodes)
            {
                if (node.Attributes != null &&
                    node.Attributes.Count > 0 &&
                    node.Attributes[attributeName] != null)
                {
                    HtmlAttribute styleAttr = node.Attributes[attributeName];
                    node.Attributes.Remove(styleAttr);
                }
                // Recurse to the children nodes.
                if (node.HasChildNodes)
                    StripAttribute(node.ChildNodes, attributeName);
            }
        }

        private static void StripElementByType(HtmlNodeCollection nodes, string elemType, bool keepGrandChildren)
        {
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    HtmlNodeCollection elements = node.SelectNodes(".//" + elemType);
                    if (elements != null)
                    {
                        try
                        {
                            foreach (HtmlNode elem in elements)
                                elem.ParentNode.RemoveChild(elem, keepGrandChildren);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
        }

        private static void StripElementByAttribute(HtmlNodeCollection nodes, string attribute, string attributeValue)
        {
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    HtmlNodeCollection elements = node.SelectNodes(".//*[" + ProcessContent.BuildXPathAttributeSelector(attributeValue.Trim('!'), attribute) + "]");

                    if (elements != null)
                        foreach (HtmlNode elem in elements)
                            elem.ParentNode.RemoveChild(elem);
                }
            }
        }

        private static void SetAbsoluteURL(HtmlNode elemNode, Uri url, string elementType)
        {
            if (elemNode == null)
                throw new ArgumentNullException("elem", "elem cannot be null");

            HtmlNodeCollection elemCollection = elemNode.SelectNodes(".//" + elementType + "[not(contains(@href, 'mailto') or contains(@href, '#') or contains(@href, 'javascript'))]");

            if (elemCollection != null)
            {
                foreach (HtmlNode elem in elemCollection)
                {
                    if (elem.Attributes != null)
                    {
                        switch (elementType.Trim().ToLower())
                        {
                            case "img":
                                if (elem.Attributes["src"] != null &&
                                    elem.Attributes["src"].Value.Trim().StartsWith("/"))
                                    elem.Attributes["src"].Value = url.Scheme + "://" + (url.Port != 80 ?
                                        url.Host + ":" + url.Port : url.Host) + elem.Attributes["src"].Value;
                                break;
                            case "input":
                                if (elem.Attributes["type"] != null &&
                                    elem.Attributes["src"] != null &&
                                    elem.Attributes["type"].Value.ToLower() == "image" &&
                                    elem.Attributes["src"].Value.Trim().StartsWith("/"))
                                    elem.Attributes["src"].Value = url.Scheme + "://" + (url.Port != 80 ?
                                        url.Host + ":" + url.Port : url.Host) + elem.Attributes["src"].Value;
                                break;
                            default:
                                if (elem.Attributes["href"] != null &&
                                    elem.Attributes["href"].Value.Trim().StartsWith("/"))
                                    elem.Attributes["href"].Value = url.Scheme + "://" + (url.Port != 80 ?
                                        url.Host + ":" + url.Port : url.Host) + elem.Attributes["href"].Value;
                                break;
                        }
                    }
                }
            }
        }

        private static string BuildXPathAttributeSelector(string elementName, string elementType)
        {
            StringBuilder sb = new StringBuilder();
            List<string> classList = elementName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList<string>();

            if (classList.Count > 0)
            {
                sb.AppendLine("[");
                foreach (string cls in classList)
                {
                    if (cls.Length > 0)
                    {
                        if (!cls.StartsWith("!"))
                        {
                            if (sb.ToString().Trim().Replace("[", "").Length > 0)
                                sb.AppendLine(" or @" + elementType + "=\"" + cls + "\" or contains(@" + elementType + ", \"" + cls + " \")");
                            else
                                sb.AppendLine("@" + elementType + "=\"" + cls + "\" or contains(@" + elementType + ", \"" + cls + " \")");
                        }
                    }
                }
                //sb.AppendLine("]");
            }
            return sb.ToString().Trim('[');
        }

        private static List<string> ClassElementFilter(string strFilter, bool negate)
        {
            if (negate)
                return strFilter.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries).Where(a => a.StartsWith("!")).Select(b => b.Trim('!')).ToList<string>();
            else
                return strFilter.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries).Where(a => !a.StartsWith("!")).Select(b => b.Trim('!')).ToList<string>();
        }

        private static string ValidateNamespace(string spaceName)
        {
            // now we need to handle the content namespace parameter
            if (!String.IsNullOrEmpty(spaceName))
            {
                // strip a trailing underscore if passed
                while (spaceName.EndsWith("_"))
                {
                    spaceName = spaceName.Substring(0, spaceName.Length - 1);
                }

                // make sure it is alpha_only
                if (Regex.IsMatch(spaceName, "^[a-zA-Z]*$"))
                {
                    spaceName = Util.StripTrim(spaceName, 255);
                }
                else
                {
                    spaceName = Param.DEFAULT_NAMESPACE_EXTRACTOR;
                }
            }
            return spaceName;
        }

        private static string OutputEncode(string strOut, string outputEncoding)
        {
            // now we need to see if we need to convert the output
            // encoding to somthing other than UTF-8
            if (!String.IsNullOrEmpty(outputEncoding))
            {
                Encoding oEnc = Encoding.GetEncoding(outputEncoding);
                if (oEnc != null && oEnc != Encoding.UTF8)
                {
                    // convert from whatever we got to UTF8 because that is what WE serve
                    byte[] cbuffer = Encoding.UTF8.GetBytes(strOut);
                    byte[] ebuffer = Encoding.Convert(Encoding.UTF8, oEnc, cbuffer, 0, cbuffer.Length);
                    strOut = oEnc.GetString(ebuffer);
                }
            }

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

            return Util.HtmlEncodeOutput(strOut);
        }

        #endregion

    }
}
