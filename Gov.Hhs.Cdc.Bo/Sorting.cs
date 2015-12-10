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

namespace Gov.Hhs.Cdc.Bo
{
    public class Sorting
    {
        public bool IsSorted { get; set; }
        private List<SortColumn> _sortColumns;

        public List<SortColumn> SortColumns
        {
            get { return _sortColumns ?? (_sortColumns = new List<SortColumn>()); }
            set { _sortColumns = value; }
        }

        public Sorting()
        {
            IsSorted = false;
        }

        public Sorting(SortColumn sortColumn)
        {
            IsSorted = true;
            SortColumns = new List<SortColumn>() { sortColumn };
        }

        public Sorting(List<SortColumn> sortColumns)
        {
            SortColumns = sortColumns;
            IsSorted = sortColumns.Count > 0;
        }
    }

}
