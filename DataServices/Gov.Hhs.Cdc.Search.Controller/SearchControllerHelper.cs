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
using Gov.Hhs.Cdc.Search.Provider;


namespace Gov.Hhs.Cdc.Search.Controller
{
    public class SearchControllerHelper
    {

        #region InjectedProperties
        internal ISortProvider SortProvider { get; set; }
        internal ISearchCacheMgr CacheMgr { get; set; }
        #endregion


        #region ClassProperties
        internal List<ISearchProvider> SearchProviders { get; set; }
        internal SearchParameters SearchParameters { get; set; }
        internal SearchControllerType SearchControllerType { get; set; }
        internal ResultSetParms ResultSetParms { get; set; }
        #endregion

        internal SearchControllerHelper(
            List<ISearchProvider> searchProviders, SearchParameters searchParameters,
            ResultSetParms resultSetParms, SearchControllerType searchControllerType,
            ISortProvider sortProvider, ISearchCacheMgr searchCacheMgr)
        {

            this.SearchProviders = searchProviders;
            this.SearchParameters = searchParameters;
            this.ResultSetParms = resultSetParms;
            this.SearchControllerType = searchControllerType;
            SortProvider = sortProvider;
            CacheMgr = searchCacheMgr;
        }

        internal DataSetResult GetOrderedDataSet()
        {
            return GetOrderedDataSet(SearchParameters);
        }

        internal DataSetResult GetOrderedDataSet(IList<int> allowableMediaIds)
        {
            return GetOrderedDataSet(SearchParameters, allowableMediaIds);
        }

        internal DataSetResult GetOrderedDataSet(SearchParameters searchParameters)
        {
            List<DataSetResult> results = SearchProviders.Select(s => s.Get(searchParameters, ResultSetParms)).ToList();

            DataSetResult result = SortProvider.Collate(results, SearchParameters.Sorting);
            if (searchParameters.Paging.IsPaged)
                result.SetPageSize(searchParameters.Paging.PageSize);
            return result;
        }

        internal DataSetResult GetOrderedDataSet(SearchParameters searchParameters, IList<int> allowableMediaIds)
        {
            List<DataSetResult> results = SearchProviders.Select(s => s.Get(searchParameters, ResultSetParms, allowableMediaIds)).ToList();

            DataSetResult result = SortProvider.Collate(results, SearchParameters.Sorting);
            if (searchParameters.Paging.IsPaged)
                result.SetPageSize(searchParameters.Paging.PageSize);
            return result;
        }

        internal ResultSetParameters GetCachedParameters()
        {
            return CacheMgr.GetCachedParameters(ResultSetParms.ResultSetId);
        }

        internal DataSetResult CacheAndGetPage(DataSetResult dataSetResult, int pageNumber, int offset = 0)
        {
            CacheResultSetParms(dataSetResult);
            CacheMgr.Insert(dataSetResult, dataSetResult.Id);
            return dataSetResult.GetPage(pageNumber, offset);
        }

        internal void CacheResultSetParms(DataSetResult result)
        {
            ResultSetParameters parms = new ResultSetParameters()
            {
                ResultSetId = this.ResultSetParms.ResultSetId,
                AllRecordsCached = result.RecordCount >= result.TotalRecordCount,
                NumberOfPagesCached = result.CompletePageCount,
                SearchParameters = SearchParameters,
                SearchControllerType = SearchControllerType
            };

            CacheMgr.Insert(parms);
        }

        internal DataSetResult GetDataSetResultFromCache()
        {
            return CacheMgr.GetDataSetResult(ResultSetParms.ResultSetId);
        }
    }
}
