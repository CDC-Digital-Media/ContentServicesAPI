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

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "organizations")]
    public sealed class SerialUserOrg
    {
        [DataMember(Name = "id", Order = 1)]
        public int id { get; set; }

        [DataMember(Name = "name", Order = 2)]
        public string name { get; set; }

        [DataMember(Name = "website", Order = 3)]
        public List<SerialUserOrgDomain> website { get; set; }

        [DataMember(Name = "type", Order = 4)]
        public string type { get; set; }

        [DataMember(Name = "description", Order = 5)]
        public string description { get; set; }

        [DataMember(Name = "address1", Order = 6)]
        public string address1 { get; set; }

        [DataMember(Name = "address2", Order = 7)]
        public string address2 { get; set; }

        [DataMember(Name = "city", Order = 8)]
        public string city { get; set; }

        [DataMember(Name = "stateProvince", Order = 9)]
        public string stateProvince { get; set; }

        [DataMember(Name = "postalCode", Order = 10)]
        public string postalCode { get; set; }

        [DataMember(Name = "county", Order = 11)]
        public string county { get; set; }

        [DataMember(Name = "country", Order = 12)]
        public string country { get; set; }

        //used for testing only
        public SerialUserOrg()
        {
        }
    }
}
