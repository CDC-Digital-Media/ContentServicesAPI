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
using System.Configuration;

namespace Gov.Hhs.Cdc.Api
{
    internal class old_RootElements
    {
        public string FeedId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalCount { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public List<old_FeedPerson> Authors { get; set; }
        public string Generator { get; set; }
        public List<old_FeedLink> Links { get; set; }
        public string ImageUrl { get; set; }

        private static string _imageUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["RSSLogo"];
            }
        }

        public old_RootElements(SerialResponse obj)
        {
            FeedId = obj.meta.resultSet.id;
            PageSize = obj.meta.pagination.max;
            PageNumber = obj.meta.pagination.pageNum;
            TotalCount = obj.meta.pagination.total;

            Title = "Centers for Disease Control and Prevention";
            Description = "The Centers for Disease Control and Prevention's (CDC) Web Content Syndication service that allows partners including federal public health agencies, state and local public health departments, non-profit organizations, academic institutions, and commercial organizations to syndicate CDC content directly to their sites. Public health partners can gain direct access to CDC Web content without having to monitor and copy updates, control which content from .....[domain]..... to use on their site, and integrate CDC content with localized content while keeping visitors on their sites. This is a free service provided by the CDC.";
            Copyright = "CDC - Centers for Disease Control and Prevention";
            LastUpdatedTime = new DateTimeOffset(DateTime.Now);

            Authors = new List<old_FeedPerson>();
            Authors.Add(new old_FeedPerson("senderEmail9@email", "Media Technology Team, OADC/DNEM, Centers for Disease Control and Prevention (CDC)", "http://www......[domain]....."));

            Generator = "CDC CS 10_1_2013";

            Links = new List<old_FeedLink>();
            if (obj.meta.pagination.currentUrl != "")
            {
                Links.Add(new old_FeedLink(obj.meta.pagination.currentUrl, "text/html", "Dynamic Feed", "Self"));
            }

            ImageUrl = _imageUrl;
        }

        public old_RootElements(SerialResponse obj, SerialMediaV2 parentMediaItem)
        {
            List<SerialMediaV2> mediaItems = (List<SerialMediaV2>)obj.results;
            FeedId = mediaItems.Count > 0 ? mediaItems[0].id.ToString() : obj.meta.resultSet.id.ToString();
            PageSize = obj.meta.pagination.max;
            PageNumber = obj.meta.pagination.pageNum;
            TotalCount = obj.meta.pagination.total;

            SerialMediaV2 mi = parentMediaItem;

            Title = mi.name;
            Description = mi.description;
            Copyright = mi.attribution;
            LastUpdatedTime = new DateTimeOffset(DateTime.Parse(mi.dateModified));

            Authors = new List<old_FeedPerson>();
            Authors.Add(new old_FeedPerson("senderEmail9@email", "Media Technology Team, OADC/DNEM, Centers for Disease Control and Prevention (CDC)", "http://www......[domain]....."));

            Generator = "CDC CS 10_1_2013";

            Links = new List<old_FeedLink>();
            if (obj.meta.pagination.currentUrl != "")
            {
                Links.Add(new old_FeedLink(obj.meta.pagination.currentUrl, "text/html", parentMediaItem.mediaType, "Self"));
            }

            if (mi.children != null && mi.children.Count() > 0)
                ImageUrl = mi.children[0].sourceUrl;

        }
    }
}
