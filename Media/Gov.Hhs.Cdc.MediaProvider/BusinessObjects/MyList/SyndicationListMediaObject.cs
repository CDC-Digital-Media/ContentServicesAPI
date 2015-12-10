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
    public class SyndicationListMediaObject : DataSourceBusinessObject, IValidationObject
    {
        public Guid? SyndicationListGuid { get; set; }
        public int MediaId { get; set; }
        public bool HasPulledCode { get; set; }
        public DateTime? LastPulledCodeDateTime { get; set; }
        public Guid ModifiedByGuid { get; set; }
        public string ModifiedByEmailAddress { get; set; }
        public bool IsActive { get; set; }

        public DateTime? ModifiedDateTime { get; set; }


        public void Complete(Guid syndicationListGuid, Guid userGuid)
        {
            SyndicationListGuid = syndicationListGuid;
            ModifiedByGuid = userGuid;
            IsActive = true;
        }

        public void Update(Guid userGuid)
        {
            ModifiedByGuid = userGuid;
            IsActive = true;
        }
    }
}
