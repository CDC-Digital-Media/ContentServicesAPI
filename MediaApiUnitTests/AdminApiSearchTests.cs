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
    public class AdminApiSearchTests
    {
        [TestMethod]
        public void CanSearchFlu()
        {
            var criteria = "?title=flu";
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
        }

        [TestMethod]
        public void CanSearchNameContains()
        {
            var criteria = "?nameContains=flu";
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
        }

        [TestMethod]
        public void CanSearchExactTitle()
        {
            var criteria = "?name=Influenza Flu";
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
        }

        [TestMethod]
        public void CanSearchAdminUserGuid()
        {
            var criteria = "?adminUserGuid=CEB728AA-9EAA-44BB-B6BF-8D3F43437EDD";
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
        }

        [TestMethod]
        public void RecordCountReturned()
        {
            var criteria = "?title=alcohol";
            var results = TestApiUtility.AdminApiSearchMeta(criteria);
            Assert.AreNotEqual(0, results.pagination.total);
        }

        [TestMethod]
        public void PageNumberReturned()
        {
            var criteria = "?title=alcohol";
            var results = TestApiUtility.AdminApiSearchMeta(criteria);
            Assert.AreNotEqual(0, results.pagination.pageNum);            
        }

        [TestMethod]
        public void TotalPagesReturned()
        {
             var criteria = "?title=alcohol";
            var results = TestApiUtility.AdminApiSearchMeta(criteria);
            Assert.AreNotEqual(0, results.pagination.totalPages);
           
        }

        [TestMethod]
        public void SearchWithNoReusltsDoesNotError()
        {
            var criteria = "?title=XSDFWSOWJERWLEMLSFML";
            var results = TestApiUtility.AdminApiSearchMeta(criteria);
        }

        [TestMethod]
        public void CanSearchByMediaType()
        {
            var criteria = "?mediatype=widget";
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
            Assert.AreEqual("Widget", results.First().mediaType);
        }

        string feedMediaIdWith2Children = "339625";
        [TestMethod]
        public void CanSearchByMediaId()
        {
            var criteria = feedMediaIdWith2Children;
            var results = TestApiUtility.AdminApiMediaSearch(criteria); ;
            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public void CanSearchByParentId()
        {
            var criteria = "?parentid=" + feedMediaIdWith2Children;
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void CanSearchMultipleMediaIds()
        {
            var childMediaIds = "343932,348599";
            var criteria = "?mediaid=" + feedMediaIdWith2Children +"," + childMediaIds; //Feed and its children
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void CanSearchMediaIdPlusMediaType()
        {
            var mediaIdsOneFeedItemOneNot = "343999,323390";
            var criteria = "?mediaid=" + mediaIdsOneFeedItemOneNot + "&mediatype=feed item";
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.AreEqual(1, results.Count());

        }

        [TestMethod]
        public void CanSearchByLanguage()
        {
            var criteria = "?language=spanish";
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
            Assert.IsTrue(results.First().language.Equals("Spanish", StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void CanSearchBySourceUrl()
        {
            var url = "http://www......[domain]...../flu";
            var criteria = "?urlcontains=" + url;
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
            Assert.IsTrue(results.First().sourceUrl.Contains(url), results.First().sourceUrl);
        }

        [TestMethod]
        public void CanSearchPartialSourceUrl()
        {
            var url = "/flu";
            var criteria = "?urlcontains=" + url;
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
            Assert.IsTrue(results.First().sourceUrl.Contains(url), results.First().sourceUrl);
        }

        [TestMethod]
        public void SourceNameSearchMatches()
        {
            var source = "Food and Drug Administration";
            var criteria = "?sourceName=" + source;
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.AreEqual(source, results.First().sourceCode);
        }
        [TestMethod]
        public void SourceNameSearchHasReasonableCount()
        {
            var source = "Food and Drug Administration";
            var criteria = "?sourceName=" + source;
            var meta = TestApiUtility.AdminApiSearchMeta(criteria);
            Assert.IsTrue(meta.pagination.total < 10000, meta.pagination.total.ToString());
        }

        [TestMethod]
        public void SourceUrlExactMatchSearchDoesNotFindNonExact()
        {
            var url = "http://www......[domain]...../flu/toolkit";
            var criteria = "?url=" + url;
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void CanSearchByStatus()
        {
            var criteria = "?status=hidden";
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
            Assert.AreEqual("Hidden", results.First().status);
        }

        [TestMethod]
        public void CanSearchByPublishDate()
        {
            var from = "11/01/2014";
            var fromDate = DateTime.Parse(from);
            var to = "11/16/2014";
            var toDate = DateTime.Parse(to);

            var criteria = "?status=published&fromdatepublished=" + from + "&todatepublished=" + to;
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
            var first = results.First();
            var pub = DateTime.Parse(first.datePublished);
            Assert.IsTrue(pub > fromDate && pub < toDate, first.datePublished);
        }

        [TestMethod]
        public void CanSearchByModifiedDate()
        {
            var from = "11/01/2015";
            var fromDate = DateTime.Parse(from);
            var to = "11/16/2016";
            var toDate = DateTime.Parse(to);

            var criteria = "?status=published&fromdatemodified=" + from + "&todatemodified=" + to;
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
            var first = results.First();
            var mod = DateTime.Parse(first.dateModified);
            Assert.IsTrue(mod > fromDate && mod < toDate, first.dateModified);
        }

        [TestMethod]
        public void CanSearchByOwningOrg()
        {
            var org = "20";
            var criteria = "?owningorg=" + org;
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
            var first = results.First();
            Assert.AreEqual(org, first.owningOrgId.ToString());
        }

        [TestMethod]
        public void CanSearchByMaintainingOrg()
        {
            var org = "20";
            var criteria = "?maintainingorg=" + org;
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            Assert.IsTrue(results.Count() > 0);
            var first = results.First();
            Assert.AreEqual(org, first.maintainingOrgId.ToString());
        }

        [TestMethod]
        public void CanSearchBunnyExclamation()
        {
            var title = "Bunny!";
            var criteria = "?title=" + title;
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            var count = results.Count();
            Assert.IsTrue(count > 0);
            Assert.IsTrue(count < 3); //don't get those OTHER bunny items
            var first = results.First();
            Assert.IsTrue(first.title.Contains(title));
        }

        [TestMethod]
        public void CanSearchWithApostrophe()
        {
            var title = "Terrie's";
            var criteria = "?title=" + title;
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            var count = results.Count();
            Assert.IsTrue(count > 0);
            Assert.IsTrue(count < 10); 
            var first = results.First();
            Assert.IsTrue(first.title.Contains(title));
        }
    }
}
