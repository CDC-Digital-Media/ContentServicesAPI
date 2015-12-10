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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataSource;
using Gov.Hhs.Cdc.Search.Provider;

namespace Gov.Hhs.Cdc.Search.Controller
{
    public class SearchControllerFactory : ISearchControllerFactory
    {

        #region InjectedProperties
        static ISortProvider _sortProvider;
        protected static ISortProvider SortProvider { get { return _sortProvider; } }

        static ISearchCacheMgr _cacheManager;
        protected static ISearchCacheMgr CacheManager { get { return _cacheManager; } }

        static ISearchProviders _searchProviders;
        protected static ISearchProviders SearchProviders { get { return _searchProviders; } }

        public static void Inject(ISortProvider sortProvider, ISearchCacheMgr cacheManager, ISearchProviders searchProviders) //, ISearchProviders allSearchProviders)
        {
            _sortProvider = sortProvider;
            _cacheManager = cacheManager;
            _searchProviders = searchProviders;
        }

        #endregion

        public ISearchController GetSearchController(SearchParameters searchParameters)
        {
            CacheManager.SetExpirationTimeInSeconds(searchParameters.SecondsToLive);
            ISearchProviders providers = SearchProviders;
            SearchControllerType searchControllerType = GetSearchControllerType(searchParameters, providers);

            SearchControllerHelper helper = new SearchControllerHelper(
                providers.ByApplication[searchParameters.ApplicationCode],
                searchParameters, new ResultSetParms(Guid.NewGuid(), searchParameters.SecondsToLive),
                searchControllerType, SortProvider, CacheManager);

            return GetSearchController(helper);
        }

        /// <summary>
        /// the search controller type (Cached, DynamicCached, IndexCached, Uncached) is determined using a set of business rules in the GetSearchControllerType method
        /// </summary>
        /// <param name="searchParameters"></param>
        /// <param name="dataSet"></param>
        /// <param name="providers"></param>
        /// <returns></returns>
        private SearchControllerType GetSearchControllerType(SearchParameters searchParameters, ISearchProviders providers)
        {
            return SearchControllerType.Uncached;
        }


        public ISearchController GetSearchController(Guid resultSetId)
        {
            ResultSetParameters parms = CacheManager.GetCachedParameters(resultSetId);
            if (parms == null)
            {
                throw new ApplicationException("Search Parameters Have Expired");
            }
            SearchParameters searchParameters = parms.SearchParameters;

            CacheManager.SetExpirationTimeInSeconds(searchParameters.SecondsToLive);
            ResultSetParms resultSetParms = new ResultSetParms(resultSetId, searchParameters.SecondsToLive);

            SearchControllerHelper helper = new SearchControllerHelper(
                SearchProviders.ByApplication[searchParameters.ApplicationCode], searchParameters, resultSetParms,
                parms.SearchControllerType,
                SortProvider, CacheManager);

            return GetSearchController(helper);

        }

        private static ISearchController GetSearchController(SearchControllerHelper helper)
        {

            switch (helper.SearchControllerType)
            {
                case SearchControllerType.Cached:
                    return new CachedSearchController(helper);
                case SearchControllerType.DynamicCached:
                    return new DynamicCachedController(helper);
                case SearchControllerType.IndexCached:
                    return new IndexedCachedController(helper);
                case SearchControllerType.Uncached:
                    return new UncachedSearchController(helper);
                default:
                    return new UncachedSearchController(helper);
            }
        }



    }
}
