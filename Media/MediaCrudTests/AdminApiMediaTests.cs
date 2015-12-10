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

namespace MediaCrudTests
{
    [TestClass]
    public class AdminApiMediaTests
    {

        [TestMethod]
        public void CanRetrievePdfSizeViaAdminApi()
        {
            var pdf = 236101;
            var media = AdminApiCalls.SingleMedia(pdf);
            Assert.IsNotNull(media);
            Assert.AreEqual("7", media.pageCount);
        }

        [TestMethod]
        public void CanRetrieveChildrenViaAdminApi()
        {
            var feed = 352277;
            var media = AdminApiCalls.SingleMedia(feed);
            Assert.IsNotNull(feed);
            Assert.IsNotNull(media.children);
            Assert.IsTrue(media.children.Count() > 3);
        }

        [TestMethod]
        public void AdminMediaContainsCreator()
        {
            var creator = "Marcie Davis (CDC\\ELY1)";
            var html = 280017;
            var media = AdminApiCalls.SingleMedia(html);
            Assert.AreEqual(creator, media.createdBy);
        }

        [TestMethod]
        public void AdminMediaContainsModifier()
        {
            var modifier = "Marcie Davis (CDC\\ELY1)";
            var html = 280017;
            var media = AdminApiCalls.SingleMedia(html);
            Assert.AreEqual(modifier, media.modifiedBy);
        }
    }
}
