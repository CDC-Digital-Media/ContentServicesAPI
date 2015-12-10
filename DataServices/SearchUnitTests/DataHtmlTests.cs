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

using System.Data;
using Gov.Hhs.Cdc.Media.Bll;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Global;

namespace SearchUnitTests
{
    [TestClass]
    public class DataHtmlTests
    {
        static DataHtmlTests()
        {
            ContentServicesDependencyBuilder.BuildAssemblies();
        }

        [TestMethod]
        public void TestHtmlAddAttributes()
        {
            HtmlObjectMgr htmlMgr = new HtmlObjectMgr();

            //Create a new Media OBject
            HtmlObject newHtmlObject = new HtmlObject()
            {
                //IsActive = true,
                //LinkText = "LinkText",
                //TextFront = "TextFront",
                //TextInside = "TextInside",
                //Html5Code = "Html5Code",
                //DisplayOrdinal = 10,
                Media = CreateNewMediaObject("HTML Test", new List<int>())
            };

            htmlMgr.Save(newHtmlObject);

            //Later, client selects object 
            HtmlObject clientMediaObject = htmlMgr.Get(newHtmlObject.Media.Id);

            clientMediaObject.Media.Audiences = CreateAttributeValues(new List<int> { 10, 11 });
            clientMediaObject.Media.Topics = CreateAttributeValues(new List<int> { 10, 11, 12, 13 });
            htmlMgr.Save(clientMediaObject);

            HtmlObject clientMediaObject2 = htmlMgr.Get(newHtmlObject.Media.Id);



            //Assert.AreEqual("LinkText", clientMediaObject2.LinkText);
            //Assert.AreEqual("TextFront", clientMediaObject2.TextFront);
            //Assert.AreEqual("TextInside", clientMediaObject2.TextInside);
            //Assert.AreEqual("Html5Code", clientMediaObject2.Html5Code);
            //Assert.AreEqual(10, clientMediaObject2.DisplayOrdinal);


            List<AttributeValue> topics2 = CreateAttributeValues(new List<int> { 10, 11, 12 });
            topics2[0].IsActive = false;
            topics2[2].DisplayOrdinal = 15;


            //10 is inactivated     - should be set to inactive
            //11 hasn't changed     - should not be changed
            //12 Order has changed  - should be changed
            //13 is missing         - should be set ot inactive

            clientMediaObject2.Media.Topics = topics2;
            //clientMediaObject2.LinkText = "Updated LinkText";
            //clientMediaObject2.TextFront = "Updated TextFront";
            //clientMediaObject2.TextInside = "Updated TextInside";
            //clientMediaObject2.Html5Code = "Updated Html5Code";
            //clientMediaObject2.DisplayOrdinal = 20;

            htmlMgr.Save(clientMediaObject2);

            HtmlObject clientMediaObject3 = htmlMgr.Get(newHtmlObject.Media.Id);
            List<AttributeValue> audiences = clientMediaObject3.Media.Audiences.ToList();
            List<AttributeValue> topics = clientMediaObject3.Media.Topics.ToList();

            //Assert.AreEqual("Updated LinkText", clientMediaObject3.LinkText);
            //Assert.AreEqual("Updated TextFront", clientMediaObject3.TextFront);
            //Assert.AreEqual("Updated TextInside", clientMediaObject3.TextInside);
            //Assert.AreEqual("Updated Html5Code", clientMediaObject3.Html5Code);
            //Assert.AreEqual(20, clientMediaObject3.DisplayOrdinal);

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


            clientMediaObject3.Media.Topics = null;
            htmlMgr.Save(clientMediaObject3);

            //10, 11, 12, 13 should all be set to inactive

            HtmlObject clientMediaObject4 = htmlMgr.Get(newHtmlObject.Media.Id);

            topics = clientMediaObject4.Media.Topics.ToList();
            Assert.AreEqual(10, topics[0].ValueKey.Id);
            Assert.AreEqual(12, topics[1].ValueKey.Id);
            Assert.AreEqual(11, topics[2].ValueKey.Id);
            Assert.AreEqual(13, topics[3].ValueKey.Id);
            Assert.AreEqual(false, topics[0].IsActive);
            Assert.AreEqual(false, topics[1].IsActive);
            Assert.AreEqual(false, topics[2].IsActive);
            Assert.AreEqual(false, topics[3].IsActive);
            htmlMgr.Delete(clientMediaObject4);

            //Assert.AreEqual(clientMediaObject.Description, "This is a test ecard");
            //Assert.AreEqual(clientMediaObject.Topics.ToList()[1].ValueName, "Lung Cancer");
            
            HtmlObject deletedMediaObject = htmlMgr.Get(newHtmlObject.Media.Id);
            Assert.AreEqual(null, deletedMediaObject);
        }



        [TestMethod]
        public void TestMediaPreferences()
        {
            HtmlObjectMgr htmlMgr = new HtmlObjectMgr();
            HtmlObject originalObject = new HtmlObject()
            {
                Media = CreateNewMediaObject("HTML Object for Testing Media Preferences", new List<int>()),
                IsActive = true
            };

            htmlMgr.Save(originalObject);
            HtmlObject savedObject = htmlMgr.Get(originalObject.Media.Id);
            Assert.AreEqual(1, savedObject.Media.Preferences.Effective.ExtractionCriteria.Count());
            Assert.AreEqual(null, savedObject.Media.Preferences.ForMediaItem);
            
            //persistedMediaObject.Preferences.ForMediaItem = GetOverloadedPreferences();

            savedObject.Media.Preferences.ForMediaItem = new MediaPreferences()
            {
                ExtractionCriteria = new List<ExtractionCriteria>()
                {
                    //new HtmlExtractionCriteria()
                    //{
                    //    IsDefault = true,
                    //    ClassNames = new List<string>(){"Syndicate"}
                    //}
                    new HtmlExtractionCriteria()
                    {
                        ExtractionName = "WebPage",
                        IsDefault = true,
                        ImageAlign = "right",
                        StripScript = true,
                        ContentNamespace="ABC",
                        ExcludedElements = new ExtractionPath()
                        {
                            ElementIds = new List<string>(){"Element1", "Element2"}
                        }
                    },
                    new HtmlExtractionCriteria()
                    {
                        ExtractionName = "Mobile",
                        IsDefault = false,
                        IncludedElements = new ExtractionPath()
                        {
                            ClassNames = new List<string>(){"msyndicate"}
                        }
                    }
                }
            };

            htmlMgr.Save(savedObject);
            HtmlObject savedObject2 = htmlMgr.Get(originalObject.Media.Id);
            ExtractionCriteria extractionCriteria = savedObject2.Media.Preferences.GetEffectiveExtractionCriteria();
            Assert.AreEqual(2, savedObject.Media.Preferences.ForMediaItem.ExtractionCriteria.Count());
            MediaPreferences preferences = savedObject.Media.Preferences.ForMediaItem;

            ValidationMessages concurrencyMessages = htmlMgr.Save(originalObject);
            List<ValidationMessage> theErrors = concurrencyMessages.Errors().ToList();
            if (theErrors.Count == 0 || theErrors[0].Message != "The data has changed since your last request")
                Assert.Fail("Expected OptimisticConcurrencyException on save of object with an old version"); // If it gets to this line, no exception was thrown
            //if( concurrencyMessages.Errors > 0 && concurrencyMessages.
            //try
            //{
                
            //    Assert.Fail("Expected OptimisticConcurrencyException on save of object with an old version"); // If it gets to this line, no exception was thrown
            //}
            //catch (OptimisticConcurrencyException) { }
            htmlMgr.Delete(savedObject2);
            string a = "abc";

        }


        private static MediaPreferences GetOverloadedPreferences()
        {
            MediaPreferences newPreferences = new MediaPreferences()
            {
                ExtractionCriteria = new List<ExtractionCriteria>(){  
                    new HtmlExtractionCriteria()
                    {
                        ImageAlign = "right",
                        StripScript = true
                    }
                }
            };
            return newPreferences;
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
                MediaTypeCode = "HTML Content",
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = title,
                Description = "This is a test HTML",
                SourceUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
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
    }
}
