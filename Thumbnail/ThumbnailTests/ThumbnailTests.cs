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
using Gov.Hhs.Cdc.Api.Admin;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.Global;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ThumbnailTests
{

    [TestClass]
    public class ThumbnailTests
    {
        private string authorizedUser = "";

        static ThumbnailTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void TestGetThumbnail()
        {
            int mediaId = 138573;
            string url = TestUrl.Protocol + "://" + TestUrl.PublicApiServer + "/api/v1/resources/media/" + mediaId.ToString() + "/thumbnail?apiroot=" + TestUrl.PublicApiServer;
            byte[] callResults = TestApiUtility.GetBytes(url);

            Console.WriteLine(callResults);
        }

        [TestMethod, Ignore] //this media id no longer exists
        public void CanGetEmbedCode()
        {
            var badThumbnailMediaId = "292647";
            var media = TestApiUtility.AdminApiMediaSearch("?mediaid="+badThumbnailMediaId).FirstOrDefault();
            Assert.IsNotNull(media);
            Console.WriteLine(media.embedcode);
        }

        [TestMethod]
        public void TestUpdateThumbnail()
        {
            var media = TestApiUtility.SinglePublishedHtml();
            Console.WriteLine(media.mediaId);
            Console.WriteLine(media.sourceUrl);
            Assert.IsNotNull(media.embedcode, media.mediaId + " has no embed code, " + media.sourceUrl);
            var adminServiceFactory = new AdminApiServiceFactory();      
            
            List<SerialThumbnail> updatedThumbnails;
            TestUrl updateUrl = adminServiceFactory.CreateTestUrl(
                    "media", media.id, "thumbnail", "apiroot=https://" + TestUrl.AdminApiServer + "&webroot=https://.....[devServer]...../dev_storefront/&w=200&h=200&bw=700&bh=700");
            ValidationMessages messages = TestApiUtility.ApiPut<SerialThumbnail>(adminServiceFactory, updateUrl, /*serialData*/null, out updatedThumbnails, authorizedUser);

            if (messages.Errors().Count() > 0)
            {
                Assert.Fail(messages.Errors().First().Message);
            }
            Assert.IsNotNull(updatedThumbnails);  //TODO:  Look at Thumbnail PUT in Admin once it is working again
          
            TestUrl getUrl = adminServiceFactory.CreateTestUrl("media", media.id, "thumbnail");

            byte[] callResults = TestApiUtility.GetBytes(getUrl.ToString());

            byte[] thumbnailFromGet = TestApiUtility.ApiGetBytes(adminServiceFactory, getUrl);

            Assert.AreEqual(1, updatedThumbnails.Count);
            var getList = thumbnailFromGet.ToList();
            var upList = updatedThumbnails[0].thumbnail.ToList();
            Assert.IsTrue(getList.SequenceEqual(upList));
        }

        [TestMethod]
        public void TestUpdateThumbnail2()
        {
            var media = TestApiUtility.AdminApiMediaSearch("281584").FirstOrDefault();
            Assert.IsNotNull(media);
            Assert.IsNotNull(media.embedcode, media.mediaId + " has no embed code.");
            var adminServiceFactory = new AdminApiServiceFactory();

            List<SerialThumbnail> updatedThumbnails;
            TestUrl updateUrl = adminServiceFactory.CreateTestUrl(
                    "media", media.id, "thumbnail", "apiroot=https://" + TestUrl.AdminApiServer + "&webroot=https://.....[devServer]...../dev_storefront/&w=200&h=200&bw=700&bh=700");
            ValidationMessages messages = TestApiUtility.ApiPut<SerialThumbnail>(adminServiceFactory, updateUrl, /*serialData*/null, out updatedThumbnails, authorizedUser);

            if (messages.Errors().Count() > 0)
            {
                Assert.Fail(messages.Errors().First().Message);
            }
            Assert.IsNotNull(updatedThumbnails);  

            TestUrl getUrl = adminServiceFactory.CreateTestUrl(
                    "media", media.id, "thumbnail");

            byte[] callResults = TestApiUtility.GetBytes(getUrl.ToString());

            byte[] thumbnailFromGet = TestApiUtility.ApiGetBytes(adminServiceFactory, getUrl);

            Assert.AreEqual(1, updatedThumbnails.Count);
            Assert.IsTrue(thumbnailFromGet.ToList().SequenceEqual(updatedThumbnails[0].thumbnail.ToList()));
        }


    }
}
