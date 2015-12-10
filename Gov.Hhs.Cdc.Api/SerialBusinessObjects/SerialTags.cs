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

using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "tags")]
    public sealed class SerialTags
    {
        [DataMember(Order = 1)]
        public List<SerialValueItem> topic { get; set; }
        
        [DataMember(Order = 2)]
        public List<SerialValueItem> audience { get; set; }

        //[DataMember(Name = "campaign", Order = 3)]
        //public List<SerialValueItem> campaign { get; set; }
    }
}
