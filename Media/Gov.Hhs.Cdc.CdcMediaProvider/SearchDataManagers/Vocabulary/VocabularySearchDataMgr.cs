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

    internal class VocabularySearchDataMgr : MediaBaseSearchDataMgr
    {
        public VocabularySearchDataMgr(DsApplication application, Filter filter, EntitiesConfigurationItems configItems)
            : base(application, filter, configItems)
        {

        }

        public override DataSetResult GetData(Plan plan, ResultSetParms resultSetParms)
        {
            using (MediaObjectContext media = new MediaObjectContext(ConfigItems))
            {
                HierarchicalVocabularyPlan p = new HierarchicalVocabularyPlan(plan);
                IQueryable<HierVocabularyItem> selectedVocabItems = p.WithChildren ?
                    GetHierarchicalVocabulary(media, p) : GetFlatVocabulary(media, p);
                DataSetResult results = CreateDataSetResults<VocabularyItem>(selectedVocabItems, plan, resultSetParms.ResultSetId);
                return results;
            }
        }

        private static IQueryable<HierVocabularyItem> GetHierarchicalVocabulary(MediaObjectContext media, HierarchicalVocabularyPlan p)
        {
            List<VocabularyItemHierarchicalDto> dtos = VocabularyItemCtl.GetHierarchical(media, p.MediaType, p.TopLevelValueSetName, 
                p.ValueSetNames, p.ActiveAsString, p.LanguageCode);
            return (new VocabularyItemHierarchy()).Build(dtos, p.IsInUse).AsQueryable();
        }

        private static IQueryable<HierVocabularyItem> GetFlatVocabulary(MediaObjectContext media, HierarchicalVocabularyPlan p)
        {
            IQueryable<VocabularyItemFlatDto> allVocabItems =
                VocabularyItemCtl.GetFlat(media, p.GetOnlyPreferredTerms);

            //string SQL1 = ((ObjectQuery)allVocabItems).ToTraceString();

            if (p.Plan.Criteria.IsFilteredBy("IsActive"))
            {
                bool isActive = ((FilterCriterionBoolean)p.Plan.Criteria.UseCriterion("IsActive")).Checked;
                allVocabItems = allVocabItems.Where(v => v.IsActive == isActive);
            }

            if (p.GetOnlyPreferredTerms)
                allVocabItems = allVocabItems.Where(v => v.IsPreferredTerm != null && (bool)v.IsPreferredTerm);

            //  Us (ValueRelationships1
            if (p.WithCounts || p.MediaType != "")
            {
                //Get value type usage counts
                IQueryable<VocabularyHierarchyMediaTypeCount> allTypeCounts = VocabHierarchyMediaTypeCountCtl.Get(media, p.MediaType);

                //Add the counts to the vocab items
                allVocabItems = VocabularyItemCtl.AddTypeCounts(allVocabItems, allTypeCounts);
            }

            if (p.WithCounts || p.MediaType != "")
            {
                //Filter the top level by the usage counts
                allVocabItems = allVocabItems.Where(v => v.HasChildrenInUse || v.MediaCount != null && v.MediaCount > 0);
            }

            if (p.LanguageCode != "")
                allVocabItems = allVocabItems.Where(v => v.ValueKey.LanguageCode == p.LanguageCode);


            IQueryable<VocabularyItemFlatDto> selectedVocabItems = allVocabItems;
            if (p.Plan.Criteria.IsFilteredBy("MediaId"))
            {
                //List<int> mediaIds = criteria.GetCriterion("MediaId").GetIntKeys();
                int mediaId = p.Plan.Criteria.UseCriterion("MediaId").GetIntKey();
                IQueryable<AttributeValue> mediaAttributes = AttributeValueCtl.Get(media);
                IQueryable<Medias> mediaItems = media.MediaDbEntities.Medias;

                selectedVocabItems = from v in allVocabItems
                                     join a in mediaAttributes
                                        on v.ValueKey equals a.ValueKey
                                     join m in mediaItems
                                        on a.MediaId equals m.MediaId
                                     where m.MediaId == mediaId
                                     select v;

            }

            if (p.Plan.Criteria.IsFilteredBy("TopValueSet"))
            {
                string valueSetName = p.TopLevelValueSetName;
                selectedVocabItems = selectedVocabItems.Where(m => m.ValueSetNames.Contains(valueSetName));
            }


            //string SQL  = ((ObjectQuery)selectedVocabItems).ToTraceString();

            return VocabularyItemCtl.Get(selectedVocabItems);
        }




        #region HierarchicalVocabularyPlan
        private class HierarchicalVocabularyPlan
        {
            public Plan Plan { get; set; }

            public bool WithChildren
            {
                get { return Plan.Criteria.IsFilteredByAndChecked("WithChildren"); }
            }
            private string _topLevelValueSetName = null;
            public string TopLevelValueSetName
            {
                get
                {
                    return _topLevelValueSetName ?? (_topLevelValueSetName =
                        Plan.Criteria.IsFilteredBy("TopValueSet") ?
                        Plan.Criteria.UseCriterion("TopValueSet").GetStringKey() : "");
                }
            }

            public string ValueSetNames
            {
                get
                {
                    return string.Equals(TopLevelValueSetName, "Categories", StringComparison.OrdinalIgnoreCase) ?
                        "Categories,Topics" : TopLevelValueSetName;
                }
            }

            public string ActiveAsString
            {
                get
                {
                    return Plan.Criteria.IsFilteredBy("IsActive") ?
                        ((FilterCriterionBoolean)Plan.Criteria.UseCriterion("IsActive")).Checked ? "Yes" : "No"
                        : "";
                }
            }

            public string _languageCode;
            public string LanguageCode
            {
                get
                {
                    return _languageCode ?? (_languageCode = Plan.Criteria.IsFilteredBy("Language") ?
                        Plan.Criteria.UseCriterion("Language").GetStringKey() : "");
                }
            }

            private string mediaType = null;
            public string MediaType
            {
                get
                {
                    return mediaType ?? (mediaType = Plan.Criteria.IsFilteredBy("MediaType") ?
                        Plan.Criteria.UseCriterion("MediaType").GetStringKey()
                        : "");
                }
            }

            public bool IsInUse { get { return Plan.Criteria.IsFilteredByAndChecked("IsInUse"); } }
            public bool WithCounts { get { return Plan.Criteria.IsFilteredByAndChecked("WithCounts"); } }
            public bool GetOnlyPreferredTerms { get { return Plan.Criteria.IsFilteredByAndChecked("IsPreferredTerm"); } }

            public HierarchicalVocabularyPlan(Plan plan)
            {
                Plan = plan;
            }
        }
        #endregion HierarchicalVocabularyPlan
    }
}
