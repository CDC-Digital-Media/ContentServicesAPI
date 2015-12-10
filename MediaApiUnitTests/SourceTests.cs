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
using Gov.Hhs.Cdc.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaApiUnitTests
{
    [TestClass]
    public class SourceTests
    {
        [TestMethod]
        public void AdminApiReturnsCdcSourceWebsite()
        {
            var url = string.Format("{0}://{1}/adminapi/v1/resources/sources", TestUrl.Protocol, TestUrl.AdminApiServer);
            string result = TestApiUtility.Get(url);
            var x = new ActionResultsWithType<List<SerialSource>>(result);
            var source = x.ResultObject.Where(s => s.acronym == "CDC");
            Assert.AreEqual(1, source.Count());
            var cdc = source.First();
            Assert.AreEqual("http://www......[domain].....", cdc.websiteUrl);
        }

        [TestMethod]
        public void PublicApiReturnsCdcSourceWebiste()
        {
            var url = string.Format("{0}://{1}/api/v2/resources/sources", TestUrl.Protocol, TestUrl.PublicApiServer);
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            var x = new ActionResultsWithType<List<SerialSource>>(result);
            var source = x.ResultObject.Where(s => s.acronym == "CDC");
            Assert.AreEqual(1, source.Count());
            var cdc = source.First();
            Assert.AreEqual("http://www......[domain].....", cdc.websiteUrl);
        }

        [TestMethod]
        public void AdminApiReturnsHhsSourceWebsite()
        {
            var url = string.Format("{0}://{1}/adminapi/v1/resources/sources", TestUrl.Protocol, TestUrl.AdminApiServer);
            string result = TestApiUtility.Get(url);
            var x = new ActionResultsWithType<List<SerialSource>>(result);
            var source = x.ResultObject.Where(s => s.acronym == "HHS");
            Assert.AreEqual(1, source.Count());
            var hhs = source.First();
            Assert.AreEqual("http://www.hhs.gov", hhs.websiteUrl);
        }

        [TestMethod]
        public void PublicApiReturnsHhsSourceWebsite()
        {
            var url = string.Format("{0}://{1}/api/v2/resources/sources", TestUrl.Protocol, TestUrl.PublicApiServer);
            string result = TestApiUtility.Get(url);
            var x = new ActionResultsWithType<List<SerialSource>>(result);
            var source = x.ResultObject.Where(s => s.acronym == "HHS");
            Assert.AreEqual(1, source.Count());
            var hhs = source.First();
            Assert.AreEqual("http://www.hhs.gov", hhs.websiteUrl);
        }
    }
}
