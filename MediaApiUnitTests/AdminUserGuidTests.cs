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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaApiUnitTests
{
    [TestClass]
    public class AdminUserGuidTests
    {
        private int mediaId;

        [TestCleanup]
        public void Cleanup()
        {
            if (mediaId > 0)
            {
                new CsMediaProvider().DeleteMedia(mediaId);
            }
        }

        static AdminUserGuidTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void CanSaveUserGuid()
        {
            var guid = Guid.Parse("CEB728AA-9EAA-44BB-B6BF-8D3F43437EDD");
            int validTopicId = 25272;
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "CanSaveUserGuid test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                AttributeValues = new List<AttributeValueObject> { 
                    new AttributeValueObject             {
                    ValueKey = new ValueKey(validTopicId, "English"),
                    AttributeId = 1,
                    AttributeName = "Topic",
                    DisplayOrdinal = 1,
                    IsActive = true}
            },
                CreatedByGuid = guid,
                ModifiedByGuid = guid
            };

            var savedMedia = new MediaObject();
            var messages = new CsMediaProvider().SaveMedia(newMediaObject, out savedMedia);
            if (messages.NumberOfErrors > 0)
            {
                Assert.Fail(messages.Messages.First().Message);
            }
            Assert.IsNotNull(savedMedia);
            mediaId = savedMedia.Id;
            Console.WriteLine(mediaId);

            var url = "title=CanSaveUserGuid";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
            {
                Assert.AreEqual("CanSaveUserGuid test", media.First().name);
            }
            else
            {
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            }

            Assert.AreEqual(guid, savedMedia.CreatedByGuid);
        }
    }
}
