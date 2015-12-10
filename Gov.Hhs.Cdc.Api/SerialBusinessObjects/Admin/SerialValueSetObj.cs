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

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "valueSet")]
    public sealed class SerialValueSetObj
    {
        [DataMember(Name = "id", Order = 1, EmitDefaultValue = true)]
        public int id { get; set; }

        [DataMember(Name = "name", Order = 2, EmitDefaultValue = true)]
        public string name { get; set; }

        [DataMember(Name = "language", Order = 3, EmitDefaultValue = true)]
        public string language { get; set; }

        [DataMember(Name = "description", Order = 4, EmitDefaultValue = true)]
        public string description { get; set; }

        [DataMember(Name = "displayOrdinal", Order = 5, EmitDefaultValue = true)]
        public string displayOrdinal { get; set; }

        [DataMember(Name = "isActive", Order = 6, EmitDefaultValue = true)]
        public string isActive { get; set; }

        [DataMember(Name = "isDefaultable", Order = 7, EmitDefaultValue = true)]
        public string isDefaultable { get; set; }

        [DataMember(Name = "isOrderable", Order = 8, EmitDefaultValue = true)]
        public string isOrderable { get; set; }

        [DataMember(Name = "isHierachical", Order = 9, EmitDefaultValue = true)]
        public string isHierachical { get; set; }

        public SerialValueSetObj()
        {
            name = string.Empty;
            language = string.Empty;
            description = string.Empty;
            displayOrdinal = "0";
            isActive = string.Empty;
            isDefaultable = string.Empty;
            isOrderable = string.Empty;
            isHierachical = string.Empty;
        }
    }
}
