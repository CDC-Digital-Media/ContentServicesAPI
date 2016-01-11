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
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gov.Hhs.Cdc.AuthorizationTests
{
    [TestClass]
    public class DeleteMediaTests
    {
        private string vocabOnlyUser = "";
        private MediaObject savedMedia = null;

        [TestMethod]
        public void CannotDeleteMediaWithoutSystemAdminRole()
        {
            CreateMedia();

            var mediaId = savedMedia.MediaId;
            var adminService = new AdminApiServiceFactory();
            var url = adminService.CreateTestUrl("media", mediaId);
            var messages = TestApiUtility.ApiDelete(adminService, url, vocabOnlyUser);
            Assert.IsNotNull(messages);
            Assert.IsTrue(messages.NumberOfErrors > 0);
            Console.WriteLine(messages);
        }

        private ValidationMessages CreateMedia()
        {
            //var badge = new SerialMediaAdmin
            //{
            //    mediaType = "Badge",
            //    sourceCode = "Centers for Disease Control and Prevention",
            //    language = "English",
            //    sourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
            //    targetUrl = "http://www......[domain].....",
            //    status = "Published",
            //    topics = validTopicIds,
            //    encoding = "utf-8",
            //    title = "Tuff Badge",
            //    description = desc
            //};
            //var result = TestApiUtility.CreateAdminMedia(badge, authorizedUser);
            var validTopicIds = new List<int> { 25272, 25329, 25651 };

            var media = new MediaObject
            {
                MediaTypeCode = "Badge",
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                CharacterEncodingCode = "utf-8",
                MediaStatusCode = "Published",
                Title = "Badge for DeleteMediaTests",
                AttributeValues = MediaTransformationHelper.ValuesFromTopics("English", validTopicIds),
                SourceUrl = "http://vignette2.wikia.nocookie.net/tuffpuppy/images/4/41/TUFF_Badge.jpg",
                TargetUrl = "http://www......[domain]....."
            };
            var messages = new CsMediaProvider().SaveMedia(media, out savedMedia);
            if (savedMedia == null) { Console.WriteLine(messages); }
            return messages;
        }
    }
}
