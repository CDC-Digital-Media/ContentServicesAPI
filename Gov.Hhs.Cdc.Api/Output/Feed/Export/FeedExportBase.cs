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
    public class FeedExportBase
    {
        protected List<MediaObject> Medias { get; set; }
        protected MediaObject Media { get; set; }

        protected FeedExportObject FeedExport { get; set; }
        protected FeedFormatObject FeedFormat { get; set; }

        protected IEnumerable<MediaObject> MediaItems { get; set; }

        public FeedExportBase(List<MediaObject> medias, FeedExportObject feedExport)
        {
            this.Medias = medias;
            this.Media = medias.FirstOrDefault();
            this.FeedExport = feedExport;
            this.FeedFormat = feedExport.FeedFormat;

            InitializeMediaItems();
        }

        private void InitializeMediaItems()
        {
            if (this.Media.Children != null)
            {
                this.MediaItems = this.Media.Children;

                if (this.FeedExport.Offset != null && this.FeedExport.Offset > 0)
                    this.MediaItems = this.MediaItems.Skip(int.Parse(this.FeedExport.Offset.ToString()));

                if (this.FeedExport.ItemCount != null && this.FeedExport.ItemCount > 0)
                    this.MediaItems = this.MediaItems.Take(int.Parse(this.FeedExport.ItemCount.ToString()));
            }
        }
    }
}
