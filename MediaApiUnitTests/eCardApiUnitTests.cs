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
using System.Xml.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Api.Public;

namespace MediaApiUnitTests
{
    [TestClass]
    public class eCardApiUnitTests
    {
        private XNamespace Namespace = "http://schemas.datacontract.org/2004/07/Gov.Hhs.Cdc.Api";

        [TestMethod]
        public void TestMediaXml()
        {
            var receiptId = "";
            var url = string.Format("{0}://{1}/api/v1/resources/media.xml", TestUrl.Protocol, TestUrl.PublicApiServer, receiptId);
            string result = TestApiUtility.Get(url);
            var doc = XDocument.Parse(result);
            Assert.IsTrue(doc.Descendants(Namespace + "mediaItem").Count() > 0);
            //var x = new ActionResultsWithType<SerialMedi>(result);
            //Assert.AreEqual("eCard Receipt Id is missing", x.ValidationMessages.Messages.First().Message);
        }


        [TestMethod]
        public void NoReceiptIdGivesErrorMessage()
        {
            var receiptId = "";
            string url = string.Format("{0}://{1}/api/v1/resources/ecards/{2}", TestUrl.Protocol, TestUrl.PublicApiServer, receiptId);
            var result = TestApiUtility.Get(url);
            var x = new ActionResultsWithType<SerialCardView>(result);
            Assert.AreEqual("eCard Receipt Id is missing", x.ValidationMessages.Messages.First().Message);
        }

        [TestMethod]
        public void InvalidReceiptIdGivesErrorMessage()
        {
            var receiptId = "junk";
            string url = string.Format("{0}://{1}/api/v1/resources/ecards/{2}/view", TestUrl.Protocol, TestUrl.PublicApiServer, receiptId);
            var result = TestApiUtility.Get(url);
            var x = new ActionResultsWithType<SerialCardView>(result);           
            Assert.AreEqual("Invalid card instance id for eCard", x.ValidationMessages.Messages.First().Message);
        }

        [TestMethod]
        public void InvalidActionGivesErrorMessage()
        {
            var receiptId = "2f509587-b274-4e3d-8ef1-b424650f8d12";
            string url = string.Format("{0}://{1}/api/v1/resources/ecards/{2}/junk", TestUrl.Protocol, TestUrl.PublicApiServer, receiptId);
            var result = TestApiUtility.Get(url);
            var x = new ActionResultsWithType<SerialCardView>(result);           
            Assert.AreEqual("invalid action parameter", x.ValidationMessages.Messages.First().Message);
       }

        [TestMethod]
        public void CanRetrieveSingleeCard()
        {
            var ecardMediaId = "145800";
            var results = TestApiUtility.AdminApiMediaSearch(ecardMediaId);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("This is a wonderful test card", results.FirstOrDefault().eCard.cardText);
        }
    }
}
