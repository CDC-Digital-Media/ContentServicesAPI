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

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "vocabularyList")]
    public sealed class SerialVocab
    {
        public SerialVocab()
        {
            this.items = new List<SerialVocab>();
        }

        [DataMember(Name = "id", Order = 1, EmitDefaultValue = false)]
        public int id { get; set; }

        [DataMember(Name = "name", Order = 2, EmitDefaultValue = false)]
        public string name { get; set; }

        [DataMember(Name = "description", Order = 3, EmitDefaultValue = false)]
        public string description { get; set; }

        [DataMember(Name = "language", Order = 4, EmitDefaultValue = false)]
        public string language { get; set; }
               
        [DataMember(Name = "mediaUsageCount", Order = 5, EmitDefaultValue = false)]
        public int? mediaUsageCount { get; set; }

        [DataMember(Name = "displayOrdinal", Order = 6, EmitDefaultValue = true)]
        public int displayOrdinal { get; set; }
        
        [DataMember(Name = "items", Order = 7, EmitDefaultValue = false)]
        public List<SerialVocab> items { get; set; }
    }
}
