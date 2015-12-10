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
using System.Reflection;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    /// <summary>
    /// The search provider interface defines the search providers that can be used by the search controller.  There
    /// can be more than one search provider for each business object to search, but currently content services only
    /// uses one search provider per business object.
    /// 
    /// The flags in this interface are used by the search controller to determine how to cache and combine the results
    /// fraom each search provider.
    /// </summary>
    public interface ISearchProvider
    {
        /// <summary>
        /// Which applications will this provider return data for (i.e., media, user)
        /// </summary>
        string ApplicationCode { get; }

        string Code { get; }

        string Name { get; }

        /// <summary>
        /// Can this search provider return specific pages from the result
        /// </summary>
        bool CanPage { get; }

        /// <summary>
        /// Can this search provider sort the results
        /// </summary>
        bool CanSort { get; }

        /// <summary>
        /// Is it efficient to select specific pages from this search provider (i.e., SQL select)
        /// </summary>
        bool SupportsEfficientIntermediateRequests { get; }

        /// <summary>
        /// Can the search provider select data starting at a certain index
        /// Note:  Currently not used
        /// </summary>
        bool CanIndexByLocation { get; }

        /// <summary>
        /// If the search has gone over the specified time limit, can it return what has been collected so far
        /// /// Note:  Currently not used
        /// </summary>
        bool SupportsPartialResults { get; }

        List<Assembly> BusinessObjectAssemblies { get;}

        /// <summary>
        /// Get the initial search results
        /// </summary>
        /// <param name="searchParameters"></param>
        /// <returns></returns>
        DataSetResult Get(SearchParameters searchParameters, ResultSetParms resultSetParms);

        DataSetResult Get(SearchParameters searchParameters, ResultSetParms resultSetParms, IList<int> allowableMediaIds);

        ISearchDataManager GetSearchDataManager(string filterCode);
    }
}
