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
using Gov.Hhs.Cdc.Search.Controller;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Search.Provider;
using Gov.Hhs.Cdc.DataSource.Bll;
using Gov.Hhs.Cdc.CdcRegistrationProvider;
using Gov.Hhs.Cdc.CdcMediaProvider;

namespace Gov.Hhs.Cdc.Global
{
    public class CsSearchProviderFactory : ISearchProviderFactory
    {
        #region InjectedProperties
        private static ISearchProviders AllSearchProviders { get; set; }
        public static void Inject(ISearchProviders allSearchProviders)
        {
            AllSearchProviders = allSearchProviders;
        }
        #endregion

        public ISearchProviders Get()
        {
            return AllSearchProviders;
        }

        //SearchProviders _searchProviders = null;
        //SearchProviders SearchProviders { get { return _searchProviders; } }

        //To Do: Replace Get with injection of search providers into the SearchControllerFactory
        //public SearchProviders Get()
        //{
        //    IFilterCriteriaProvider filterCriteriaProvider = new FilterCriteriaProvider();
        //    List<ISearchProvider> searchProviderList = new List<ISearchProvider>()
        //    {
        //        (ISearchProvider) new CsMediaSearchProvider(filterCriteriaProvider),
        //        (ISearchProvider) new RegistrationSearchProvider(filterCriteriaProvider)
        //    };
        //    SearchProviders searchProviders = new SearchProviders(searchProviderList);
        //    return searchProviders;

        //}
    }
}
