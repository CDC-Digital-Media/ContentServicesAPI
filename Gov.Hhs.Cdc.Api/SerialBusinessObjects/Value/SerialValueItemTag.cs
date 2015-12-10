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

using System.Web;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "tag")]
    public sealed class SerialValueItemTag
    {
        [DataMember(Order = 1, EmitDefaultValue = true)]
        public int id { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = true)]
        public string name { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = true)]
        public string language { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = true)]
        public string type { get; set; }
    }
}
