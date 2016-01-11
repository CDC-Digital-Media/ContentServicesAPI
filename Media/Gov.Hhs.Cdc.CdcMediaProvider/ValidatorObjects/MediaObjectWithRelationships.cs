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
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    internal class MediaObjectWithRelationships
    {
        public MediaObject MediaObject { get; set; }
        public MediaRelationshipUpdateMgr RelationshipUpdateMgr { get; set; }
        public IMediaTypeSpecificUpdateMgr MediaTypeSpecificUpdateMgr { get; set; }
        public MediaGeoDataUpdateMgr MediaGeoDataMgr { get; set; }
        public StorageUpdateMgr StorageMgr { get; set; }
        public ExtendedAttributeUpdateMgr ExtendedAttributeDataMgr { get; set; }
        public MediaImageUpdateMgr ImageMgr { get; set; }

        public EnclosureUpdateMgr EnclosureMgr { get; set; }
        public AggregateUpdateMgr AggregateMgr { get; set; }

        public FeedExportObjectUpdateMgr FeedExportSettingMgr { get; set; }

        public MediaObjectWithRelationships(MediaObject mediaObject)
        {
            if (mediaObject == null)
            {
                throw new InvalidOperationException("mediaObject not found");
            }
            MediaObject = mediaObject;
            var relationships = mediaObject.MediaRelationships ?? new List<MediaRelationshipObject>();
            RelationshipUpdateMgr = new MediaRelationshipUpdateMgr(relationships.ToList());

            MediaTypeSpecificUpdateMgr = CreateMediaTypeSpecificUpdateMgr(mediaObject.MediaTypeCode);

            IEnumerable<MediaGeoDataObject> geoDatas = mediaObject.MediaGeoData ?? new List<MediaGeoDataObject>();
            MediaGeoDataMgr = new MediaGeoDataUpdateMgr(geoDatas.ToList());

            IEnumerable<StorageObject> storage = mediaObject.AlternateImages ?? new List<StorageObject>();
            StorageMgr = new StorageUpdateMgr(storage.ToList());

            var extAttrs = mediaObject.ExtendedAttributes ?? new List<CsBusinessObjects.Media.ExtendedAttribute>();
            ExtendedAttributeDataMgr = new ExtendedAttributeUpdateMgr(extAttrs.ToList());

            ImageMgr = new MediaImageUpdateMgr(mediaObject.AssociatedImage);

            IEnumerable<EnclosureObject> enclosures = mediaObject.Enclosures ?? new List<EnclosureObject>();
            EnclosureMgr = new EnclosureUpdateMgr(enclosures.ToList());

            IEnumerable<AggregateObject> aggregates = mediaObject.Aggregates ?? new List<AggregateObject>();
            AggregateMgr = new AggregateUpdateMgr(aggregates.ToList());

            IEnumerable<FeedExportObject> feedExport = mediaObject.ExportSettings ?? new List<FeedExportObject>();
            FeedExportSettingMgr = new FeedExportObjectUpdateMgr(feedExport.ToList());
        }

        public static IMediaTypeSpecificUpdateMgr CreateMediaTypeSpecificUpdateMgr(string mediaTypeCode)
        {
            switch (mediaTypeCode.ToLower())
            {
                case "ecard":
                    return new ECardObjectUpdateMgr();
                case "feed":
                case "feed - proxy":
                case "feed - import":
                case "feed - aggregate":
                    return new FeedObjectUpdateMgr();
                default:
                    return null;
            }
        }
    }
}
