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

using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public static class HierVocabularyItemCtl
    {
        public static IQueryable<VocabularyItemHierarchicalDto> GetHierarchical(MediaObjectContext media, string mediaType, string topLevelValueSetIds,
            string valueSetIds, string activeString, string valueIds, string languageCode)
        {
            ObjectResult<GetHierarchicalVocabulary_Result> results = media.MediaDbEntities.GetHierarchicalVocabulary(
                mediaType, topLevelValueSetIds, valueSetIds, activeString, valueIds, languageCode);

            IQueryable<VocabularyItemHierarchicalDto> dtos = (from v in results
                                                              let parentKey = v.ParentValueID != null ? new ValueKey((int)v.ParentValueID, v.ParentLanguageCode) : null
                                                              select new VocabularyItemHierarchicalDto()
                                                              {
                                                                  ValueKey = new ValueKey() { Id = (int)v.ValueID, LanguageCode = v.LanguageCode },
                                                                  ValueName = v.ValueName,
                                                                  Description = v.Description,
                                                                  DisplayOrdinal = (int)v.DisplayOrdinal,
                                                                  IsActive = v.Active == "Yes",

                                                                  MediaCount = v.MediaCount,
                                                                  Level = (int)v.Lvl,
                                                                  ParentKey = parentKey != null && v.RelationshipTypeName == "Is Child Of" ? parentKey : null,
                                                                  UsedForKey = parentKey != null && v.RelationshipTypeName == "Used For" ? parentKey : null
                                                              }).AsQueryable();
            return dtos;
        }
    }
}
