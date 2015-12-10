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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Api;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.CdcRegistrationProvider;
using Gov.Hhs.Cdc.CdcEmailProvider;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.Connection;

namespace RegistrationApiUnitTests
{
    [TestClass]
    public class LocationTests
    {
        static LocationTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void TestUnitedStatesInCountryList()
        {
            //6255149 is North America
            string url = String.Format("{0}://{1}/api/v1/resources/locations/6255149?max=0", TestUrl.Protocol, TestUrl.PublicApiServer);
            string callResults = TestApiUtility.Get(url);
            Console.WriteLine(callResults);
            var obj = new JavaScriptSerializer().Deserialize<SerialLocationResults>(callResults);
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail();
            }

            Assert.IsTrue(obj.results.Where(r => r.name == "United States").Count() > 0);              
        }

        [TestMethod]
        public void TestUnitedStatesHasAtLeast50States()
        {
            //6252001 is United States
            string url = String.Format("{0}://{1}/api/v1/resources/locations/6252001?max=0", TestUrl.Protocol, TestUrl.PublicApiServer);
            string callResults = TestApiUtility.Get(url);
            Console.WriteLine(callResults);
            SerialResponse obj = new JavaScriptSerializer().Deserialize<SerialResponse>(callResults);
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail();
            }

            var results = obj.results as Array;
            Assert.IsTrue(results.Length >= 50);
            
        }

        private class SerialLocationResults
        {
            public SerialLocationResults() { }
            public SerialMeta meta { get; set; }
            public List<SerialLocationItem> results { get; set; }
        }
    }
}
