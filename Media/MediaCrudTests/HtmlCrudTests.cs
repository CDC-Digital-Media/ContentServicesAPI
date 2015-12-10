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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaCrudTests
{
    [TestClass]

    public class HtmlCrudTests
    {
        IApiServiceFactory adminService = new AdminApiServiceFactory();
        string authorizedUser = "";
        int mediaId;

        static HtmlCrudTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (mediaId > 0)
            {
                new CsMediaProvider().DeleteMedia(mediaId);
            }
        }

        [TestMethod]
        public void CanRetrieveUnencodedTitle()
        {
            List<SerialMediaAdmin> mediaRightAfterInsert = null;
            var topicIds = new List<int>() { 25272, 25329 };
            string unencodedTitle = @"Isn't this the HTML <data> Object & for Testing Media Preferences";
            string encodedTitle = Util.HtmlEncodeOutput(unencodedTitle);

            string unencodedDescription = @"Encode special characters &<>'";
            string encodedDescription = Util.HtmlEncodeOutput(unencodedDescription);

            var newMedia = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "UTF-8",
                title = unencodedTitle,
                description = unencodedDescription,
                sourceUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                targetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                ratingMinimum = 1,
                ratingMaximum = 5,
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToString(),
                topics = topicIds
            };

            ValidationMessages postMessages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media", "", "", ""), newMedia, out mediaRightAfterInsert, authorizedUser);

            if (postMessages.Errors().Count() > 0)
            {
                Assert.Fail(postMessages.Errors().First().Message);
            }

            mediaId = int.Parse(mediaRightAfterInsert[0].id);

            Assert.IsTrue(mediaRightAfterInsert.Count() > 0);
            Assert.AreEqual(unencodedTitle, mediaRightAfterInsert[0].title);
            Assert.AreEqual(unencodedDescription, mediaRightAfterInsert[0].description);

            Assert.AreEqual(1, mediaRightAfterInsert.Count());
            Assert.AreEqual(1, mediaRightAfterInsert[0].effectivePreferences.Count());
            Assert.AreEqual(null, mediaRightAfterInsert[0].preferences);
            TestUrl mediaUrl = adminService.CreateTestUrl("media", mediaRightAfterInsert[0].id, "", "");

            string originalRowVersion = mediaRightAfterInsert[0].rowVersion;

            mediaRightAfterInsert[0].preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = true,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        imageAlign = "right",
                        stripScript = true,
                        contentNamespace="ABC",
                        excludedElements = new SerialExtractionPath()
                        {
                            elementIds = new List<string>(){"Element1", "Element2"}
                        }
                    }
                },
                new SerialPreferenceSet()
                {
                    type = "Mobile",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            classNames = new List<string>(){"msyndicate"}
                        }
                    }
                }
            };

            var createdPreferences = PreferenceTransformation.CreateMediaPreferences(mediaRightAfterInsert[0].preferences);
            string serializedCreatedPreferences = createdPreferences.ToXElement<MediaPreferenceSetCollection>().ToString();

            var tempPreferencesSet = new PreferencesSet(0, "", "", "");
            var deserializedObject = tempPreferencesSet.GetMediaPreferences(serializedCreatedPreferences, "tempSource");

            List<SerialMediaAdmin> mediaAfterUpdate;

            ValidationMessages updateMessages = TestApiUtility.ApiPut<SerialMediaAdmin>(adminService, mediaUrl, mediaRightAfterInsert[0], out mediaAfterUpdate, authorizedUser);

            if (updateMessages.Errors().Count() > 0)
            {
                Console.WriteLine(updateMessages.Errors().First().Message);
                Assert.Fail(updateMessages.Errors().First().Message);
            }
            Assert.AreEqual(1, mediaAfterUpdate.Count());

            string updateRowVersion = mediaAfterUpdate[0].rowVersion;
            List<SerialPreferenceSet> extractionCriteria = mediaAfterUpdate[0].effectivePreferences;
            Assert.AreEqual(2, extractionCriteria.Count());
            List<SerialPreferenceSet> preferences = mediaRightAfterInsert[0].preferences;

            mediaRightAfterInsert[0].rowVersion = originalRowVersion;
            ValidationMessages concurrencyMessages = TestApiUtility.ApiPut<SerialMediaAdmin>(adminService, mediaUrl, mediaRightAfterInsert[0], out mediaAfterUpdate, authorizedUser);
            List<ValidationMessage> theErrors = concurrencyMessages.Errors().ToList();
            if (theErrors.Count == 0 || theErrors[0].Message != "The data has changed since your last request")
                Assert.Fail("Expected OptimisticConcurrencyException on save of object with an old version"); // If it gets to this line, no exception was thrown

        }

    }
}
