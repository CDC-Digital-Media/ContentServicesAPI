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
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "proxycache")]
    public sealed class SerialProxyCache
    {
        
        
        [DataMember(Name = "id", Order = 1)]
        public string id { get; set; }

        [DataMember(Name = "url", Order = 2)]
        public string url { get; set; }

        [DataMember(Name = "datasetid", Order = 3)]
        public string datasetId { get; set; }

        [DataMember(Name = "data", Order = 4)]
        public string data { get; set; }

        [DataMember(Name = "expirationdatetime", Order = 5)]
        public string expirationDateTime { get; set; }

        [DataMember(Name = "expirationinterval", Order = 6)]
        public string expirationInterval { get; set; }

        [DataMember(Name = "needsrefresh", Order = 7)]
        public string needsRefresh { get; set; }

        [DataMember(Name = "status", Order = 8)]
        public string status { get; set; }

        [DataMember(Name = "failures", Order = 9)]
        public string failures { get; set; }
    }
}
