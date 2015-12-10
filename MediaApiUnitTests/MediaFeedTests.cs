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
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Api.Admin;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Connection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaApiUnitTests
{
    [TestClass]
    public class MediaFeedTests
    {

        public MediaFeedTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        private string authorizedUser = "";
        public static List<int> topicIds = new List<int>() { 25272, 25329 };
        private CsMediaProvider mediaMgr = new CsMediaProvider();
        private Stack<SerialMediaAdmin> savedSerialAdmins = new Stack<SerialMediaAdmin>();
        private Stack<MediaObject> savedMedias = new Stack<MediaObject>();

        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (var media in savedSerialAdmins)
            {
                if (media != null && !String.IsNullOrEmpty(media.id))
                    mediaMgr.DeleteMedia(Convert.ToInt32(media.id));
            }

            foreach (var media in savedMedias)
            {
                if (media != null && media.Id > 0)
                    mediaMgr.DeleteMedia(media.Id);
            }
        }

        [TestMethod]
        public void CannotAddDuplicateFeedItem()
        {
            SerialMediaAdmin feedToTest = TestApiUtility.SinglePublishedFeed();
            //SerialMediaAdmin feedToTest = null;
            //foreach (SerialMediaAdmin tempFeed in feeds)
            //{
            //    if (tempFeed.mediaType == "Feed" && tempFeed.childCount > 0)
            //    {
            //        feedToTest = tempFeed;
            //        break;
            //    }
            //}


            //check the children
            Assert.IsNotNull(feedToTest);
            ValidationMessages messages = new ValidationMessages();

            //var criteria = "?mediatype=feed&status=published";
            //var results = TestApiUtility.AdminApiMediaSearch(criteria);
            CsMediaProvider mediaProvider = new CsMediaProvider();
            MediaObject parent = mediaProvider.GetMedia(Convert.ToInt32(feedToTest.mediaId), out messages);

            //we know it has children, so let's create a new feed item
            MediaObject newMediaFeedItem1 = new MediaObject() 
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = "Feed Item",
                CharacterEncodingCode = "UTF-8",
                Title = "CreateNewFeedItem",
                Description = "test",
                SourceUrlForApi = "http://www.foxnews.com/rss/index.html",
                SourceUrl = "http://www.foxnews.com/rss/index.html",
                TargetUrl = "http://www.foxnews.com/rss/index.html",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null,
                Parents=new List<MediaObject>() {parent},
                RelatedMediaId = Convert.ToInt32(feedToTest.mediaId),
                RelationshipTypeName = "Is Child Of",
            };
            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicIds);
            newMediaFeedItem1.AttributeValues = topics;

            MediaRelationshipObject mediaRelObj = new MediaRelationshipObject();
            mediaRelObj.RelatedMediaId = Convert.ToInt32(feedToTest.mediaId);
            mediaRelObj.RelationshipTypeName = "Is Child Of";
            List<MediaRelationshipObject> rels = new List<MediaRelationshipObject>();
            rels.Add(mediaRelObj);
            newMediaFeedItem1.MediaRelationships = rels;
            

            var saved1 = new MediaObject();

            //TODO:  Modify to not use out parameter
            messages = mediaMgr.SaveMedia(newMediaFeedItem1, out saved1);
            savedMedias.Push(saved1);

            //Add the second one
            MediaObject newMediaFeedItem2 = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = "Feed Item",
                CharacterEncodingCode = "UTF-8",
                Title = "CreateNewFeedItem",
                Description = "test",
                SourceUrlForApi = "http://www.foxnews.com/rss/index.html",
                SourceUrl = "http://www.foxnews.com/rss/index.html",
                TargetUrl = "http://www.foxnews.com/rss/index.html",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null,
                Parents = new List<MediaObject>() { parent },
                RelatedMediaId = Convert.ToInt32(feedToTest.mediaId),
                RelationshipTypeName = "Is Child Of"
            };
            newMediaFeedItem2.AttributeValues = topics;
            newMediaFeedItem2.MediaRelationships = rels;

            var saved2 = new MediaObject();
            messages.Add(mediaMgr.SaveMedia(newMediaFeedItem2, out saved2));
            Assert.IsNull(saved2);
            if (saved2 != null)
            {
                savedMedias.Push(saved2);
            }
            
        }

        [TestMethod]
        public void CanRetrieveRSSFeed()
        {
            var mediaId = 138929;
            var url = string.Format("{0}://{1}/api/{2}/resources/{3}/{4}", TestUrl.Protocol, TestUrl.PublicApiServer, "v2", "media", mediaId.ToString() + ".rss?showchildlevel=1");
            string result = TestApiUtility.Get(url);
            Console.WriteLine(url);
            Console.WriteLine(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void CanRetrieveAtomFeed()
        {
            var mediaId = 138929;
            var url = string.Format("{0}://{1}/api/{2}/resources/{3}/{4}", TestUrl.Protocol, TestUrl.PublicApiServer, "v2", "media", mediaId.ToString() + ".atom?showchildlevel=1");
            string result = TestApiUtility.Get(url);
            Console.WriteLine(url);
            Console.WriteLine(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void CanRetrieveCollection()
        {
            var mediaId = 143619;
            var url = string.Format("{0}://{1}/api/{2}/resources/{3}/{4}", TestUrl.Protocol, TestUrl.PublicApiServer, "v2", "media", mediaId.ToString() + "?showchildlevel=1");
            string result = TestApiUtility.Get(url);
            Console.WriteLine(url);
            Console.WriteLine(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void CanRetrieveFeedDetailsViaOldSearch()
        {

            var feeds = TestApiUtility.GetAllPublishedFeeds();
            //get a feed with images
            var feedFiltered = feeds.Where(i => i.feed.imageTitle != null);

            var feed = feedFiltered.First();
            var mediaId = feed.mediaId;

            Assert.IsNotNull(feed.feed);
            Assert.IsNotNull(feed.feed.copyright);
            Assert.IsNotNull(feed.feed.editorialManager);
            Assert.IsNotNull(feed.feed.webMasterEmail);
            Assert.IsNotNull(feed.feed.imageTitle, mediaId + " has no feed image");
            Assert.IsNotNull(feed.feed.imageDescription);
            Assert.IsNotNull(feed.feed.imageSource, "Feed images are being added without an image source url");
            Assert.IsNotNull(feed.feed.imageLink);
            Assert.IsNotNull(feed.feed.imageHeight);
            Assert.IsNotNull(feed.feed.imageWidth);
        }

        [TestMethod]
        public void CanRetrieveFeedDetailsViaNewSearch()
        {
            var feeds = TestApiUtility.GetAllPublishedFeeds();
            //get a feed with images
            var feedFiltered = feeds.Where(i => i.feed.imageTitle != null);

            var feed = feedFiltered.First();

            Assert.IsNotNull(feed.feed);
            Assert.IsNotNull(feed.feed.copyright);
            Assert.IsNotNull(feed.feed.editorialManager);
            Assert.IsNotNull(feed.feed.webMasterEmail);
            Assert.IsNotNull(feed.feed.imageTitle, feed.mediaId + " does not have a feed image.");
            Assert.IsNotNull(feed.feed.imageDescription);
            Assert.IsNotNull(feed.feed.imageSource, "Feed images are being added without an image source url");
            Assert.IsNotNull(feed.feed.imageLink);
            Assert.IsNotNull(feed.feed.imageHeight);
            Assert.IsNotNull(feed.feed.imageWidth);
        }

        private static MediaObject CreateNewFeed()
        {
            string imageUrl = "http://phil......[domain]...../PHIL_Images/18171/18171_lores.jpg";
            string nameUniqueMaker = DateTime.Now.Ticks.ToString();

            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = "Feed",
                CharacterEncodingCode = "UTF-8",
                Title = "CreateNewFeed" + nameUniqueMaker,
                Description = "test",
                SourceUrlForApi = "http://www.foxnews.com/rss/index.html",
                TargetUrl = "http://www.foxnews.com/rss/index.html",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null
            };

            FeedDetailObject newFeedDetail = new FeedDetailObject();
            newFeedDetail.AssociatedImage = new MediaImage();
            newFeedDetail.AssociatedImage.SourceUrl = imageUrl;
            newFeedDetail.AssociatedImage.Title = "testimage";
            newFeedDetail.AssociatedImage.Height = 100;
            newFeedDetail.AssociatedImage.Width = 50;
            newFeedDetail.AssociatedImage.Description = "test image description";
            newFeedDetail.AssociatedImage.TargetUrl = imageUrl;

            newMediaObject.MediaTypeSpecificDetail = newFeedDetail;

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicIds);
            newMediaObject.AttributeValues = topics;


            return newMediaObject;
        }
        private static List<AttributeValueObject> CreateAttributeValues(string attributeName, List<int> valueIds)
        {
            List<AttributeValueObject> attributeValues = new List<AttributeValueObject>();
            int displayOrder = 10;
            foreach (int valueId in valueIds)
            {
                attributeValues.Add(NewAttributeValue(attributeName, valueId, displayOrder));
                displayOrder += 10;
            };
            return attributeValues;
        }
        private static AttributeValueObject NewAttributeValue(string attributeName, int valueId, int displayOrder)
        {
            AttributeValueObject attributeValue = new AttributeValueObject()
            {
                ValueKey = new ValueKey(valueId, "English"),
                AttributeId = 1,
                AttributeName = attributeName,
                DisplayOrdinal = displayOrder,
                IsActive = true
            };
            return attributeValue;
        }

        [TestMethod]
        public void CanRetrieveFeedItemImageDetails()
        {
            MediaObject media = CreateNewFeed();
            var saved = new MediaObject();

            //TODO:  Modify to not use out parameter

            var messages = mediaMgr.SaveMedia(media, out saved);

            var feed = TestApiUtility.AdminApiMediaSearch(saved.MediaId.ToString()).First();

            Assert.IsNotNull(feed.feed);
            Assert.IsNotNull(feed.feed.imageTitle, saved.MediaId.ToString() + " has no feed item image title");
            Assert.IsNotNull(feed.feed.imageDescription);
            Assert.IsNotNull(feed.feed.imageSource, "Feed item images are being added without an image source url");
            Assert.IsNotNull(feed.feed.imageLink);
            Assert.IsNotNull(feed.feed.imageHeight);
            Assert.IsNotNull(feed.feed.imageWidth);

            savedMedias.Push(saved);
            //mediaMgr.DeleteMedia(saved.Id);
        }

        [TestMethod]
        public void CanAddFeed()
        {
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            List<SerialMediaAdmin> feeds;

            ValidationMessages messages = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media", "", "", ""),
                "{" +
                    "'sourcecode': 'Centers for Disease Control and Prevention'," +
                    "'mediatype': 'Feed'," +
                    "'mimetype': 'text/xml'," +
                    "'encoding': 'utf-8'," +
                    "'title': 'Test Adding a Feed " + DateTime.Now.Ticks + "'," +
                    "'description': 'SMP Test'," +
                    "'sourceurl': 'http://www......[domain]...../foodsafety/rawmilk/raw-milk-index.html?adfa=dfa'," +
                    "'targeturl': 'http://www......[domain]...../foodsafety/rawmilk/raw-milk-index.html?adfa=dfa'," +
                    "'language': 'english'," +
                    "'status': 'Published'," +
                    "'author': 'The Man'," +
                    "'topics': [25493, 25526]," +
                    "'feed': {" +
                        "'editorialManager': 'IMTechNot@email'," +
                        "'webMasterEmail': 'IMTechNot@email'," +
                        "'copyright': 'Copyright 2016'," +
                        "'imageTitle': 'Micrograph of Smallpox'," +
                        "'imageSource': 'http://phil......[domain]...../PHIL_Images/15733/15733_lores.jpg'," +
                        "'imageLink': 'http://phil......[domain]...../PHIL_Images/15733/15733_lores.jpg'," +
                        "'imageHeight': 1," +
                        "'imageWidth': 2," +
                        "'imageDescription': 'Under a high magnification'" +
                    "}" +
                "}",
                out feeds, authorizedUser);

            if (messages.Errors().Any())
            {
                Assert.Fail(messages.Errors().First().Message);
            }
            Console.WriteLine(feeds[0].id);
            Assert.IsNotNull(feeds[0].feed);
            Assert.IsTrue(feeds[0].feed.copyright == "Copyright 2016");
            Assert.AreEqual("Micrograph of Smallpox", feeds[0].feed.imageTitle);
            Assert.AreEqual("http://phil......[domain]...../PHIL_Images/15733/15733_lores.jpg", feeds[0].feed.imageSource);
            Assert.IsTrue(feeds[0].feed.imageLink == "http://phil......[domain]...../PHIL_Images/15733/15733_lores.jpg");
            Assert.IsTrue(feeds[0].feed.imageHeight == "1");
            Assert.IsTrue(feeds[0].feed.imageWidth == "2");
            Assert.IsTrue(feeds[0].feed.imageDescription == "Under a high magnification");

            messages = TestApiUtility.ApiDelete(adminService, adminService.CreateTestUrl("media", feeds[0].id, "", ""), "", authorizedUser);
            if (messages.NumberOfErrors > 0)
            {
                Assert.Fail(messages.Errors().FirstOrDefault().Message);
            }

            messages = TestApiUtility.ApiGet<SerialMediaAdmin>(adminService, adminService.CreateTestUrl("media", feeds[0].id, "", ""), out feeds);
            Assert.AreEqual(0, messages.Errors().Count());
            Assert.AreEqual(0, feeds.Count());
        }

        [TestMethod]
        public void CanAddValidFeedTitle()
        {
            string uniqueName = "Marcie-" + DateTime.Now.Ticks.ToString();

            IApiServiceFactory adminService = new AdminApiServiceFactory();
            List<SerialMediaAdmin> feeds;

            ValidationMessages messages = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media", "", "", ""),
                "{" +
                    "'sourcecode': 'Centers for Disease Control and Prevention'," +
                    "'mediatype': 'Feed'," +
                    "'mimetype': 'text/xml'," +
                    "'encoding': 'utf-8'," +
                    "'title': '" + uniqueName + "'," +
                    "'description': 'SMP Test'," +
                    "'sourceurl': 'http://www......[domain]...../foodsafety/rawmilk/raw-milk-index.html?adfa=dfa'," +
                    "'targeturl': 'http://www......[domain]...../foodsafety/rawmilk/raw-milk-index.html?adfa=dfa'," +
                    "'language': 'english'," +
                    "'status': 'Published'," +
                    "'author': 'The Man'," +
                    "'topics': [25493, 25526]," +
                    "'feed': {" +
                        "'editorialManager': 'IMTechNot@email'," +
                        "'webMasterEmail': 'IMTechNot@email'," +
                        "'copyright': 'Copyright 2016'," +
                        "'imageTitle': 'Micrograph of Smallpox'," +
                        "'imageSource': 'http://phil......[domain]...../PHIL_Images/15733/15733_lores.jpg'," +
                        "'imageLink': 'http://phil......[domain]...../PHIL_Images/15733/15733_lores.jpg'," +
                        "'imageHeight': 1," +
                        "'imageWidth': 2," +
                        "'imageDescription': 'Under a high magnification'" +
                    "}" +
                "}",
                out feeds, authorizedUser);

            if (messages.Errors().Any())
            {
                Assert.Fail(messages.Errors().First().Message);
            }
            Assert.IsNotNull(feeds[0].feed);
            Assert.AreEqual(uniqueName, feeds[0].title);
            Assert.IsTrue(feeds[0].title == uniqueName);

            messages = TestApiUtility.ApiDelete(adminService, adminService.CreateTestUrl("media", feeds[0].id, "", ""), "", authorizedUser);
            if (messages.Errors().Any())
            {
                Assert.Fail(messages.Errors().First().Message);
            }

            messages = TestApiUtility.ApiGet<SerialMediaAdmin>(adminService, adminService.CreateTestUrl("media", feeds[0].id, "", ""), out feeds);
            if (messages.Errors().Any())
            {
                Assert.Fail(messages.Errors().First().Message);
            }
            Assert.AreEqual(0, feeds.Count());
        }

        [TestMethod]
        public void CanAddNewFeed()
        {
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            List<SerialMediaAdmin> feeds;
            string uniqueName = "Marcie-" + DateTime.Now.Ticks.ToString();

            ValidationMessages messages = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media", "", "", ""),
                "{" +
                    "'sourcecode': 'Centers for Disease Control and Prevention'," +
                    "'mediatype': 'Feed'," +
                    "'mimetype': 'text/xml'," +
                    "'encoding': 'utf-8'," +
                    "'title': '" + uniqueName + "'," +
                    "'description': 'SMP Test'," +
                    "'sourceurl': 'http://www......[domain]...../foodsafety/rawmilk/raw-milk-index.html?adfa=dfa'," +
                    "'targeturl': 'http://www......[domain]...../foodsafety/rawmilk/raw-milk-index.html?adfa=dfa'," +
                    "'language': 'english'," +
                    "'status': 'Published'," +
                    "'author': 'The Man'," +
                    "'topics': [25493, 25526]," +
                    "'feed': {" +
                        "'editorialManager': 'IMTechNot@email'," +
                        "'webMasterEmail': 'IMTechNot@email'," +
                        "'copyright': 'Copyright 2016'" +

                    "}" +
                "}",
                out feeds, authorizedUser);

            if (messages.Errors().Any())
            {
                Assert.Fail(messages.Errors().First().Message);
            }
            Assert.IsNotNull(feeds[0].feed);
            Assert.AreEqual(uniqueName, feeds[0].title);
            Assert.IsTrue(feeds[0].title == uniqueName);

            messages = TestApiUtility.ApiDelete(adminService, adminService.CreateTestUrl("media", feeds[0].id, "", ""), "", authorizedUser);
            if (messages.Errors().Any())
            {
                Assert.Fail(messages.Errors().First().Message);
            }

            messages = TestApiUtility.ApiGet<SerialMediaAdmin>(adminService, adminService.CreateTestUrl("media", feeds[0].id, "", ""), out feeds);
            if (messages.Errors().Any())
            {
                Assert.Fail(messages.Errors().First().Message);
            }
            Assert.AreEqual(0, feeds.Count());
        }
        [TestMethod]
        public void CanUpdateFeedImage()
        {
            MediaObject media = CreateNewFeed();
            var saved = new MediaObject();

            var messages = mediaMgr.SaveMedia(media, out saved);

            var image1 = "http://phil......[domain]...../PHIL_Images/18171/18171_lores.jpg";
            var image2 = "http://phil......[domain]...../PHIL_Images/18172/18172_lores.jpg";
            string updatedTo = "";

            var existingFeedWithImage = saved.Id;

            var existing = TestApiUtility.SingleAdminMedia(existingFeedWithImage);
            Assert.IsNotNull(existing.feed);
            Assert.IsNotNull(existing.feed.imageSource);

            if (existing.feed.imageSource == image1)
            {
                updatedTo = image2;
            }
            else
            {
                updatedTo = image1;
            }

            existing.feed.imageSource = updatedTo;
            existing.feed.imageTitle = "Updated on " + DateTime.UtcNow.ToString();

            var output = TestApiUtility.UpdateAdminMedia(existing, authorizedUser);
            if (output.ResultObject.Count != 1)
            {
                Assert.Fail(output.Meta.message.First().userMessage);
            }
            Assert.AreEqual(1, output.ResultObject.Count);

            Assert.AreEqual(updatedTo, output.ResultObject.First().feed.imageSource);
            savedMedias.Push(media);
        }


        [TestMethod]
        public void CannotSaveBlankFeedItem()
        {
            var item = new SerialMediaAdmin
            {
                mediaType = "Feed item",
                //sourceCode = "Centers for Disease Control and Prevention",
                //language = "English",
                //sourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                //targetUrl = "http://www......[domain].....",
                //status = "Published",
                //topics = topicIds,
                //encoding = "utf-8",
                //title = "Tuff Badge",
                //description = "desc"
            };
            var result = TestApiUtility.CreateAdminMedia(item, authorizedUser);
            Assert.IsTrue(result.Meta.message.Count > 0);
            foreach (var message in result.ValidationMessages.Messages)
            {
                Console.WriteLine(message);
            }

            savedSerialAdmins.Push(item);
        }

        [TestMethod]
        public void CannotSaveFeedItemTitleWith101Characters()
        {
            var title = "test".PadRight(1025,'0');
            Assert.AreEqual(1025, title.Length);

            var item = new SerialMediaAdmin
            {
                mediaType = "Feed item",
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                sourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                targetUrl = "http://www......[domain].....",
                status = "Published",
                topics = new List<int>(topicIds),
                encoding = "utf-8",
                title = title,
                //description = desc
            };

            var result = TestApiUtility.CreateAdminMedia(item, authorizedUser);
            Assert.IsTrue(result.Meta.message.Count > 0);
            foreach (var message in result.ValidationMessages.Messages)
            {
                Console.WriteLine(message);
            }
            Assert.IsTrue((result.ValidationMessages.Messages.Any(m => m.Message.Contains("Title must"))));

            savedSerialAdmins.Push(item);
        }

        [TestMethod]
        public void TargetUrlNotRequiredForFeedItems()
        {
            var item = new SerialMediaAdmin
            {
                mediaType = "Feed item",
            };
            var result = TestApiUtility.CreateAdminMedia(item, authorizedUser);
            Assert.IsFalse(result.ValidationMessages.Messages.Any(m => m.Message.Contains("TargetUrl")));
        }

        [TestMethod]
        public void CannotSaveFeedItemWithoutFeed()
        {
            var item = new SerialMediaAdmin
            {
                mediaType = "Feed item",
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                sourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                targetUrl = "http://www......[domain].....",
                status = "Staged",
                //topics = validTopicIds,
                encoding = "utf-8",
                title = "test",
                //description = desc
            };

            var result = TestApiUtility.CreateAdminMedia(item, authorizedUser);
            Assert.IsTrue(result.Meta.message.Count > 0);
            foreach (var message in result.ValidationMessages.Messages)
            {
                Console.WriteLine(message);
            }
            Assert.IsTrue((result.ValidationMessages.Messages.Any(m => m.Message.Contains("parent"))));
        }

        [TestMethod]
        public void CannotSaveFeedItemWithNonFeedParent()
        {
            var nonFeedMediaId = 138478;
            var item = new SerialMediaAdmin
            {
                mediaType = "Feed item",
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                sourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                targetUrl = "http://www......[domain].....",
                status = "Staged",
                //topics = validTopicIds,
                encoding = "utf-8",
                title = "test",
                parentRelationships = new List<SerialMediaRelationship> { new SerialMediaRelationship { relatedMediaId = nonFeedMediaId } }
                //description = desc
            };
            var result = TestApiUtility.CreateAdminMedia(item, authorizedUser);
            Assert.IsTrue(result.Meta.message.Count > 0);
            foreach (var message in result.ValidationMessages.Messages)
            {
                Console.WriteLine(message);
            }
            Assert.IsTrue((result.ValidationMessages.Messages.Any(m => m.Message.Contains("not a feed"))));

        }

        [TestMethod]
        public void CannotSaveFeedItemWithNonexistentParent()
        {
            var nonexistentMediaId = -7;
            var item = new SerialMediaAdmin
            {
                mediaType = "Feed item",
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                sourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                targetUrl = "http://www......[domain].....",
                status = "Staged",
                //topics = validTopicIds,
                encoding = "utf-8",
                title = "test",
                parentRelationships = new List<SerialMediaRelationship> { new SerialMediaRelationship { relatedMediaId = nonexistentMediaId } }
                //description = desc
            };
            var result = TestApiUtility.CreateAdminMedia(item, authorizedUser);
            Assert.IsTrue(result.Meta.message.Count > 0);
            foreach (var message in result.ValidationMessages.Messages)
            {
                Console.WriteLine(message);
            }
            Assert.IsTrue((result.ValidationMessages.Messages.Any(m => m.Message.Contains("not found"))));
        }

        [TestMethod]
        public void CannotSaveFeedItemWithoutSourceUrl()
        {
            var feedMediaId = 298161;
            var item = new SerialMediaAdmin
            {
                mediaType = "Feed item",
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                targetUrl = "http://www......[domain].....",
                status = "Staged",
                //topics = validTopicIds,
                encoding = "utf-8",
                title = "test",
                parentRelationships = new List<SerialMediaRelationship> { new SerialMediaRelationship { relatedMediaId = feedMediaId } }
                //description = desc
            };
            var result = TestApiUtility.CreateAdminMedia(item, authorizedUser);
            Assert.IsTrue(result.Meta.message.Count > 0);
            foreach (var message in result.ValidationMessages.Messages)
            {
                Console.WriteLine(message);
            }
            Assert.IsTrue((result.ValidationMessages.Messages.Any(m => m.Message.Contains("Source"))));

        }
    }
}
