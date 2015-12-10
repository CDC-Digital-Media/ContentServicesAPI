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


using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "passwordReset")]
    public sealed class SerialPasswordReset
    {
        [XmlIgnore]
        [DataMember(Name = "email")]
        public string email { get; set; }

        [XmlIgnore]
        [DataMember(Name = "passwordResetUrl")]
        public string passwordResetUrl { get; set; }

        [XmlIgnore]
        [DataMember(Name = "passwordToken")]
        public string passwordToken { get; set; }

        [XmlIgnore]
        [DataMember(Name = "newPassword")]
        public string newPassword { get; set; }

        [XmlIgnore]
        [DataMember(Name = "newPasswordRepeat")]
        public string newPasswordRepeat { get; set; }

    }
}
