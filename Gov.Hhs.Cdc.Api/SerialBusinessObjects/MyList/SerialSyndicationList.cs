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
using System.Web;

using System.Runtime.Serialization;
using System.Web.Script.Serialization;

using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "syndicationList")]
    public sealed class SerialSyndicationList
    {
        [DataMember(Name = "syndicationListId", Order = 1, EmitDefaultValue = true)]
        public string syndicationListId { get; set; }

        [DataMember(Name = "domainName", Order = 2, EmitDefaultValue = true)]
        public string domainName { get; set; }

        [DataMember(Name = "userId", Order = 3, EmitDefaultValue = true)]
        public string userId { get; set; }

        [DataMember(Name = "listName", Order = 4, EmitDefaultValue = true)]
        public string listName { get; set; }

        [DataMember(Name = "statusCode", Order = 5, EmitDefaultValue = true)]
        public string statusCode { get; set; }

        [DataMember(Name = "isActive", Order = 6, EmitDefaultValue = true)]
        public bool isActive { get; set; }

        [DataMember(Name = "rowVersion", Order = 7, EmitDefaultValue = true)]
        public string rowVersion { get; set; }

        [DataMember(Name = "media", Order = 8, EmitDefaultValue = true)]
        public List<SerialSyndicationListMedia> media { get; set; }

        [DataMember(Name = "lastUpdatedUserEmailAddress", Order = 9, EmitDefaultValue = true)]
        public string lastUpdatedUserEmailAddress { get; set; }

    }

}
