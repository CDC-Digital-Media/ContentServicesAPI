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
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "eCardDetail")]
    public class SerialECardDetail  // : SerialMediaTypeSpecificDetail
    {
        [DataMember(Order = 1)]
        public string mobileCardName { get; set; }

        [DataMember(Order = 2)]
        public string html5Source { get; set; }

        [DataMember(Order = 3)]
        public string caption { get; set; }

        [DataMember(Order = 4)]
        public string cardText { get; set; }

        [DataMember(Order = 5)]
        public string cardTextOutside { get; set; }

        [DataMember(Order = 6)]
        public string cardTextInside { get; set; }

        [DataMember(Order = 7)]
        public string imageSourceInsideLarge { get; set; }

        [DataMember(Order = 8)]
        public string imageSourceInsideSmall { get; set; }

        [DataMember(Order = 9)]
        public string imageSourceOutsideLarge { get; set; }

        [DataMember(Order = 10)]
        public string imageSourceOutsideSmall { get; set; }

        [DataMember(Order = 101)]
        public bool isMobile { get; set; }

        [DataMember(Order = 102)]
        public string mobileTargetUrl { get; set; }

        [DataMember(Order = 201)]
        public bool isActive { get; set; }

        [DataMember(Order = 202)]
        public int displayOrdinal { get; set; }

    }
}
