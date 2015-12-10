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


namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "syndicationListMedia")]
    public sealed class SerialSyndicationListMedia
    {
        [DataMember(Name = "hasPulledCode", Order = 1, EmitDefaultValue = true)]
        public bool hasPulledCode { get; set; }

        [DataMember(Name = "lastPulledCodeDateTime", Order = 2, EmitDefaultValue = true)]
        public DateTime? lastPulledCodeDateTime { get; set; }

        [DataMember(Name = "mediaId", Order = 3, EmitDefaultValue = true)]
        public int mediaId { get; set; }

        //[DataMember(Name = "rowVersion", Order = 4, EmitDefaultValue = true)]
        //public string rowVersion { get; set; }

        public SerialSyndicationListMedia()
        {
        }

        public SerialSyndicationListMedia(int mediaId)
        {
            this.mediaId = mediaId;
            hasPulledCode = true;
            lastPulledCodeDateTime = DateTime.Now;          
        }

        public static List<SerialSyndicationListMedia> CreateList(params int[] mediaIds)
        {
            return mediaIds.Select(id => new SerialSyndicationListMedia(id)).ToList();
        }
    }

}
