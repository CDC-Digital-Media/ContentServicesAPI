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

namespace Gov.Hhs.Cdc.Media.Api
{
    public class MediaItemDescriptor
    {
        public string Status { get; set; }
        public string MediaId { get; set; }
        public string Title { get; set; }
        public string CampaignId { get; set; }
        public string MediaType { get; set; }
        public string TopicId { get; set; }
        public string AudienceId { get; set; }
        public string Language { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime AvailableDate { get; set; }
        public bool CanBeManaged { get; set; }
        public bool CanBeShared { get; set; }


        /// <summary>
        /// For use by testing apps, etc.
        /// that aren't serializing
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return
                Status + "\n" +
                MediaId + "\n" +
                Title + "\n" +
                MediaType + "\n" +
                TopicId + "\n" +
                AudienceId + "\n" +
                Language + "\n" +
                CreateDate.ToShortDateString() + "\n" +
                AvailableDate.ToShortDateString() + "\n" +
                CanBeManaged.ToString() + "\n" +
                CanBeShared.ToString();
        }
    }

}
