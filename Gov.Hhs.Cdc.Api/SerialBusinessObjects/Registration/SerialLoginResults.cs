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

using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "loginResults")]
    public sealed class SerialLoginResults
    {
        [DataMember(Name = "user", Order = 1)]
        public SerialUser User { get; set; }

        [DataMember(Name = "syndicationLists", Order = 1)]
        public List<SerialShortSyndicationList> syndicationLists { get; set; }

        public SerialLoginResults()
        {
        }
        public SerialLoginResults(LoginResults loginResults)
        {
            User = UserTransformation.CreateSerialUser(loginResults.User, loginResults.UserToken);
            //User = new SerialUser(loginResults.User, loginResults.UserToken);
            if (loginResults.SyndicationLists != null)
                syndicationLists = loginResults.SyndicationLists.Select(l => new SerialShortSyndicationList(l)).ToList();
        }    
    }

}
