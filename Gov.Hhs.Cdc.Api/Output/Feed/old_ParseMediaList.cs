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

namespace Gov.Hhs.Cdc.Api
{
    internal class old_ParsedMediaList
    {
        public old_RootElements RootElements { get; set; }
        public List<ISerialRelatedMediaV2> MediaItems { get; set; }

        public old_ParsedMediaList(SerialResponse obj, string mediaTypeFilter = "")
        {
            List<SerialMediaV2> mediaItems = (List<SerialMediaV2>)obj.results;
            if (mediaItems.Count == 1 && (mediaItems[0].mediaType == "Podcast Series")) // AM I A PODCAST?
            {
                //podcast structure:
                //{ results[
                //      mediaItem{
                //          items[
                //              mediaItem[0] // <- Media type=image. Graphic for use with podcast. 
                //              mediaItem[1-n] // <- Media type=podcast. List of podcasts. 
                //              {items[]} // media for podcasts. Media Type= Audio, Video, Transcript.
                //          ]
                //      } 
                //]}
                //SerialCombineMediaItem rootItem = mediaItems[0];
                //List<SerialCombineMediaItem> tempItems = new List<SerialCombineMediaItem>();
                //rootItem.items.ForEach(itm =>
                //{
                //    tempItems.AddRange(itm.items);
                //});

                //if (!string.IsNullOrEmpty(mediaTypeFilter))
                //{
                //    tempItems = tempItems.Where(mItem => mItem.mediaType.ToLower() == mediaTypeFilter).ToList();
                //}
                //mediaItems = tempItems;

                this.MediaItems = mediaItems[0].children.SelectMany(c => c.children)
                    .Where(c => string.IsNullOrEmpty(mediaTypeFilter) || string.Equals(c.mediaType, mediaTypeFilter, StringComparison.OrdinalIgnoreCase))
                    .Select(c => (ISerialRelatedMediaV2)c)
                    .ToList();

                this.RootElements = new old_RootElements(obj, mediaItems[0]);
            }
            else // I'M NOT A PODCAST.
            {
                this.RootElements = new old_RootElements(obj);
                this.MediaItems = mediaItems
                    .Select(c => (ISerialRelatedMediaV2)c)
                    .ToList();
            }
        }
    }
}
