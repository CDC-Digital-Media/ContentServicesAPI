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
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gov.Hhs.Cdc.AuthorizationTests
{
    [TestClass]
    public class MediaSetContentTests
    {
        private string mediaAdminUser = "jwe8";

        [TestMethod]
        public void UserHasMediaAdminRole()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/adminusers/" + mediaAdminUser, TestUrl.Protocol, TestUrl.AdminApiServer);
            Console.WriteLine(url);
            string callResults = TestApiUtility.Get(url);
            var obj = new JavaScriptSerializer().Deserialize<SerialAdminUserResults>(callResults);
            if (obj == null)
            {
                Assert.Fail(callResults);
            }
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail(obj.meta.message[0].userMessage);
            }
            Assert.AreEqual(1, obj.results.Count());
            Assert.IsTrue(obj.results.First().roles.Contains("Media Admin"));
        }

        [TestMethod, Ignore] //Come back to this when we implement media set authorization
        public void OnlyOneMediaReturned()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/media", TestUrl.Protocol, TestUrl.AdminApiServer);
            string callResults = TestApiUtility.Get(url, mediaAdminUser);
            Console.WriteLine(callResults);
            var obj = new JavaScriptSerializer().Deserialize<SerialAdminUserResults>(callResults);
            Assert.IsNotNull(obj);
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail();
            }
            Assert.AreEqual(1, obj.results.Count());
        }
    }
}
