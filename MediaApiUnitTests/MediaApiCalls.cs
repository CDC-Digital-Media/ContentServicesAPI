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
using Gov.Hhs.Cdc.Api;

namespace MediaApiUnitTests
{
    public static class MediaApiCalls
    {
        public static ICollection<SerialMediaV2> SingleMedia(int mediaId)
        {
            var url = string.Format("{0}://{1}/api/{2}/resources/{3}/{4}", TestUrl.Protocol, TestUrl.PublicApiServer, "v2", "media", mediaId);
            string result = TestApiUtility.Get(url);
            Console.WriteLine(url);
            Console.WriteLine(result);

            var x = new ActionResultsWithType<List<SerialMediaV2>>(result);
            Console.WriteLine(x);
            return x.ResultObject;
        }
    }
}
