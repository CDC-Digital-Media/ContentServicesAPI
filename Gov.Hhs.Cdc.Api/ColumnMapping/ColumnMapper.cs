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

namespace Gov.Hhs.Cdc.Api
{
    public class ColumnMapper
    {
        private Dictionary<string, string> Map { get; set; }
        private List<SortColumn> DefaultSortColumns { get; set; }

        private bool HasDefaultSortColumns { get { return DefaultSortColumns != null && DefaultSortColumns.Count() > 0; } }

        public ColumnMapper(Dictionary<string, string> map, List<SortColumn> defaultSortColumns)
        {
            Map = map;
            DefaultSortColumns = defaultSortColumns;
        }

        public Sorting GetMappedSortColumns(List<SortColumn> sortColumns)
        {
            if (sortColumns != null && sortColumns.Count > 0)
            {
                return new Sorting(MapTheSortColumns(sortColumns));
            }
            else if (HasDefaultSortColumns)
            {
                return new Sorting(MapTheSortColumns(DefaultSortColumns));
            }
            else
            {
                return new Sorting();
            }
        }

        private List<SortColumn> MapTheSortColumns(IEnumerable<SortColumn> sources)
        {
            return sources.Select(c => CreateSortColumn(c)).ToList();
        }

        private SortColumn CreateSortColumn(SortColumn c)
        {
            string value = Map.ContainsKey(c.Column) ? Map[c.Column] : c.Column;
            return new SortColumn(value, c.SortOrder);
        }
    }

}
