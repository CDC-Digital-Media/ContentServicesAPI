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

namespace Gov.Hhs.Cdc.CsBusinessObjects.Admin
{
    public class AdminUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get { return FirstName + " " + LastName; } }
        public string UserName { get; set; }
        public string Email { get; set; }
        public Guid UserGuid { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<MediaSet> MediaSets { get; set; }
    }

}
