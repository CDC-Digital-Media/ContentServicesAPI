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

namespace Gov.Hhs.Cdc.Api
{
    public class TwitterFeedExporter : FeedExportBase
    {
        public TwitterFeedExporter(List<MediaObject> medias, FeedExportObject feedExport)
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
                            sbFeedItem.AppendFormat(this.FeedFormat.FeedItemTemplate,
                                item.TargetUrl,
                                item.Title
                                );
                        }
                    }

                    StringBuilder sbFeed = new StringBuilder();
                    string sourceUrl = "", title = "";

                    var feed = this.Media.MediaTypeSpecificDetail as FeedDetailObject;
                    if (feed != null && feed.AssociatedImage != null)
                    {
                        sourceUrl = string.IsNullOrEmpty(feed.AssociatedImage.SourceUrl) ? "" : feed.AssociatedImage.SourceUrl;
                        title = string.IsNullOrEmpty(feed.AssociatedImage.Title) ? "" : feed.AssociatedImage.Title;
                    }

                    sbFeed.AppendFormat(this.FeedFormat.FeedTemplate,
                        this.Media.TargetUrl,
                        (string.IsNullOrEmpty(sourceUrl)) ? "https://pbs.twimg.com/profile_images/447192716221218818/AJ__Nd3C.png" : sourceUrl,
                        (string.IsNullOrEmpty(title)) ? "CDCgov profile" : title,
                        sbFeedItem.ToString());
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
