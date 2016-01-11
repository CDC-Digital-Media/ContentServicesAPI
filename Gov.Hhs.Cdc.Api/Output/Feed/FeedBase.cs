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
using System.Xml;
using System.Xml.Linq;

using System.Configuration;
using System.Web;

using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using System.ServiceModel.Syndication;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;

using Gov.Hhs.Cdc.CsBusinessObjects.Media;
using Gov.Hhs.Cdc.CdcMediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public abstract class FeedBase : IFeedBase
    {
        public abstract string Generate();

        private List<SyndicationItem> Items { get; set; }

        protected IEnumerable<MediaObject> MediaItems { get; set; }

        protected SyndicationFeed Feed { get; set; }
        protected List<MediaObject> Medias { get; set; }
        protected MediaObject Media { get; set; }
        protected ICallParser Parser { get; set; }

        protected string strReturn { get; set; }

        public FeedBase(List<MediaObject> medias, ICallParser parser)
        {
            this.Medias = medias;
            this.Media = medias.FirstOrDefault();

            this.Parser = parser;
            this.Feed = new SyndicationFeed();

            InitializeMediaItems();
        }

        private void InitializeMediaItems()
        {
            if (this.Media.Children != null)
            {
                this.MediaItems = this.Media.Children;

                if (Parser.Query.ItemOffset != null && Parser.Query.ItemOffset > 0)
                    this.MediaItems = this.MediaItems.Skip(int.Parse(Parser.Query.ItemOffset.ToString()));

                if (Parser.Query.ItemCount != null && Parser.Query.ItemCount > 0)
                    this.MediaItems = this.MediaItems.Take(int.Parse(Parser.Query.ItemCount.ToString()));
            }
        }

        #region "Generate Feed"

        protected void GetManagedFeed()
        {
            var output = new StringWriter();
            var writer = new XmlTextWriter(output);

            switch (this.Parser.Query.Format)
            {
                case FormatType.rss:
                    GenerateSyndicationFeed();
                    new Rss20FeedFormatter(this.Feed, false).WriteTo(writer);
                    this.strReturn += XElement.Parse(output.ToString()).ToString();
                    break;
                case FormatType.atom:
                    GenerateSyndicationFeed();
                    new Atom10FeedFormatter(this.Feed).WriteTo(writer);
                    this.strReturn += XElement.Parse(output.ToString()).ToString();
                    break;
            }

            writer.Close();
        }

        private void GenerateSyndicationFeed()
        {
            this.strReturn = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>";
            BuildFeed();
            BuildFeedItem();
        }

        #endregion


        #region build feed

        protected void BuildFeed()
        {
            this.Media = this.Medias.FirstOrDefault();

            this.Feed.Id = this.Media.Id.ToString();
            this.Feed.Title = new TextSyndicationContent(this.Media.Title);
            this.Feed.Description = new TextSyndicationContent(this.Media.Description);

            this.Feed.Language = this.Media.LangItem.ISOLanguageCode2;
            this.Feed.LastUpdatedTime = new DateTimeOffset((DateTime)this.Media.ModifiedDateTime);

            // categories
            BuildFeedCategories();

            // links
            BuildFeedLinks();

            var feed = this.Media.MediaTypeSpecificDetail as FeedDetailObject;
            if (feed != null)
            {
                if (this.Media.HasFeedEditorData)
                {
                    this.Feed.Copyright = new TextSyndicationContent(feed.Copyright);

                    if (this.Parser.Query.Format == FormatType.rss)
                        this.Feed.ElementExtensions.Add(new XElement("webMaster", feed.WebMasterEmail + " (" + feed.EditorialManager + ")").CreateReader());
                    else if (this.Parser.Query.Format == FormatType.atom)
                        this.Feed.Authors.Add(new SyndicationPerson(feed.WebMasterEmail, feed.EditorialManager, ""));
                }

                // imageUrl
                if (this.Media.HasFeedImage)
                {
                    BuildFeedImageMedia(feed);
                }
            }
        }

        private void BuildFeedCategories()
        {
            foreach (var item in this.Media.AttributeValues.ToList())
            {
                this.Feed.Categories.Add(new SyndicationCategory(item.ValueName, "", item.AttributeName));
            }
        }

        private void BuildFeedLinks()
        {
            if (!string.IsNullOrEmpty(this.Media.TargetUrl))
            {
                if (this.Parser.Query.Format == FormatType.rss)
                {
                    this.Feed.ElementExtensions.Add(new XElement("link", new Uri(this.Media.TargetUrl)).CreateReader());
                }
                else if (this.Parser.Query.Format == FormatType.atom)
                {
                    this.Feed.Links.Add(new SyndicationLink(new Uri(this.Media.TargetUrl), "self", "", "application/rss+xml", 0));
                    this.Feed.Links.Add(new SyndicationLink(new Uri(this.Media.TargetUrl), "alternate", "", "text/html", 0));
                }
            }
        }

        private void BuildFeedImageMedia(FeedDetailObject feed)
        {
            if (this.Parser.Query.Format == FormatType.rss)
                this.Feed.ElementExtensions.Add(new XElement("image",
                    new XElement("url", null, feed.AssociatedImage.SourceUrl),
                    new XElement("title", null, feed.AssociatedImage.Title),
                    new XElement("link", null, feed.AssociatedImage.TargetUrl),
                    new XElement("width", null, feed.AssociatedImage.Width == null ? "0" : feed.AssociatedImage.Width.ToString()),
                    new XElement("height", null, feed.AssociatedImage.Height == null ? "0" : feed.AssociatedImage.Height.ToString())).CreateReader());
            else if (this.Parser.Query.Format == FormatType.atom)
            {
                if (!string.IsNullOrEmpty(feed.AssociatedImage.SourceUrl))
                {
                    this.Feed.ElementExtensions.Add(new XElement("logo", new Uri(feed.AssociatedImage.SourceUrl)).CreateReader());
                }
            }
        }

        #endregion

        #region build feed item

        protected void BuildFeedItem()
        {
            this.Items = new List<SyndicationItem>();

            if (this.MediaItems != null)
            {
                var feedItems = new List<MediaObject>();

                if (this.Parser.IsPublicFacing)
                    feedItems = this.MediaItems.ToList();
                else
                    feedItems = GetChildItemFromXmlSearch(status: "Published");
                //feedItems = this.Media.Children.Where(c => c.MediaStatusCode == "Published").ToList();

                foreach (var mediaItem in feedItems.Where(a => !a.MediaTypeParms.IsFeedImage).ToList())
                {
                    var feedItem = new SyndicationItem();
                    feedItem.Id = mediaItem.Id.ToString();
                    feedItem.Title = new TextSyndicationContent(mediaItem.Title ?? "");
                    feedItem.Summary = new TextSyndicationContent(mediaItem.Description ?? "");

                    if (mediaItem.PublishedDateTime != null)
                        feedItem.PublishDate = new DateTimeOffset((DateTime)mediaItem.PublishedDateTime);

                    if (mediaItem.ModifiedDateTime != null)
                        feedItem.LastUpdatedTime = new DateTimeOffset((DateTime)mediaItem.ModifiedDateTime);

                    // add item level categories
                    foreach (var vocabCategory in mediaItem.AttributeValues.ToList())
                    {
                        feedItem.Categories.Add(new SyndicationCategory(vocabCategory.ValueName, "", vocabCategory.AttributeName));
                    }

                    if (!string.IsNullOrEmpty(mediaItem.TargetUrl))
                    {
                        feedItem.AddPermalink(new Uri(mediaItem.TargetUrl));
                        feedItem.Links.Add(new SyndicationLink(new Uri(mediaItem.TargetUrl)));
                    }

                    // add enclosure
                    if (mediaItem.Enclosures != null)
                    {
                        if (this.Parser.Query.Format == FormatType.rss)
                        {
                            foreach (var enclosureItem in mediaItem.Enclosures.ToList())
                            {
                                var enclosure = new XElement("enclosure");
                                enclosure.SetAttributeValue("url", enclosureItem.ResourceUrl);
                                enclosure.SetAttributeValue("length", enclosureItem.Size);
                                enclosure.SetAttributeValue("type", enclosureItem.ContentType);

                                feedItem.ElementExtensions.Add(enclosure);
                            }
                        }
                        else if (this.Parser.Query.Format == FormatType.atom)
                        {
                            foreach (var enclosureItem in mediaItem.Enclosures.ToList())
                            {
                                var url = new Uri(enclosureItem.ResourceUrl);
                                feedItem.Links.Add(new SyndicationLink(url, "enclosure", "", enclosureItem.ContentType, enclosureItem.Size));
                            }
                        }
                    }

                    FeedDetailObject feedDetail = (FeedDetailObject)mediaItem.MediaTypeSpecificDetail;

                    if (feedDetail.AssociatedImage != null && !String.IsNullOrEmpty(feedDetail.AssociatedImage.SourceUrl))
                    {
                        if (this.Parser.Query.Format == FormatType.rss)
                        {
                            var image = new XElement("image");
                            image.SetAttributeValue("url", feedDetail.AssociatedImage.SourceUrl);
                            image.SetAttributeValue("length", 0);
                            image.SetAttributeValue("type", "image/jpeg");

                            feedItem.ElementExtensions.Add(image);
                        }
                        else if (this.Parser.Query.Format == FormatType.atom)
                        {
                            var url = new Uri(feedDetail.AssociatedImage.SourceUrl);
                            feedItem.Links.Add(new SyndicationLink(url, "media:thumbnail", "", "image/jpeg", 0));
                        }
                    }

                    this.Items.Add(feedItem);
                }
            }

            this.Feed.Items = this.Items;
        }

        private List<MediaObject> GetChildItemFromXmlSearch(string status)
        {
            this.Parser.Criteria2.ParentId = this.Media.MediaId.ToString();
            this.Parser.Criteria2.Status = status;

            this.MediaItems = this.Media.Children;

            if (Parser.Query.ItemOffset != null && Parser.Query.ItemOffset > 0)
                this.Parser.Criteria2.PageOffset = Parser.Query.ItemOffset.ToString();

            if (Parser.Query.ItemCount != null && Parser.Query.ItemCount > 0)
                this.Parser.Criteria2.RowsPerPage = Parser.Query.ItemCount.ToString();

            IEnumerable<MediaObject> mediaObjects = CsMediaSearchProvider.Search(Parser.Criteria2);

            return mediaObjects.ToList();
        }

        #endregion

        protected static string PerformWebRequest(HttpWebRequest request)
        {
            string result = null;
            try
            {
                request.Accept = "*/*";
                request.Timeout = int.Parse(ConfigurationManager.AppSettings["EXTERNAL_API_TIMEOUT"]);

                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode >= HttpStatusCode.BadRequest)
                {
                    throw new Exception("The URL is invalid and resulted in a bad request");
                }

                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    result = reader.ReadToEnd();
                }

                HttpContext.Current.Response.ContentType = response.ContentType; // "application/xml; charset=UTF-8";
                return result;
            }
            catch (UriFormatException uex)
            {
                throw uex;  // return "The URL not properly formatted: " + url;
            }
            catch (WebException ex)
            {
                throw ex;   // return "The URL is invalid: " + url;
            }
            catch (Exception ex)
            {
                throw ex;   // return "Exception while requesting url: " + url;
            }
        }

        //for testing purpose only, accept any dodgy certificate... 
        protected static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
