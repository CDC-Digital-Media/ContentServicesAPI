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
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.MediaValidatonProvider;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcMediaValidationProvider
{
    public class HtmlMediaValidator 
    {

        protected static Regex FindCommentsExpression = new Regex("<!--(.*?)-->",
                RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        protected static Regex FindBreaksExpression = new Regex("<br(.*?)>",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public string HtmlTagsToLower(string html)
        {
            string matchExpression = @"((?:</?)\w+)(?=.*?>)";
            MatchEvaluator tolower = MatchEvaluatorToLower;
            return Regex.Replace(html, matchExpression, tolower);
        }

        private static string MatchEvaluatorToLower(Match m)
        {
            return m.Value.ToLower();
        }

        public ExtractionResult ExtractAndValidate(MediaAddress mediaAddress, CollectedData collectedData,
            MediaValidatorConfig config, MediaTypeValidationItem mediaType, bool isExtraction)
        {
            HtmlMediaAddress htmlMediaAddress = (HtmlMediaAddress)mediaAddress;
            EffectiveHtmlExtractionCriteria extractionCriteria = htmlMediaAddress.GetExtractionCriteriaWithDefaults();
            if (!collectedData.IsCollectionValid)
            {
                return new ExtractionResult(htmlMediaAddress, collectedData, null);
            }
            ExtractedDetail htmlExtractedDetail = ExtractHtmlDetail(extractionCriteria, htmlMediaAddress,
                collectedData, config, mediaType, isExtraction);

            htmlExtractedDetail.Data = Encoder.Encode(htmlExtractedDetail.Data, extractionCriteria.OutputFormat, extractionCriteria.OutputEncoding);

            htmlExtractedDetail.NumberOfAppliedMediaTypeFilterItems = mediaType.FilterItems.Count;
            return new ExtractionResult(htmlMediaAddress, collectedData, htmlExtractedDetail);
        }

        private ExtractedDetail ExtractHtmlDetail(EffectiveHtmlExtractionCriteria extractionCriteria,
            HtmlMediaAddress address, CollectedData collectedData,
            MediaValidatorConfig config, MediaTypeValidationItem mediaType, bool isExtraction)
        {
            ValidationMessages messages = new ValidationMessages();
            string data = collectedData.Data;

            if (!string.IsNullOrEmpty(data))
            {
                data = FilterContent(data, mediaType.FilterItems);
            }


            if (extractionCriteria.StripScript && data.Contains("<script"))
            {
                ScriptStripper stripper = new ScriptStripper();
                data = stripper.Strip(data);
            }

            if (extractionCriteria.StripComment)
            {
                data = FindCommentsExpression.Replace(data, "");
            }

            if (extractionCriteria.StripBreak)
            {
                data = FindBreaksExpression.Replace(data, "");
            }

            //Tidy will change all the tags to lower case, so we don't need to do this
            data = HtmlTagsToLower(data);

            TidyResults tidyResults = TidyMgmt.TidyContent(data, address.PreferredRenderingType);
            messages = messages.Union(tidyResults.Messages);

            if (tidyResults.Data.Length > 0)
            {
                data = tidyResults.Data;
            }

            DateTime start = DateTime.Now;
            try
            {
                using (var document = new CdcSiteXmlDocument(data, config.XMLCatalogPath, extractionCriteria.ContentNamespace, address.Uri))
                {
                    ExtractedDetail detail;
                    if (document != null && document.IsValid && document.SelectNodesAnywhere("html").Count > 0)
                    {

                        detail = ExtractDetailNodes(extractionCriteria, document, address, isExtraction);
                        detail.PrependMessages(messages);
                    }
                    else
                    {
                        detail = new ExtractedDetail(data, messages);
                    }


                    if (document != null && document.IsValid)
                    {
                        detail.XHtmlLoadable = true;
                        detail.Title = document.GetFirstInnerText("title");
                    }
                    else
                    {
                        detail.XHtmlLoadable = false;
                        detail.Messages.AddError("XML Exception", "Page cannot be loaded as an XML document");
                    }

                    return detail;
                }
            }
            catch (XmlException ex)
            {
                //messages.AddError("Unable to load page as XHtml document");
                Logger.LogError(ex, "Load XML error for: " + address.Uri);
                messages.AddError("XML Exception", "XML Exception: " + ex.Message);
            }
            catch (WebException wex)
            {
                if (wex.Message == "The operation has timed out")
                {
                    messages.AddError("XML Exception", string.Format("Loading XML Document for URL ({0}) has timed out probably due to included web reference. Seconds: {1} ",
                        address.Uri.AbsoluteUri, DateTime.Now.Subtract(start).TotalSeconds.ToString()));
                }
                else
                {
                    Logging.Logger.LogError(wex, "HtmlMediaValidator:ExtractHtmlDetail: " + "Unexpected web exception: " + wex.Message);
                    messages.AddError("Web Excemption", "Unexpected web exception: " + wex.Message);
                }
            }

            return new ExtractedDetail(data, messages)
            {
                XHtmlLoadable = false
            };

        }


        /// <summary>
        /// Filters out content based on a series of regular expressions defined in an external, static XML file
        /// </summary>
        /// <param name="request"></param>
        private static string FilterContent(string content, IEnumerable<MediaTypeFilterItem> filters)
        {
            string result = content;
            foreach (MediaTypeFilterItem filter in filters)
            {
                Regex regex = new Regex(filter.FilterRegEx);
                string replacementString = string.IsNullOrEmpty(filter.Replacement) ? string.Empty : filter.Replacement;
                result = regex.Replace(result, replacementString);
            }
            return result;
        }


        private ExtractedDetail ExtractDetailNodes(EffectiveHtmlExtractionCriteria extractionCriteria, CdcSiteXmlDocument document, HtmlMediaAddress address, bool isExtraction)
        {
            Uri uri = address.Uri;

            //TODO: Need to set the data on the extracted detail
            ExtractedDetail detail = new ExtractedDetail();
            XmlNodeList syndicatedNodes = GetSyndicatedNodes(document, extractionCriteria.IncludedElements);

             //remove because of incorrect requirements 11/03/2015
            //if (syndicatedNodes.Count == 0) 
            //{
            //    if (address.MediaObject.MediaTypeParms.IsFeedItem)
            //    {
            //        syndicatedNodes = document.SelectNodesAnywhere("body");
            //    }
            ////}

            if (!isExtraction)
                GetInitialCountsForValidation(document, detail, syndicatedNodes);

            // or contains(@href, '#')
            AttributeSelection hyperLinkExclusions = new AttributeSelection("[not(contains(@href, 'mailto') or starts-with(@href, '#') or contains(@href, 'javascript'))]");

            string hyperLinkWindowAttribute = extractionCriteria.NewWindow ? "_blank" : "_self";
            string imageAlignValue = extractionCriteria.ShallAlignImages ? "float:" + extractionCriteria.ImageAlign : "";

            foreach (XmlNode syndicatedNode in syndicatedNodes)
            {
                if (extractionCriteria.ExcludedElements != null)
                {
                    if (extractionCriteria.ExcludedElements.UseXPath)
                        document.RemoveNodesFrom(syndicatedNode, new XPathSelection(null, new AttributeSelection() { Value = extractionCriteria.ExcludedElements.XPath }));
                    else
                        document.RemoveNodesFrom(syndicatedNode, GetAttributeSelection(extractionCriteria.ExcludedElements));
                }

                MakeIdsUnique(document.ContentNamespace, document.SelectNodesFrom(syndicatedNode, new XPathSelection("*", new AttributeSelection("id"))));
                MakeIdsUnique(document.ContentNamespace, (XmlElement)syndicatedNode);

                //We can strip scripts via the document instead of before hand, but doing it first will decrease the chances
                //that we have a problematic script that we will have to deal with
                //if (extractionCriteria.ShallStripScripts)
                //    document.RemoveNodesFrom(syndicatedNode, "script");

                CdcSiteXmlDocument.SetAttribute(document.SelectNodesFrom(syndicatedNode, "a", hyperLinkExclusions),
                    "target", hyperLinkWindowAttribute);

                if (extractionCriteria.StripStyle)
                {
                    document.RemoveAttributeFrom(syndicatedNode, "*", "style");
                }

                if (extractionCriteria.StripImage)
                {
                    document.RemoveNodesFrom(syndicatedNode, new XPathSelection("img"));
                }

                if (extractionCriteria.StripAnchor)
                {
                    document.RemoveAttributeFrom(syndicatedNode, "a", "href");
                }

                if (extractionCriteria.ShallAlignImages)
                {
                    document.AppendAttribute(document.SelectNodesFrom(syndicatedNode, "img"), "style", imageAlignValue);
                }

                if (syndicatedNode.Name.ToLower() == "img")
                {
                    SetAbsoluteUrl(document.SelectNodesFrom(syndicatedNode.ParentNode, "img"), "src", uri);
                }
                else
                {
                    SetAbsoluteUrl(document.SelectNodesFrom(syndicatedNode, "img"), "src", uri);
                }

                if (syndicatedNode.Name.ToLower() == "a")
                {
                    SetAbsoluteUrl(document.SelectNodesFrom(syndicatedNode.ParentNode, "a", hyperLinkExclusions), "href", uri);
                }
                else
                {
                    SetAbsoluteUrl(document.SelectNodesFrom(syndicatedNode, "a", hyperLinkExclusions), "href", uri);
                }

                if (syndicatedNode.Name.ToLower() == "area")
                {
                    SetAbsoluteUrl(document.SelectNodesFrom(syndicatedNode.ParentNode, "area"), "href", uri);
                }
                else
                {
                    SetAbsoluteUrl(document.SelectNodesFrom(syndicatedNode, "area"), "href", uri);
                }


                if (syndicatedNode.Name.ToLower() == "input")
                {
                    SetAbsoluteUrl(document.SelectNodesFrom(syndicatedNode.ParentNode, "input", new AttributeSelection("type", "image")), "src", uri);
                }
                else
                {
                    SetAbsoluteUrl(document.SelectNodesFrom(syndicatedNode, "input", new AttributeSelection("type", "image")), "src", uri);
                }
                //<li class="twitter" style=" background: transparent url(/TemplatePackage/images/twitter.png) 0 center no-repeat;">
                //<a href="http://twitter.com/CDCFlu" class="noLinking">CDC Flu on Twitter</a>
                //</li>
                //SetAbsoluteUrl(document.SelectNodesFrom(syndicatedNode, "*", new AttributeSelection("style", "url", true)), "style", uri);

                if (!isExtraction)
                {
                    CountDetailItemsForValidation(document, syndicatedNode, detail);
                }
            }

            if (!isExtraction)
            {
                TotalCountsForValidation(detail);
            }

            string output = "";
            foreach (XmlNode syndicatedNode in syndicatedNodes)
            {
                output += syndicatedNode.OuterXml;
            }
            detail.Data = output;
            return detail;
        }

        private static void GetInitialCountsForValidation(CdcSiteXmlDocument document, ExtractedDetail detail, XmlNodeList syndicatedNodes)
        {
            detail.NumberOfInlineStyles = document.SelectNodesFromBody(new XPathSelection("*", new AttributeSelection("style"))).Count;
            detail.NumberOfElements = syndicatedNodes.Count;
            detail.SizeOfElements = 0;
            detail.NumberOfForms = 0;
            detail.NumberOfEmbeddedVideos = 0;

            detail.NumberOfOtherEmbeddedVideos =
                  document.SelectNodesAnywhere("video").Count
                + document.SelectNodesAnywhere("embed").Count
                + document.SelectNodesAnywhere("object").Count;

            detail.NumberOfOtherForms = document.SelectNodesAnywhere("form").Count;
        }

        private static void CountDetailItemsForValidation(CdcSiteXmlDocument document, XmlNode syndicatedNode, ExtractedDetail detail)
        {
            detail.SizeOfElements += syndicatedNode.OuterXml.Length;
            //XmlNodeList formNodes = syndicatedElement.SelectNodes("//ul");
            detail.NumberOfForms += document.SelectNodesFrom(syndicatedNode, "form").Count;

            //http://www.w3schools.com/html/html_videos.asp
            //http://www......[domain]...../CDCTV/StoryOfIyal/
            //XmlNodeList embeddedNodes = document.SelectNodesFrom(syndicatedNode, "embed");

            detail.NumberOfEmbeddedVideos += document.SelectNodesFrom(syndicatedNode, "video").Count;
            detail.NumberOfEmbeddedVideos += document.SelectNodesFrom(syndicatedNode, "embed").Count;
            detail.NumberOfEmbeddedVideos += document.SelectNodesFrom(syndicatedNode, "object").Count;
        }

        private static void TotalCountsForValidation(ExtractedDetail detail)
        {
            detail.NumberOfOtherEmbeddedVideos = detail.NumberOfOtherEmbeddedVideos - detail.NumberOfEmbeddedVideos;
            detail.NumberOfOtherForms = detail.NumberOfOtherForms - detail.NumberOfForms;
        }

        private static XmlNodeList GetSyndicatedNodes(CdcSiteXmlDocument document, ExtractionPath path)
        {
            if (path != null && path.UseXPath)
            {
                return document.SelectNodesByXpath(path.XPath);
            }
            XPathSelection selection = GetAttributeSelection(path);
            if (selection == null)
            {
                selection = new XPathSelection("div", new AttributeSelection("class", "syndicate"));
            }
            return document.SelectNodesFromBody(selection);
        }

        private static XPathSelection GetAttributeSelection(ExtractionPath path)
        {
            if (path != null)
            {
                if (path.UseElementIds)
                //Use wildcard as an Id can be any kind of HTML tag
                {
                    return new XPathSelection("*", new AttributeSelection("id", path.ElementIds));
                }
                if (path.UseClassNames)
                {
                    return new XPathSelection("*", new AttributeSelection("class", path.ClassNames));
                }
            }
            return null;
        }


        private static void SetAbsoluteUrl(XmlNodeList nodes, string attributeName, Uri url)
        {
            if (nodes == null || nodes.Count == 0)
            {
                return;
            }
            foreach (XmlNode node in nodes)
            {
                if (IsValidAttribute(node.Attributes, attributeName))
                {
                    node.Attributes[attributeName].Value = GetAbsoluteUrl(url, node.Attributes[attributeName].Value);
                }
            }
        }
        
        private static bool IsValidAttribute(XmlAttributeCollection attributes, string attributeName)
        {
            if (attributes == null || attributes[attributeName] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static string GetAbsoluteUrl(Uri url, string href)
        {
            string sParentFolder = url.ToString();
            if (sParentFolder.Contains("/"))
            {
                sParentFolder = sParentFolder.Substring(0, sParentFolder.LastIndexOf('/') + 1);
            }

            Uri path = new Uri(sParentFolder);

            if (!href.StartsWith("http"))
            {
                if (href.StartsWith("/"))
                {
                    href = path.Scheme + "://" + path.Authority + href;
                }
                else
                {
                    string sPath = path.GetLeftPart(UriPartial.Path);

                    while (href.StartsWith("../"))
                    {
                        href = href.Substring(3);
                        sPath = UpParent(sPath);
                    }
                    href = sPath + href;
                }
            }
            else
            {
                // if the content is NOT coming from wwwdev, we make an attempt here to correct any urls on the page
                // mistakenly left pointed at wwwdev or wwwlink.
                if (path.Authority.ToLower().Contains(".....[domain].....") && !path.Authority.ToLower().Contains("dev......[domain]....."))
                {
                    href = FixDevAndLinkIssues(href);
                }
            }

            return href;
        }

        private static string UpParent(string Path)
        {
            string sReturn = Path;
            if (sReturn.EndsWith("/"))
            {
                sReturn = sReturn.Substring(0, sReturn.Length - 1);
            }
            sReturn = sReturn.Substring(0, sReturn.LastIndexOf("/"));
            if (!sReturn.EndsWith("/"))
            {
                sReturn += "/";
            }

            return sReturn;
        }

        private static string FixDevAndLinkIssues(string hRef)
        {
            string sReturn = hRef;

            Regex dev = new Regex("wwwdev......[domain].....", RegexOptions.IgnoreCase);
            Regex link = new Regex("wwwlink......[domain].....", RegexOptions.IgnoreCase);

            try
            {
                sReturn = dev.Replace(sReturn, "www......[domain].....");
                sReturn = link.Replace(sReturn, "www......[domain].....");
            }
            catch (Exception ex)
            {
                sReturn = hRef;
                Logging.Logger.LogError(ex);
            }
            return sReturn;
        }


        private static void MakeIdsUnique(string name, XmlNodeList nodes)
        {
            if (nodes == null || nodes.Count == 0)
            {
                return;
            }
            foreach (XmlElement node in nodes)
            {
                MakeIdsUnique(name, node);
            }
        }

        private static void MakeIdsUnique(string name, XmlElement node)
        {
            if (node.Attributes["id"] != null)
            {
                node.SetAttribute("id", name + "_" + node.Attributes["id"].Value);
            }
        }

    }
}
