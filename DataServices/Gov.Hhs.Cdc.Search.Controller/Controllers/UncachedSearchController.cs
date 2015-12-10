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
using Gov.Hhs.Cdc.Search.Provider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Search.Controller
{
    /// <summary>
    /// The UncachedSearchController is used when only the plan is to be cached, and not the 
    /// data
    /// </summary>
    public class UncachedSearchController : ISearchController
    {
        private SearchControllerHelper Helper { get; set; }
        public UncachedSearchController(SearchControllerHelper helper)
        {
            Helper = helper;
        }

        public DataSetResult Get()
        {
            DataSetResult dataSetResult = Helper.GetOrderedDataSet();
            Helper.CacheResultSetParms(dataSetResult);
            return dataSetResult;
        }

        public DataSetResult Get(IList<int> allowableMediaIds)
        {
            DataSetResult dataSetResult = Helper.GetOrderedDataSet(allowableMediaIds);
            Helper.CacheResultSetParms(dataSetResult);
            return dataSetResult;
        }

        

        public DataSetResult NavigatePages(int pageNumber, int offset = 0)
        {
            Helper.GetCachedParameters();
            return Helper.GetOrderedDataSet();
        }

    }
}
