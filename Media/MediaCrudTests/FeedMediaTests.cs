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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.CsBusinessObjects.Media;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Api;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaCrudTests
{
    [TestClass]
    public class FeedMediaTests
    {
        string authorizedUser = "";
        int mediaId;

        static FeedMediaTests()
        {
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
        public void CanAddFeedWithProxyUrl()
        {
            var item = new SerialMediaAdmin
            {
                mediaType = "Feed",
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                sourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                targetUrl = "http://www......[domain].....",
                status = "Staged",
                encoding = "utf-8",
                title = "1A A1 Test1",
                description = "proxyUrl Save",
                omnitureChannel = "saving omniture works",
                feed = new SerialFeedDetail()
                {
                    webMasterEmail = "IMTechNot@email",
                    editorialManager = "IMTechNot@email",
                    copyright = "Copyright 2016"
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

            mediaId = int.Parse(result.ResultObject.First().mediaId);
        }

        [TestMethod]
        public void CanRetrieveFeedCopyright()
        {
            var result = FeedMe();

            Assert.IsTrue(result.Count() > 0);
            var first = result.First();

            Assert.IsNotNull((first.MediaTypeSpecificDetail as FeedDetailObject).Copyright);
        }

        private static IEnumerable<MediaObject> FeedMe()
        {
            var criteria = new SearchCriteria { MediaType = "Feed" };
            var search = CsMediaSearchProvider.Search(criteria);
            var result = search
                .Where(i => i.MediaTypeSpecificDetail != null
                    && (i.MediaTypeSpecificDetail as FeedDetailObject).AssociatedImage != null
                    && (i.MediaTypeSpecificDetail as FeedDetailObject).AssociatedImage.Title != null);
            return result;
        }

        [TestMethod]
        public void CanRetrieveGuid()
        {
            var result = FeedMe();

            Assert.IsTrue(result.Count() > 0);
            var first = result.First();
            Assert.IsNotNull(first);

            Assert.IsNotNull(first.MediaGuid);
        }

        [TestMethod]
        public void CanRetrieveEditorialManager()
        {
            var result = FeedMe();

            Assert.IsTrue(result.Count() > 0);
            var first = result.First();

            Assert.IsNotNull(first);
            Assert.IsNotNull(first.MediaTypeSpecificDetail);
            Assert.IsInstanceOfType(first.MediaTypeSpecificDetail, typeof(FeedDetailObject));

            Assert.IsNotNull((first.MediaTypeSpecificDetail as FeedDetailObject).EditorialManager);
        }

        [TestMethod]
        public void CanRetrieveExistingFeedItemImage()
        {
            var feedMedia = TestApiUtility.SinglePublishedFeedWithChildrenAndAltImages();
            //SerialMediaAdmin kid = feedMedia.children.Where(c => c.alternateImages.Count > 0).First();

            Assert.IsNotNull(feedMedia);

            var feedIdWithFeedItemImage = feedMedia.id;
            var feedItem = Convert.ToInt32(feedMedia.id);

            var criteria = new SearchCriteria { MediaId = feedIdWithFeedItemImage.ToString() };
            var search = CsMediaSearchProvider.Search(criteria);
            Assert.AreEqual(1, search.Count());
            var feed = search.First();
            Assert.AreEqual(feedIdWithFeedItemImage, feed.MediaId);

            var item = feed.Children.FirstOrDefault(c => c.MediaId == feedItem);
            Assert.IsNotNull(item);

            var criteria2 = new SearchCriteria { MediaId = feedItem.ToString() };
            var search2 = CsMediaSearchProvider.Search(criteria2);

            var detailItem = search2.FirstOrDefault();
            Assert.IsNotNull(detailItem);
            var feedInfo = detailItem.MediaTypeSpecificDetail as FeedDetailObject;

            Assert.AreEqual(400, feedInfo.AssociatedImage.Height);
            Assert.AreEqual("Micrograph of Smallpox", feedInfo.AssociatedImage.Title);
        }

        [TestMethod]
        public void CanRetrieveWebmaster()
        {
            var result = FeedMe();

            Assert.IsTrue(result.Count() > 0);
            var first = result.First();
            Console.WriteLine(first.MediaId);

            Assert.IsNotNull(first);
            Assert.IsNotNull(first.MediaTypeSpecificDetail);
            Assert.IsInstanceOfType(first.MediaTypeSpecificDetail, typeof(FeedDetailObject));

            Assert.IsNotNull((first.MediaTypeSpecificDetail as FeedDetailObject).WebMasterEmail);
        }

        [TestMethod]
        public void CanRetrieveImageTitle()
        {
            var result = FeedMe();

            Assert.IsTrue(result.Count() > 0);
            var first = result.First();
            Assert.IsNotNull(first);

            Assert.IsNotNull(first.Children, first.MediaId.ToString() + " has no children.");

            Assert.IsNotNull(first.MediaTypeSpecificDetail);
            Assert.IsInstanceOfType(first.MediaTypeSpecificDetail, typeof(FeedDetailObject));

            Assert.IsNotNull((first.MediaTypeSpecificDetail as FeedDetailObject).AssociatedImage.Title, first.MediaId.ToString());
        }

        [TestMethod]
        public void CanRetrieveFeedImageUrl()
        {
            var result = FeedMe();

            Assert.IsTrue(result.Count() > 0);
            var first = result.First();
            Assert.IsNotNull(first);
            Assert.IsNotNull(first.MediaTypeSpecificDetail);
            Assert.IsInstanceOfType(first.MediaTypeSpecificDetail, typeof(FeedDetailObject));

            Console.WriteLine(first.MediaId);

            Assert.IsNotNull((first.MediaTypeSpecificDetail as FeedDetailObject).AssociatedImage.SourceUrl, "Feed Images are being added without urls.");
        }

        [TestMethod]
        public void CanRetrieveImageLink()
        {
            var result = FeedMe();

            Assert.IsTrue(result.Count() > 0);
            var first = result.First();
            Assert.IsNotNull(first);
            Assert.IsNotNull(first.MediaTypeSpecificDetail);
            Assert.IsInstanceOfType(first.MediaTypeSpecificDetail, typeof(FeedDetailObject));

            Assert.IsNotNull((first.MediaTypeSpecificDetail as FeedDetailObject).AssociatedImage.TargetUrl);
        }

        [TestMethod]
        public void CanRetrieveImageHeight()
        {
            var result = FeedMe();

            Assert.IsTrue(result.Count() > 0);
            var first = result.First();
            Assert.IsNotNull(first);
            Assert.IsNotNull(first.MediaTypeSpecificDetail);
            Assert.IsInstanceOfType(first.MediaTypeSpecificDetail, typeof(FeedDetailObject));

            Assert.IsNotNull((first.MediaTypeSpecificDetail as FeedDetailObject).AssociatedImage.Height);
        }

        [TestMethod]
        public void CanRetrieveImageWidth()
        {
            var result = FeedMe();

            Assert.IsTrue(result.Count() > 0);
            var first = result.First();
            Assert.IsNotNull(first);
            Assert.IsNotNull(first.MediaTypeSpecificDetail);
            Assert.IsInstanceOfType(first.MediaTypeSpecificDetail, typeof(FeedDetailObject));

            Assert.IsNotNull((first.MediaTypeSpecificDetail as FeedDetailObject).AssociatedImage.Width);
        }

        [TestMethod]
        public void CanRetrieveProxyUrl()
        {
            var result = FeedMe();

            Assert.IsTrue(result.Count() > 0);
            var first = result.First();
            Assert.IsNotNull(first);
            Assert.IsNotNull(first.MediaTypeSpecificDetail);
            Assert.IsInstanceOfType(first.MediaTypeSpecificDetail, typeof(FeedDetailObject));

            Assert.IsNotNull((first.MediaTypeSpecificDetail as FeedDetailObject).AssociatedImage.Width);

        }

        [TestMethod]
        public void CanParseFeedItemEnum()
        {
            var mediaType = "Feed Item";
            var trimmed = mediaType.Replace(" ", "");
            Assert.AreEqual("FeedItem", trimmed);

            var parm = new MediaTypeParms(mediaType);
            Assert.AreEqual("FeedItem", parm.Code);
        }

        [TestMethod]
        public void FeedItemIsNotFeed()
        {
            var mediaType = "Feed Item";
            var trimmed = mediaType.Replace(" ", "");

            var parm = new MediaTypeParms(mediaType);
            Assert.IsFalse(parm.IsFeed);
        }

        [TestMethod]
        public void FeedItemIsFeedItem()
        {
            var mediaType = "Feed Item";
            var trimmed = mediaType.Replace(" ", "");

            var parm = new MediaTypeParms(mediaType);
            Assert.IsTrue(parm.IsFeedItem);
        }

        [TestMethod]
        public void FeedImageIsFeedImage()
        {
            var mediaType = "Feed Image";
            var media = new MediaObject { MediaTypeCode = mediaType };
            Assert.IsTrue(media.MediaTypeParms.IsFeedImage);
        }

        [TestMethod]
        public void CollectionIsCollection()
        {
            var mediaType = "Collection";
            var media = new MediaObject { MediaTypeCode = mediaType };
            Assert.IsTrue(media.MediaTypeParms.IsCollection);

        }

    }
}
