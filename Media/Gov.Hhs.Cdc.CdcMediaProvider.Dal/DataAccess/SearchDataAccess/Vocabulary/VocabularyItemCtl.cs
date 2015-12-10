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
using Gov.Hhs.Cdc.DataServices.Bo;
using System.Data.Objects;

namespace Gov.Hhs.Cdc.DataSource.Media
{
    internal class VocabularyItemCtl
    {
        public static IQueryable<VocabularyItemFlatDto> GetFlat(MediaObjectContext media, bool getPreferredTermRelationships)
        {
            return GetFlat(media.MediaDbEntities.Values, getPreferredTermRelationships);
        }

        public static IQueryable<VocabularyItemFlatDto> GetFlat(IQueryable<Value> values, bool getPreferredTermRelationships)
        {
            return values.Select(v =>
                new VocabularyItemFlatDto
                    {
                        ValueKey = new ValueKey() { Id = (int)v.ValueID, LanguageCode = v.LanguageCode },
                        ValueName = v.ValueName,
                        Description = v.Description,  
                        DisplayOrdinal = (int)v.DisplayOrdinal,
                        IsActive = v.Active == "Yes",


                        MediaCount = null,
                        ValueSetNames = from vvs in v.ValueToValueSets
                                        where vvs.ValueSet.Active == "Yes"
                                        && vvs.ValueSet.Active == "Yes"
                                        select vvs.ValueSet.ValueSetName,
                        IsPreferredTerm = getPreferredTermRelationships
                            ? !(v.ValueRelationships1.Where(r => r.RelationshipTypeName == "Use" && r.Active == "Yes").Any())
                            : (bool?)null,
                        HasChildrenInUse = false
                    }

                );
        }

        public static IQueryable<VocabularyItemFlatDto> AddTypeCounts(IQueryable<VocabularyItemFlatDto> allVocabItems, 
            IQueryable<VocabularyHierarchyMediaTypeCount> allTypeCounts)
        {   
            return allVocabItems.Select(v =>
                            new VocabularyItemFlatDto
                            {
                                ValueKey = v.ValueKey,
                                ValueName = v.ValueName,
                                Description = v.Description,
                                DisplayOrdinal = v.DisplayOrdinal,
                                IsActive = v.IsActive,

                                MediaCount = allTypeCounts.Where(c => c.ChildValueId == v.ValueKey.Id).Select(c => c.TypeCount).FirstOrDefault(),
                                ValueSetNames = v.ValueSetNames,
                                IsPreferredTerm = v.IsPreferredTerm,
                                HasChildrenInUse = allTypeCounts.Where(c => c.ParentValueId == v.ValueKey.Id).Any()
                            });
        }




        //public static VocabularyItemDto Get(Value v)
        //{
        //    return new VocabularyItemDto
        //    {
        //        UsageCount = null,
        //        Value = v,
        //        ParentIds = from r in v.ValueRelationships
        //                    where r.RelationshipTypeName == "Is Parent Of"
        //                      && r.Active == "Yes"
        //                    select r.ValueID,
        //        ValueSetNames = from vvs in v.ValueToValueSets
        //                        where vvs.ValueSet.Active == "Yes"
        //                        && vvs.ValueSet.Active == "Yes"
        //                        select vvs.ValueSet.ValueSetName,
        //        HasChildrenInUse = false,
        //        IsActive = v.Active == "Yes"

        //    };
        //}

        


        //public static VocabularyItem Get(VocabularyItemDto vocabItemDto)
        //{
        //    return new VocabularyItem
        //    {
        //        //ValueSetIds = from vvs in v.ValueToValueSets
        //        //            where vvs.ValueSet.Active == "Yes"
        //        //            select vvs.ValueSetID,
        //        ValueSetNames = vocabItemDto.ValueSetNames,
        //        ParentIds = vocabItemDto.ParentIds,
        //        ValueId = vocabItemDto.Value.ValueID,
        //        ValueName = vocabItemDto.Value.ValueName,
        //        LanguageCode = vocabItemDto.Value.LanguageCode,
        //        Description = vocabItemDto.Value.Description,
        //        DisplayOrdinal = vocabItemDto.Value.DisplayOrdinal,
        //        IsActive = vocabItemDto.IsActive,
        //        MediaUsageCount = vocabItemDto.UsageCount == null ? 0 : (int)vocabItemDto.UsageCount
        //    };
        //}

        //public static IQueryable<VocabularyItem> Get(MediaDataContext media)
        //{
        //    IQueryable<VocabularyItem> mediaItems =
        //                                      from v in media.Values
        //                                      where v.Active == "Yes"
        //                                      select new VocabularyItem
        //                                      {
        //                                          //ValueSetIds = from vvs in v.ValueToValueSets
        //                                          //            where vvs.ValueSet.Active == "Yes"
        //                                          //            select vvs.ValueSetID,
        //                                          ValueSetNames = from vvs in v.ValueToValueSets
        //                                                          where vvs.ValueSet.Active == "Yes"
        //                                                          && vvs.ValueSet.Active == "Yes"
        //                                                          select vvs.ValueSet.ValueSetName,
        //                                          ParentIds = from r in v.ValueRelationships
        //                                                      where r.RelationshipTypeName == "Is Parent Of"
        //                                                        && r.Active == "Yes"
        //                                                      select r.ValueID,
        //                                          ValueId = v.ValueID,
        //                                          ValueName = v.ValueName,
        //                                          LanguageCode = v.LanguageCode,
        //                                          Description = v.Description,
        //                                          DisplayOrdinal = v.DisplayOrdinal,
        //                                          Active = (v.Active == "Yes")
        //                                      };
        //    return mediaItems.AsQueryable();
        //}




    }
}
