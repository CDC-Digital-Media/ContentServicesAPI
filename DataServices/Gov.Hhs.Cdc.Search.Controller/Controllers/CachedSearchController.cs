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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Search.Controller
{
    /// <summary>
    /// The CachedSearchController caches all pages of a dataset.  It is selected when the data set has the flag
    ///  IsSmallEnoughToBeCached set, or all search providers are configured to cache the entire dataset
    ///             dataSet.IsSmallEnoughToBeCached || providers.AllMustCacheEntireDataSet(searchParameters.ApplicationCode, searchParameters.Sorting.IsSorted))
    ///             
    /// </summary>
    public class CachedSearchController : ISearchController
    {
        private SearchControllerHelper Helper { get; set; }
        public CachedSearchController(SearchControllerHelper helper)
        {
            Helper = helper;
        }

        public DataSetResult Get(IList<int> allowableMediaIds)
        {
            throw new NotImplementedException();
        }

        public DataSetResult Get()
        {
            DataSetResult dataSetResult = Helper.GetOrderedDataSet(Helper.SearchParameters.WithAllPages());
            return Helper.CacheAndGetPage(dataSetResult, Helper.SearchParameters.Paging.PageNumber);
        }

        public DataSetResult NavigatePages(int pageNumber, int offset = 0)
        {
            ResultSetParameters parms = Helper.GetCachedParameters();
            return Helper.GetDataSetResultFromCache().GetPage(pageNumber);
        }
    }
}
