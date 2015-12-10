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
    [DataContract(Name = "vocabularyRelation")]
    public sealed class SerialVocabularyRelation
    {
        [DataMember(Name = "valueId", Order = 1, EmitDefaultValue = true)]
        public int valueId { get; set; }

        [DataMember(Name = "type", Order = 2, EmitDefaultValue = true)]
        public string type { get; set; }

        [DataMember(Name = "description", Order = 3, EmitDefaultValue = true)]
        public string description { get; set; }

        [DataMember(Name = "relatedValueId", Order = 4, EmitDefaultValue = true)]
        public int relatedValueId { get; set; }

        [DataMember(Name = "relatedValueLanguageCode", Order = 5, EmitDefaultValue = true)]
        public string relatedValueLanguageCode { get; set; }

        [DataMember(Name = "relatedValueName", Order = 6, EmitDefaultValue = true)]
        public string relatedValueName { get; set; }
    }
}
