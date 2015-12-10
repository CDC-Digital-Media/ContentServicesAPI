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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class FlatVocabValueSearchDataMgr : MediaBaseSearchMgr<FlatVocabValueItem>
    {
        public override DataSetResult GetData(SearchPlan searchPlan, ResultSetParms resultSetParms = null)
        {
            ResultSetParms resultSetParameters = resultSetParms ?? new ResultSetParms();

            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                IQueryable<ValueRelationshipObject> allRelationships = ValueRelationshipCtl.Get(media);
                IQueryable<ValueObject> allValues = ValueCtl.Get(media);

                if (searchPlan.Plan.Criteria.IsFilteredBy("ValueId"))
                {
                    List<int> valueIds = searchPlan.Plan.Criteria.UseCriterion("ValueId").GetIntKeys();
                    allValues = allValues.Where(i => valueIds.Contains(i.ValueId));
                }

                FilterCriterionMultiSelect languageCriterion = (FilterCriterionMultiSelect)searchPlan.Plan.Criteria.UseCriterion("Language");
                if (languageCriterion.IsFiltered)
                {
                    List<string> languages = languageCriterion.GetStringKeys();
                    allValues = allValues.Where(q => languages.Contains(q.LanguageCode));
                }

                string fullSearchText = searchPlan.Plan.Criteria.IsFilteredBy("FullSearch") ? searchPlan.Plan.Criteria.UseCriterion("FullSearch").GetStringKey() : null;
                if (fullSearchText != null)
                {
                    allValues = allValues.Where(q => q.Description.Contains(fullSearchText) || q.ValueName.Contains(fullSearchText));
                }

                ValueSetParameterUtility p = new ValueSetParameterUtility(media, searchPlan.Plan.Criteria, languageCriterion);
                if (p.FilteredByValueSet)
                {
                    List<int> valueSetIds = p.ValueSetIds;

                    IQueryable<ValueToValueSet> selectedValueToValueSets = media.MediaDbEntities.ValueToValueSets.Where(v =>
                        valueSetIds.Contains(v.ValueSetID));

                    allValues = from v in allValues
                                join vvs in selectedValueToValueSets on v.ValueId equals vvs.ValueID
                                select v;

                    allRelationships = from r in allRelationships
                                       join vvs in selectedValueToValueSets
                                    on r.RelatedValueId equals vvs.ValueID
                                       select r;
                }

                var a = ValueRelationshipCtl.Get(media);
                IQueryable<FlatVocabValueItem> items = from v in allValues
                                                       select new FlatVocabValueItem()
                                                       {
                                                           ValueObject = v,
                                                           Relations = allRelationships.Where(r => r.ValueId == v.ValueId)
                                                       };


                return CreateDataSetResults(items, searchPlan, resultSetParameters.ResultSetId);
            }

        }

    }
}
