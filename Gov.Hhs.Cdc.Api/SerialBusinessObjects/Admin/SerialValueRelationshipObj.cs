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

using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "relationships")]
    public sealed class SerialValueRelationshipObj
    {
        [DataMember(Name = "type", Order = 1, EmitDefaultValue = true)]
        public string type { get; set; }

        [DataMember(Name = "valueId", Order = 2, EmitDefaultValue = true)]
        public string valueId { get; set; }

        [DataMember(Name = "isActive", Order = 3, EmitDefaultValue = true)]
        public string isActive { get; set; }

        public SerialValueRelationshipObj()
        {
            valueId = "0";
            isActive = "true";
        }

        public ValueRelationshipObject GetValueRelationship(ValueObject value)
        {
            return new ValueRelationshipObject()
                {
                    ShortType = type,
                    ValueId = Convert.ToInt32(value.ValueId),
                    ValueLanguageCode = value.LanguageCode,
                    RelatedValueId = Convert.ToInt32(valueId),
                    RelatedValueLanguageCode = value.LanguageCode,
                    DisplayOrdinal = 0,
                    IsActive = Convert.ToBoolean(isActive),
                    IsBeingDeleted = false
                };

        }
    }
}
