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

using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Api.Admin;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaCrudTests
{
    [TestClass]
    public class CollectionTests
    {
        int collectionMediaId = 138478;

        static CollectionTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        //could be a global method
        private static List<AttributeValueObject> CreateAttributeValues(string attributeName, List<int> valueIds)
        {
            var attributeValues = new List<AttributeValueObject>();
            int displayOrder = 10;
            foreach (int valueId in valueIds)
            {
                attributeValues.Add(NewAttributeValue(attributeName, valueId, displayOrder));
                displayOrder += 10;
            };
            return attributeValues;
        }

        //could be a global method
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

        public static MediaObject CreateNewCollection()
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = "Collection",
                CharacterEncodingCode = "UTF-8",
                Title = "CreateNewCollection",
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
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
            };
            List<int> topicIds = new List<int>() { 25272, 25329 };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicIds);
            newMediaObject.AttributeValues = topics;


            return newMediaObject;
        }

        [TestMethod]
        public void CanAddItemToCollection()
        {
            var adminService = new AdminApiServiceFactory();
            var media = AdminApiCalls.SingleCollection();

            //media that is not already part of a collection
            var child = TestApiUtility.PublishedHtml().First(h => h.parentCount == 0);
            int[] testChildMediaIdsToInsert = { int.Parse(child.mediaId) };

            TestMediaItems media1 = new TestMediaItems(media, adminService, testChildMediaIdsToInsert);

            Assert.AreEqual(1, media1.Media.childRelationships.Count);

            media1.UpdateChildren(testChildMediaIdsToInsert);

            var mediaFromGet = AdminApiCalls.SingleMedia(child.mediaId);
            Assert.AreEqual(1, mediaFromGet.parentCount);
        }

        [TestMethod]
        public void CanAddItemTo2Collections()
        {
            var adminService = new AdminApiServiceFactory();
            var collectionsWithoutItems = AdminApiCalls.Collections().Where(c => c.childRelationshipCount == 0);
            var collection1 = collectionsWithoutItems.First();
            var collection2 = collectionsWithoutItems.First(c => c.mediaId != collection1.mediaId);
            Assert.AreNotEqual(collection1.mediaId, collection2.mediaId);

            var child = TestApiUtility.PublishedHtml().Where(h => h.childRelationshipCount == 0).First();
            int[] testChildMediaIdsToInsert = { int.Parse(child.mediaId) };
            Console.WriteLine("Media ID to add to 2 collections: " + child.mediaId);

            var tmi1 = new TestMediaItems(collection1, adminService, testChildMediaIdsToInsert);

            Assert.AreEqual(1, tmi1.Media.childRelationships.Count);

            tmi1.UpdateChildren(testChildMediaIdsToInsert);

            var mediaFromGet = AdminApiCalls.SingleMedia(collection1.mediaId);
            Assert.AreEqual(1, mediaFromGet.childRelationshipCount);

            var tmi2 = new TestMediaItems(collection2, adminService, testChildMediaIdsToInsert);

            Assert.AreEqual(1, tmi2.Media.childRelationships.Count);

            tmi2.UpdateChildren(testChildMediaIdsToInsert);

            var coll2FromGet = AdminApiCalls.SingleMedia(collection2.mediaId);
            Assert.AreEqual(1, coll2FromGet.childRelationshipCount);
        }

        [TestMethod]
        public void CanSaveItemAddedTo2Collections()
        {
            Console.WriteLine("start of test");
            var adminService = new AdminApiServiceFactory();
            var collectionsWithoutItems = AdminApiCalls.Collections().Where(c => c.childRelationshipCount == 0);
            Console.WriteLine(collectionsWithoutItems.Count().ToString());
            var collection1 = collectionsWithoutItems.First();
            Console.WriteLine(collection1.mediaId + " is collection1");
            var collection2 = collectionsWithoutItems.First(c => c.mediaId != collection1.mediaId);
            Assert.AreNotEqual(collection1.mediaId, collection2.mediaId);

            var child = TestApiUtility.PublishedHtml().Where(h => h.parentCount == 0).First();
            Assert.IsNotNull(child);
            Console.WriteLine("Media ID to add to 2 collections: " + child.mediaId);
            int[] testChildMediaIdsToInsert = { int.Parse(child.mediaId) };

            var tmi1 = new TestMediaItems(collection1, adminService, testChildMediaIdsToInsert);
            tmi1.UpdateChildren(testChildMediaIdsToInsert);

            var tmi2 = new TestMediaItems(collection2, adminService, testChildMediaIdsToInsert);
            tmi2.UpdateChildren(testChildMediaIdsToInsert);

            var mediaFromGet = AdminApiCalls.SingleMedia(child.mediaId);
            Assert.AreEqual(2, mediaFromGet.parentRelationships.Count);

            List<SerialMediaAdmin> updatedAdmins;
            var url = adminService.CreateTestUrl("media", child.mediaId);
            string authorizedUser = "";
            var messages = TestApiUtility.ApiPut<SerialMediaAdmin>(adminService, url, child, out updatedAdmins, authorizedUser);
            if (messages.NumberOfErrors > 0)
            {
                var firstError = messages.Errors().First();
                Assert.Fail(firstError.Message + " : " + firstError.DeveloperMessage);
            }
            mediaFromGet = AdminApiCalls.SingleMedia(child.mediaId);
            Assert.AreEqual(2, mediaFromGet.parentRelationships.Count);
        }


        /// <summary>
        // If we add 3 medias where 1 => 2, 2 => 3, 3=> 1, we get a recursive loop error
        /// </summary>
        [TestMethod]
        public void TestRecursiveLoopError()
        {
            var adminService = new AdminApiServiceFactory();
            List<SerialMediaAdmin> sampleMedias;

            int[] testMediaIds = { 1, 2 };

            TestApiUtility.ApiGet<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media", collectionMediaId), out sampleMedias);

            TestMediaItems media1 = new TestMediaItems(sampleMedias[0], adminService, testMediaIds);
            media1.Insert();

            TestMediaItems media2 = new TestMediaItems(sampleMedias[0], adminService, media1.MediaId);
            media2.Media.sourceUrl = null;
            media2.Media.mediaType = "Collection";
            media2.Insert();

            TestMediaItems media3 = new TestMediaItems(sampleMedias[0], adminService, media2.MediaId);
            media3.Insert();

            media1.UpdateChildren(media3.MediaId);
            Console.WriteLine(media1.UpdateValidationMessages.Errors().First().Message);
            Assert.AreEqual(4, media1.UpdateValidationMessages.Errors().Count());
            string errorMessage = "Relationship already has an inherited";
            Assert.IsTrue(string.Equals(errorMessage,
                media1.UpdateValidationMessages.Errors().ToList()[0].Message.Substring(0, errorMessage.Length)
                ));

            media1.Delete();
            media2.Delete();
            media3.Delete();

        }


        private class TestMediaItems
        {
            public SerialMediaAdmin Media { get; set; }
            AdminApiServiceFactory AdminService { get; set; }
            public int MediaId { get { return int.Parse(Media.id); } }
            public ValidationMessages InsertValidationMessages { get; set; }
            public ValidationMessages UpdateValidationMessages { get; set; }
            private string authorizedUser = "";


            public TestMediaItems(SerialMediaAdmin mediaObject, AdminApiServiceFactory adminService, params int[] children)
            {
                if (mediaObject == null) { throw new InvalidOperationException("media not found"); }
                Media = mediaObject;
                AdminService = adminService;
                Media.id = mediaObject.id;
                Media.mediaType = "Collection";
                Media.childRelationships = children
                    .Select(i => new SerialMediaRelationship() { relatedMediaId = i, displayOrdinal = 20 - i }).ToList();
            }

            public void Insert()
            {
                List<SerialMediaAdmin> updatedAdmins;
                TestUrl insertUrl = AdminService.CreateTestUrl("media");
                InsertValidationMessages = TestApiUtility.ApiPost<SerialMediaAdmin>(AdminService, insertUrl, Media, out updatedAdmins, authorizedUser);
                //not getting a media object back when inserting a child
                //maybe because it doesn't exist?
                Assert.AreEqual(1, updatedAdmins.Count, "media " + Media.mediaId + " did not insert as child");
                Media = updatedAdmins[0];
            }

            public TestUrl UrlWithId
            {
                get { return AdminService.CreateTestUrl("media", Media.id); }
            }

            public void UpdateChildren(params int[] children)
            {
                Media.childRelationships = children
                    .Select(i => new SerialMediaRelationship() { relatedMediaId = i, displayOrdinal = 20 - i }).ToList();
                List<SerialMediaAdmin> updatedAdmins;
                UpdateValidationMessages = TestApiUtility.ApiPut<SerialMediaAdmin>(AdminService, UrlWithId, Media, out updatedAdmins, authorizedUser);
                if (updatedAdmins == null)
                {
                    Assert.Fail("Media ID: " + Media.id + " " + UpdateValidationMessages.Errors().FirstOrDefault().Message);
                }
                if (UpdateValidationMessages.Errors().Count() == 0)
                {
                    Assert.AreEqual(1, updatedAdmins.Count);
                    Media = updatedAdmins[0];
                }
                else
                {
                    Assert.Fail("Media ID: " + Media.id + " " + UpdateValidationMessages.Errors().FirstOrDefault().Message);
                }
            }

            public void Delete()
            {
                ValidationMessages deleteMessages = TestApiUtility.ApiDelete(AdminService, UrlWithId, authorizedUser);
                if (deleteMessages.Errors().Count() > 0)
                {
                    Assert.Fail(deleteMessages.Errors().First().Message);
                }
            }
        }

    }
}
