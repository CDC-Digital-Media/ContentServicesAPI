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
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.DataServices
{
    public static class FilterCriterionFactory
    {
        public static List<FilterCriterion> CreateFilterCriterion(IEnumerable<FilterCriteriaDefinition> criterionDtos)
        {
            return criterionDtos.Select(c => FilterCriterionFactory.Create(c)).ToList();
        }

        private static FilterCriterion Create(FilterCriteriaDefinition dto)
        {
            FilterCriterion parameter = null;
            switch (dto.Type.ToLower())
            {
                case "singleselect":
                    parameter = new FilterCriterionSingleSelect();
                    FilterCriterionSingleSelect criterionSingleSelect = (FilterCriterionSingleSelect)parameter;
                    criterionSingleSelect.KeyType = ListItem.GetKeyType(dto.ListKeyType);
                    break;
                case "multiselect":
                    parameter = new FilterCriterionMultiSelect();
                    FilterCriterionMultiSelect criterionMultiSelect = (FilterCriterionMultiSelect)parameter;
                    criterionMultiSelect.KeyType = ListItem.GetKeyType(dto.ListKeyType);
                    break;
                case "hiermultiselect":
                    parameter = new FilterCriterionHierarchicalMultiSelect();
                    FilterCriterionHierarchicalMultiSelect hierParm = (FilterCriterionHierarchicalMultiSelect)parameter;
                    hierParm.KeyType = ListItem.GetKeyType(dto.ListKeyType);
                    break;
                case "text" :
                    parameter = FilterCriterionText.CreateWithTextType(dto.TextType);
                    break;
                case "boolean":
                    parameter = new FilterCriterionBoolean();
                    break;
                case "daterange":
                    parameter = new FilterCriterionDateRange(dto.AllowDateInPast, dto.AllowDateInFuture);
                    break;
                default:
                    throw new ApplicationException("Parameter Type '" + dto.Type + "' is invalid");
            }
            parameter.ApplicationCode = dto.ApplicationCode;
            parameter.FilterCode = dto.FilterCode;
            parameter.Code = dto.Code;
            parameter.Name = dto.DisplayName;
            parameter.DbColumnName = dto.DbColumnName;  
            parameter.Required = dto.IsRequired;
            parameter.DisplayNote = dto.DisplayNote;
            parameter.GroupCode = dto.GroupCode;
            parameter.GroupName = dto.GroupName;
            return parameter;
        }



    }
}
