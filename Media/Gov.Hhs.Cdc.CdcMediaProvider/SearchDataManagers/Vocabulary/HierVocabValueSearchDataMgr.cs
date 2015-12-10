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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class HierVocabValueSearchDataMgr : MediaBaseSearchMgr<HierVocabValueItem>
    {
        public override DataSetResult GetData(SearchPlan searchPlan, ResultSetParms resultSetParms)
        {
            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                string mediaType = searchPlan.Plan.Criteria.IsFilteredBy("MediaType") ?
                    searchPlan.Plan.Criteria.UseCriterion("MediaType").GetStringKeys()[0] : "";
                string activeAsString = searchPlan.Plan.Criteria.IsFilteredBy("IsActive") ?
                        ((FilterCriterionBoolean)searchPlan.Plan.Criteria.UseCriterion("IsActive")).Value ? "Yes" : "No"
                        : "";
                string valueIds = searchPlan.Plan.Criteria.IsFilteredBy("Id") ?
                        searchPlan.Plan.Criteria.UseCriterion("Id").GetStringKey() : "";

                ValueSetParameterUtility p = new ValueSetParameterUtility(media, searchPlan.Plan.Criteria, null);
                var dtos = HierVocabularyItemCtl.GetHierarchical(media, mediaType,
                    string.Join(",", p.ValueSetIds), string.Join(",", p.ChildValueSetIds),
                    activeAsString, valueIds, languageCode: "");

                IQueryable<HierVocabValueItem> selectedVocabItems = (new HierVocabularyUtility())
                    .Build(dtos.ToList(), searchPlan.Plan.Criteria.IsFilteredByAndChecked("IsInUse")).AsQueryable();

                DataSetResult results = CreateDataSetResults(selectedVocabItems, searchPlan, resultSetParms.ResultSetId);
                return results;
            }
        }


    }
}
