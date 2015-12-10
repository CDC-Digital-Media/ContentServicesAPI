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
using Gov.Hhs.Cdc.DataServicesCacheProvider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    [CacheSelection(/*GroupCode = "Framework",*/ ReturnsList = true, IsPersisted = true)]
    public class SearchProviderReference : DefaultableBusinessObject
    {
        [CacheSelectionKey]
        public string ApplicationCode { get; set; }

        [CacheSelectionKey]
        public string DataSetCode { get; set; }

        public string Code { get; set; }

        //public string Name { get; set; }
        //public string ClassName { get; set; }
        //public string AssemblyName { get; set; }
    }
}
