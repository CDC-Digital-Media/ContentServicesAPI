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
using System.Linq;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.CsCaching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SearchUnitTests
{
    [TestClass]
    public class CacheManagerTests
    {
        [TestMethod]
        public void ItemsPresentInCache()
        {
            CacheManager.Cache("test", "value");
            var keys = CacheManager.CachedKeys();
            Assert.IsNotNull(keys);
            Assert.AreNotEqual(0, keys.Count());
        }

        [TestMethod]
        public void NoItemsInCacheAfterClear()
        {
            CacheManager.Cache("test", "value");
            CacheManager.ClearAll();
            var keys = CacheManager.CachedKeys();
            Assert.AreEqual(0, keys.Count());
        }

        [TestMethod, Ignore]
        public void ItemsPresentInAdminApiCache()
        {
            var keys = TestApiUtility.AdminApiCache();
            Assert.IsNotNull(keys);
            if (keys.Count() == 0)
            {
                keys = TestApiUtility.AdminApiCache(); //try it again to load something in
            }
            Assert.AreNotEqual(0, keys.Count());
        }

        [TestMethod]
        public void NoItemsInAdminCacheAfterClear()
        {
            TestApiUtility.ClearAdminApiCache();
            var keys = TestApiUtility.AdminApiCache();
            Assert.AreEqual(0, keys.Count(), keys.Count()); //apiKey comes back as soon as you call this?
        }

        [TestMethod]
        public void ItemsPresentInPublicApiCache()
        {
            var keys = TestApiUtility.PublicApiCache();
            Assert.IsNotNull(keys);
            Assert.AreNotEqual(0, keys.Count());
        }

        [TestMethod]
        public void SearchAddsToPublicApiCache()
        {
            var keys = TestApiUtility.PublicApiCache();
            var count1 = keys.Count();
            var ticks = DateTime.Now.Ticks.ToString();
            var use = ticks.Substring(ticks.Length - 2);
            var criteria = "syndicationVisibleBeforeDate=" + "19" + use + "-05-31T18:01:00Z";
            TestApiUtility.PublicApiV2MediaSearch(criteria);
            keys = TestApiUtility.PublicApiCache();
            var count2 = keys.Count();
            //Assert.IsTrue(count2 > count1, count1 + ", " + count2);
            Assert.AreNotEqual(count1, count2);
        }

    }
}
