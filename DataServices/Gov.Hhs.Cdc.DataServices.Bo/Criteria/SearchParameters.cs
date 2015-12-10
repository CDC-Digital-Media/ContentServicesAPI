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

using Gov.Hhs.Cdc.Bo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gov.Hhs.Cdc.DataServices.Bo
{

    public class SearchParameters
    {
        public string ApplicationCode { get; set; }
        public string _dataSetCode = null;
        public string DataSetCode 
        {             
            get 
            { 
                return string.IsNullOrEmpty(_dataSetCode) ? _filterCode : _dataSetCode;
            }
            set 
            {
                _dataSetCode = value;
            }
        }

        public string _filterCode = null;
        public string FilterCode { 
            get 
            { 
                return string.IsNullOrEmpty(_filterCode) ? _dataSetCode : _filterCode;
            }
            set 
            {
                _filterCode = value;
            }
        }
        public string ControlCode { get; set; }
        public string EnvironmentCode { get; set; }
        public string ActionCode { get; set; }

        public Criteria BasicCriteria { get; set; }
        public FilterCriteria TheCriteria { get; set; }

        private Paging _paging;
        public Paging Paging
        {
            get { return _paging ?? (_paging = new Paging()); }
            set { _paging = value; }
        }

        private Sorting _sorting;
        public Sorting Sorting
        {
            get { return _sorting ?? (_sorting = new Sorting()); }
            set { _sorting = value; }
        }

        public int SecondsToLive { get; set; }

        public SearchParameters(){}

        private SearchParameters(string applicationCode, string dataSetCode, string actionCode, Paging paging, int secondsToLive, Sorting sorting)
        {
            this.ApplicationCode = applicationCode;
            this.DataSetCode = dataSetCode;
            this.ActionCode = actionCode;
            this.Sorting = sorting;
            this.Paging = paging;
            this.SecondsToLive = secondsToLive;
        }

        public SearchParameters(string applicationCode, string dataSetCode, params Criterion[] criteria)
        {
            this.ApplicationCode = applicationCode;
            this.DataSetCode = dataSetCode;
            this.BasicCriteria = new Criteria(criteria);
        }

        public SearchParameters(string applicationCode, string dataSetCode, SortColumn sortColumn, params Criterion[] criteria)
            : this(applicationCode, dataSetCode, null, null, 0, new Sorting(sortColumn))
        {
            this.BasicCriteria = new Criteria(criteria);
        }
       
       [Obsolete]
        public SearchParameters(string applicationCode, string dataSetCode, string actionCode, Paging paging, int secondsToLive, Sorting sorting, params Criterion[] criteria)
            :this(applicationCode, dataSetCode, actionCode, paging, secondsToLive, sorting)
        {            
            this.BasicCriteria = new Criteria(criteria);
        }

        //Use this instead
        public SearchParameters(string applicationCode, string dataSetCode, Paging paging, int secondsToLive, Sorting sorting, FilterCriteria criteria)
            : this(applicationCode, dataSetCode, null, paging, secondsToLive, sorting)
        {
            this.TheCriteria = criteria;
        }

        public SearchParameters(string applicationCode, string dataSetCode, Paging paging, Sorting sorting, params Criterion[] criteria)
            : this(applicationCode, dataSetCode, null, paging, 0, sorting, criteria)
        {
        }


        public SearchParameters(params Criterion[] criteria)
            : this(null, null, null, null, 0, (Sorting) null, criteria)
        {
        }

        public SearchParameters(SortColumn sortColumn, params Criterion[] criteria)
            : this(null, null, null, null, 0, new Sorting(sortColumn), criteria)
        {
        }

        public SearchParameters(string applicationCode, string dataSetCode, Sorting sorting, params Criterion[] criteria)
            : this(applicationCode, dataSetCode, null, null, 0, sorting, criteria)
        {
        }
        public SearchParameters(SearchParameters original)
        {
            this.ApplicationCode = original.ApplicationCode;
            this.DataSetCode = original.DataSetCode;
            this.FilterCode = original.FilterCode;
            this.ControlCode = original.ControlCode;
            this.BasicCriteria = original.BasicCriteria;


            this.Paging = original.Paging;

            this.Sorting = original.Sorting;

            this.SecondsToLive = original.SecondsToLive;
        }

        public SearchParameters(SearchParameters original, Paging pageInfo, params Criterion[] criteria)
            : this(original)
        {
            this.Paging = pageInfo;
            this.Sorting = original.Sorting;
            this.SecondsToLive = original.SecondsToLive;

            if (criteria.ToList().Count > 0)
            {
                this.BasicCriteria = new Criteria(criteria);
            }
        }

        public SearchParameters WithAllPages()
        {
            return new SearchParameters(this, Paging.WithAllPages());
        }

        public SearchParameters WithPages(int startPage, int throughPage)
        {
            return new SearchParameters(this, Paging.WithPages(startPage, throughPage));
        }

    }
}
