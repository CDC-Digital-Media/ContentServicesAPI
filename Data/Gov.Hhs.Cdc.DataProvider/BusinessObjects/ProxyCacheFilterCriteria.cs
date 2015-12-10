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

namespace Gov.Hhs.Cdc.DataProvider
{
    public class ProxyCacheFilterCriteria
    {
        FilterCriteria _filterCriteria;
        public FilterCriteria TheFilterCriteria
        {
            get { return _filterCriteria ?? (_filterCriteria = new FilterCriteria()); }
        }

        public string Url
        {
            get { return TheFilterCriteria.GetStringKey("Url"); }
            set { TheFilterCriteria.Set("Url", new FilterCriterionText(value)); }
        }

        public string UrlContains
        {
            get { return TheFilterCriteria.GetStringKey("UrlContains"); }
            set { TheFilterCriteria.Set("UrlContains", new FilterCriterionText(value)); }
        }

        public List<string> DatasetId
        {
            get { return TheFilterCriteria.GetStringKeys("DatasetId"); }
            set { TheFilterCriteria.Set("DatasetId", new FilterCriterionMultiSelect(value)); }
        }

        public FilterCriterionDateRange ExpirationDate
        {
            get { return (FilterCriterionDateRange)TheFilterCriteria.GetCriterion("ExpirationDate"); }
            set { TheFilterCriteria.Set("ExpirationDate", value); }
        }

        public FilterCriterionDateRange IsExpired
        {
            get { return (FilterCriterionDateRange)TheFilterCriteria.GetCriterion("IsExpired"); }
            set { TheFilterCriteria.Set("IsExpired", value); }
        }

        public bool? NeedsRefresh
        {
            get { return TheFilterCriteria.GetCriterionBoolValue("NeedsRefresh"); }
            set { TheFilterCriteria.Set("NeedsRefresh", new FilterCriterionBoolean(value)); }
        }
    }
}
