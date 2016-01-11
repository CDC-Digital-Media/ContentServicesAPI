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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.Api;
using Newtonsoft.Json;
using Gov.Hhs.Cdc.Api.Admin;
using Newtonsoft.Json.Linq;

namespace Gov.Hhs.Cdc.AuthorizationTests
{
    [TestClass]
    public class MediaSetTests
    {
        string mediaAdminUserWithMediaSets = "jwe8";
        string systemAdminUser = "";
        AdminApiServiceFactory adminService = new AdminApiServiceFactory();

        [TestInitialize]
        public void Init()
        {

            var deleteMe = new List<string> { "CanCreateMediaSet", "DeletingMediaSetDoesNotCauseError" };

            //var sets = TestApiUtility.GetResource<SerialMediaSet>(adminService, adminService.CreateTestUrl("mediasets")).Where(ms => deleteMe.Contains(ms.name));

            var sets2 = TestApiUtility.GetJson(adminService.CreateTestUrl("mediasets"));
            //Console.WriteLine(sets2);
            var obj = JObject.Parse(sets2);

            var sets3 = obj["results"].Where(o => deleteMe.Contains((string)o["name"]));

            foreach (var set in sets3)
            {
                var response = TestApiUtility.ApiDelete(adminService, adminService.CreateTestUrl("mediasets", (string)set["name"]), systemAdminUser);
            }
        }

        [TestMethod]
        public void CanRetrieveMediaSets()
        {
            var sets = TestApiUtility.GetJson(adminService.CreateTestUrl("mediasets"));
            Assert.IsNotNull(sets);
        }

        [TestMethod]
        public void MediaSetHasCriteria()
        {
            var sets = TestApiUtility.GetJson(adminService.CreateTestUrl("mediasets"));
            Console.WriteLine(sets);
            var set = JObject.Parse(sets)["results"].First;
            Assert.IsFalse(string.IsNullOrEmpty((string)set["criteria"]));
        }

        [TestMethod]
        public void UserMediaSetHasCriteria()
        {
            var user = TestApiUtility.GetResource<SerialAdminUser>(adminService.CreateTestUrl("adminusers", mediaAdminUserWithMediaSets)).First();
            Assert.IsFalse(string.IsNullOrEmpty(user.mediaSets.First().Criteria)); //TODO:  Query JSON instead
        }

        [TestMethod]
        public void CanRetrieveUserMediaSets()
        {
            var user = TestApiUtility.GetResource<SerialAdminUser>(adminService.CreateTestUrl("adminusers", mediaAdminUserWithMediaSets)).First();
            Assert.IsNotNull(user.mediaSets);
        }

        [TestMethod]
        public void UserMediaSetHasName()
        {
            var user = TestApiUtility.GetResource<SerialAdminUser>(adminService.CreateTestUrl("adminusers", mediaAdminUserWithMediaSets)).First();
            Assert.IsFalse(string.IsNullOrEmpty(user.mediaSets.First().Name));

        }

        [TestMethod]
        public void InvalidMediaSetDoesNotCreate()
        {
            var str = @"{""criteriaJUNKJUNKJUNK"":""123""}";
            var url = adminService.CreateTestUrl("mediasets");
            //var initialCountOfMediaSets = TestApiUtility.GetResource<SerialMediaSet>(adminService, url).Count();
            var sets = TestApiUtility.GetJson(url);
            var obj = JObject.Parse(sets)["results"];
            var initialCountOfMediaSets = obj.Count();

            TestApiUtility.PostJson(url, str, systemAdminUser);

            var mediaSetCountAfterAdd = JObject.Parse(TestApiUtility.GetJson(url))["results"].Count();
            Assert.AreEqual(initialCountOfMediaSets, mediaSetCountAfterAdd);
        }

        [TestMethod]
        public void PostNewMediaSetDoesNotGenerateError()
        {
            var media = TestApiUtility.SinglePublishedHtml();
            var set = JObject.FromObject(new { name = "CanCreateMediaSet", criteria = "<SearchCriteria><MediaId>" + media.mediaId + "</MediaId></SearchCriteria>" });
            var url = adminService.CreateTestUrl("mediasets");
            var result = TestApiUtility.PostJson(url, set.ToString(), systemAdminUser);
            var meta = JObject.Parse(result)["meta"];
            var messages = meta["message"];
            if (messages.Count() > 0)
            {
                var message = messages.First();
                Assert.Fail(message["message"] + ": " + message["developerMessage"]);
            }
        }

        [TestMethod]
        public void PostNewMediaSetIncreasesMediaSetCount()
        {
            var media = TestApiUtility.SinglePublishedHtml();
            var set = JObject.FromObject(new { name = "CanCreateMediaSet", criteria = "<SearchCriteria><MediaId>" + media.mediaId + "</MediaId></SearchCriteria>" });
            var url = adminService.CreateTestUrl("mediasets");
            var initialCountOfMediaSets = JObject.Parse(TestApiUtility.GetJson(url))["results"].Count();

            var messages = TestApiUtility.PostJson(url, set.ToString(), systemAdminUser);

            Assert.IsTrue(initialCountOfMediaSets < JObject.Parse(TestApiUtility.GetJson(url))["results"].Count());
        }

        [TestMethod]
        public void UserWithoutSystemAdminRoleCannotCreateMediaSets()
        {
            var set = JObject.FromObject(new { name = "CanCreateMediaSet", criteria = "<SearchCriteria><MediaId>123</MediaId></SearchCriteria>" });
            var url = adminService.CreateTestUrl("mediasets");
            var result = TestApiUtility.PostJson(url, set.ToString(), mediaAdminUserWithMediaSets);
            var meta = JObject.Parse(result)["meta"];
            var messages = meta["message"];

            Assert.IsTrue(messages.Count() > 0);
            var message = (string)messages.First()["userMessage"];
            Assert.IsTrue(message.Contains("is not authorized"), message);
        }

        [TestMethod]
        public void CannotCreateMediaSetWithDuplicateName()
        {
            var media = TestApiUtility.SinglePublishedHtml();
            var set = JObject.FromObject(new { name = "CanCreateMediaSet", criteria = "<SearchCriteria><MediaId>" + media.mediaId + "</MediaId></SearchCriteria>" });
            var url = adminService.CreateTestUrl("mediasets");

            var response = TestApiUtility.PostJson(url, set.ToString(), systemAdminUser);
            var meta = JObject.Parse(response)["meta"];
            var messages = meta["message"];
            if (messages.Count() == 0)
            {
                response = TestApiUtility.PostJson(url, set, systemAdminUser);
                meta = JObject.Parse(response)["meta"];
                messages = meta["message"];
            }
            Assert.IsTrue(messages.Count() > 0);
            var message = (string)messages.First()["developerMessage"];
            Assert.IsTrue(message.Contains("PRIMARY KEY"), message);
        }

        [TestMethod]
        public void DeletingMediaSetDoesNotCauseError()
        {
            var set = JObject.FromObject(new { name = "DeletingMediaSetDoesNotCauseError", criteria = "" });
            var url = adminService.CreateTestUrl("mediasets");
            var addResponse = TestApiUtility.PostJson(url, set, systemAdminUser);
            var meta = JObject.Parse(addResponse)["meta"];
            var messages = meta["message"];
            if (messages.Count() > 0)
            {
                var message = messages.First();
                Assert.Fail((string)message["message"] + " : " + (string)message["developerMessage"]);
            }

            var sets = TestApiUtility.GetJson(url);
            var obj = JObject.Parse(sets)["results"];

            Assert.IsTrue(obj.Any(s => (string)s["name"] == "DeletingMediaSetDoesNotCauseError"), "media set DeletingMediaSetDoesNotCauseError was not added.");

            var response = TestApiUtility.ApiDelete(adminService, adminService.CreateTestUrl("mediasets", "DeletingMediaSetDoesNotCauseError"), systemAdminUser);
            if (response.NumberOfErrors > 0)
            {
                Assert.Fail(response.Errors().First().Message + " : " + response.Errors().First().DeveloperMessage);
            }
        }

        [TestMethod]
        public void DeletingMediaSetReducesMediaSetCount()
        {
            var set = JObject.FromObject( new  { name = "DeletingMediaSetReducesMediaSetCount", criteria = "" });
            var url = adminService.CreateTestUrl("mediasets");
            var response = TestApiUtility.PostJson(url, set, systemAdminUser);
            var meta = JObject.Parse(response)["meta"];
            var messages = meta["message"];
            if (messages.Count() > 0)
            {
                var message = messages.First();
                Assert.Fail((string)message["message"] + " : " + (string)message["developerMessage"]);
            }
            var initialCount = JObject.Parse(TestApiUtility.GetJson(url))["results"].Count();

            var deleteMessages = TestApiUtility.ApiDelete(adminService, adminService.CreateTestUrl("mediasets", "DeletingMediaSetReducesMediaSetCount"), systemAdminUser);
            Assert.IsTrue(initialCount > JObject.Parse(TestApiUtility.GetJson(url))["results"].Count());
        }
    }
}
