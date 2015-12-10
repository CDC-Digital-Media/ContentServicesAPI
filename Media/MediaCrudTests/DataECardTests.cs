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
using Gov.Hhs.Cdc.Api.Admin;
using Gov.Hhs.Cdc.Api.Public;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace SearchUnitTests
{
    [TestClass]
    public class DataECardTests
    {
        private string authorizedUser = "";

        static DataECardTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void TestAddEcardAttributes()
        {
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            IApiServiceFactory publicService = new PublicApiServiceFactory();
          
            var media = TestApiUtility.SinglePublishedEcard();
            var ecardTestMediaId = media.mediaId;
            Console.WriteLine(ecardTestMediaId);

            //List<SerialMediaV2> mediaPublics;
            //TestApiUtility.ApiGet<SerialMediaV2>(publicService,
            //    adminService.CreateSecureUrl("media", ecardTestMediaId, "", ""), out mediaPublics);


            media.id = null;
            media.eCard = new SerialECardDetail()
            {
                mobileCardName = "mobileCardName",
                html5Source = "http://HTMLSourceUrl......[domain].....",
                caption = "caption",
                cardText = "cardText",
                cardTextOutside = "cardTextOutside",
                cardTextInside = "cardTextInside",
                imageSourceInsideLarge = "cardTextInside",
                imageSourceInsideSmall = "imageSourceInsideSmall",
                imageSourceOutsideLarge = "imageSourceOutsideLarge",
                imageSourceOutsideSmall = "imageSourceOutsideSmall",
                isMobile = true,
                mobileTargetUrl = "http://targetUrlForMobile.com",
                isActive = true,
                displayOrdinal = 10
            };
            media.datePublished = "1997-07-16T18:20:00Z";

            List<SerialMediaAdmin> updatedAdmins;


            media.sourceUrl = "";

            var postUrl = adminService.CreateTestUrl("media", "", "", "");           

            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, postUrl, media, out updatedAdmins, authorizedUser);

            if (messages.Errors().Count() > 0)
            {
                Assert.Fail(messages.Errors().First().Message);
            }

            Assert.AreEqual(1, updatedAdmins.Count());
            Console.WriteLine(updatedAdmins[0].id);

            SerialMediaAdmin updatedMedia = GetSerialMedia(adminService, updatedAdmins[0].id);

            Assert.AreEqual("1997-07-16T18:20:00Z", updatedMedia.datePublished);
            Assert.AreEqual("cardTextInside", updatedMedia.eCard.imageSourceInsideLarge);
            Assert.AreEqual(true, updatedMedia.eCard.isMobile);

            media.id = "";
        }


        private ValidationMessages Put(IApiServiceFactory serviceFactory, string newMedia, out List<SerialMediaAdmin> mediaAdmins)
        {
            return TestApiUtility.ApiPutSerializedData<SerialMediaAdmin>(serviceFactory,
                serviceFactory.CreateTestUrl("media", "121070", "", ""), 
                newMedia, 
                out mediaAdmins);
        }

        private ValidationMessages PostSerialMedia(IApiServiceFactory serviceFactory, SerialMediaAdmin newMedia, out List<SerialMediaAdmin> mediaAdmins)
        {
            return TestApiUtility.ApiPost<SerialMediaAdmin>(serviceFactory,
                serviceFactory.CreateTestUrl("media", "", "", ""), newMedia, out mediaAdmins);
        }


        private static SerialMediaAdmin GetSerialMedia(IApiServiceFactory serviceFactory, string mediaId)
        {
            List<SerialMediaAdmin> mediaAdmins;
            TestApiUtility.ApiGet<SerialMediaAdmin>(serviceFactory,
                serviceFactory.CreateTestUrl("media", mediaId, "", ""), out mediaAdmins);
            return mediaAdmins[0];
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
                SourceUrlForApi = "smoking/ecard4smokefree.swf",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published
            };

            newMediaObject.AttributeValues =  CreateAttributeValues("Topic", topicValueIds);
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
                AttributeName = attributeName,
                ValueKey = new ValueKey(valueId, "English"),
                DisplayOrdinal = displayOrder,
                IsActive = true
            };
            return attributeValue;
        }
    }
}
