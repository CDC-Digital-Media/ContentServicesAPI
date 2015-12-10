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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataSource.Media;
using Gov.Hhs.Cdc.DataSource;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Media.Bll;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.Search.Controller;

namespace SearchUnitTests
{
    [TestClass]
    public class DataMediaTests
    {
        static DataMediaTests()
        {
            ContentServicesDependencyBuilder.BuildAssemblies();
        }
        SearchControllerFactory SearchControllerFactory = ContentServicesDependencyBuilder.GetSearchControllerFactory();

        [TestMethod]
        public void GetCompositeMediaObject()
        {
            MediaObjectMgr mediaMgr = new MediaObjectMgr();
            int id = 110897;
            CompositeMedia grandParentObject = mediaMgr.GetCollection(id);
            List<RelatedMediaItem> itemsWithGrandchildren = grandParentObject.Children.Where(o => o.Children != null && o.Children.Count > 0).ToList();

            CompositeMedia parentObject = mediaMgr.GetCollection(itemsWithGrandchildren[0].Id);
            Assert.AreEqual(id, parentObject.Parents[0].Id);
            Assert.AreEqual(itemsWithGrandchildren[0].Children[0].Id,  parentObject.Children[0].Id);

        }


        [TestMethod]
        public void UpdatePersistentUrl()
        {
            MediaObjectMgr mediaMgr = new MediaObjectMgr();
            int id = 118218;
            //id = 118167;
            MediaObject originalObject = mediaMgr.Get(id);
            MediaObject updatedObject = null;
            ValidationMessages messages = mediaMgr.UpdateMediaWithNewPersistentUrl(id, out updatedObject);
            MediaObject finalObject = mediaMgr.Get(id);
            Assert.AreEqual(0, messages.Errors().Count());
            Assert.AreNotEqual(originalObject.PersistentUrlToken, finalObject.PersistentUrlToken);
            

            
        }



        [TestMethod]
        public void TestGetMedia()
        {
            //MediaObjectMgr mediaMgr = (MediaObjectMgr) CdcDataSource.GetDataManager("Media", "Media");
            MediaObjectMgr mediaMgr = new MediaObjectMgr();

            //Create a new Media OBject
            MediaObject newMediaObject = CreateNewMediaObject("Test eCard Number 1", new List<int>{10, 11});
            //newMediaObject.Preferences = new MediaPreferences()
            //{
            //    ExtractionCriteria = new List<ExtractionCriteria>()
            //    {
            //        new HtmlExtractionCriteria()
            //        {
            //            ExtractionName = "WebPage",
            //            IsDefault = true,
            //            ElementIds = new List<string>(){"Element1", "Element2"}
            //        },
            //        new HtmlExtractionCriteria()
            //        {
            //            ExtractionName = "Mobile",
            //            IsDefault = false,
            //            ClassNames = new List<string>(){"mSyndicate"}
            //        }
            //    }
            //};

            ValidationMessages messages1 = mediaMgr.Save(newMediaObject);

            //Later, client selects object 
            MediaObject clientMediaObject = mediaMgr.Get(newMediaObject.Id);
            MediaPreferences preferences = clientMediaObject.Preferences.ForMediaItem;

            Assert.AreEqual(clientMediaObject.Description, "This is a test ecard");
            Assert.AreEqual(clientMediaObject.Topics.ToList()[1].ValueName, "Lung Cancer");




            //Client changes object and sends it back for update
            clientMediaObject.Description = "Updated Description";

            //If client doesn't necessarily have all the fields defined in the business rule and is only updating partial fields, 
            //Get the object for update and update the fields
            MediaObject persistedMediaObject = mediaMgr.Get(newMediaObject.Id);
            persistedMediaObject.Description = clientMediaObject.Description;
            mediaMgr.Save(persistedMediaObject);

            ////else if client is updating all the fields defined in the business rule, instead use
            //mediaMgr.Save(clientMediaObject);
            MediaObject objectWithUpdateDescription = mediaMgr.Get(newMediaObject.Id);
            Assert.AreEqual("Updated Description", objectWithUpdateDescription.Description);

            mediaMgr.Delete(clientMediaObject);
            MediaObject deletedMediaObject = mediaMgr.Get(newMediaObject.Id);
            Assert.AreEqual(null, deletedMediaObject);


            //string abc = newMediaObject.ToString();
        }



        [TestMethod]
        public void TestAddAttributes()
        {

            MediaObjectMgr mediaMgr = new MediaObjectMgr();
            string title = "ID:A12X000001 Test for Adding Attributes";
            DeleteAnyOldMedias(title);

            //Create a new Media OBject
            MediaObject newMediaObject = CreateNewMediaObject(title, new List<int>());
            List<AttributeValue> topics1 = CreateAttributeValues(new List<int> { 
                10 
            });
            newMediaObject.MediaStatusCodeValue = MediaStatusCodeValue.Staged;
            newMediaObject.MediaStatusCodeValue = MediaStatusCodeValue.Published;
            newMediaObject.Topics = topics1;

            ValidationMessages messages1 = mediaMgr.Save(newMediaObject);
            Assert.AreEqual(0, messages1.Errors().Count());

            //Later, client selects object 
            MediaObject clientMediaObject = mediaMgr.Get(newMediaObject.Id);

            clientMediaObject.Audiences = CreateAttributeValues(new List<int>{10, 11});
            clientMediaObject.Topics = CreateAttributeValues(new List<int> { 10, 11, 12, 13 });
            ValidationMessages messages2 = mediaMgr.Save(clientMediaObject);
            Assert.AreEqual(0, messages2.Errors().Count());

            MediaObject clientMediaObject2 = mediaMgr.Get(newMediaObject.Id);
            List<AttributeValue> topics2 = CreateAttributeValues(new List<int> { 10, 11, 12 });
            topics2[0].IsActive = false;
            topics2[2].DisplayOrdinal = 15;

            
            //10 is inactivated     - should be set to inactive
            //11 hasn't changed     - should not be changed
            //12 Order has changed  - should be changed
            //13 is missing         - should be set ot inactive

            clientMediaObject2.Topics = topics2;
            ValidationMessages messages3 = mediaMgr.Save(clientMediaObject2);
            Assert.AreEqual(0, messages3.Errors().Count());

            MediaObject clientMediaObject3 = mediaMgr.Get(newMediaObject.Id);
            List<AttributeValue> audiences = clientMediaObject3.Audiences.ToList();
            List<AttributeValue> topics = clientMediaObject3.Topics.ToList();

            Assert.AreEqual(2, audiences.Count);
            Assert.AreEqual(4, topics.Count());
            Assert.AreEqual(10, topics[0].ValueKey.Id);
            Assert.AreEqual(12, topics[1].ValueKey.Id);
            Assert.AreEqual(11, topics[2].ValueKey.Id);
            Assert.AreEqual(13, topics[3].ValueKey.Id);
            Assert.AreEqual(false, topics[0].IsActive);
            Assert.AreEqual(true, topics[1].IsActive);
            Assert.AreEqual(true, topics[2].IsActive);
            Assert.AreEqual(false, topics[3].IsActive);

            clientMediaObject3.Topics = null;
            clientMediaObject3.MediaStatusCodeValue = MediaStatusCodeValue.Staged;
            ValidationMessages messages4 = mediaMgr.Save(clientMediaObject3);
            Assert.AreEqual(0, messages4.Errors().Count());

            //10, 11, 12, 13 should all be set to inactive

            MediaObject clientMediaObject4 = mediaMgr.Get(newMediaObject.Id);

            topics = clientMediaObject4.Topics.ToList();
            Assert.AreEqual(10, topics[0].ValueKey.Id);
            Assert.AreEqual(12, topics[1].ValueKey.Id);
            Assert.AreEqual(11, topics[2].ValueKey.Id);
            Assert.AreEqual(13, topics[3].ValueKey.Id);
            Assert.AreEqual(false, topics[0].IsActive);
            Assert.AreEqual(false, topics[1].IsActive);
            Assert.AreEqual(false, topics[2].IsActive);
            Assert.AreEqual(false, topics[3].IsActive);
            mediaMgr.Delete(clientMediaObject4);
            
            //Assert.AreEqual(clientMediaObject.Description, "This is a test ecard");
            //Assert.AreEqual(clientMediaObject.Topics.ToList()[1].ValueName, "Lung Cancer");



            MediaObject deletedMediaObject = mediaMgr.Get(newMediaObject.Id);
            Assert.AreEqual(null, deletedMediaObject);


            //string abc = newMediaObject.ToString();
        }

        public void DeleteAnyOldMedias(string title)
        {
            MediaObjectMgr mediaMgr = new MediaObjectMgr();

            SearchParameters parms = new SearchParameters("Media", "CombinedMedia", "FullSearch".Is(title));
            ISearchController controller = SearchControllerFactory.GetSearchController(parms);
            DataSetResult page1 = controller.Get();
            IEnumerable<CombinedMediaItem> matches = ((IEnumerable<CombinedMediaItem>)page1.Records).Where(r => r.Title == title);
            foreach (CombinedMediaItem item in matches)
            {
                mediaMgr.Delete(new MediaObject() { Id = item.Id });
            }
        }

        [TestMethod]
        public void TestAddAttributes2()
        {
            MediaObjectMgr mediaMgr = new MediaObjectMgr();

            string title = "ID:A12X000002 Test for Adding Attributes 2";
            DeleteAnyOldMedias(title);
            //Create a new Media OBject
            MediaObject newMediaObject = CreateNewMediaObject(title, new List<int>());
            newMediaObject.Topics = CreateAttributeValues(new List<int> { 9, 22 });

            ValidationMessages saveMessages = mediaMgr.Save(newMediaObject);
            Assert.AreEqual(0, saveMessages.Errors().Count());

            //Later, client selects object 
            MediaObject clientMediaObject = mediaMgr.Get(newMediaObject.Id);

            clientMediaObject.Audiences = CreateAttributeValues(new List<int> { 20, 21 });
            ValidationMessages saveMessages2 = mediaMgr.Save(clientMediaObject);
            Assert.AreEqual(0, saveMessages2.Errors().Count());

            MediaObject clientMediaObject3 = mediaMgr.Get(newMediaObject.Id);
            List<AttributeValue> audiences = clientMediaObject3.Audiences.ToList();
            List<AttributeValue> topics = clientMediaObject3.Topics.ToList();


            Assert.AreEqual(2, topics.Count());
            Assert.AreEqual(9, topics[0].ValueKey.Id);
            Assert.AreEqual(22, topics[1].ValueKey.Id);

            Assert.AreEqual(2, audiences.Count);
            Assert.AreEqual(20, audiences[0].ValueKey.Id);
            Assert.AreEqual(21, audiences[1].ValueKey.Id);

            mediaMgr.Delete(clientMediaObject3);

            //Assert.AreEqual(clientMediaObject.Description, "This is a test ecard");
            //Assert.AreEqual(clientMediaObject.Topics.ToList()[1].ValueName, "Lung Cancer");



            MediaObject deletedMediaObject = mediaMgr.Get(newMediaObject.Id);
            Assert.AreEqual(null, deletedMediaObject);


            //string abc = newMediaObject.ToString();
        }



        private static List<AttributeValue> CreateAttributeValues(List<int> valueIds)
        {
            List<AttributeValue> attributeValues = new List<AttributeValue>();
            int displayOrder = 10;
            foreach (int valueId in valueIds)
            {
                attributeValues.Add(NewAttributeValue(valueId, displayOrder));
                displayOrder += 10;
            };
            return attributeValues;
        }

        private static AttributeValue NewAttributeValue(int valueId, int displayOrder)
        {
            AttributeValue attributeValue = new AttributeValue()
                    {
                        ValueKey = new ValueKey(valueId, "English"),
                        AttributeId = 1,
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
                SourceUrl = "http://www......[domain]...../smoking/ecard4smokefree.swf",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                IsActive = true,
                Thumbnail = strBytes
            };
            List<AttributeValue> topics = CreateAttributeValues(topicValueIds);
            newMediaObject.Topics = topics;
            return newMediaObject;
        }

    }
}
