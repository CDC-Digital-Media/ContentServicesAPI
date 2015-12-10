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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataProvider
{
    [FilteredDataSet]
    public class ProxyCacheAppKeyObject : DataSourceBusinessObject
    {
        public string ProxyCacheAppKeyId { get; set;  }

        [FilterSelection(Code = "DescriptionContains", CriterionType = FilterCriterionType.Text, TextType = FilterTextType.Contains)]
        public string Description { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.Boolean)]
        public bool IsActive { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }
}
