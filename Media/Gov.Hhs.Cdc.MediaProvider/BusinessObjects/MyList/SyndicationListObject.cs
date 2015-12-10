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
using Gov.Hhs.Cdc.Bo;


namespace Gov.Hhs.Cdc.MediaProvider
{
    public class SyndicationListObject : DataSourceBusinessObject, IValidationObject
    {
        public Guid? SyndicationListGuid { get; set; }
        public string DomainName { get; set; }
        public Guid? UserGuid { get; set; }
        public string ListName { get; set; }
        public string SyndicationListStatusCode { get; set; }
        public IEnumerable<SyndicationListMediaObject> Medias { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
