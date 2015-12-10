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

namespace Gov.Hhs.Cdc.Bo
{
    public class MediaTypeParms
    {
        private enum MediaTypeEnum
        {
            Unknown, Html, Infographic, Image, Video, Widget, Button, Badge, Collection, Ecard, Microsite, Pdf,
            Feed, FeedImage, FeedItem, FeedProxy, FeedImport, FeedAggregate
        };

        public const string FeedImageMediaType = "Feed Image";

        public MediaTypeParms(string mediaType)
        {
            MediaTypeEnum mediaTypeEnum;
            if (string.IsNullOrEmpty(mediaType))
            {
                mediaTypeEnum = MediaTypeEnum.Unknown;
            }
            else
            {
                var trimmed = mediaType.Replace(" ", "");
                trimmed = trimmed.Replace("-", "");
                if (!Enum.TryParse(trimmed, /*ignoreCase:*/true, out mediaTypeEnum))
                {
                    mediaTypeEnum = MediaTypeEnum.Unknown;
                }
            }
            MediaType = mediaTypeEnum;
        }

        private MediaTypeEnum MediaType { get; set; }
        public string Code { get { return MediaType.ToString(); } }

        public static string DefaultHtmlMediaType { get { return MediaTypeEnum.Html.ToString().ToUpper(); } }

        public bool CreateEmbedHtml
        {
            get { return MediaType == MediaTypeEnum.Html; }
        }

        public bool SetRequestUrl
        {
            get { return MediaType == MediaTypeEnum.Html; }
        }

        public bool SetCollectedDataFromResponseStream
        {
            get { return MediaType == MediaTypeEnum.Html; }
        }

        private static List<string> _contentTypesWithPersistentUrl = new List<string>() { MediaTypeEnum.Html.ToString() };
        public static List<string> ContentTypesWithPersistentUrl { get { return _contentTypesWithPersistentUrl; } }

        public bool IsStaticImageMedia
        {
            get
            {
                return MediaType == MediaTypeEnum.Image ||
                    MediaType == MediaTypeEnum.Infographic ||
                     MediaType == MediaTypeEnum.Button ||
                     MediaType == MediaTypeEnum.Badge;
            }
        }

        public bool IsEcard
        {
            get { return MediaType == MediaTypeEnum.Ecard; }
        }

        public bool IsVideo
        {
            get { return MediaType == MediaTypeEnum.Video; }
        }

        public bool IsWidget
        {
            get { return MediaType == MediaTypeEnum.Widget; }
        }

        public bool IsMicrosite
        {
            get { return MediaType == MediaTypeEnum.Microsite; }
        }

        public bool IsBadgeButtonVideo
        {
            get
            {
                return MediaType == MediaTypeEnum.Badge ||
                    MediaType == MediaTypeEnum.Button ||
                    MediaType == MediaTypeEnum.Video;
            }
        }

        public bool IsPdf { get { return MediaType == MediaTypeEnum.Pdf; } }
        public bool IsCollection { get { return MediaType == MediaTypeEnum.Collection; } }

        public bool IsFeed
        {
            get
            {
                return MediaType == MediaTypeEnum.Feed ||
                    MediaType == MediaTypeEnum.FeedProxy ||
                    MediaType == MediaTypeEnum.FeedImport ||
                    MediaType == MediaTypeEnum.FeedAggregate;
            }
        }

        public bool IsManagedFeed { get { return MediaType == MediaTypeEnum.Feed; } }

        public bool IsFeedProxy { get { return MediaType == MediaTypeEnum.FeedProxy; } }

        public bool IsFeedImport { get { return MediaType == MediaTypeEnum.FeedImport; } }

        public bool IsFeedImage { get { return MediaType == MediaTypeEnum.FeedImage; } }

        public bool IsFeedItem { get { return MediaType == MediaTypeEnum.FeedItem; } }

        public bool IsFeedAggregate { get { return MediaType == MediaTypeEnum.FeedAggregate; } }
    }
}
