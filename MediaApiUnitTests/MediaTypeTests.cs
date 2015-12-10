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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaApiUnitTests
{
    [TestClass]
    public class MediaTypeTests
    {
        [TestMethod]
        public void CanRetrieveMediaTypes()
        {
            var url = string.Format("{0}://{1}/api/{2}/resources/{3}", TestUrl.Protocol, TestUrl.PublicApiServer, "v2", "mediatypes");
            string result = TestApiUtility.Get(url);

            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void HtmlMediaCountIsGreaterThanZero()
        {
            var url = string.Format("{0}://{1}/api/{2}/resources/{3}", TestUrl.Protocol, TestUrl.PublicApiServer, "v2", "mediatypes");
            string result = TestApiUtility.Get(url);

            var action = new ActionResultsWithType<List<SerialMediaType>>(result);

            var obj = action.ResultObject[0];
            var html = action.ResultObject.First(mt => mt.name == "HTML");

            Assert.IsTrue(html.mediaCount > 0);
        }

        [TestMethod]
        public void HtmlMediaCountHigherThanPodcastMediaCount()
        {
            var url = string.Format("{0}://{1}/api/{2}/resources/{3}", TestUrl.Protocol, TestUrl.PublicApiServer, "v2", "mediatypes");
            string result = TestApiUtility.Get(url);

            var action = new ActionResultsWithType<List<SerialMediaType>>(result);

            var obj = action.ResultObject[0];
            var html = action.ResultObject.Where(mt => mt.name == "HTML");
            var podcast = action.ResultObject.Where(mt => mt.name == "Podcast");

            Assert.IsTrue(html.Count() > podcast.Count(), html.Count().ToString() + " html media found, " + podcast.Count().ToString() + " podcasts.");
        }
    }
}
