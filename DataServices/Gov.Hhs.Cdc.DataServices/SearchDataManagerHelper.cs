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
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.DataServices
{
    public static class SearchDataManagerHelper
    {

        public static SearchPlan GetSearchPlan(SearchParameters searchParameters, string applicationCode, string filterCode, CriteriaDefinitionsForFilter criteriaDefinition = null)
        {
            List<FilterCriterion> metaCriteria = FilterCriterionFactory.CreateFilterCriterion(
                criteriaDefinition != null ? criteriaDefinition.Criteria :
                null);

            Plan plan = new Plan()
            {
                ApplicationCode = applicationCode,
                FilterCode = filterCode,
                DataSetCode = filterCode,
                ActionCode = searchParameters.ActionCode,
                ReportCode = null,
                EnvironmentCode = null
            };

            if (searchParameters == null)
            {
                return new SearchPlan() { Plan = plan };
            }

            plan.Criteria = new FilterCriteria(MergeSearchParametersWithMetaData(metaCriteria, searchParameters));

            return new SearchPlan()
            {
                Paging = new Paging(searchParameters.Paging),
                Sorting = searchParameters.Sorting,
                Plan = plan
            };
        }

        private static List<FilterCriterion> MergeSearchParametersWithMetaData(IEnumerable<FilterCriterion> criteriaMetaData, SearchParameters searchParameters)
        {
            //Criteria in the search parameters can be specified as either the complete "FilterCriteria" class or a basic criteria list
            if (searchParameters.TheCriteria != null)
            {
                MergeSearchParametersWithMetaData(criteriaMetaData, searchParameters.TheCriteria.CriteriaDictionary);
            }
            else if (searchParameters.BasicCriteria != null)
            {
                MergeSearchParametersWithMetaData(criteriaMetaData, searchParameters.BasicCriteria.List);
            }
            return criteriaMetaData.ToList();
        }


        private static void MergeSearchParametersWithMetaData(IEnumerable<FilterCriterion> criteriaMetaData, IEnumerable<Criterion> specifiedCriteria)
        {
            var allCriteria = from baseCriteria in criteriaMetaData
                              join selectedCriteria in specifiedCriteria on baseCriteria.Code equals selectedCriteria.Code
                              select new { metaData = baseCriteria, selectedCriteria = selectedCriteria };

            foreach (var criterion in allCriteria)
            {
                criterion.metaData.SetCriteriaValues(criterion.selectedCriteria);
            }
        }

        private static void MergeSearchParametersWithMetaData(IEnumerable<FilterCriterion> criteriaMetaData, Dictionary<string, FilterCriterion> specifiedCriteria)
        {
            var allCriteria = from baseCriteria in criteriaMetaData
                              join selectedCriteria in specifiedCriteria on baseCriteria.Code equals selectedCriteria.Key
                              select new { metaData = baseCriteria, selectedCriteria = selectedCriteria.Value };

            foreach (var criterion in allCriteria)
            {
                criterion.metaData.SetCriteriaValues(criterion.selectedCriteria);
            }
        }

    }

}
