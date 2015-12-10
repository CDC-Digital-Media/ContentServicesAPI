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
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.Search.Controller;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Connection;

namespace MediaCrudTests
{
    [TestClass]
    public class DataMediaV2DateCreateTests
    {
        static DataMediaV2DateCreateTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        static IMediaProvider mediaMgr;
        static MediaObject savedMedia;


        [TestInitialize()]
        public void MyTestInitialize()
        {
            mediaMgr = new CsMediaProvider();
            savedMedia = new MediaObject();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (savedMedia != null && savedMedia.Id > 0)
                mediaMgr.DeleteMedia(savedMedia.Id);
        }

        [TestMethod]
        public void MediaV2DateCreate()
        {
            string strDate = "2014-05-30T18:01:00Z";
            var topicIds = new List<int>() { 25272, 25329 };

            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "TestMediaV2DateCreate",
                Description = "This is a test HTML",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = date,
                DateContentAuthored = date,
                DateContentReviewed = date,
                DateContentUpdated = date,
                DateContentPublished = date,
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicIds);
            newMediaObject.AttributeValues = topics;

            ValidationMessages messages1 = mediaMgr.SaveMedia(newMediaObject, out savedMedia);
            if (messages1.NumberOfErrors > 0)
            {
                Assert.Fail("SaveMedia error: " + messages1.Messages.First().Message);
            }
            Assert.IsNotNull(savedMedia);

            //Later, client selects object 
            ValidationMessages getMessages;
            MediaObject clientMediaObject = mediaMgr.GetMedia(savedMedia.Id, out getMessages);

            Assert.AreEqual(date, clientMediaObject.DateContentAuthored);
            Assert.AreEqual(date, clientMediaObject.DateContentPublished);
            Assert.AreEqual(date, clientMediaObject.DateContentReviewed);
            Assert.AreEqual(date, clientMediaObject.DateContentUpdated);
            Assert.AreEqual(date, clientMediaObject.DateSyndicationVisible);
        }


        //could be a global method
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

        //could be a global method
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

        //could be a global method
        private static DateTime? ParseUtcDate(string strDate)
        {
            DateTimeOffset result;
            return DateTimeOffset.TryParse(strDate, out result) ? result.UtcDateTime : (DateTime?)null;
        }

    }
}
