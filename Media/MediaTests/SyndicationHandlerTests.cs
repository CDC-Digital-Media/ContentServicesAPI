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
using Gov.Hhs.Cdc.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTests
{
    [TestClass]
    public class SyndicationHandlerTests
    {
        static SyndicationHandlerTests()
        {
            CurrentConnection.Name = "ContentServicesDb";
        }
        int apiVersion = 2;

        [TestMethod]
        public void CanSyndicateExistingMedia()
        {
            var mediaId = "290968";
            SyndicationHandler.GetSyndicatedContent(mediaId, apiVersion, "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQR91WsBYV37QW0bTuLp-Tj1Vr-dfmnYcj3zg7b9JkTRHxe9vgGeg");
        }

        [TestMethod]
        public void CanSyndicateExistingMedia2()
        {
            var mediaId = "294130";
            SyndicationHandler.GetSyndicatedContent(mediaId, apiVersion, "");
        }

        [TestMethod]
        public void H1SyndicatedOnce()
        {
            var mediaid = "278684";
            var h1 = "<h1>Kid-friendly Fact Sheet</h1>";
            var content = SyndicationHandler.GetSyndicatedContent(mediaid, apiVersion, "");
            var src = content.Content;
            var count = src.Select((c, i) => src.Substring(i)).Count(sub => sub.StartsWith(h1));
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void CanSyndicateWidget()
        {
            var mediaId = "138665";
            var content = SyndicationHandler.GetSyndicatedContent(mediaId, apiVersion, "");
            Assert.IsNotNull(content);
            Assert.IsTrue(content.Content.Contains("Lyme"));
        }



    }
}
