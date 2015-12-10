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
using System.Runtime.Serialization;
using System.Text;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "htmlPreferences")]
    public class SerialHtmlPreferences
    {
        [DataMember(Order = 101)]
        public SerialExtractionPath includedElements { get; set; }

        [DataMember(Order = 102)]
        public SerialExtractionPath excludedElements { get; set; }

        [DataMember(Order = 209)]
        public bool? stripBreak { get; set; }    // = false;

        [DataMember(Order = 210)]
        public bool? stripAnchor { get; set; }    // = false;

        [DataMember(Order = 220)]
        public bool? stripComment { get; set; }    // = true;

        [DataMember(Order = 230)]
        public bool? stripImage { get; set; }    // = false;

        [DataMember(Order = 240)]
        public bool? stripScript { get; set; }    // = true

        [DataMember(Order = 250)]
        public bool? stripStyle { get; set; }    // = true;

        [DataMember(Order = 260)]
        public bool? newWindow { get; set; }    // = true;

        [DataMember(Order = 270)]
        public bool? iframe { get; set; }    // = true;

        [DataMember(Order = 310)]
        public string imageAlign { get; set; }

        [DataMember(Order = 320)]
        public string outputEncoding { get; set; }

        [DataMember(Order = 330)]
        public string outputFormat { get; set; }

        [DataMember(Order = 340)]
        public string contentNamespace { get; set; }
    }
}
