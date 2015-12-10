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
    /// The Dynamic Cached Controller will cache extra pages around the result set in order to improve the performance
    /// </summary>
    public class DynamicCachedController : ISearchController
    {
        private const int ExtraPageNumbersToGet = 5;
        private SearchControllerHelper Helper { get; set; }
        public DynamicCachedController(SearchControllerHelper helper)
        {
            Helper = helper;
        }

        public DataSetResult Get(IList<int> allowableMediaIds)
        { 
            throw new NotImplementedException(); 
        }

        public DataSetResult Get()
        {
            DataSetResult dataSetResult = Helper.GetOrderedDataSet(
                Helper.SearchParameters.WithPages(1, Helper.SearchParameters.Paging.PageNumber + ExtraPageNumbersToGet));    //Get up to 5 extra pages

            if (Helper.SearchParameters.Paging.Offset > 0)
            {
                return Helper.CacheAndGetPage(dataSetResult, dataSetResult.PageNumber, Helper.SearchParameters.Paging.Offset);
            }
            else
            {
                return Helper.CacheAndGetPage(dataSetResult, Helper.SearchParameters.Paging.PageNumber);
            }
        }

        public DataSetResult NavigatePages(int pageNumber, int offset = 0)
        {
            ResultSetParameters parms = Helper.GetCachedParameters();

            if (parms.AllRecordsCached || parms.NumberOfPagesCached >= pageNumber)
            {
                return Helper.GetDataSetResultFromCache().GetPage(pageNumber, offset);
            }

            DataSetResult dataSetResult = Helper.GetOrderedDataSet(
                Helper.SearchParameters.WithPages(1, pageNumber + ExtraPageNumbersToGet));    //Get up to page 5
            return Helper.CacheAndGetPage(dataSetResult, pageNumber);
        }

    }
}
