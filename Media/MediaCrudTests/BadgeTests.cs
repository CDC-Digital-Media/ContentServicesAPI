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
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaCrudTests
{
    [TestClass]
    public class BadgeTests
    {
        public int mediaId;
        public string authorizedUser = "";

        [TestCleanup]
        public void Cleanup()
        {
            if (mediaId > 0)
            {
                new CsMediaProvider().DeleteMedia(mediaId);
            }
        }

        [TestMethod]
        public void CanSearchBadgeDescription()
        {
            var validTopicIds = new List<int> { 25272, 25329, 25651 };

            var desc = "WeirdUnlikelyPhrase";
            CurrentConnection.Name = "ContentServicesDb";
            var badge = new SerialMediaAdmin
            {
                mediaType = "Badge",
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                sourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                targetUrl = "http://www......[domain].....",
                status = "Published",
                topics = validTopicIds,
                encoding = "utf-8",
                title = "Tuff Badge",
                description = desc
            };
            var result = TestApiUtility.CreateAdminMedia(badge, authorizedUser);
            if (result.ValidationMessages.NumberOfErrors > 0)
            {
                Assert.Fail(result.ValidationMessages.Errors().First().Message);
            }
            Assert.IsNotNull(result.ResultObject);
            var first = result.ResultObject.First();
            mediaId = int.Parse(first.mediaId);

            //do a storefront search for desc
            var url = "description=" + desc;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            Assert.AreEqual(1, media.Count());
        }


        [TestMethod]
        public void JunkEncodingIsInvalid()
        {
            var validTopicIds = new List<int> { 25272, 25329, 25651 };

            CurrentConnection.Name = "ContentServicesDb";
            var badge = new SerialMediaAdmin
            {
                mediaType = "Badge",
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                sourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                targetUrl = "http://www......[domain].....",
                status = "Published",
                topics = validTopicIds,
                encoding = "junk",
                title = "Tuff Badge",
                description = "desc"
            };
            var result = TestApiUtility.CreateAdminMedia(badge, authorizedUser);
            if (result.ResultObject != null && result.ResultObject.Count > 0)
            {
                var first = result.ResultObject.First();
                mediaId = int.Parse(first.mediaId);
            }

            Assert.AreEqual(1, result.ValidationMessages.NumberOfErrors);
            Assert.AreEqual("Encoding is invalid", result.ValidationMessages.Errors().First().Message);
        }

        [TestMethod]
        public void DescriptionChangeDisplaysWhenCacheCleared()
        {
            var desc = "WeirdUnlikelyPhrase";
            CurrentConnection.Name = "ContentServicesDb";
            var badge = new SerialMediaAdmin
            {
                mediaType = "Badge",
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                sourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                targetUrl = "http://www......[domain].....",
                status = "Published",
                encoding = "utf-8",
                title = "Tuff Badge",
                description = desc,
                topics = new List<int>() { 25272, 25329 }
            };
            var result = TestApiUtility.CreateAdminMedia(badge, authorizedUser);
            if (result.ValidationMessages.NumberOfErrors > 0)
            {
                Assert.Fail(result.ValidationMessages.Errors().First().Message);
            }
            Assert.IsNotNull(result.ResultObject);
            var first = result.ResultObject.First();
            mediaId = int.Parse(first.mediaId);
            Console.WriteLine(mediaId);

            var newDesc = "EvenLessLikely" + DateTime.Now.Ticks;
            first.description = newDesc;
            var result2 = AdminApiCalls.UpdateAdminMedia(first, authorizedUser);

            TestApiUtility.ClearStorefrontApiCache();

            var url = "description=" + newDesc;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);
            Assert.AreEqual(mediaId, media.First().id);
            Assert.AreEqual(newDesc, media.First().description);
        }
    }
}
