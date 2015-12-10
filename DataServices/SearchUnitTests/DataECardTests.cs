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
using System.Web;
using Gov.Hhs.Cdc.Media.Bll;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;

namespace SearchUnitTests
{
    [TestClass]
    public class DataECardTests
    {

        static DataECardTests()
        {
            ContentServicesDependencyBuilder.BuildAssemblies();
        }
        //[TestMethod]
        //public void TestGetMedia()
        //{
        //    MediaObjectMgr mediaMgr = (MediaObjectMgr)CdcDataSource.GetDataMgr("Media", "MediaBO");

        //    //Create a new Media OBject
        //    MediaObject newMediaObject = CreateNewMediaObject("Test eCard Number 1", new List<int> { 10, 11 });

        //    mediaMgr.Save(newMediaObject);

        //    //Later, client selects object 
        //    MediaObject clientMediaObject = mediaMgr.Get(newMediaObject.Id);

        //    Assert.AreEqual(clientMediaObject.Description, "This is a test ecard");
        //    Assert.AreEqual(clientMediaObject.Topics.ToList()[1].ValueName, "Lung Cancer");




        //    //Client changes object and sends it back for update
        //    clientMediaObject.Description = "Updated Description";

        //    //If client doesn't necessarily have all the fields defined in the business rule and is only updating partial fields, 
        //    //Get the object for update and update the fields
        //    MediaObject persistedMediaObject = mediaMgr.Get(newMediaObject.Id);
        //    persistedMediaObject.Description = clientMediaObject.Description;
        //    mediaMgr.Save(persistedMediaObject);

        //    ////else if client is updating all the fields defined in the business rule, instead use
        //    //mediaMgr.Save(clientMediaObject);
        //    MediaObject objectWithUpdateDescription = mediaMgr.Get(newMediaObject.Id);
        //    Assert.AreEqual("Updated Description", objectWithUpdateDescription.Description);

        //    mediaMgr.Delete(clientMediaObject);
        //    MediaObject deletedMediaObject = mediaMgr.Get(newMediaObject.Id);
        //    Assert.AreEqual(null, deletedMediaObject);


        //    //string abc = newMediaObject.ToString();
        //}

        [TestMethod]
        public void TestAddAttributes()
        {
            //HttpContext.Current = MockHelper.GetNewFakeHttpContext();

            ECardObjectMgr eCardMgr = new ECardObjectMgr();

            //Create a new Media OBject
            ECardObject newECardObject = new ECardObject()
            {
                IsActive = true,
                LinkText = "LinkText",
                TextFront = "TextFront",
                TextInside = "TextInside",
                Html5Code = "Html5Code",
                DisplayOrdinal = 10,
                Media = CreateNewMediaObject("ECard Test", new List<int>())
            };

            ValidationMessages save1messages = eCardMgr.Save(newECardObject);

            //Later, client selects object 
            ECardObject clientMediaObject = eCardMgr.Get(newECardObject.Media.Id);

            clientMediaObject.Media.Audiences = CreateAttributeValues(new List<int> { 10, 11 });
            clientMediaObject.Media.Topics = CreateAttributeValues(new List<int> { 10, 11, 12, 13 });
            eCardMgr.Save(clientMediaObject);

            ECardObject clientMediaObject2 = eCardMgr.Get(newECardObject.Media.Id);

            Assert.AreEqual("LinkText", clientMediaObject2.LinkText);
            Assert.AreEqual("TextFront", clientMediaObject2.TextFront);
            Assert.AreEqual("TextInside", clientMediaObject2.TextInside);
            Assert.AreEqual("Html5Code", clientMediaObject2.Html5Code);
            Assert.AreEqual(10, clientMediaObject2.DisplayOrdinal);


            List<AttributeValue> topics2 = CreateAttributeValues(new List<int> { 10, 11, 12 });
            topics2[0].IsActive = false;
            topics2[2].DisplayOrdinal = 15;


            //10 is inactivated     - should be set to inactive
            //11 hasn't changed     - should not be changed
            //12 Order has changed  - should be changed
            //13 is missing         - should be set ot inactive

            clientMediaObject2.Media.Topics = topics2;
            clientMediaObject2.LinkText = "Updated LinkText";
            clientMediaObject2.TextFront = "Updated TextFront";
            clientMediaObject2.TextInside = "Updated TextInside";
            clientMediaObject2.Html5Code = "Updated Html5Code";
            clientMediaObject2.DisplayOrdinal = 20;

            eCardMgr.Save(clientMediaObject2);

            ECardObject clientMediaObject3 = eCardMgr.Get(newECardObject.Media.Id);
            List<AttributeValue> audiences = clientMediaObject3.Media.Audiences.ToList();
            List<AttributeValue> topics = clientMediaObject3.Media.Topics.ToList();

            Assert.AreEqual("Updated LinkText", clientMediaObject3.LinkText);
            Assert.AreEqual("Updated TextFront", clientMediaObject3.TextFront);
            Assert.AreEqual("Updated TextInside", clientMediaObject3.TextInside);
            Assert.AreEqual("Updated Html5Code", clientMediaObject3.Html5Code);
            Assert.AreEqual(20, clientMediaObject3.DisplayOrdinal);

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
            eCardMgr.Save(clientMediaObject3);

            //10, 11, 12, 13 should all be set to inactive

            ECardObject clientMediaObject4 = eCardMgr.Get(newECardObject.Media.Id);

            topics = clientMediaObject4.Media.Topics.ToList();
            Assert.AreEqual(10, topics[0].ValueKey.Id);
            Assert.AreEqual(12, topics[1].ValueKey.Id);
            Assert.AreEqual(11, topics[2].ValueKey.Id);
            Assert.AreEqual(13, topics[3].ValueKey.Id);
            Assert.AreEqual(false, topics[0].IsActive);
            Assert.AreEqual(false, topics[1].IsActive);
            Assert.AreEqual(false, topics[2].IsActive);
            Assert.AreEqual(false, topics[3].IsActive);
            eCardMgr.Delete(clientMediaObject4);

            //Assert.AreEqual(clientMediaObject.Description, "This is a test ecard");
            //Assert.AreEqual(clientMediaObject.Topics.ToList()[1].ValueName, "Lung Cancer");



            ECardObject deletedMediaObject = eCardMgr.Get(newECardObject.Media.Id);
            Assert.AreEqual(null, deletedMediaObject);


            //string abc = newMediaObject.ToString();
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
                SourceUrl = "smoking/ecard4smokefree.swf",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                IsActive = true,
                Thumbnail = strBytes,
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
