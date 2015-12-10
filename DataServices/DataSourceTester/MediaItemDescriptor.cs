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

namespace DataSourceTester
{
    [DataContract]
    public class MediaItemDescriptor {
        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string MediaId { get; set; }

        [DataMember]
        public string MediaType { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string CampaignId { get; set; }

        [DataMember]
        public string TopicId { get; set; }

        [DataMember]
        public string AudienceId { get; set; }

        [DataMember]
        public string Language { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public DateTime AvailableDate { get; set; }

        [DataMember]
        public bool CanBeManaged { get; set; }

        [DataMember]
        public bool CanBeShared { get; set; }
    }
}
