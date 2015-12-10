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
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "organization")]
    public sealed class SerialUserOrgItem
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public string name { get; set; }

        [DataMember(Order = 3)]
        public List<SerialUserOrgDomain> website { get; set; }

        [DataMember(Order = 4)]
        public string type { get; set; }

        [ScriptIgnore]
        [DataMember(Order = 5)]
        public string typeOther { get; set; }

        [DataMember(Order = 6)]
        public string description { get; set; }

        [DataMember(Name = "default", Order = 7)]
        public bool @default { get; set; }

        [DataMember(Order = 8)]
        public string address { get; set; }

        [DataMember(Order = 9)]
        public string addressContinued { get; set; }

        [DataMember(Order = 10)]
        public string city { get; set; }

        [DataMember(Order = 11)]
        public string stateProvince { get; set; }

        [DataMember(Order = 12)]
        public string postalCode { get; set; }

        [DataMember(Order = 13)]
        public string county { get; set; }

        [DataMember(Order = 14)]
        public string country { get; set; }

        [DataMember(Order = 15)]
        public int? geoNameId { get; set; }

        public SerialUserOrgItem()
        {
        }

        public SerialUserOrgItem(int id)
        {
            this.id = id;
        }

    }
}
