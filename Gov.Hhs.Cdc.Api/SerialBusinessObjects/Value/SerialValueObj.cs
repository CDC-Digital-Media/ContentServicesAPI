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
    [DataContract(Name = "value")]
    public sealed class SerialValueObj
    {
        [DataMember(Name = "valueId", Order = 1, EmitDefaultValue = true)]
        public int valueId { get; set; }

        [DataMember(Name = "valueName", Order = 2, EmitDefaultValue = true)]
        public string valueName { get; set; }

        [DataMember(Name = "languageCode", Order = 3, EmitDefaultValue = true)]
        public string languageCode { get; set; }

        [DataMember(Name = "description", Order = 4, EmitDefaultValue = true)]
        public string description { get; set; }

        [DataMember(Name = "displayOrdinal", Order = 5, EmitDefaultValue = true)]
        public string displayOrdinal { get; set; }

        [DataMember(Name = "isActive", Order = 6, EmitDefaultValue = true)]
        public string isActive { get; set; }

        [DataMember(Name = "valueSetId", Order = 7, EmitDefaultValue = true)]
        public string valueSetId { get; set; }

        [DataMember(Name = "valueSet", Order = 8, EmitDefaultValue = true)]
        public string valueSet { get; set; }

        [DataMember(Name = "valueSet_isDefault", Order = 9, EmitDefaultValue = true)]
        public string valueSet_isDefault { get; set; }

        [DataMember(Name = "relationships", Order = 10, EmitDefaultValue = true)]
        public List<SerialValueRelationshipObj> relationships { get; set; }
        
        public SerialValueObj()
        {
            valueName = "";
            languageCode = "";
            description = "";
            displayOrdinal = "0";
            isActive = "true";
            valueSetId = "0";
            valueSet_isDefault = "false";
            relationships = new List<SerialValueRelationshipObj>();
        }

        public ValueObject GetValue(int valueId = 0)
        {
            ValueObject value = new ValueObject(valueId);

            if (!string.IsNullOrEmpty(displayOrdinal))
                value.DisplayOrdinal = Convert.ToInt32(displayOrdinal);

            if (!string.IsNullOrEmpty(valueName))
                value.ValueName = valueName;

            if (!string.IsNullOrEmpty(languageCode))
                value.LanguageCode = languageCode;

            if (!string.IsNullOrEmpty(description))
                value.Description = description;

            if (!string.IsNullOrEmpty(displayOrdinal))
                value.DisplayOrdinal = Convert.ToInt32(displayOrdinal);

            if (!string.IsNullOrEmpty(isActive))
                value.IsActive = Convert.ToBoolean(isActive);

            value.ValueToValueSetObjects = new List<ValueToValueSetObject>()
            {
                GetValueToValueSet(valueId, value.LanguageCode)
            };
            value.ValueRelationships = relationships.Select(r => r.GetValueRelationship(value));

            return value;
        }


        private ValueToValueSetObject GetValueToValueSet(int valueId, string languageCode)
        {
            ValueToValueSetObject valueToValueSet = new ValueToValueSetObject();
            valueToValueSet.ValueId = valueId;
            valueToValueSet.ValueLanguageCode = languageCode;
            //valueToValueSet.ValueToValueSetName 

            if (!string.IsNullOrEmpty(valueSetId))
                valueToValueSet.ValueSetId = Convert.ToInt32(valueSetId);

            valueToValueSet.ValueSetLanguageCode = languageCode;
            //valueToValueSet.DisplayOrdinal { get; set; }
            if (!string.IsNullOrEmpty(valueSet_isDefault))
                valueToValueSet.IsDefault = Convert.ToBoolean(valueSet_isDefault);

            return valueToValueSet;
        }

    }

}
