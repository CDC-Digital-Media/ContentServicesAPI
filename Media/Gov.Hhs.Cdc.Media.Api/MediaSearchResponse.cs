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
    public class MediaSearchResponse
    {
        public int MediaCount { get; set; }
        public int PageCount { get; set; }
        public int Duration { get; set; }
        public bool Complete { get; set; }
        public string ResultSetId { get; set; }
        public int CurrentPage { get; set; }
        public List<MediaItemDescriptor> Result { get; set; }


        /// <summary>
        /// For use by testing apps, etc. that aren't serializing
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var result = 
                MediaCount.ToString() + "\n";

            result +=
                PageCount.ToString() + "\n";

            result += 
                Duration.ToString() + "\n";

            result +=
                (Complete ? "Y" : "N");

            result +=
                ResultSetId + "\n";

            result +=
                CurrentPage.ToString() + "\n";

            foreach (var mediaItemDescriptor in Result) {
                result +=
                    mediaItemDescriptor.ToString() + "\n";
            }


            return result;
        }
    }
}
