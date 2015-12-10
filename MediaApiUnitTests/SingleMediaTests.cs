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
    public class SingleMediaTests
    {

        [TestMethod]
        public void TestSimpleRetrieveExisting()
        {
            var existingMediaId = 138478;
            var media = MediaApiCalls.SingleMedia(existingMediaId);
            Assert.IsNotNull(media);
        }

        [TestMethod]
        public void CanRetrievePdfSizeViaApi()
        {
            var pdf = 236101;
            var obj = MediaApiCalls.SingleMedia(pdf);
            Assert.IsNotNull(obj);
            var media = obj.FirstOrDefault();
            Assert.IsNotNull(media);
            Assert.AreEqual(pdf, media.id);
            Assert.AreEqual("7", media.pageCount);
        }
    }
}
