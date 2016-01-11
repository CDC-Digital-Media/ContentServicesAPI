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
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using System.Web;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;

using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

using Gov.Hhs.Cdc.MediaProvider;
using Newtonsoft.Json.Serialization;

namespace Gov.Hhs.Cdc.Api
{

    public class OutputWriter : IOutputWriter
    {
        private ICallParser TheParser { get; set; }

        private string strReturn { get; set; }

        public OutputWriter(ICallParser parser)
        {
            TheParser = parser;
        }

        public bool IsImageFormat
        {
            get
            {
                return TheParser.Query.Format == FormatType.png
                    || TheParser.Query.Format == FormatType.jpeg
                    || TheParser.Query.Format == FormatType.jpg;
            }
        }

        public void CreateAndWriteSerialResponse(object results, ValidationMessages messages = null)
        {
            SetHttpStatusCode(messages);

            SerialResponse response = new SerialResponse()
            {
                results = results,
                meta = new SerialMeta()
                {
                    status = HttpContext.Current.Response.StatusCode,
                    message = messages == null ? new List<SerialValidationMessage>() :
                        SerializeValidationMessages(messages)
                }
            };

            WriteWithFormat(response, isWritingFromCache: false);
        }

        private void PopulateSerialResponse(SerialResponse serialResponse, ValidationMessages messages)
        {
            SerializeValidationMessages(messages).ForEach(a => serialResponse.meta.message.Add(a));

            if (serialResponse != null && serialResponse.meta != null && serialResponse.meta.message != null && serialResponse.meta.message.Any())
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            serialResponse.meta.status = HttpContext.Current.Response.StatusCode;
        }

        private static void SetHttpStatusCode(ValidationMessages messages)
        {
            if (messages != null && messages.Errors().Any())
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }

        public void Write(object theObject, ValidationMessages messages = null)
        {
            SetBeforeWrite(theObject, messages);

            WriteWithFormat(theObject, isWritingFromCache: false);
        }

        private void SetBeforeWrite(object theObject, ValidationMessages messages)
        {
            ValidationMessages theMessages = messages ?? new ValidationMessages();
            if (theObject == null)
                throw new ArgumentNullException("theObject", "theObject cannot be null.");

            if (TheParser == null)
                SetMissingParserError();

            if (theObject is SerialResponse)
                PopulateSerialResponse((SerialResponse)theObject, theMessages);
            else if (theObject is string && string.IsNullOrEmpty((string)theObject))
                throw new ArgumentException("Argument cannot be null or empty.", "theObject");
        }

        public ValidationMessages Write(ValidationMessages messages)
        {
            Write(new SerialResponse(), messages);
            return messages;
        }

        private void SetMissingParserError()
        {
            JsonConvert.SerializeObject("Please initialize the search object.");
            HttpContext.Current.Response.ContentType = "application/json; charset=UTF-8";
            HttpContext.Current.Response.Write(strReturn);
        }

        public static List<SerialValidationMessage> SerializeValidationMessages(ValidationMessages validationMessages)
        {
            if (validationMessages == null)
                return new List<SerialValidationMessage>();

            SerialValidationMessages serialMsg = new SerialValidationMessages();
            serialMsg.items = validationMessages.Messages.Select(a =>
                new SerialValidationMessage()
                {
                    type = a.Severity.ToString(),
                    code = a.Key,
                    id = a.Id,
                    userMessage = a.Message,
                    developerMessage = a.DeveloperMessage
                }).ToList();

            return serialMsg.items;
        }

        private void WriteWithFormat(object theObject, bool isWritingFromCache)
        {
            GenerateOutput(theObject, isWritingFromCache);

            WriteToOutputStream(theObject, isWritingFromCache);
        }

        private void GenerateOutput(object theObject, bool isWritingFromCache)
        {
            if (TheParser.Query.IsKindOfFeedFormatType == false)
            {
                if (theObject is SerialResponse)
                {
                    // test to null so that property is not serialized
                    var sr = (SerialResponse)theObject;
                    sr.dataset = null;
                    sr.mediaObjects = null;
                    sr.param = null;

                    theObject = sr;
                }
            }

            if ((TheParser.Query.Format == FormatType.json || TheParser.Query.Format == FormatType.jsonp) && (theObject is SerialResponse))
            {
                strReturn = Serialize(theObject);
            }

            if (theObject is byte[] && !IsImageFormat)
            {
                TheParser.Query.Format = FormatType.png;
            }

            switch (TheParser.Query.Format)
            {
                case FormatType.html:
                    HttpContext.Current.Response.ContentType = "text/html; charset=UTF-8";
                    if (theObject is ISerialSyndication)
                    {
                        strReturn = Util.HtmlDecodeOutput(((ISerialSyndication)theObject).content);
                    }
                    break;
                case FormatType.atom:
                    HttpContext.Current.Response.ContentType = "application/xml; charset=UTF-8";

                    theObject = AddMetricsRedirectForFeedItems((SerialResponse)theObject);
                    strReturn = new FeedFactory(((SerialResponse)theObject).mediaObjects.ToList(), TheParser).Create();

                    List<MediaProvider.MediaObject> mediaObjs = ((SerialResponse)theObject).mediaObjects.ToList();
                    AddFeedItemImageDetailsAtom(mediaObjs);
                    break;
                case FormatType.rss:
                    HttpContext.Current.Response.ContentType = "application/xml; charset=UTF-8";

                    theObject = AddMetricsRedirectForFeedItems((SerialResponse)theObject);
                    strReturn = new FeedFactory(((SerialResponse)theObject).mediaObjects.ToList(), TheParser).Create();

                    mediaObjs = ((SerialResponse)theObject).mediaObjects.ToList();
                    AddFeedItemImageDetailsRss(mediaObjs);
                    break;
                case FormatType.itunes:
                    HttpContext.Current.Response.ContentType = "application/xml; charset=UTF-8";
                    strReturn = old_FeedFormatter.GenerateFeed((Gov.Hhs.Cdc.Api.SerialResponse)theObject, TheParser.Query.Format.ToString());
                    break;
                case FormatType.xml:
                    HttpContext.Current.Response.ContentType = "application/xml; charset=UTF-8";

                    //// write xml out to httpOutputStream
                    //DataContractSerializer xmler = new DataContractSerializer(theObject.GetType());
                    //xmler.WriteObject(HttpContext.Current.Response.OutputStream, theObject);

                    //// Add to cache
                    //if (isWritingFromCache == false)
                    //    ServiceUtility.AddApplicationCachePublicApi(theObject, TheParser);

                    using (var stream = new MemoryStream())
                    {
                        DataContractSerializer xmler = new DataContractSerializer(theObject.GetType());
                        xmler.WriteObject(stream, theObject);

                        stream.Position = 0;
                        using (var reader = new StreamReader(stream))
                        {
                            strReturn = reader.ReadToEnd();
                        }
                    }
                    break;
                case FormatType.jpg:
                case FormatType.jpeg:
                    byte[] jpegbuffer = (byte[])theObject;
                    HttpContext.Current.Response.ContentType = "image/jpeg; charset=UTF-8";

                    WriteImageType(jpegbuffer, isWritingFromCache);
                    return;
                case FormatType.png:
                    if (theObject is SerialResponse) return;
                    byte[] pngbuffer = (byte[])theObject;
                    HttpContext.Current.Response.ContentType = "image/png; charset=UTF-8";

                    WriteImageType(pngbuffer, isWritingFromCache);
                    return;
                default:
                    // FormatType.json
                    HttpContext.Current.Response.ContentType = "application/json; charset=UTF-8";
                    break;
            }

            if (string.IsNullOrEmpty(strReturn) && (theObject is string))
            {
                strReturn = (string)theObject;
            }
        }

        private string Serialize(object theObject)
        {
            //Set all JSON key names to begin with a lower case
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            if (IsCustomSerialize())
            {
                settings.Converters = new List<JsonConverter> { new DictionaryJsonDotNetConverter(), new MediaV2CustomJsonSerializer() };
                return JsonConvert.SerializeObject(theObject, settings);
            }
            else
            {
                settings.Converters = new List<JsonConverter> { new DictionaryJsonDotNetConverter() };
                return JsonConvert.SerializeObject(theObject, settings);
            }
        }

        public void WriteToOutputStream(object theObject, bool isWritingFromCache)
        {
            // allow jsonp response for all requests
            if (TheParser.ParamDictionary.ContainsKey(Param.CALLBACK) && !string.IsNullOrEmpty(TheParser.ParamDictionary[Param.CALLBACK]))
            {
                HttpContext.Current.Response.ContentType = "application/javascript; charset=UTF-8";
                strReturn = TheParser.ParamDictionary[Param.CALLBACK] + "(" + strReturn + ")";
            }

            // write output to the response
            HttpContext.Current.Response.Write(strReturn);

            // Add to cache
            if (isWritingFromCache == false)
            {
                ServiceUtility.AddApplicationCachePublicApi(theObject, TheParser);
            }
        }

        private SerialResponse AddMetricsRedirectForFeedItems(SerialResponse theObject)
        {
            //Split the request url
            NameValueCollection qsPieces = HttpContext.Current.Request.QueryString;
            bool doRedirect = false;

            foreach (string s in qsPieces.AllKeys)
            {
                if (s == "rewrite")
                {
                    if (!qsPieces[s].Equals("1"))
                    {
                        return theObject;
                    }
                    else
                    {
                        doRedirect = true;
                        break;
                    }
                }
            }

            if (doRedirect)
            {
                //should only be one, but just in case we ever have a feed of feeds
                if (theObject.mediaObjects != null)
                {
                    List<MediaProvider.MediaObject> mos = theObject.mediaObjects.ToList();

                    foreach (MediaProvider.MediaObject parent in mos)
                    {
                        if (parent.Children != null)
                        {
                            foreach (MediaProvider.MediaObject child in parent.Children)
                            {
                                //Testing with an actual feed item url with ampersands
                                //child.TargetUrl = "http://www2c......[domain]...../podcasts/download.asp?af=h&f=8638222";

                                var origTargetUrl = child.TargetUrl;
                                //TODO add the metrics stuff
                                child.TargetUrl = String.Format("{0}?contenttitle={1}&channel={2}&urlreferrer={3}&language={6}&url={4}&documenttitle={1}&hostname={5}"
                                    , ConfigurationManager.AppSettings["METRICS_PAGE"]
                                    , HttpUtility.UrlEncode(child.Title)
                                    , HttpUtility.UrlEncode("RSS: Click Through") //per Cass: when a user clicks a link from an RSS feed
                                    , HttpContext.Current.Request.Url.AbsoluteUri //HttpContext.Current.Request.UrlReferrer == null ? "" : HttpUtility.UrlEncode(HttpContext.Current.Request.UrlReferrer.ToString())
                                    , HttpUtility.UrlEncode(origTargetUrl)
                                    , HttpContext.Current.Request.Url.Host
                                    , "eng");  //not sure how to get the language
                            }
                        }
                    }
                }
            }

            return theObject;
        }

        private void AddFeedItemImageDetailsRss(List<MediaProvider.MediaObject> mediaObjs)
        {
            string feedImageString = "";
            if (mediaObjs.Count == 1)
            {
                //is there an alternate image
                if (mediaObjs[0].Children != null)
                {
                    foreach (MediaProvider.MediaObject child in mediaObjs[0].Children)
                    {
                        //If the child has a mediaTypeSpecificDetail that is a feedDetail, get the mediaImage inside it
                        if (child.MediaTypeSpecificDetail != null && child.MediaTypeSpecificDetail is MediaProvider.FeedDetailObject)
                        {
                            MediaProvider.FeedDetailObject feedDetail = (MediaProvider.FeedDetailObject)child.MediaTypeSpecificDetail;
                            if (String.IsNullOrEmpty(feedDetail.AssociatedImage.SourceUrl))
                            {
                                continue;
                            }

                            feedImageString = "<img src=\"" + feedDetail.AssociatedImage.SourceUrl + "\" height=\"" + feedDetail.AssociatedImage.Height + "\" width=\"" + feedDetail.AssociatedImage.Width + "\" />";

                            XmlDocument xdoc = new XmlDocument();
                            xdoc.LoadXml(strReturn);

                            //now we have an xml document.  Get the description
                            if (xdoc.HasChildNodes)
                            {
                                XmlNode rssNode = xdoc.SelectSingleNode("rss");
                                if (rssNode != null)
                                {
                                    XmlNode channelNode = rssNode.SelectSingleNode("channel");

                                    if (channelNode != null)
                                    {
                                        //find the child node
                                        XmlNodeList nl = channelNode.SelectNodes("item");
                                        //match the link
                                        foreach (XmlNode node in nl)
                                        {
                                            XmlNode linkNode = node.SelectSingleNode("link");
                                            if (linkNode != null)
                                            {
                                                if (linkNode.InnerText.Equals(child.SourceUrl) || linkNode.InnerText.Equals(child.SourceUrl + @"/"))
                                                {
                                                    //this is the node
                                                    XmlNode descNode = node.SelectSingleNode("description");
                                                    var cdata = xdoc.CreateCDataSection(feedImageString);

                                                    cdata.InnerText = "\r\n" + feedImageString;
                                                    descNode.AppendChild(cdata);
                                                    //descNode.InnerText += feedImageString;


                                                    //the item should have media:contenct
                                                    XmlNode mediaContentNode = xdoc.CreateElement("media", "content", "media:content");

                                                    XmlAttribute urlAttribute = xdoc.CreateAttribute("url");
                                                    urlAttribute.Value = feedDetail.AssociatedImage.SourceUrl;
                                                    mediaContentNode.Attributes.Append(urlAttribute);

                                                    XmlAttribute typeAttribute = xdoc.CreateAttribute("type");
                                                    typeAttribute.Value = "image/jpeg";
                                                    mediaContentNode.Attributes.Append(typeAttribute);

                                                    XmlAttribute heightAttribute = xdoc.CreateAttribute("height");
                                                    heightAttribute.Value = feedDetail.AssociatedImage.Height.ToString();
                                                    mediaContentNode.Attributes.Append(heightAttribute);

                                                    XmlAttribute widthAttribute = xdoc.CreateAttribute("width");
                                                    widthAttribute.Value = feedDetail.AssociatedImage.Width.ToString();
                                                    mediaContentNode.Attributes.Append(widthAttribute);


                                                    node.AppendChild(mediaContentNode);

                                                    strReturn = xdoc.InnerXml;
                                                    break;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddFeedItemImageDetailsAtom(List<MediaProvider.MediaObject> mediaObjs)
        {
            string feedImageString = "";
            if (mediaObjs.Count == 1)
            {
                //is there an alternate image
                if (mediaObjs[0].Children != null)
                {
                    foreach (MediaProvider.MediaObject child in mediaObjs[0].Children)
                    {
                        //If the child has a mediaTypeSpecificDetail that is a feedDetail, get the mediaImage inside it
                        if (child.MediaTypeSpecificDetail != null && child.MediaTypeSpecificDetail is MediaProvider.FeedDetailObject)
                        {
                            MediaProvider.FeedDetailObject feedDetail = (MediaProvider.FeedDetailObject)child.MediaTypeSpecificDetail;
                            if (String.IsNullOrEmpty(feedDetail.AssociatedImage.SourceUrl))
                            {
                                continue;
                            }

                            feedImageString = "<img src=\"" + feedDetail.AssociatedImage.SourceUrl + "\" height=\"" + feedDetail.AssociatedImage.Height + "\" width=\"" + feedDetail.AssociatedImage.Width + "\" />";

                            XmlDocument xdoc = new XmlDocument();
                            xdoc.LoadXml(strReturn);

                            //now we have an xml document.  Get the description
                            if (xdoc.HasChildNodes)
                            {
                                XmlNodeList nl = xdoc.GetElementsByTagName("entry");
                                foreach (XmlNode node in nl)
                                {
                                    //get the id
                                    XmlNodeList nlId = node.ChildNodes;
                                    bool foundTheNode = false;
                                    XmlNode theNode = null;

                                    foreach (XmlNode childNode in nlId)
                                    {
                                        if (childNode.InnerText.Equals(child.SourceUrl) || childNode.InnerText.Equals(child.SourceUrl + @"/"))
                                        {
                                            foundTheNode = true;
                                            theNode = childNode;
                                            break;
                                        }
                                    }
                                    if (!foundTheNode || theNode == null)
                                    {
                                        continue;
                                    }

                                    //loop through to find the summary
                                    XmlNodeList summaryNodes = node.ChildNodes;
                                    foreach (XmlElement curNode in summaryNodes)
                                    {
                                        if (curNode.Name.Equals("summary"))
                                        {
                                            ////this is the node
                                            //var cdata = xdoc.CreateCDataSection(feedImageString);

                                            ////cdata.InnerText = feedImageString;
                                            //curNode.AppendChild(cdata);


                                            XmlAttribute summaryTypeAttribute = xdoc.CreateAttribute("type");
                                            summaryTypeAttribute.Value = "xhtml";
                                            curNode.Attributes.Append(summaryTypeAttribute);

                                            //add a div
                                            curNode.InnerText += "<div xmlns=\"http://www.w3.org/1999/xhtml\">";
                                            curNode.InnerText += feedImageString;
                                            curNode.InnerText += "</div>";

                                            strReturn = xdoc.InnerXml;
                                            break;
                                        }
                                    }

                                    ////now for the media:content
                                    //XmlNode mediaContentNode = xdoc.CreateElement("media", "content", "media:content");

                                    //XmlAttribute urlAttribute = xdoc.CreateAttribute("url");
                                    //urlAttribute.Value = feedDetail.AssociatedImage.SourceUrl;
                                    //mediaContentNode.Attributes.Append(urlAttribute);

                                    //XmlAttribute typeAttribute = xdoc.CreateAttribute("type");
                                    //typeAttribute.Value = "image/jpeg";
                                    //mediaContentNode.Attributes.Append(typeAttribute);

                                    //XmlAttribute heightAttribute = xdoc.CreateAttribute("height");
                                    //heightAttribute.Value = feedDetail.AssociatedImage.Height.ToString();
                                    //mediaContentNode.Attributes.Append(heightAttribute);

                                    //XmlAttribute widthAttribute = xdoc.CreateAttribute("width");
                                    //widthAttribute.Value = feedDetail.AssociatedImage.Width.ToString();
                                    //mediaContentNode.Attributes.Append(widthAttribute);

                                    //XmlNode contentNode = xdoc.CreateNode(XmlNodeType.Element, "content", "");
                                    //var cdata2 = xdoc.CreateCDataSection(feedImageString);

                                    //contentNode.AppendChild(cdata2);

                                    //node.AppendChild(contentNode);

                                    //strReturn = xdoc.InnerXml;

                                }
                            }
                        }
                    }
                }
            }
        }

        private void WriteImageType(byte[] imageBuffer, bool IsWritingFromCache)
        {
            if (imageBuffer.Length > 0)
            {
                HttpContext.Current.Response.OutputStream.Write(imageBuffer, 0, imageBuffer.Length);

                if (IsWritingFromCache == false)
                {
                    ServiceUtility.AddApplicationCachePublicApi(imageBuffer, TheParser);
                }
            }
            else
            {
                HttpContext.Current.Response.Write(" ");
            }
        }

        private static bool IsCustomSerialize()
        {
            bool filterSerialize = false;
            try
            {
                if (HttpContext.Current.Request.QueryString[Param.FIELDS] != null && HttpContext.Current.Request.QueryString[Param.FIELDS].Any())
                {
                    filterSerialize = true;
                }
            }
            catch (Exception)
            {
            }

            return filterSerialize;
        }

        public void AddHeader(string header, string value)
        {
            WebOperationContext.Current.OutgoingResponse.Headers.Add(header, value);
        }

        public void WriteFromCache(object cacheObject)
        {
            WriteWithFormat(cacheObject, isWritingFromCache: true);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int ImpersonateLoggedOnUser(
            IntPtr hToken
        );

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern int RevertToSelf();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
        const int LOGON32_PROVIDER_DEFAULT = 0;

        public void WriteToFile(object theObject, FeedExportObject feedExport)
        {
            SetBeforeWrite(theObject, messages: null);

            switch (TheParser.Query.Format)
            {
                case FormatType.facebook:
                    HttpContext.Current.Response.ContentType = "application/xml; charset=UTF-8";
                    strReturn = new FacebookFeedExporter(((SerialResponse)theObject).mediaObjects.ToList(), feedExport).Generate();

                    break;
                case FormatType.twitter:
                    HttpContext.Current.Response.ContentType = "application/xml; charset=UTF-8";
                    strReturn = new TwitterFeedExporter(((SerialResponse)theObject).mediaObjects.ToList(), feedExport).Generate();

                    break;
                case FormatType.outbreaks:
                    HttpContext.Current.Response.ContentType = "application/xml; charset=UTF-8";
                    strReturn = new OutbreaksFeedExporter(((SerialResponse)theObject).mediaObjects.ToList(), feedExport).Generate();

                    break;
                case FormatType.generic:
                    HttpContext.Current.Response.ContentType = "application/xml; charset=UTF-8";
                    strReturn = new GenericExporter(((SerialResponse)theObject).mediaObjects.ToList(), feedExport).Generate();

                    break;
                default:
                    GenerateOutput(theObject, isWritingFromCache: false);
                    break;
            }

            // impersonate the ContentServiceFeedExport user account
            IntPtr lnToken;
            string domain = ".";
            string username = ConfigurationManager.AppSettings["AdminFeedExportUsername"];
            if (username.IndexOf("\\") > -1)
            {
                domain = username.Substring(0, username.IndexOf("\\"));
                username = username.Substring(username.LastIndexOf("\\") + 1);
            }

            int TResult = LogonUser(username, domain, ConfigurationManager.AppSettings["AdminFeedExportPassword"], LOGON32_LOGON_NEW_CREDENTIALS, LOGON32_PROVIDER_DEFAULT, out lnToken);
            if (TResult > 0)
            {
                try
                {
                    ImpersonateLoggedOnUser(lnToken);
                    WriteFileUnc(feedExport.FilePath, this.strReturn);
                }
                // Prevent exceptions from propagating
                catch (Exception ex)
                {
                    Logging.Logger.LogError(ex.ToString());
                    throw ex;
                }
                finally
                {
                    // Revert impersonation
                    RevertToSelf();
                    CloseHandle(lnToken);

                    // file saved without any errors
                }
            }
            else
            {
                Logging.Logger.LogError("Could not authenticate user using forms authentication. Result = " + TResult + ", Processed User Name = " + username + ", Domain = " + domain);
                throw new Exception("Could not authenticate user using forms authentication.");
            }

        }

        private void WriteFileUnc(string filePath, string formattedOutput)
        {
            int retrycount = 0;

            while (retrycount < 4)
            {
                try
                {
                    FileStream fs = null;
                    FileInfo fi = new FileInfo(filePath);
                    if (fi.Exists)
                    {
                        fs = fi.Open(FileMode.Truncate);
                    }
                    else
                    {
                        fs = fi.Create();
                    }
                    byte[] bytes = UTF8Encoding.UTF8.GetBytes(formattedOutput);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                    fs.Close();

                    return;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    Logging.Logger.LogError("Error while trying to write file: " + filePath + ", Processed User Name = " + ConfigurationManager.AppSettings["AdminFeedExportUsername"] + ", " + ex.ToString());

                    System.Threading.Thread.Sleep(1500);
                    retrycount++;
                }
            }
        }


    }
}
