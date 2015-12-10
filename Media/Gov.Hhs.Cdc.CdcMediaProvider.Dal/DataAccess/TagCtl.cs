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
using System.Linq;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class SyndicationMediaValueDto
    {
        public int ValueId { get; set; }
        public Guid SyndicationListGuid { get; set; }
    }

    public class RelatedTagDto
    {
        public int ValueId { get; set; }
        public int RelatedValueId { get; set; }
    }

    public static class TagCtl
    {
        public static IQueryable<TagObject> Get(MediaObjectContext mediaDb, bool forUpdate = false)
        {


            IQueryable<TagObject> tagTypes = from t in mediaDb.MediaDbEntities.Tags
                                             select new TagObject()
                                                 {
                                                     TagId = t.ValueID,
                                                     TagName = t.ValueName,
                                                     TagTypeName = t.AttributeName,
                                                     TagTypeId = t.AttributeID,
                                                     LanguageCode = t.LanguageCode,
                                                     DbObject = forUpdate ? t : null
                                                 };
            return tagTypes;
        }

        public static IQueryable<int> GetTagIdsForMedia(MediaObjectContext mediaDb, int mediaId)
        {
            return mediaDb.MediaDbEntities.MediaTags.Where(mt => mt.MediaId == mediaId).Select(mt => mt.ValueId);

        }

        public static IQueryable<RelatedTagDto> GetRelatedTagDtos(MediaObjectContext mediaDb)
        {

            var list = from mt in mediaDb.MediaDbEntities.RelatedTags
                       select new RelatedTagDto { ValueId = mt.ValueId, RelatedValueId = mt.RelatedValueID };
            return list;

        }
    }
}
