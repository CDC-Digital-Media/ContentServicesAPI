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
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Search.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace MediaCrudTests
{
    [TestClass]
    public class DataMediaTests
    {
        private static int bmiTopic = 25329;
        private static int antismokingTopic = 25272;
        private List<int> topicIds = new List<int> { antismokingTopic, bmiTopic };

        static DataMediaTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }
        ISearchControllerFactory SearchControllerFactory = ContentServicesDependencyBuilder.SearchControllerFactory;

        [TestMethod]
        public void GetCompositeMediaObject()
        {
            var mediaMgr = new CsMediaProvider();
            int collectionWithSubcollection = 138929; //98081
            CompositeMedia grandParentObject = mediaMgr.GetMediaCollection<MediaObject>(collectionWithSubcollection, false);
            Assert.IsNotNull(grandParentObject, collectionWithSubcollection.ToString() + " is not a valid collection");
            List<MediaObject> itemsWithGrandchildren = grandParentObject.Children.Where(o => o.Children != null && o.Children.Count > 0).ToList();
            //Assert.IsTrue(itemsWithGrandchildren.Count > 0, collectionWithSubcollection.ToString() + " does not have grandchildren");

            //CompositeMedia parentObject = mediaMgr.GetMediaCollection<MediaObject>(itemsWithGrandchildren[0].Id, false);
            //Assert.AreEqual(collectionWithSubcollection, parentObject.Parents[0].Id);
            //Assert.AreEqual(itemsWithGrandchildren[0].Children[0].Id, parentObject.Children[0].Id);
        }

        [TestMethod]
        public void UpdatePersistentUrl()
        {
            IMediaProvider mediaMgr = new CsMediaProvider();
            var media = TestApiUtility.SinglePublishedHtml();
            int testHtmlMediaId = int.Parse(media.mediaId);

            ValidationMessages getMessages;
            MediaObject originalObject = mediaMgr.GetMedia(testHtmlMediaId, out getMessages);
            Assert.IsNotNull(originalObject);

            MediaObject updatedObject = null;
            ValidationMessages messages = mediaMgr.UpdateMediaWithNewPersistentUrl(testHtmlMediaId, out updatedObject);
            MediaObject finalObject = mediaMgr.GetMedia(testHtmlMediaId, out getMessages);

            if (messages.Errors().Any())
            {
                Console.WriteLine(messages.Errors().First().Message);
                Console.WriteLine(messages.Errors().First().DeveloperMessage);
                Assert.Fail();
            }

            Assert.AreNotEqual(originalObject.PersistentUrlToken, finalObject.PersistentUrlToken);
        }

        [TestMethod]
        public void CanRetrievePdfSize()
        {
            IMediaProvider mediaMgr = new CsMediaProvider();
            var pdf = 236101;

            ValidationMessages getMessages;
            MediaObject obj = mediaMgr.GetMedia(pdf, out getMessages);
            Assert.IsNotNull(obj);
            Assert.AreEqual(7, obj.DocumentPageCount);
        }

        [TestMethod]
        public void TestAddAttributes()
        {
            IMediaProvider mediaMgr = new CsMediaProvider();
            string title = "ID:A12X000001 Test for Adding Attributes";
            DeleteAnyOldMedias(title);

            //Create a new Media OBject
            MediaObject newMediaObject = CreateNewMediaObject(title, new List<int>());
            var antismokingTopic = 25272;
            var topic = new List<int> { antismokingTopic };
            List<AttributeValueObject> topics1 = CreateAttributeValues("Topic", topic);
            newMediaObject.MediaStatusCodeValue = MediaStatusCodeValue.Staged;
            newMediaObject.MediaStatusCodeValue = MediaStatusCodeValue.Published;
            newMediaObject.AttributeValues = topics1;

            MediaObject updatedMedia;
            ValidationMessages messages1 = mediaMgr.SaveMedia(newMediaObject, out updatedMedia);
            Assert.AreEqual(0, messages1.Errors().Count());

            //Later, client selects object 
            ValidationMessages messages230;
            MediaObject clientMediaObject = mediaMgr.GetMedia(newMediaObject.Id, out messages230);

            //var topicIds2 = new List<int> { 19647, 19648, 19649, 19650 };
            clientMediaObject.AttributeValues = CreateAttributeValues("Audience", topicIds).Concat(
                CreateAttributeValues("Topic", topicIds));
            ValidationMessages messages2 = mediaMgr.SaveMedia(clientMediaObject, out updatedMedia);
            Assert.AreEqual(0, messages2.Errors().Count());

            //var topicIds3 = new List<int> { 19647, 19648, 19649};
            MediaObject clientMediaObject2 = mediaMgr.GetMedia(newMediaObject.Id, out messages230);
            List<AttributeValueObject> topics2 = CreateAttributeValues("Topic", topicIds);
            topics2[0].IsActive = false;
            topics2[1].DisplayOrdinal = 15;


            //10 is inactivated     - should be set to inactive
            //11 hasn't changed     - should not be changed
            //12 Order has changed  - should be changed
            //13 is missing         - should be set ot inactive

            clientMediaObject2.AttributeValues = clientMediaObject2.GetAttributeValues("Audience").Concat(topics2);

            ValidationMessages messages3 = mediaMgr.SaveMedia(clientMediaObject2, out updatedMedia);
            Assert.AreEqual(0, messages3.Errors().Count());

            MediaObject clientMediaObject3 = mediaMgr.GetMedia(newMediaObject.Id, out messages230);
            List<AttributeValueObject> audiences = clientMediaObject3.GetAttributeValues("Audience").ToList();
            List<AttributeValueObject> topics = clientMediaObject3.GetAttributeValues("Topic").ToList();

            Assert.AreEqual(2, audiences.Count);
            Assert.AreEqual(1, topics.Count());
            Assert.AreEqual(bmiTopic, topics[0].ValueKey.Id);
            Assert.AreEqual(true, topics[0].IsActive);

            clientMediaObject3.AttributeValues = null;
            clientMediaObject3.MediaStatusCodeValue = MediaStatusCodeValue.Staged;
            ValidationMessages messages4 = mediaMgr.SaveMedia(clientMediaObject3, out updatedMedia);
            Assert.AreEqual(0, messages4.Errors().Count());

            //10, 11, 12, 13 should all be set to inactive
            ValidationMessages getMessages;
            MediaObject clientMediaObject4 = mediaMgr.GetMedia(newMediaObject.Id, out getMessages);

            topics = clientMediaObject4.GetAttributeValues("Topic").ToList();
            Assert.AreEqual(0, topics.Count());
            mediaMgr.DeleteMedia(clientMediaObject4.Id);

            //Assert.AreEqual(clientMediaObject.Description, "This is a test ecard");
            //Assert.AreEqual(clientMediaObject.Topics.ToList()[1].ValueName, "Lung Cancer");



            MediaObject deletedMediaObject = mediaMgr.GetMedia(newMediaObject.Id, out messages230);
            Assert.AreEqual(null, deletedMediaObject);


            //string abc = newMediaObject.ToString();
        }

        public void DeleteAnyOldMedias(string title)
        {
            IMediaProvider mediaMgr = new CsMediaProvider();

            SearchParameters parms = new SearchParameters("Media", "Media", "FullSearch".Is(title));
            ISearchController controller = SearchControllerFactory.GetSearchController(parms);
            DataSetResult page1 = controller.Get();
            IEnumerable<MediaObject> matches = ((IEnumerable<MediaObject>)page1.Records).Where(r => r.Title == title);
            foreach (MediaObject item in matches)
            {
                mediaMgr.DeleteMedia(item.Id);
            }
        }

        [TestMethod]
        public void TestAddAttributes2()
        {
            IMediaProvider mediaMgr = new CsMediaProvider();

            string title = "ID:A12X000002 Test for Adding Attributes 2";
            DeleteAnyOldMedias(title);
            //Create a new Media OBject
            MediaObject newMediaObject = CreateNewMediaObject(title, new List<int>());
            //var topicsOld = new List<int> { 9, 19 };
            var topics1 = CreateAttributeValues("Topic", topicIds);

            newMediaObject.AttributeValues = topics1;

            MediaObject updatedMedia2;
            ValidationMessages saveMessages = mediaMgr.SaveMedia(newMediaObject, out updatedMedia2);
            Assert.AreEqual(0, saveMessages.Errors().Count());

            //Later, client selects object 
            ValidationMessages messages234;
            MediaObject clientMediaObject = mediaMgr.GetMedia(newMediaObject.Id, out messages234);

            //Removing audiences for now
            //clientMediaObject.AttributeValues = topics1.Concat(CreateAttributeValues("Audience", new List<int> { 20, 21 }));
            //MediaObject updatedMedia;
            //ValidationMessages saveMessages2 = mediaMgr.SaveMedia(clientMediaObject, out updatedMedia);
            //Assert.AreEqual(0, saveMessages2.Errors().Count());

            ValidationMessages getMessages;
            MediaObject clientMediaObject3 = mediaMgr.GetMedia(newMediaObject.Id, out getMessages);
            List<AttributeValueObject> audiences = clientMediaObject3.GetAttributeValues("Audience").ToList();
            List<AttributeValueObject> topics = clientMediaObject3.GetAttributeValues("Topic").ToList();


            Assert.AreEqual(2, topics.Count());
            Assert.AreEqual(antismokingTopic, topics[0].ValueKey.Id);

            Assert.AreEqual(0, audiences.Count);
            //Assert.AreEqual(20, audiences[0].ValueKey.Id);
            //Assert.AreEqual(21, audiences[1].ValueKey.Id);

            mediaMgr.DeleteMedia(clientMediaObject3.Id);

            //Assert.AreEqual(clientMediaObject.Description, "This is a test ecard");
            //Assert.AreEqual(clientMediaObject.Topics.ToList()[1].ValueName, "Lung Cancer");



            MediaObject deletedMediaObject = mediaMgr.GetMedia(newMediaObject.Id, out getMessages);
            Assert.AreEqual(null, deletedMediaObject);
        }

        [TestMethod]
        public void TestAddOmniture()
        {
            IMediaProvider mediaMgr = new CsMediaProvider();

            string title = "Test for Adding Omniture";
            DeleteAnyOldMedias(title);
            //Create a new Media Object
            MediaObject newMediaObject = CreateNewMediaObject(title, new List<int>());
            //var topicsOld = new List<int> { 9, 19 };
            var topics1 = CreateAttributeValues("Topic", topicIds);

            newMediaObject.AttributeValues = topics1;

            MediaObject updatedMedia2;
            ValidationMessages saveMessages = mediaMgr.SaveMedia(newMediaObject, out updatedMedia2);
            
            Assert.AreEqual(0, saveMessages.Errors().Count());
            Assert.AreEqual(newMediaObject.OmnitureChannel, updatedMedia2.OmnitureChannel);

            mediaMgr.DeleteMedia(updatedMedia2.Id);
            
            ValidationMessages getMessages;
            MediaObject deletedMediaObject = mediaMgr.GetMedia(updatedMedia2.Id, out getMessages);
            
            Assert.AreEqual(null, deletedMediaObject);
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
                        AttributeId = 0,
                        AttributeName = attributeName,
                        DisplayOrdinal = displayOrder,
                        IsActive = true
                    };
            return attributeValue;
        }

        private static MediaObject CreateNewMediaObject(string title, List<int> topicValueIds)
        {
            string str = "testString";
            byte[] strBytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, strBytes, 0, strBytes.Length);

            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = "Image",
                MimeTypeCode = ".swf",
                CharacterEncodingCode = "UTF-8",
                Title = title,
                Description = "This is a test ecard",
                SourceUrlForApi = "http://www......[domain]...../smoking/ecard4smokefree.swf",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                OmnitureChannel = "test adding omniture"
            };
            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;
            return newMediaObject;
        }

    }
}
