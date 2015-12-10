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
using Gov.Hhs.Cdc.SearchProvider.Bo;

namespace Gov.Hhs.Cdc.SearchProvider
{
    public class GenericSearchProvider<T> : ISearchProvider
        where T : DataSetResult
    {
        bool ISearchProvider.CanPage
        {
            get { return false; }
        }

        bool ISearchProvider.CanSort
        {
            get { throw new NotImplementedException(); }
        }

        bool ISearchProvider.SupportsEfficientIntermediateRequests
        {
            get { throw new NotImplementedException(); }
        }

        bool ISearchProvider.CanIndexByLocation
        {
            get { throw new NotImplementedException(); }
        }

        bool ISearchProvider.SupportsPartialResults
        {
            get { throw new NotImplementedException(); }
        }

        DataSetResult ISearchProvider.Get(SearchParameters searchParameters)
        {
            return Search(searchParameters) as DataSetResult;
        }

        DataSetResult ISearchProvider.NavigatePages(string resultSetId, int pageNumber)
        {
            throw new NotImplementedException();
        }

        public virtual T Search(SearchParameters searchParameters)
        {
            return (this as ISearchProvider).Get(searchParameters) as T;
        }
    }
}
