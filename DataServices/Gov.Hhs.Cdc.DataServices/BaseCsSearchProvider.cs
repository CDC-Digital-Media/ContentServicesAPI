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
using System.Reflection;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.DataServices
{
    public abstract class BaseCsSearchProvider : ISearchProvider
    {
        public abstract string ApplicationCode { get; }
        public abstract List<Assembly> BusinessObjectAssemblies { get; }

        public DataSetResult Get(SearchParameters searchParameters, ResultSetParms resultSetParms)
        {
            ISearchDataManager dataManager = GetSearchDataManager(searchParameters.FilterCode);
            return dataManager.GetData(searchParameters, resultSetParms);
        }

        public DataSetResult Get(SearchParameters searchParameters, ResultSetParms resultSetParms, IList<int> allowableMediaIds)
        {
            ISearchDataManager dataManager = GetSearchDataManager(searchParameters.FilterCode);
            return dataManager.GetData(searchParameters, resultSetParms, allowableMediaIds);
        }
        

        public abstract ISearchDataManager GetSearchDataManager(string filterCode);

        public static string SafeToLower(string value)
        {
            return value == null ? null : value.ToLower();
        }

        public abstract string Code { get; }
        public abstract string Name { get; }

        public bool CanPage { get { return true; } }
        public bool CanSort { get { return true; } }
        public bool SupportsEfficientIntermediateRequests { get { return true; } }
        public bool CanIndexByLocation { get { return false; } }
        public bool SupportsPartialResults { get { return false; } }


    }

}
