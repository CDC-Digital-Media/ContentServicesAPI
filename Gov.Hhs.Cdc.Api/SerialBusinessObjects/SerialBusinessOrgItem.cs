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
    [DataContract(Name = "businessorg")]
    public sealed class SerialBusinessOrgItem
    {
        [DataMember(Name = "id", Order = 1)]
        public int id { get; set; }

        [DataMember(Name = "parentId", Order = 2)]
        public int? parentId { get; set; }

        [DataMember(Name = "name", Order = 3)]
        public string name { get; set; }

        [DataMember(Name = "sourceCode", Order = 4)]
        public string sourceCode { get; set; }

        [DataMember(Name = "active", Order = 5)]
        public bool active { get; set; }

        [DataMember(Name = "typeCode", Order = 6)]
        public string typeCode { get; set; }

        [DataMember(Name = "geoNameId", Order = 7)]
        public int? geoNameId { get; set; }
    }
}
