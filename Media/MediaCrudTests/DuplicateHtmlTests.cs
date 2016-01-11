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
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaCrudTests
{
    [TestClass]
    public class DuplicateHtmlTests
    {
        private string authorizedUser = "";

        static DuplicateHtmlTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void CannotAddContentThatAlreadyExists()
        {
            var adminService = new AdminApiServiceFactory();

            string data = @"{""mediatype"":""HTML"",""mimetype"":"".html"",""encoding"":""utf-8"",""title"":""CDC - Seasonal Influenza (Flu)"",""description"":""xpath test"",""sourceUrl"":""http://www......[domain]...../flu/index.htm"",""targetUrl"":""http://www......[domain]...../flu/index.htm"",""rowVersion"":"""",""preferences"":[{""type"":""WebPage"",""isDefault"":true,""htmlPreferences"":{""includedElements"":{""xPath"":""//html:body/descendant::html:img[@alt='Influenza-Like Illness Activity Level Indicator, United States.']"",""elementIds"":null,""classNames"":null},""excludedElements"":null,""stripAnchor"":null,""stripComment"":null,""stripImage"":null,""stripScript"":true,""stripStyle"":null,""newWindow"":null,""imageAlign"":""right"",""outputEncoding"":null,""outputFormat"":null,""contentNamespace"":null},""ecardPreferences"":null},{""type"":""Mobile"",""isDefault"":true,""htmlPreferences"":{""includedElements"":{""xPath"":""//html:body/descendant::html:img[@alt='Influenza-Like Illness Activity Level Indicator, United States.']"",""elementIds"":null,""classNames"":null},""excludedElements"":null,""stripAnchor"":null,""stripComment"":null,""stripImage"":null,""stripScript"":true,""stripStyle"":null,""newWindow"":null,""imageAlign"":""right"",""outputEncoding"":null,""outputFormat"":null,""contentNamespace"":null},""ecardPreferences"":null}],""sourceCode"":""Centers for Disease Control and Prevention"",""language"":""English"",""status"":""archived"",""datepublished"":""2014-05-22T19:38:00.000Z"",""owningOrgId"":"""",""maintainingOrgId"":""""}";
            var obj = new JavaScriptSerializer().Deserialize<SerialMediaAdmin>(data);

            TestUrl mediaUrl2 = adminService.CreateTestUrl("media");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, obj, out mediaAfterUpdate, authorizedUser);
            Assert.AreEqual(1, messages.Errors().Count());
            Assert.AreEqual("This content has already been syndicated.", messages.Errors().First().Message);
        }

        [TestMethod]
        public void CannotAddFlu()
        {
            List<int> topicIds = new List<int>() { 25272, 25329 };

            var media = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "utf-8",
                title = "test",
                description = "test",
                sourceUrl = "http://www......[domain]...../flu/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };

            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages updateMessages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            Assert.AreEqual(1, updateMessages.Errors().Count());
            Assert.AreEqual("This content has already been syndicated.", updateMessages.Errors().First().Message);
        }

        [TestMethod]
        public void CannotAddCats()
        {
            List<int> topicIds = new List<int>() { 25272, 25329 };

            var media = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "utf-8",
                title = "test",
                description = "test",
                sourceUrl = "http://.....[devServer]...../sample/CDC%20-%20Bringing%20an%20Animal%20into%20U.S.%20%20Cats%20-%20Animal%20Importation.htm",
                targetUrl = "http://.....[devServer]...../sample/CDC%20-%20Bringing%20an%20Animal%20into%20U.S.%20%20Cats%20-%20Animal%20Importation.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };

            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages updateMessages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            Assert.AreEqual(1, updateMessages.Errors().Count());
            Assert.AreEqual("This content has already been syndicated.", updateMessages.Errors().First().Message);
        }

        [TestMethod]
        public void CannotAddEbolaWithDefaultExtraction()
        {
            List<int> topicIds = new List<int>() { 25272, 25329 };

            var media = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "utf-8",
                title = "test",
                description = "test",
                sourceUrl = "http://www......[domain]...../vhf/ebola/transmission/index.html?34=34",
                targetUrl = "http://www......[domain]...../vhf/ebola/transmission/index.html?34=34",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };

            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages updateMessages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            Assert.AreEqual(1, updateMessages.Errors().Count());
            Assert.AreEqual("This content has already been syndicated.", updateMessages.Errors().First().Message);
        }

        [TestMethod]
        public void CannotAddAWithDefaultExtraction()
        {
            var url = "http://www......[domain]...../az/a.html"; //has existing URL with default admin extraction criteria

            List<int> topicIds = new List<int>() { 25272, 25329 };

            var media = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "utf-8",
                title = "test",
                description = "test",
                sourceUrl = url,
                targetUrl = url,
                status = MediaStatusCodeValue.Staged.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };

            var adminService = new AdminApiServiceFactory();
            var mediaUrl2 = adminService.CreateTestUrl("media");
            List<SerialMediaAdmin> mediaAfterUpdate;
            var updateMessages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            Assert.AreEqual(1, updateMessages.Errors().Count());
            Assert.AreEqual("This content has already been syndicated.", updateMessages.Errors().First().Message);
        }

    }
}
