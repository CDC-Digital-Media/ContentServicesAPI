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
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "user")]
    public sealed class SerialUser : ISerialUserObject
    {
        [DataMember(Order = 1)]
        public string id { get; set; }

        [DataMember(Order = 2)]
        public string firstName { get; set; }

        [DataMember(Order = 3)]
        public string middleName { get; set; }

        [DataMember(Order = 4)]
        public string lastName { get; set; }

        [DataMember(Order = 5)]
        public string email { get; set; }

        [DataMember(Order = 6)]
        public bool agreedToUsageGuidelines { get; set; }

        [DataMember(Order = 7)]
        public List<SerialUserOrgItem> organizations { get; set; }

        [XmlIgnore]
        [ScriptIgnore]
        public string userToken { get; set; }

        [XmlIgnore]
        [ScriptIgnore]
        public Int64 expiresAt { get; set; }

        [XmlIgnore]
        [ScriptIgnore]
        [DataMember(Order = 8)]
        public string password { get; set; }

        [XmlIgnore]
        [ScriptIgnore]
        [DataMember(Order = 9)]
        public string passwordRepeat { get; set; }

        [DataMember(Order = 10)]
        public bool active { get; set; }

        public SerialUser()
        {
        }

    }
}
