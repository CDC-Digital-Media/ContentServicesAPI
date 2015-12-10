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
using System.Web;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "widgetItem")]
    public sealed class SerialWidgetItem
    {
        [DataMember(Name = "mediaId", Order = 1)]
        public int mediaId { get; set; }

        [DataMember(Name = "title", Order = 2)]
        public string title { get; set; }

        [DataMember(Name = "language", Order = 3)]
        public string language { get; set; }

        [DataMember(Name = "embedCode", Order = 4)]
        public string embedCode { get; set; }

        [DataMember(Name = "description", Order = 5)]
        public string description { get; set; }

        [DataMember(Name = "sourceUrl", Order = 6)]
        public string sourceUrl { get; set; }

        [DataMember(Name = "targetUrl", Order = 7)]
        public string targetUrl { get; set; }
    }
}
