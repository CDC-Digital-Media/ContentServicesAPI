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

using System.Linq;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public static class ValueSetRelationItemCtl
    {
        public static IQueryable<ValueSetRelationItem> Get(MediaObjectContext media)
        {
            return Get(media.MediaDbEntities.ValueSetRelations);
        }

        public static IQueryable<ValueSetRelationItem> Get(IQueryable<ValueSetRelation> valueSetRelations)
        {
            return valueSetRelations.Select(v => new ValueSetRelationItem()
            {
                RelationshipTypeName = v.RelationshipTypeName,
                ValueSetId = v.ValueSetId, 
                ValueSetLanguageCode = v.LanguageCode, 
                RelatedValueSetId = v.RelatedValueSetId,
                RelatedValueSetLanguageCode = v.RelatedLanguageCode,
                IsRelationshipAllowed = v.RelationshipAllowed == "yes"
            });
        }

    }
}
