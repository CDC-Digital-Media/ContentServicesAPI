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
using System.Web.Script.Serialization;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "source")]
    public sealed class SerialSource
    {
        [DataMember(Name = "name", Order = 1)]
        public string name { get; set; }

        [DataMember(Name = "acronym", Order = 2)]
        public string acronym { get; set; }

        //[DataMember(Name = "description", Order = 3)]
        //public string description { get; set; }

        [DataMember(Name = "websiteUrl", Order = 3)]
        public string websiteUrl { get; set; }

        [DataMember(Name = "largeLogoUrl", Order = 4)]
        public string largeLogoUrl { get; set; }

        [DataMember(Name = "smallLogoUrl", Order = 5)]
        public string smallLogoUrl { get; set; }

        //[DataMember(Name = "comments", Order = 6)]
        //public string comments { get; set; }

        //[DataMember(Name = "displayOrdinal", Order = 7)]
        //public int displayOrdinal { get; set; }

        //[DataMember(Name = "active", Order = 8)]
        //public bool active { get; set; }
    }
}
