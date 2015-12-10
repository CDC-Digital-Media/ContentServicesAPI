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

using System.Runtime.Serialization;
using System.Collections.Generic;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "domain")]
    public class SerialUserOrgDomain
    {
        //[DataMember(Name = "orgId", Order = 1)]
        //public int orgId { get; set; }

        [DataMember(Name = "url", Order = 1)]
        public string url { get; set; }

        [DataMember(Name = "isDefault", Order = 2)]
        public bool? isDefault { get; set; }

        public SerialUserOrgDomain(OrganizationDomainObject item)
        {
            //orgId = item.OrganizationID;
            url = item.DomainName;
            isDefault = item.IsDefault;
        }

        public SerialUserOrgDomain()
        {
        }
    }
}
