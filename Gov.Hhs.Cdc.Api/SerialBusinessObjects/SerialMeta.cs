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

using System.Web;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Bo;
//using System.Net;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "meta")]
    public sealed class SerialMeta
    {
        [DataMember(Name = "status", Order = 1, EmitDefaultValue = true)]
        public int status { get; set; }

        [DataMember(Name = "message", Order = 2, EmitDefaultValue = true)]
        public List<SerialValidationMessage> message { get; set; }

        [DataMember(Name = "resultSet", Order = 3, EmitDefaultValue = true)]
        public SerialResultSet resultSet { get; set; }

        [DataMember(Name = "pagination", Order = 4, EmitDefaultValue = true)]
        public SerialPagination pagination { get; set; }
        
        public SerialMeta()
        {
            message = new List<SerialValidationMessage>();
            resultSet = new SerialResultSet();
            pagination = new SerialPagination();
        }

        public ValidationMessages GetUnserializedMessages()
        {
            return new ValidationMessages()
            {
                Messages = message.Select(m => new ValidationMessage(m.type, m.code, m.userMessage)).ToList()
            };
        }


    }
}
