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
using System.Text;

using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "valueSet")]
    public sealed class SerialValueSetItem
    {
        [DataMember(Name = "id", Order = 1, EmitDefaultValue = true)]
        public int id { get; set; }

        [DataMember(Name = "languageCode", Order = 2, EmitDefaultValue = true)]
        public string languageCode { get; set; }

        //[DataMember(Name = "LanguageDescription", Order = 3, EmitDefaultValue = true)]
        //public string LanguageDescription { get; set; }

        [DataMember(Name = "name", Order = 4, EmitDefaultValue = true)]
        public string name { get; set; }

        [DataMember(Name = "description", Order = 5, EmitDefaultValue = true)]
        public string description { get; set; }

        [DataMember(Name = "attributeId", Order = 6, EmitDefaultValue = true)]
        public int? attributeId { get; set; }

        [DataMember(Name = "attributeName", Order = 7, EmitDefaultValue = true)]
        public string attributeName { get; set; }

        [DataMember(Name = "attributeDescription", Order = 8, EmitDefaultValue = true)]
        public string attributeDescription { get; set; }

        [DataMember(Name = "displayOrdinal", Order = 9, EmitDefaultValue = true)]
        public int displayOrdinal { get; set; }

        [DataMember(Name = "isActive", Order = 10, EmitDefaultValue = true)]
        public bool isActive { get; set; }

        [DataMember(Name = "isDefaultable", Order = 11, EmitDefaultValue = true)]
        public bool isDefaultable { get; set; }

        [DataMember(Name = "isOrderable", Order = 12, EmitDefaultValue = true)]
        public bool isOrderable { get; set; }

        [DataMember(Name = "isHierachical", Order = 13, EmitDefaultValue = true)]
        public bool isHierachical { get; set; }
    }
}
