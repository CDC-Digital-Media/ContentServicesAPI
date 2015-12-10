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
using Gov.Hhs.Cdc.Api.Admin;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.Global;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaCrudTests
{
    [TestClass]
    public class AddFeedItemTests
    {
        IApiServiceFactory adminService = new AdminApiServiceFactory();
        string authorizedUser = "";
        static int feedMediaId;
        //static CsMediaProvider mediaMgr = new CsMediaProvider();

        int mediaId;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            CurrentConnection.Name = "ContentServicesDb";

            feedMediaId = Convert.ToInt32(TestApiUtility.SinglePublishedFeedWithChildren().mediaId);
        }

        static AddFeedItemTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";            
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (mediaId > 0)
            {
                new CsMediaProvider().DeleteMedia(mediaId);
            }
        }

        [TestMethod]
        public void CanAddFeedItem()
        {
            SerialMediaAdmin parentItem = TestApiUtility.SinglePublishedFeedWithChildren();

            var item = new SerialMediaAdmin
            {
                mediaType = "Feed Item",
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                sourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                targetUrl = "http://www......[domain].....",
                status = "Staged",
                encoding = "utf-8",
                title = "test",
                parentRelationships = new List<SerialMediaRelationship> { new SerialMediaRelationship { relatedMediaId = Convert.ToInt32(parentItem.mediaId) } }
            };
            var result = TestApiUtility.CreateAdminMedia(item, authorizedUser);

            if (result.Meta.message.Count > 0)
            {
                foreach (var message in result.ValidationMessages.Messages)
                {
                    Console.WriteLine(message);
                }
                Assert.Fail(result.ValidationMessages.Messages.First().Message);
            }

            //CsMediaProvider mediaMgr = new CsMediaProvider();
            mediaId = int.Parse(result.ResultObject.First().mediaId);
            //mediaMgr.DeleteMedia(mediaId);
        }

        [TestMethod]
        public void CanSaveFeedItemWithFeedItemImage()
        {
            SerialMediaAdmin parentItem = TestApiUtility.SinglePublishedFeed();

            var item = new SerialMediaAdmin
            {
                mediaType = "Feed Item",
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                sourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                targetUrl = "http://www......[domain].....",
                status = "Staged",
                encoding = "utf-8",
                title = "test",
                parentRelationships = new List<SerialMediaRelationship> { new SerialMediaRelationship { relatedMediaId = Convert.ToInt32(parentItem.mediaId) } },
                feed = new SerialFeedDetail
                {
                    imageDescription = "some image desc",
                    imageHeight = "9",
                    imageWidth = "4",
                    imageSource = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                    imageLink = "http://vignette2.wikia.nocookie.net",
                    imageTitle = "feed item image title",
                }
            };
            var result = TestApiUtility.CreateAdminMedia(item, authorizedUser);

            if (result.Meta.message.Count > 0)
            {
                foreach (var message in result.ValidationMessages.Messages)
                {
                    Console.WriteLine(message);
                }
                Assert.Fail(result.ValidationMessages.Messages.First().Message);
            }

            var obj = result.ResultObject.First();
            Console.WriteLine(obj.mediaId);
            //mediaId = int.Parse(obj.mediaId);

            Assert.IsNotNull(obj.feed);

            //CsMediaProvider mediaMgr = new CsMediaProvider();
            mediaId = int.Parse(result.ResultObject.First().mediaId);
            //mediaMgr.DeleteMedia(mediaId);
        }

        [TestMethod]
        public void CanUpdateFeedItem()
        {
            var feed = TestApiUtility.SinglePublishedFeedWithChildren();
            Assert.IsNotNull(feed);

            Assert.IsTrue(feed.childRelationshipCount > 0);
            var childRelationship = feed.childRelationships.FirstOrDefault();
            Assert.IsNotNull(childRelationship);
            Console.WriteLine(childRelationship.relatedMediaId);

            var mediaIds = string.Join(",", feed.childRelationships.Select(cr => cr.relatedMediaId).ToList());

            var children = TestApiUtility.AdminApiMediaSearch("?mediaid=" + mediaIds);

            var child = children.Where(c => c.mediaType == "Feed Item").FirstOrDefault();
            Assert.IsNotNull(child);

            Assert.AreEqual("Feed Item", child.mediaType);

            var desc = "new description " + DateTime.Now.Ticks;
            var oldDesc = child.description;

            child.description = desc ;
            child.parentRelationships = new List<SerialMediaRelationship> { 
                new SerialMediaRelationship { relatedMediaId = feedMediaId } 
            };
            var results = TestApiUtility.UpdateAdminMedia(child, authorizedUser);
            if (results.Meta.status != 200)
            {
                Assert.Fail(results.Meta.message.First().userMessage);
            }
            Assert.AreEqual(200, results.Meta.status);
            Assert.AreEqual(1, results.ResultObject.Count);
            Assert.AreEqual(desc, results.ResultObject.First().description);

            ////save it back
            //child.description = oldDesc;
            //results = TestApiUtility.UpdateAdminMedia(child, authorizedUser);
            //if (results.Meta.status != 200)
            //{
            //    Assert.Fail(results.Meta.message.First().userMessage);
            //}
            //Assert.AreEqual(200, results.Meta.status);
            //Assert.AreEqual(1, results.ResultObject.Count);
            //Assert.AreEqual(oldDesc, results.ResultObject.First().description);
        }
    }


}
