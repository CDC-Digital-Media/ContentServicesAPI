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
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Global;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace RegistrationApiUnitTests
{
    [TestClass]
    public class OrganizationRetrievalTests
    {
        static OrganizationRetrievalTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        //https://.....[devApiServer]...../api/v1/resources/organizations?&ttl=900&callback=
        [TestMethod]
        public void TestRetrieveOrgs()
        {
            string url = String.Format("{0}://{1}/api/v1/resources/organizations", TestUrl.Protocol, TestUrl.PublicApiServer);
            string callResults = TestApiUtility.Get(url);
            Console.WriteLine(callResults);
            SerialResponse obj = new JavaScriptSerializer().Deserialize<SerialResponse>(callResults);
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestRetrieveOrgTypes()
        {
            //https://.....[devApiServer]...../api/v1/resources/userorgtypes   
            string url = String.Format("{0}://{1}/api/v1/resources/organizationtypes", TestUrl.Protocol, TestUrl.PublicApiServer);
            string callResults = TestApiUtility.Get(url);
            Console.WriteLine(callResults);
            SerialResponse obj = new JavaScriptSerializer().Deserialize<SerialResponse>(callResults);
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestOrgsStartWithM()
        {

            string url = String.Format("{0}://{1}/api/v1/resources/organizations?max=0", TestUrl.Protocol, TestUrl.PublicApiServer);
            string callResults = TestApiUtility.Get(url);
            Console.WriteLine(callResults);
            var obj = new JavaScriptSerializer().Deserialize<SerialOrgResponse>(callResults);
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail();
            }
            
            Assert.IsTrue(obj.results.Count > 20);
            Assert.IsTrue(obj.results.Where(i => i.name.StartsWith("M")).Count() > 0);
        }

        [TestMethod]
        public void RetrieveSingleOrgHasDefaultDomain()
        {
            var orgId = "5446";
            string url = String.Format("{0}://{1}/api/v1/resources/organizations/{2}", TestUrl.Protocol, TestUrl.PublicApiServer, orgId);
            string callResults = TestApiUtility.Get(url);
            Console.WriteLine(callResults);
            var obj = new JavaScriptSerializer().Deserialize<SerialOrgResponse>(callResults);
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail();
            }

            Assert.AreEqual(1, obj.results.Count);
            Assert.IsTrue(obj.results.First().website.First().isDefault.Value);
        }

        private class SerialOrgResponse
        {
            public SerialOrgResponse() {}

            public SerialMeta meta { get; set; }
            public List<SerialUserOrgItem> results { get; set; }
        }
    }
}
