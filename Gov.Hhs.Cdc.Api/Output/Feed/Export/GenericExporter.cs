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

using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public class GenericExporter : FeedExportBase
    {
        public GenericExporter(List<MediaObject> medias, FeedExportObject feedExport)
            : base (medias,feedExport)
        {
        }

        public string Generate()
        {
            string formattedOutput = null;
            try
            {
                if (this.FeedFormat != null && !string.IsNullOrEmpty(this.FeedFormat.FeedTemplate) && !string.IsNullOrEmpty(this.FeedFormat.FeedItemTemplate))
                {
                    StringBuilder sbFeedItem = new StringBuilder();
                    if (this.MediaItems != null)
                    {
                        foreach (var item in this.MediaItems.ToList())
                        {
                            string itemImageTitle = "", itemImageDescription = "", itemImageSourceUrl = "", itemImageWidth = "", itemImageHeight = "", itemImageLink = "";

                            var feedItemDetail = item.MediaTypeSpecificDetail as FeedDetailObject;
                            if (feedItemDetail != null && feedItemDetail.AssociatedImage != null)
                            {
                                itemImageTitle = string.IsNullOrEmpty(feedItemDetail.AssociatedImage.Title) ? "" : feedItemDetail.AssociatedImage.Title;
                                itemImageDescription = string.IsNullOrEmpty(feedItemDetail.AssociatedImage.Description) ? "" : feedItemDetail.AssociatedImage.Description;
                                itemImageSourceUrl = string.IsNullOrEmpty(feedItemDetail.AssociatedImage.SourceUrl) ? "" : feedItemDetail.AssociatedImage.SourceUrl;
                                itemImageWidth = feedItemDetail.AssociatedImage.Width == null ? "0" : feedItemDetail.AssociatedImage.Width.ToString();
                                itemImageHeight = feedItemDetail.AssociatedImage.Height == null ? "0" : feedItemDetail.AssociatedImage.Height.ToString();
                                itemImageLink = string.IsNullOrEmpty(feedItemDetail.AssociatedImage.TargetUrl) ? "" : feedItemDetail.AssociatedImage.TargetUrl;
                            }

                            sbFeedItem.AppendFormat(this.FeedFormat.FeedItemTemplate,
                                item.Title,
                                item.SubTitle,
                                item.Description,
                                item.TargetUrl,
                                item.PublishedDateTime.FormatAsUtcString(),
                                item.LanguageCode,
                                itemImageTitle,
                                itemImageDescription,
                                itemImageSourceUrl,
                                itemImageWidth,
                                itemImageHeight,
                                itemImageLink,
                                "",     //ThumbnailTitle
                                "",     //ThumbnailSourceUrl
                                "",     //ThumbnailWidth
                                "",     //ThumbnailHeight
                                item.CreatedDateTime.FormatAsUtcString(),
                                item.ModifiedDateTime.FormatAsUtcString()
                                );
                        }
                    }

                    StringBuilder sbFeed = new StringBuilder();
                    string imageTitle = "", imageDescription = "", imageSourceUrl = "", imageWidth = "", imageHeight = "", imageLink = "",
                        copyright = "", editorName = "", webMasterName = "", webMasterEmail = "";

                    var feed = this.Media.MediaTypeSpecificDetail as FeedDetailObject;
                    if (feed != null && feed.AssociatedImage != null)
                    {
                        imageTitle = string.IsNullOrEmpty(feed.AssociatedImage.Title) ? "" : feed.AssociatedImage.Title;
                            imageDescription = string.IsNullOrEmpty(feed.AssociatedImage.Description) ? "" : feed.AssociatedImage.Description;
                            imageSourceUrl = string.IsNullOrEmpty(feed.AssociatedImage.SourceUrl) ? "" : feed.AssociatedImage.SourceUrl;
                            imageWidth = feed.AssociatedImage.Width == null ? "0" : feed.AssociatedImage.Width.ToString();
                            imageHeight = feed.AssociatedImage.Height == null ? "0" : feed.AssociatedImage.Height.ToString();
                            imageLink = string.IsNullOrEmpty(feed.AssociatedImage.TargetUrl) ? "" : feed.AssociatedImage.TargetUrl;
                    }

                    if (feed != null) 
                    {
                        copyright = string.IsNullOrEmpty(feed.Copyright) ? "" : feed.Copyright;
                        editorName = string.IsNullOrEmpty(feed.EditorialManager) ? "" : feed.EditorialManager;
                        webMasterEmail = string.IsNullOrEmpty(feed.WebMasterEmail) ? "" : feed.WebMasterEmail;                        
                    }

                    sbFeed.AppendFormat(this.FeedFormat.FeedTemplate,
                        sbFeedItem.ToString(),
                        this.Media.Title,
                        this.Media.SubTitle,
                        this.Media.Description,
                        this.Media.TargetUrl,
                        this.Media.LanguageCode,
                        this.Media.PublishedDateTime.FormatAsUtcString(),
                        imageTitle,
                        imageDescription,
                        imageSourceUrl,
                        imageWidth,
                        imageHeight,
                        imageLink,
                        copyright,
                        this.Media.Author ?? "",
                        editorName,
                        webMasterName,     //WebMasterName is not set
                        webMasterEmail,
                        this.Media.CreatedDateTime.FormatAsUtcString(),
                        this.Media.ModifiedDateTime.FormatAsUtcString()
                        );
                    
                    formattedOutput = sbFeed.ToString();
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.LogError(ex.ToString());
                throw ex;
            }

            return formattedOutput;
        }

    }
}
