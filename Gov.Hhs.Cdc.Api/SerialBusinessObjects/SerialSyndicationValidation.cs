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
    [DataContract(Name = "validation")]
    public sealed class SerialValidation
    {
        public SerialValidation()
        {
            messages = new List<string>();
        }

        [DataMember(Order = 10, EmitDefaultValue = true)]
        public bool urlAlreadyExists { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = true)]
        public bool isDuplicate { get; set; }

        [DataMember(Order = 25, EmitDefaultValue = true)]
        public int existingMediaId { get; set; }

        [DataMember(Order = 26, EmitDefaultValue = true)]
        public bool isValid { get; set; }

        [DataMember(Order = 30, EmitDefaultValue = true)]
        public string sourceCode { get; set; }
        
        [DataMember(Order = 40, EmitDefaultValue = true)]
        public string domainName { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = true)]
        public int numberOfElements { get; set; }

        [DataMember(Order = 60, EmitDefaultValue = true)]
        public bool isLoadable { get; set; }

        [DataMember(Order = 70, EmitDefaultValue = true)]
        public bool xHtmlLoadable { get; set; }
        
        [DataMember(Order = 90, EmitDefaultValue = true)]
        public List<string> messages { get; set; }
    }
}
