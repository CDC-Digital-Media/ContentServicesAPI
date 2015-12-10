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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;

namespace Gov.Hhs.Cdc.Api
{
    public sealed class old_FeedFormatter
    {
        private static string _imageUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["RSSLogo"];
            }
        }

        private static string SyndicationMediaNamespace = "http://www......[domain]...../socialmedia/syndication/SyndicationMedia.xsd";

        public static string GenerateFeed(SerialResponse obj, string feedFormat)
        {
            string s = "";
            var output = new StringWriter();
            var writer = new XmlTextWriter(output);
            var feed = new SyndicationFeed();
            old_ParsedMediaList pml = new old_ParsedMediaList(obj);

            switch (feedFormat)
            {
                case "rdf":
                case "rss1":
                    s = GenerateRSS1Feed(pml);
                    break;
                case "rss":
                case "rss2":
                    feed = GenerateSyndicationFeed(pml);
                    new Rss20FeedFormatter(feed, false).WriteTo(writer);
                    s = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                    s += XElement.Parse(output.ToString()).ToString();
                    break;
                case "atom":
                    feed = GenerateSyndicationFeed(pml);
                    new Atom10FeedFormatter(feed).WriteTo(writer);
                    s = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                    s += XElement.Parse(output.ToString()).ToString();
                    break;
                case "itunes":
                    feed = GenerateItunesFeed(pml);
                    new Rss20FeedFormatter(feed).WriteTo(writer);
                    s = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                    s += XElement.Parse(output.ToString()).ToString();
                    break;
            }

            writer.Close();

            return s;
        }

        #region regular syndication feed (atom, rss2.0)

        private static SyndicationFeed GenerateSyndicationFeed(old_ParsedMediaList pml)
        {
            SyndicationFeed feed = new SyndicationFeed();
            PopulateRootElementsForFeed(feed, pml.RootElements);

            List<SyndicationItem> items = new List<SyndicationItem>();

            pml.MediaItems.ForEach(mi =>
            {
                SyndicationItem item = new SyndicationItem();

                // populate common root element
                PopulateRootElementsForItem(item, mi);

                // add syndication media block
                //XElement smbXML = XElement.Parse(SyndicationMediaBlock(mi));
                //item.ElementExtensions.Add(smbXML);

                items.Add(item);
            });

            feed.Items = items;

            return feed;
        }

        #endregion

        #region RSS1.0/RDF Output

        private static string GenerateRSS1Feed(old_ParsedMediaList pml)
        {
            var output = new StringWriter();
            var writer = new XmlTextWriter(output);

            string RdfNamespaceUri = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
            string NamespaceUri = "http://purl.org/rss/1.0/";
            string Rss10_NameSpace = "http://purl.org/rss/1.0/modules/syndication/";
            string Dublin_Core_Namespace = "http://purl.org/dc/elements/1.1/";

            writer.WriteStartElement("rdf", "RDF", RdfNamespaceUri);      // Write <RDF>
            writer.WriteAttributeString("xmlns", NamespaceUri);

            writer.WriteAttributeString("xmlns", "sy", null, Rss10_NameSpace);
            writer.WriteAttributeString("xmlns", "dc", null, Dublin_Core_Namespace);

            writer.WriteAttributeString("PageSize", pml.RootElements.PageSize.ToString());
            writer.WriteAttributeString("PageNumber", pml.RootElements.PageNumber.ToString());
            writer.WriteAttributeString("TotalCount", pml.RootElements.TotalCount.ToString());

            SyndicationLink selflink;
            old_FeedLink fl = pml.RootElements.Links.Where(l => l.RelationshipType == "Self").FirstOrDefault();
            if (fl != null)
            {
                if (fl.Uri == null) fl.Uri = "http://www......[domain].....";
                selflink = new SyndicationLink(new Uri(fl.Uri), fl.RelationshipType, fl.Title, fl.MediaType, 1000);
            }
            else
            {
                selflink = new SyndicationLink(new Uri("http://www......[domain]....."), "Alt", "CDC Content Feed", "text/html", 1000);
            }

            writer.WriteStartElement("channel");     // Write <channel>
            writer.WriteAttributeString("about", RdfNamespaceUri, selflink.Uri.ToString());

            writer.WriteElementString("title", "Centers for Disease Control and Prevention");       // Write <title>
            writer.WriteElementString("link", selflink.Uri.ToString());       // Write <link>
            writer.WriteElementString("description", "The Centers for Disease Control and Prevention's (CDC) Web Content Syndication service that allows partners including federal public health agencies, state and local public health departments, non-profit organizations, academic institutions, and commercial organizations to syndicate CDC content directly to their sites. Public health partners can gain direct access to CDC Web content without having to monitor and copy updates, control which content from .....[domain]..... to use on their site, and integrate CDC content with localized content while keeping visitors on their sites. This is a free service provided by the CDC.");       // Write <description>

            writer.WriteStartElement("items");              // Write <items>
            writer.WriteStartElement("Seq", RdfNamespaceUri);

            pml.MediaItems.ForEach(mi =>
            {
                Uri link = string.IsNullOrEmpty(mi.targetUrl) ? new Uri(mi.sourceUrl) : new Uri(mi.targetUrl);

                writer.WriteStartElement("li", RdfNamespaceUri);
                writer.WriteAttributeString("resource", RdfNamespaceUri, link.ToString());
                writer.WriteEndElement();
            });

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();       // Write </channel>

            pml.MediaItems.ForEach(mi =>
            {
                Uri link = string.IsNullOrEmpty(mi.targetUrl) ? new Uri(mi.sourceUrl) : new Uri(mi.targetUrl);

                writer.WriteStartElement("item");           // Write <item>
                writer.WriteAttributeString("about", RdfNamespaceUri, new Uri(mi.sourceUrl).ToString());

                writer.WriteElementString("title", mi.name);       // Write <title>
                writer.WriteElementString("link", link.ToString());       // Write <link>
                if (!string.IsNullOrEmpty(mi.description))
                {
                    writer.WriteElementString("description", mi.description);       // Write the optional <description>
                }

                // add syndication media block
                //writer.WriteRaw(SyndicationMediaBlock(mi));

                writer.WriteEndElement();
            });

            writer.WriteEndElement();

            return XElement.Parse(output.ToString()).ToString();

        }

        #endregion

        #region iTunes Output

        private static SyndicationFeed GenerateItunesFeed(old_ParsedMediaList pml)
        {
            SyndicationFeed feed = new SyndicationFeed();
            PopulateRootElementsForFeed(feed, pml.RootElements);

            // itunes markup 
            old_FeedPerson owner = null;
            if (pml.RootElements.Authors.FirstOrDefault() != null)
            {
                owner = pml.RootElements.Authors.FirstOrDefault();
            }

            XNamespace itunesNS = "http://www.itunes.com/dtds/podcast-1.0.dtd";
            string prefix = "itunes";
            feed.AttributeExtensions.Add(new XmlQualifiedName(prefix, "http://www.w3.org/2000/xmlns/"), itunesNS.NamespaceName);

            var extensions = feed.ElementExtensions;
            extensions.Add(new XElement(itunesNS + "image", new XAttribute("href", _imageUrl)).CreateReader());

            if (owner != null)
            {
                extensions.Add(new XElement(itunesNS + "author", owner.Name).CreateReader());
                extensions.Add(new XDocument(new XElement(itunesNS + "owner", new XElement(itunesNS + "name", owner.Name), new XElement(itunesNS + "email", owner.Email))).CreateReader());
            }

            if (!string.IsNullOrEmpty(pml.RootElements.Description))
            {
                extensions.Add(new XElement(itunesNS + "summary", pml.RootElements.Description).CreateReader());
            }

            extensions.Add(new XElement(itunesNS + "explicit", "no").CreateReader());
            var feedItems = new List<SyndicationItem>();

            foreach (var mi in pml.MediaItems)
            {
                SyndicationItem item = new SyndicationItem();
                PopulateRootElementsForItem(item, mi);

                // itunes markup
                var itemExt = item.ElementExtensions;

                itemExt.Add(new XElement(itunesNS + "subtitle", mi.name).CreateReader());
                itemExt.Add(new XElement(itunesNS + "summary", mi.description).CreateReader());

                //TODO: OCT-243 Fix the following code to select tags.
                //List<string> topics = mi.tags.topic.Select(t => t.name).ToList();

                List<string> attributeValues = new List<string>();
                itemExt.Add(new XElement(itunesNS + "keywords", string.Join(",", attributeValues.ToArray())).CreateReader());

                itemExt.Add(new XElement(itunesNS + "explicit", "no").CreateReader());

                //TODO: Get "type" from mi.targetUrl or sourceUrl file extension
                //if (!string.IsNullOrEmpty(mi.mimeType))
                //{
                //    itemExt.Add(new XElement("enclosure", new XAttribute("url", mi.targetUrl),
                //        new XAttribute("length", 1000), new XAttribute("type", mi.mimeType)));
                //}
                //else {
                itemExt.Add(new XElement("enclosure", new XAttribute("url", mi.targetUrl),
                    new XAttribute("length", 1000)));
                //}

                // end itunes markup
                // add syndication block
                //XElement smbXML = XElement.Parse(SyndicationMediaBlock(mi));
                //item.ElementExtensions.Add(smbXML);

                feedItems.Add(item);
            }

            feed.Items = feedItems;

            return feed;
        }

        #endregion

        #region common utils

        private static void PopulateRootElementsForFeed(SyndicationFeed feed, old_RootElements root)
        {
            // set the feed ID to the main URL of your Website
            feed.Id = root.FeedId;

            // set some properties on the feed
            feed.Title = new TextSyndicationContent(root.Title);
            feed.Description = new TextSyndicationContent(root.Description);
            feed.Copyright = new TextSyndicationContent(root.Copyright);
            feed.LastUpdatedTime = root.LastUpdatedTime;

            root.Authors.ForEach(auth =>
            {
                feed.Authors.Add(new SyndicationPerson(auth.Email, auth.Name, auth.Uri));
            });

            feed.Generator = root.Generator;
            feed.ImageUrl = new Uri(root.ImageUrl);

            feed.AttributeExtensions.Add(new XmlQualifiedName("PageSize", ""), root.PageSize.ToString());
            feed.AttributeExtensions.Add(new XmlQualifiedName("PageNumber", ""), root.PageNumber.ToString());
            feed.AttributeExtensions.Add(new XmlQualifiedName("TotalCount", ""), root.TotalCount.ToString());

            // Add the URL that will link to your published feed when it's done
            root.Links
                .Where(l => l.Uri != null).ToList()
                .ForEach(l =>
                {
                    SyndicationLink link = new SyndicationLink(new Uri(l.Uri));
                    link.RelationshipType = l.RelationshipType;
                    link.MediaType = l.MediaType;
                    link.Title = l.Title;
                    feed.Links.Add(link);
                });
        }

        private static void PopulateRootElementsForItem(SyndicationItem item, ISerialRelatedMediaV2 mi)
        {
            Uri itmLink = string.IsNullOrEmpty(mi.targetUrl) ? new Uri(mi.sourceUrl) : new Uri(mi.targetUrl);

            item.Title = new TextSyndicationContent(mi.name);
            item.Summary = new TextSyndicationContent(mi.description);
            item.Id = mi.id.ToString();

            //TODO: Get mediaType using itmLink (i.e. url)
            SyndicationLink link = new SyndicationLink(itmLink, "alternate", itmLink.ToString(), "text/html", 1000);
            item.Links.Add(link);
 
            //TODO: Convert UTC date time
            item.LastUpdatedTime = Convert.ToDateTime(mi.dateModified);
            if (!string.IsNullOrEmpty(mi.datePublished))
                item.PublishDate = (DateTimeOffset)DateTime.Parse(mi.datePublished);
        }

        //private static string SyndicationMediaBlock(ISerialRelatedMediaV2 mediaItem)
        //{
        //    var output = new StringWriter();
        //    var writer = new XmlTextWriter(output);

        //    writer.WriteStartElement("MediaItem");
        //    writer.WriteAttributeString("xmlns", "mi", null, SyndicationMediaNamespace);

        //    writer.WriteAttributeString("MediaId", mediaItem.id.ToString());
        //    writer.WriteAttributeString("SourceUrl", mediaItem.sourceUrl);
        //    writer.WriteAttributeString("ModifiedDateTime", mediaItem.dateModified);

        //    writer.WriteStartElement("mi", "SelectionCriteria", SyndicationMediaNamespace);

        //    writer.WriteAttributeString("ClassList", "...");
        //    writer.WriteAttributeString("ElementList", "...");
        //    writer.WriteAttributeString("XPath", "...");

        //    writer.WriteStartElement("mi", "Categorization", SyndicationMediaNamespace);

        //    PropertyInfo[] propertyInfos;
        //    propertyInfos = typeof(SerialTags).GetProperties();
        //    foreach (PropertyInfo propertyInfo in propertyInfos)
        //    {
        //        //TODO: OCT-243 Fix the following code to select tags.

        //        //// cast categorization property to type List<SerialValueItem>
        //        //List<SerialValueItem> catItems = (List<SerialValueItem>)mediaItem.tags.GetType()
        //        //                                .GetProperty(propertyInfo.Name)
        //        //                                .GetValue(mediaItem.tags, null);

        //        List<SerialValueItem> catItems = new List<SerialValueItem>();

        //        if (catItems.Count > 0)
        //        {
        //            //Need to make this dynamic
        //            writer.WriteStartElement("mi", "Category", SyndicationMediaNamespace);
        //            writer.WriteAttributeString("Name", propertyInfo.Name);
        //            writer.WriteAttributeString("Source", "CDC");

        //            catItems.ForEach(ci =>
        //            {
        //                writer.WriteStartElement("mi", "Value", SyndicationMediaNamespace);

        //                writer.WriteAttributeString("Id", ci.id.ToString());
        //                writer.WriteAttributeString("Value", ci.name);
 
        //                writer.WriteEndElement(); // end <Value>
        //            });

        //            writer.WriteEndElement(); // end <Category>
        //        }
        //    }

        //    writer.WriteEndElement(); // end <Categorization>
        //    writer.WriteEndElement(); // end <SelectionCriteria>
        //    writer.WriteEndElement(); // end <MediaItem>

        //    return XElement.Parse(output.ToString()).ToString();
        //}

        #endregion
    }
}







