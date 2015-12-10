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
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Gov.Hhs.Cdc.Api
{
    public interface ISerialUserObject
    {
        //[DataMember(Order = 2)]
        string firstName { get; set; }

        //[DataMember(Order = 3)]
        string middleName { get; set; }

        //[DataMember(Order = 4)]
        string lastName { get; set; }

        //[DataMember(Order = 5)]
        string email { get; set; }

        //[XmlIgnore]   
        //[ScriptIgnore]
        string userToken { get; set; }

        //[XmlIgnore]   
        //[ScriptIgnore]
        Int64 expiresAt { get; set; }

        //public SerialUserObjectBase()
        //{
        //}

        //public SerialUserObjectBase(SerialUserObjectBase original)
        //{
        //    firstName = original.firstName;
        //    middleName = original.middleName;
        //    lastName = original.lastName;
        //    email = original.email;
        //}

    }
}
