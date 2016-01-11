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

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "items")]
    public sealed class SerialValidationMessage
    {
        [DataMember(Name = "type", Order = 1, EmitDefaultValue = true)]
        public string type { get; set; }

        [DataMember(Name = "code", Order = 2, EmitDefaultValue = true)]
        public string code { get; set; }

        [DataMember(Name = "id", Order = 3, EmitDefaultValue = true)]
        public string id { get; set; }

        [DataMember(Name = "userMessage", Order = 4, EmitDefaultValue = true)]
        public string userMessage { get; set; }

        [DataMember(Name = "developerMessage", Order = 5, EmitDefaultValue = true)]
        public string developerMessage { get; set; }

        public SerialValidationMessage() { }

        public SerialValidationMessage Error(Exception ex)
        {
            type = ValidationMessage.ValidationSeverity.Error.ToString();
            code = "";
            id = "";
            userMessage = ex.Message;
            developerMessage = ex.GetBaseException().Message;

            return this;
        }

        public string combinedMessage { get { return userMessage + " : " + developerMessage; } }
    }
}
