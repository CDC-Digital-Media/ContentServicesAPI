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
    [DataContract(Name = "mediaValidation")]
    public sealed class SerialMediaValidation
    {
        public SerialMediaValidation()
        {
            content = "";
            validation = new SerialValidation();
        }

        [DataMember(Name = "mediaId", Order = 1, EmitDefaultValue = false)]
        public string mediaId { get; set; }

        [DataMember(Name = "mediaType", Order = 2, EmitDefaultValue = false)]
        public string mediaType { get; set; }

        [DataMember(Name = "sourceUrl", Order = 3, EmitDefaultValue = false)]
        public string sourceUrl { get; set; }

        [DataMember(Name = "targetUrl", Order = 4, EmitDefaultValue = false)]
        public string targetUrl { get; set; }

        [DataMember(Name = "title", Order = 5, EmitDefaultValue = false)]
        public string title { get; set; }

        [DataMember(Name = "description", Order = 6, EmitDefaultValue = false)]
        public string description { get; set; }

        [DataMember(Name = "contentType", Order = 7, EmitDefaultValue = true)]
        public string contentType { get; set; }

        [DataMember(Name = "content", Order = 8, EmitDefaultValue = true)]
        public string content { get; set; }

        [DataMember(Name = "validation", Order = 9, EmitDefaultValue = false)]
        public SerialValidation validation { get; set; }
    }
}
