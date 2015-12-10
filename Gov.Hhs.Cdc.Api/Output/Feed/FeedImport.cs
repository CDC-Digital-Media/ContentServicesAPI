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
using System.Net;
using System.Xml;

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using System.ServiceModel.Syndication;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;

using Gov.Hhs.Cdc.CsBusinessObjects.Media;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;

namespace Gov.Hhs.Cdc.Api
{
    public class FeedImport : FeedBase
    {
        private static MediaObject theMediaObject = null;

        public FeedImport(List<MediaObject> mediaList, ICallParser parser)
            : base(mediaList, parser)
        {
        }

        public override string Generate()
        {
            GetManagedFeed();
            return this.strReturn;
        }

        private static bool ParseStringForUrlAndCheckValidity(string input, string pattern)
        {
            bool results = true;

            string[] m = System.Text.RegularExpressions.Regex.Split(input, pattern);
            for (int i = 0; i < m.Length; i++)
            { 
                if (i % 2 == 0)
                {
                    System.Text.RegularExpressions.Match urlMatch = System.Text.RegularExpressions.Regex.Match(m[i], "^(?<url>[a-zA-Z0-9/?=&.]+)", System.Text.RegularExpressions.RegexOptions.Singleline);
                    if (urlMatch.Success) { 
                        //check if valid
                        results = false;
                    }
                }
            }

            return results;
        }

        public static MediaObject CreateYouTubeFeedMedia(MediaObject mediaObjectIn, AdminUser adminUser)
        {
            theMediaObject = mediaObjectIn;
            if (theMediaObject == null)
            {
                return theMediaObject;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(theMediaObject.SourceUrl);
            //Youtube is json

            string responseString = PerformWebRequest(request);

            var responseObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);


            MediaObject feedMedia = mediaObjectIn;

            if(responseObject != null && responseObject["items"] != null)
            {
                SyndicationFeed synFeed = new SyndicationFeed();
                List<SyndicationItem> children = new List<SyndicationItem>();

                foreach (Newtonsoft.Json.Linq.JObject jobject in responseObject["items"])
                {
                    if (jobject.GetValue("id")["videoId"] == null)
                    {
                        continue;
                    }
                    SyndicationItem child = new SyndicationItem();
                    child.Title = new TextSyndicationContent(jobject.GetValue("snippet")["title"].ToString());
                    child.Summary = new TextSyndicationContent(jobject.GetValue("snippet")["description"].ToString());
                    //child.Links.Add(new SyndicationLink(new Uri(String.Format("https://www.youtube.com/watch?v={0}", jobject.GetValue("id")["videoId"].ToString()))));
                    
                    if(Convert.ToDateTime(jobject.GetValue("snippet")["publishedAt"].ToString()) <= DateTime.MinValue 
                        || Convert.ToDateTime(jobject.GetValue("snippet")["publishedAt"].ToString()) >= DateTime.MaxValue)
                    {
                        child.LastUpdatedTime = DateTime.UtcNow;
                    }
                    else
                    {
                        child.LastUpdatedTime = Convert.ToDateTime(jobject.GetValue("snippet")["publishedAt"].ToString());
                    }

                    child.BaseUri = new Uri(String.Format("https://www.youtube.com/watch?v={0}", jobject.GetValue("id")["videoId"].ToString()));
                    child.Links.Add(new SyndicationLink(new Uri(String.Format("https://www.youtube.com/watch?v={0}", jobject.GetValue("id")["videoId"].ToString()))));

                    //now for enclosures
                    if (jobject["snippet"]["thumbnails"] != null)
                    {
                        //jobject["snippet"]["thumbnails"]["default"]["url"].ToString()
                        if (jobject["snippet"]["thumbnails"]["default"] != null)
                        {
                            if (jobject["snippet"]["thumbnails"]["default"]["url"] != null)
                            {
                                SyndicationLink enclosure = new SyndicationLink();
                                enclosure.RelationshipType = "enclosure";
                                enclosure.MediaType = "image/jpeg";
                                enclosure.Uri = new Uri(jobject["snippet"]["thumbnails"]["default"]["url"].ToString());
                                child.Links.Add(enclosure);
                            }
                        }
                        if (jobject["snippet"]["thumbnails"]["medium"] != null)
                        {
                            if (jobject["snippet"]["thumbnails"]["medium"]["url"] != null)
                            {
                                SyndicationLink enclosure = new SyndicationLink();
                                enclosure.RelationshipType = "enclosure";
                                enclosure.MediaType = "image/jpeg";
                                enclosure.Uri = new Uri(jobject["snippet"]["thumbnails"]["medium"]["url"].ToString());
                                child.Links.Add(enclosure);
                            }
                        }
                        if (jobject["snippet"]["thumbnails"]["high"] != null)
                        {
                            if (jobject["snippet"]["thumbnails"]["high"]["url"] != null)
                            {
                                SyndicationLink enclosure = new SyndicationLink();
                                enclosure.RelationshipType = "enclosure";
                                enclosure.MediaType = "image/jpeg";
                                enclosure.Uri = new Uri(jobject["snippet"]["thumbnails"]["high"]["url"].ToString());
                                child.Links.Add(enclosure);
                            }
                        }                      
                    }
                    children.Add(child);
                    synFeed.Items = children;

                }
                feedMedia.Children= CreateMediaChildren(synFeed, adminUser);
            }


            return feedMedia;
        }

        public static MediaObject CreateMedia(MediaObject mediaObjectIn, AdminUser adminUser)
        {
            theMediaObject = mediaObjectIn;
            if (theMediaObject == null)
            {
                return theMediaObject;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(theMediaObject.SourceUrl);

            string responseString = PerformWebRequest(request);

            if (responseString.Substring(0, 6) != "<?xml ")
            {
                responseString = "<?xml version=\"1.0\"?>" + responseString;
            }

            // Get the XML as a string so we can fix up the data.
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(responseString);
            

            XmlNodeList xnl = doc.GetElementsByTagName("pubDate");
            if (xnl != null && xnl.Count > 0)
            {
                DateTime dt = new DateTime();
                for (int i = 0; i < xnl.Count; i++)
                {
                    dt = DateTime.Now;
                    string unformattedDateTime = xnl[i].InnerText;
                    if (unformattedDateTime.IndexOf(",") > 0)
                    {
                        unformattedDateTime = unformattedDateTime.Substring(unformattedDateTime.IndexOf(",") + 1);
                    }
                    if (!String.IsNullOrEmpty(unformattedDateTime)) {
                        if (DateTime.TryParse(unformattedDateTime.Substring(0, unformattedDateTime.Length - 5), out dt))
                        {
                            xnl[i].InnerText = dt.ToString("ddd, dd MMM yyyy hh:mm:ss") + " EDT";
                        }
                        else
                        {
                            xnl[i].InnerText = dt.ToString("ddd, dd MMM yyyy hh:mm:ss zzz");
                        }
                    }
                    else
                    {
                        xnl[i].InnerText = DateTime.Now.ToString("ddd, dd MMM yyyy hh:mm:ss") + " EDT";
                    }
                }
            }

            responseString = doc.OuterXml;

            //MAR 9/14/2015 - Some of the featured podcast feed returns a bad url, so I added this to clean it up
            //xml = xml.Replace(@"//://", @"//");
            List<int> nodesToRemove = new List<int>();

            xnl = doc.GetElementsByTagName("entry");

            for (int i = 0; i < xnl.Count; i++)
            {
                XmlNode node = xnl[i];
                //validate the url

                //get the child nodes
                foreach(XmlNode childNode in node.ChildNodes)
                {
                    string urlToTest;

                    if (childNode.Attributes["href"] != null)
                    {
                        urlToTest = childNode.Attributes["href"].Value;
                        if (!string.IsNullOrEmpty(urlToTest))
                        {
                            if (!Uri.IsWellFormedUriString(urlToTest, UriKind.Absolute))
                            {
                                nodesToRemove.Add(i);
                                break;
                            }
                        }
                    } else {
                        continue;
                    }

                }
            }

            //MAR 9/18/2015 - validate bad urls here.  http://://OneandOnlyCampaign.org will cause it to kick the whole feed out
            //It is in the link: <link>http://://OneandOnlyCampaign.org</link>
            xnl = doc.GetElementsByTagName("link");
            for (int i=0;i<xnl.Count;i++)
            {
                XmlNode node = xnl[i];
                if (node.InnerText != null)
                {
                    if(node.InnerText.ToLower().Contains("http:")||node.InnerText.ToLower().Contains("https:"))
                    {
                        //it has a link, so validate it
                        if (!Uri.IsWellFormedUriString(node.InnerText.ToLower(),UriKind.Absolute))
                        {
                            //get rid of this entry
                            nodesToRemove.Add(i);
                        }
                    }
                }
            }

            int removeCounter = 0;
            foreach (int i in nodesToRemove)
            {
                xnl[i - removeCounter].ParentNode.RemoveChild(xnl[i - removeCounter]);
                removeCounter++;
            }
            responseString = doc.OuterXml;

            //xml = xml.Replace(@"http://://OneandOnlyCampaign.org", @"http://OneandOnlyCampaign.org");



            TextReader tr = new StringReader(responseString);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            XmlReader nr = XmlReader.Create(tr, settings);

            // Create SyndicationFeed object from the cleaned up XML.
            SyndicationFeed syndFeed = null;
            if (nr != null && !nr.EOF)
            {
                syndFeed = SyndicationFeed.Load(nr);
            }

            MediaObject feedMedia = null;
            // If we've got a feed and it has items then process it.
            if (syndFeed != null && syndFeed.Items != null)
            {
                feedMedia = CreateMedia(syndFeed);
                if (feedMedia != null)
                {
                    feedMedia.Children = CreateMediaChildren(syndFeed, adminUser);
                }
            }
            else
            {
                throw new Exception("Could not parse the feed: " + theMediaObject.SourceUrl);
            }

            //now for thumbnails
            if(feedMedia.Children.Count > 0)
            {
                xnl = doc.GetElementsByTagName("yt:videoId");
                foreach (XmlNode node in xnl)
                {
                    //Find the corresponding child
                    if (node.ParentNode == null)
                    {
                        continue;
                    }

                    //get the title for the node
                    if (node.ParentNode["title"] == null)
                    {
                        continue;
                    }
                    string nodeTitle = node.ParentNode["title"].InnerText;

                    //search the children of the mediaObject for the title
                    List<MediaObject> kids = feedMedia.Children.Where(c => c.Title == nodeTitle).ToList();
                    foreach (MediaObject mok in kids)
                    {
                        List<Gov.Hhs.Cdc.CsBusinessObjects.Media.ExtendedAttribute> extAttrs = new List<CsBusinessObjects.Media.ExtendedAttribute>();
                        if (mok.ExtendedAttributes != null)
                        {
                            extAttrs = (List<Gov.Hhs.Cdc.CsBusinessObjects.Media.ExtendedAttribute>)mok.ExtendedAttributes;
                        }
                        Gov.Hhs.Cdc.CsBusinessObjects.Media.ExtendedAttribute atr = new CsBusinessObjects.Media.ExtendedAttribute();
                        atr.Name = "yt:videoId";
                        atr.Value = node.InnerText;
                        extAttrs.Add(atr);
                        mok.ExtendedAttributes = extAttrs;
                    }
                }

                xnl = doc.GetElementsByTagName("media:title");
                foreach (XmlNode node in xnl)
                {
                    string nodeTitle = node.InnerText;
                    List<MediaObject> kids = feedMedia.Children.Where(c => c.Title == nodeTitle).ToList();

                    if(node.ParentNode["media:description"] != null)
                    {
                        //search the children of the mediaObject for the title
                        foreach (MediaObject mok in kids)
                        {
                            mok.Description = node.ParentNode["media:description"].InnerText;
                        }
                    }

                    if (node.ParentNode["media:thumbnail"] != null)
                    {
                        //search the children of the mediaObject for the title
                        foreach (MediaObject mok in kids)
                        {
                            //Create a new enclosure
                            EnclosureObject encObject = new EnclosureObject();
                            if (node.ParentNode["media:thumbnail"].Attributes["url"] == null)
                            {
                                continue;
                            }
                            string thumbnailString = node.ParentNode["media:thumbnail"].Attributes["url"].Value;

                            if (!Uri.IsWellFormedUriString(thumbnailString, UriKind.Absolute))
                            {
                                continue;
                            }

                            encObject.ResourceUrl = thumbnailString;
                            encObject.ContentType = "image/jpeg"; //set a default for now, will overwrite later
                            encObject.Size = 0; //set a default for now, will overwrite later

                            try
                            {
                                //get the file size and content type
                                WebRequest encGetter = WebRequest.Create(thumbnailString);
                                encGetter.Method = "HEAD";
                                int contentLength = 0;
                                WebResponse encResponse = encGetter.GetResponse();
                                int.TryParse(encResponse.Headers.Get("Content-Length"), out contentLength);
                                encObject.Size = contentLength;
                                encObject.ContentType = encResponse.Headers.Get("Content-Type");
                            }
                            catch (Exception ex)
                            {
                                Gov.Hhs.Cdc.Logging.Logger.LogError(ex, "Error occurred in FeedImport.CreateMedia while trying to add thumbnail to enclosures");
                            }
                            ((List<EnclosureObject>)mok.Enclosures).Add(encObject);

                        }
                    }
                }

                if(theMediaObject.SourceUrl.ToLower().Contains("websta.me"))
                {
                    //instagram
                    xnl = doc.GetElementsByTagName("image");
                    //instagram comes in with a funny format, so this is to clean it up
                    foreach (XmlNode node in xnl)
                    {
                        //Find the corresponding child
                        if (node.ParentNode == null)
                        {
                            continue;
                        }

                        //get the title for the node
                        if (node.ParentNode["title"] == null)
                        {
                            continue;
                        }
                        string nodeTitle = node.ParentNode["title"].InnerText;

                        //search the children of the mediaObject for the title
                        List<MediaObject> kids = feedMedia.Children.Where(c => c.Title == nodeTitle).ToList();
                        foreach (MediaObject mok in kids)
                        {
                            //Create a new enclosure
                            EnclosureObject encObject = new EnclosureObject();
                            if (node["url"] == null)
                            {
                                continue;
                            }
                            string thumbnailString = node["url"].InnerText;

                            if (!Uri.IsWellFormedUriString(thumbnailString, UriKind.Absolute))
                            {
                                continue;
                            }

                            encObject.ResourceUrl = thumbnailString;
                            encObject.ContentType = "image/jpeg"; //set a default for now, will overwrite later
                            encObject.Size = 0; //set a default for now, will overwrite later

                            try
                            {
                                //get the file size and content type
                                WebRequest encGetter = WebRequest.Create(thumbnailString);
                                encGetter.Method = "HEAD";
                                int contentLength = 0;
                                WebResponse encResponse = encGetter.GetResponse();
                                int.TryParse(encResponse.Headers.Get("Content-Length"), out contentLength);
                                encObject.Size = contentLength;
                                encObject.ContentType = encResponse.Headers.Get("Content-Type");
                            }
                            catch (Exception ex)
                            {
                                Gov.Hhs.Cdc.Logging.Logger.LogError(ex, "Error occurred in FeedImport.CreateMedia while trying to add thumbnail to enclosures");
                            }
                            ((List<EnclosureObject>)mok.Enclosures).Add(encObject);

                        }
                    }
                }
            }

            return feedMedia;
        }

        private static MediaObject CreateMedia(SyndicationFeed syndFeed)
        {
            var media = new MediaObject();

            if (theMediaObject != null)
            {
                media.Id = theMediaObject.Id;
                media.SourceCode = theMediaObject.SourceCode;
                media.MediaTypeCode = theMediaObject.MediaTypeCode;
                media.MimeTypeCode = theMediaObject.MimeTypeCode;
                media.MediaStatusCode = theMediaObject.MediaStatusCode;
            }

            media.Title = syndFeed.Title.Text;
            media.Name = syndFeed.Title.Text;
            media.SubTitle = syndFeed.Title.Text;

            if (syndFeed.Description != null)
            {
                media.Description = syndFeed.Description.Text;
            }
            else
            {
                media.Description = syndFeed.Title.Text;
            }

            if (syndFeed.LastUpdatedTime.DateTime <= DateTime.MinValue || syndFeed.LastUpdatedTime.DateTime >= DateTime.MaxValue)
            {
                media.PublishedDateTime = DateTime.UtcNow;
            }
            else
            {
                media.PublishedDateTime = syndFeed.LastUpdatedTime.UtcDateTime;
            }

            return media;
        }

        private static List<MediaObject> CreateMediaChildren(SyndicationFeed syndFeed, AdminUser adminUser)
        {
            var mediaChildren = new List<MediaObject>();

            if (syndFeed.Items.Any())
            {
                foreach (var item in syndFeed.Items)
                {
                    if (item.Links != null || item.Links.Count > 0)
                    {
                        var media = new MediaObject();

                        media.Name = item.Title.Text;
                        media.Title = item.Title.Text;
                        media.SubTitle = item.Title.Text;

                        if (adminUser != null)
                        {
                            media.CreatedByGuid = adminUser.UserGuid;
                            media.ModifiedByGuid = adminUser.UserGuid;
                        }

                        // Properties from the parent object
                        if (theMediaObject != null)
                        {
                            media.MediaTypeCode = "Feed Item";
                            media.LanguageCode = theMediaObject.LanguageCode;
                            media.SourceCode = theMediaObject.SourceCode;
                            media.CharacterEncodingCode = theMediaObject.CharacterEncodingCode;
                            media.MimeTypeCode = theMediaObject.MimeTypeCode;
                            media.MaintainingBusinessUnitName = theMediaObject.MaintainingBusinessUnitName;
                            media.OwningBusinessUnitName = theMediaObject.OwningBusinessUnitName;
                            media.AttributeValues = theMediaObject.AttributeValues.Where(a => a.AttributeName != "ContentGroup");

                            // add relationship
                            media.MediaRelationships = new List<MediaRelationshipObject>()                    
                            {
                                new MediaRelationshipObject ()
                                {
                                    IsActive = true,
                                    IsCommitted = true,
                                    RelationshipTypeName = "Is Child Of",
                                    RelatedMediaId = theMediaObject.Id == 0 ? theMediaObject.MediaId : theMediaObject.Id
                                }
                            }.AsEnumerable();
                        }

                        string description = (item.Summary ?? item.Title).Text;
                        if (!string.IsNullOrEmpty(description))
                        {
                            media.Description = description;
                        }
                        else
                        {
                            media.Description = item.Title.Text;
                        }

                        media.SourceUrl = item.Links[0].Uri.ToString();
                        media.TargetUrl = item.Links[0].Uri.ToString();

                        if (item.PublishDate.DateTime <= DateTime.MinValue || item.PublishDate.DateTime >= DateTime.MaxValue)
                        {
                            if (item.LastUpdatedTime.DateTime > DateTime.MinValue && item.LastUpdatedTime.DateTime < DateTime.MaxValue)
                            {
                                media.PublishedDateTime = item.LastUpdatedTime.UtcDateTime;
                            }
                            else
                            {
                                media.PublishedDateTime = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            media.PublishedDateTime = item.PublishDate.UtcDateTime;
                        }

                        if (item.LastUpdatedTime.DateTime <= DateTime.MinValue || item.LastUpdatedTime.DateTime >= DateTime.MaxValue)
                        {
                            media.ModifiedDateTime = (DateTime)media.PublishedDateTime;
                        }
                        else
                        {
                            media.ModifiedDateTime = item.LastUpdatedTime.UtcDateTime;
                        }

                        //Now for enclosures
                        //I know there are Links in the item, so loop through each one and check the relationshiptype
                        List<EnclosureObject> enclosures = new List<EnclosureObject>();
                        foreach(SyndicationLink link in item.Links)
                        {
                            if(link.RelationshipType=="enclosure")
                            {
                                //Create a new enclosure
                                EnclosureObject encObject = new EnclosureObject();
                                encObject.ResourceUrl = link.Uri.AbsoluteUri;
                                encObject.ContentType = link.MediaType;

                                encObject.Size = (int)link.Length;

                                try
                                {
                                    if (encObject.Size <= 0)
                                    {
                                        //get the file size
                                        WebRequest encGetter = WebRequest.Create(link.Uri.AbsoluteUri);
                                        encGetter.Method = "HEAD";
                                        int contentLength = 0;
                                        WebResponse encResponse = encGetter.GetResponse();
                                        if (int.TryParse(encResponse.Headers.Get("Content-Length"), out contentLength))
                                        {
                                            encObject.Size = contentLength;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Gov.Hhs.Cdc.Logging.Logger.LogError(ex, "Error occurred in FeedImport.CreateMediaChildren while trying to get the enclosure size");
                                }
                                enclosures.Add(encObject);
                            }
                        }
                        media.Enclosures = enclosures;

                        mediaChildren.Add(media);
                    }
                    else
                    {
                        throw new Exception("Syndication item has no links specified.");
                    }
                }
            }

            return mediaChildren;
        }

    }
}
