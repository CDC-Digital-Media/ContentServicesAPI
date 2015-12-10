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
using System.IO;
using System.Linq;
using System.Reflection;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.Search.Provider
{
    public class SearchProviders : ISearchProviders
    {
        private Dictionary<string, List<ISearchProvider>> _byApplication = null;
        public Dictionary<string, List<ISearchProvider>> ByApplication {get { return _byApplication; } }

        public List<ISearchProvider> List
        {
            get { return ByApplication.SelectMany(p => (List<ISearchProvider>) p.Value).ToList(); }
        }

        public SearchProviders(params ISearchProvider[] providers)
        {
            _byApplication = new Dictionary<string,List<ISearchProvider>>();
            foreach (ISearchProvider provider in providers)
            {
                AddProviderToDictionary(provider);
            }            
        }

        public bool AllMustCacheEntireDataSet(string applicationCode, bool sortingRequired)
        {
            bool anyDontHaveToCache = (from p in ByApplication[applicationCode]
                                       where SearchProviderHelper.DontHaveToCacheEntireDataSet(p, sortingRequired)
                        select p).Any();

            return !anyDontHaveToCache;
        }

        public bool AnyMustCacheEntireDataSet(string applicationCode, bool sortingRequired)
        {
            bool anyMustHaveToCache = (from p in ByApplication[applicationCode]
                                       where SearchProviderHelper.MustCacheEntireDataSet(p, sortingRequired)
                                       select p).Any();

            return !anyMustHaveToCache;
        }

        private void AddProviderToDictionary(ISearchProvider provider)
        {
            if (ByApplication.ContainsKey(provider.ApplicationCode))
                ByApplication[provider.ApplicationCode].Add(provider);
            else
                ByApplication.Add(provider.ApplicationCode, new List<ISearchProvider>() { provider });
        }


    }

}
