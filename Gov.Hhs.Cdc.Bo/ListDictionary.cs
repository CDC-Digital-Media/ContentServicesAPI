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
    public class ListDictionary<k, d> : Dictionary<k, List<d>>
    {
        public void Add(k key, d dataItem)
        {
            if (this.ContainsKey(key))
            {
                this[key].Add(dataItem);
            }
            else
            {
                this.Add(key, new List<d>() { dataItem });
            }
        }

    }
}
