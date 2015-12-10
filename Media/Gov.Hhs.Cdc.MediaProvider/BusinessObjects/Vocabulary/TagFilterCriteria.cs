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

using Gov.Hhs.Cdc.DataServices.Bo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gov.Hhs.Cdc.MediaProvider
{
    public class TagFilterCriteria
    {
        FilterCriteria _filterCriteria;
        public FilterCriteria TheFilterCriteria
        {
            get { return _filterCriteria ?? (_filterCriteria = new FilterCriteria()); }
        }

        public List<string> LanguageCode
        {
            get { return TheFilterCriteria.GetStringKeys("LanguageCode"); }
            set { TheFilterCriteria.Set("LanguageCode", new FilterCriterionMultiSelect(value)); }
        }

        public int? RelatedTagId
        {
            get { return TheFilterCriteria.GetIntKey("RelatedTagId"); }
            set { TheFilterCriteria.Set("RelatedTagId", new FilterCriterionSingleSelect(value)); }
        }

        public int? MediaId
        {
            get { return TheFilterCriteria.GetIntKey("MediaId"); }
            set { TheFilterCriteria.Set("MediaId", new FilterCriterionSingleSelect(value)); }
        }

        public List<int> TagId
        {
            get { return TheFilterCriteria.GetIntKeys("TagId"); }
            set { TheFilterCriteria.Set("TagId", new FilterCriterionMultiSelect(value)); }
        }

        public List<string> TagName
        {
            get { return TheFilterCriteria.GetStringKeys("TagName"); }
            set { TheFilterCriteria.Set("TagName", new FilterCriterionMultiSelect(value)); }
        }

        public string TagNameContains
        {
            get { return TheFilterCriteria.GetStringKey("TagNameContains"); }
            set { TheFilterCriteria.Set("TagNameContains", new FilterCriterionText(value)); }
        }

        public List<int> TagTypeId
        {
            get { return TheFilterCriteria.GetIntKeys("TagTypeId"); }
            set { TheFilterCriteria.Set("TagTypeId", new FilterCriterionMultiSelect(value)); }
        }

        public List<string> TagTypeName
        {
            get { return TheFilterCriteria.GetStringKeys("TagTypeName"); }
            set { TheFilterCriteria.Set("TagTypeName", new FilterCriterionMultiSelect(value)); }
        }

    }
}
