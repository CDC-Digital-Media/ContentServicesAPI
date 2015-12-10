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

namespace Gov.Hhs.Cdc.SearchProvider
{
    public class SearchResults
    {
        public List<object> OutputRecords { get; set; }
        public int TotalRecordsFound { get; set; }
        public int NumberRecordsReturned { get; set; }
        public bool ResultsAreSorted { get; set; }

        //TODO: If we page this result set, but add other search providers that are to be sorted with this dataset, then this paging does not work.
        //How do we improve performance of this result.
        public bool ResultsArePaged { get; set; }

    }
}
