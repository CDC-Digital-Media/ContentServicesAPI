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

namespace Gov.Hhs.Cdc.Search.Controller
{
    public class SearchControllerIocContainer : ISearchControllerIocContainer
    {
        private static ISearchControllerFactory _searchControllerFactory;
        public ISearchControllerFactory TheSearchControllerFactory { get { return _searchControllerFactory; } }

        static SearchControllerIocContainer()
        {
            _searchControllerFactory = new SearchControllerFactory();
        }

        public static void Inject(ISearchProviders searchProviders)
        {
            ISortProvider sortProvider = new SortProvider();
            ISearchCacheMgr searchCacheManager = new SearchCacheMgr();

            SearchControllerFactory.Inject(
                sortProvider,       //Does this really need to be injected since it is local?
                searchCacheManager,       //Does this really need to be injected since it is local?
                searchProviders);
        }
    }
}
