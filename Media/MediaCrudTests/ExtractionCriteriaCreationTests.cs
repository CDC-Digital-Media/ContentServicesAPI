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
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaCrudTests
{
    [TestClass]
    public class ExtractionCriteriaCreationTests
    {
        static ExtractionCriteriaCreationTests()
        {
            CurrentConnection.Name = "ContentServicesDb";
        }

        public int mediaId;
        private string authorizedUser = "";
        List<int> topicIds = new List<int>() { 25272, 25329 };

        [TestInitialize]
        public void Init()
        {
            DeleteMediaBySourceUrl();
        }

        private static void DeleteMediaBySourceUrl()
        {
            string sourceUrlToDelete = "http://www......[domain]...../flu2342342/index.htm";
            var provider = new CsMediaProvider();
            var criteria = "?urlcontains=" + sourceUrlToDelete;
            var searchResult = TestApiUtility.AdminApiMediaSearch(criteria);

            foreach (var item in searchResult)
            {
                provider.DeleteMedia(int.Parse(item.mediaId));
            }
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
        public void CanPopulateClassNamesFromPreferences()
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
                sourceUrl = "http://www......[domain]...../flu2342342/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };
            media.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
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

            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media", "", "", "");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().First().Message);
            }
            var pref = new JavaScriptSerializer().Serialize(mediaAfterUpdate[0].preferences);
            mediaId = int.Parse(mediaAfterUpdate[0].mediaId);
            Console.WriteLine(pref);
            Assert.AreEqual("msyndicate", mediaAfterUpdate[0].classNames);
        }

        [TestMethod]
        public void CanPopulateClassNamesWithBothWebAndMobile()
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
                sourceUrl = "http://www......[domain]...../flu2342342/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };
            media.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            classNames = new List<string>(){"syndicate"}
                        }
                    }
                }
,
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

            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media", "", "", "");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().First().Message);
            }
            var pref = new JavaScriptSerializer().Serialize(mediaAfterUpdate[0].preferences);
            mediaId = int.Parse(mediaAfterUpdate[0].mediaId);
            Console.WriteLine(pref);
            Assert.AreEqual("syndicate,msyndicate", mediaAfterUpdate[0].classNames);
        }

        [TestMethod]
        public void WhenBothArePopulatedMobileElementIdAloneCountedAsDuplicated()
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
                sourceUrl = "http://www......[domain]...../flu2342342/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };
            media.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            elementIds = new List<string>(){"el1"}
                        }
                    }
                }
,
                new SerialPreferenceSet()
                {
                    type = "Mobile",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            elementIds = new List<string>(){"el2"}
                        }
                    }
                }
                
            };

            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl = adminService.CreateTestUrl("media", "", "", "");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl, media, out mediaAfterUpdate, authorizedUser);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().First().Message);
            }
            var pref = new JavaScriptSerializer().Serialize(mediaAfterUpdate[0].preferences);
            mediaId = int.Parse(mediaAfterUpdate[0].mediaId);


            var media2 = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "utf-8",
                title = "test",
                description = "test",
                sourceUrl = "http://www......[domain]...../flu2342342/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };
            media2.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "Mobile",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            elementIds = new List<string>(){"el2"}
                        }
                    }
                }
                            };

            var messages2 = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl, media, out mediaAfterUpdate, authorizedUser);
            Assert.AreEqual(1, messages2.Errors().Count());
            Assert.AreEqual("This content has already been syndicated.", messages2.Errors().First().Message);


        }

        [TestMethod]
        public void CanPopulateElementIdsFromPreferences()
        {
            var media = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "utf-8",
                title = "test",
                description = "test",
                sourceUrl = "http://www......[domain]...../flu2342342/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };
            media.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            elementIds = new List<string>(){"Element1", "Element2"}
                        }
                    }
                }
                
            };

            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media", "", "", "");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().First().Message);
            }
            var pref = new JavaScriptSerializer().Serialize(mediaAfterUpdate[0].preferences);
            mediaId = int.Parse(mediaAfterUpdate[0].mediaId);
            Assert.AreEqual("Element1,Element2", mediaAfterUpdate[0].elementIds);
        }

        [TestMethod]
        public void CanPopulateXPathFromPreferences()
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
                sourceUrl = "http://www......[domain]...../flu2342342/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };
            media.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            xPath = "some xpath"
                        }
                    }
                }
                
            };

            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media", "", "", "");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().First().Message);
            }
            var pref = new JavaScriptSerializer().Serialize(mediaAfterUpdate[0].preferences);
            mediaId = int.Parse(mediaAfterUpdate[0].mediaId);
            Assert.AreEqual("some xpath", mediaAfterUpdate[0].xPath);
        }

        [TestMethod]
        public void CannotAddSameXpathTwice()
        {

            var media = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "utf-8",
                title = "test",
                description = "test",
                sourceUrl = "http://www......[domain]...../flu2342342/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };
            media.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            xPath = "some xpath"
                        }
                    }
                }
                
            };

            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media", "", "", "");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().First().Message);
            }
            var pref = new JavaScriptSerializer().Serialize(mediaAfterUpdate[0].preferences);
            mediaId = int.Parse(mediaAfterUpdate[0].mediaId);
            Console.WriteLine(mediaId);

            var messages2 = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            Assert.AreEqual(1, messages2.Errors().Count());
            Assert.AreEqual("This content has already been syndicated.", messages2.Errors().First().Message);

        }

        [TestMethod]
        public void CannotAddDuplicateClassNames()
        {
            var media = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "utf-8",
                title = "test",
                description = "test",
                sourceUrl = "http://www......[domain]...../flu2342342/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };
            media.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            classNames = new List<string>(){"marcie"}
                        }
                    }
                } 
            };
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media", "", "", "");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().First().Message);
            }
            mediaId = int.Parse(mediaAfterUpdate[0].mediaId);


            var messages2 = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            Assert.AreEqual(1, messages2.Errors().Count());
            Assert.AreEqual("This content has already been syndicated.", messages2.Errors().First().Message);


        }

        [TestMethod, Ignore]
        public void TestSequenceEqual()
        {
            string[] m1 = { "m1", "m2" };
            string[] m2 = { "m2", "m1" };

            Assert.IsTrue(m1.SequenceEqual(m2));
        }

        [TestMethod]
        public void TestExcept()
        {
            string[] m1 = { "m1", "m2" };
            string[] m2 = { "m2", "m1" };
            var diff = m1.Except(m2);
            Assert.IsFalse(diff.Any());
        }

        [TestMethod]
        public void DuplicateClassNamesOrderDoesNotMatter()
        {
            var media = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "utf-8",
                title = "test",
                description = "test",
                sourceUrl = "http://www......[domain]...../flu2342342/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };
            media.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            classNames = new List<string>(){"m1", "m2"}
                        }
                    }
                } 
            };
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media", "", "", "");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().First().Message);
            }
            mediaId = int.Parse(mediaAfterUpdate[0].mediaId);

            media.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            classNames = new List<string>(){"m2", "m1"}
                        }
                    }
                } 
            };
            var messages2 = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            Assert.AreEqual(1, messages2.Errors().Count());
            Assert.AreEqual("This content has already been syndicated.", messages2.Errors().First().Message);
        }

        [TestMethod]
        public void DuplicateElementIdsOrderDoesNotMatter()
        {
            var media = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "utf-8",
                title = "test",
                description = "test",
                sourceUrl = "http://www......[domain]...../flu2342342/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };
            media.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            elementIds = new List<string>(){"Element1", "Element2"}
                        }
                    }
                }
                
            };
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media", "", "", "");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().First().Message);
            }
            mediaId = int.Parse(mediaAfterUpdate[0].mediaId);

            media.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            elementIds = new List<string>(){"Element2", "Element1"}
                        }
                    }
                }
                
            };
            var messages2 = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            Assert.AreEqual(1, messages2.Errors().Count());
            Assert.AreEqual("This content has already been syndicated.", messages2.Errors().First().Message);

        }

        [TestMethod]
        public void CannotAddDuplicateElementIds()
        {
            var media = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "utf-8",
                title = "test",
                description = "test",
                sourceUrl = "http://www......[domain]...../flu2342342/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };
            media.preferences = new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            elementIds = new List<string>(){"Element1", "Element2"}
                        }
                    }
                }
                
            };

            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media", "", "", "");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().First().Message);
            }
            mediaId = int.Parse(mediaAfterUpdate[0].mediaId);


            var messages2 = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            Assert.AreEqual(1, messages2.Errors().Count());
            Assert.AreEqual("This content has already been syndicated.", messages2.Errors().First().Message);


        }

        [TestMethod]
        public void NotSpecifyingExtractionCriteriaUsesDefault()
        {
            var media = new SerialMediaAdmin()
            {
                sourceCode = "Centers for Disease Control and Prevention",
                language = "English",
                mediaType = MediaTypeParms.DefaultHtmlMediaType,
                mimeType = ".html",
                encoding = "utf-8",
                title = "test",
                description = "test",
                sourceUrl = "http://www......[domain]...../flu2342342/index.htm",
                targetUrl = "http://www......[domain]...../flu/index.htm",
                status = MediaStatusCodeValue.Published.ToString(),
                datePublished = DateTime.UtcNow.ToShortTimeString(),
                topics = topicIds
            };

            IApiServiceFactory adminService = new AdminApiServiceFactory();
            TestUrl mediaUrl2 = adminService.CreateTestUrl("media", "", "", "");
            List<SerialMediaAdmin> mediaAfterUpdate;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService, mediaUrl2, media, out mediaAfterUpdate, authorizedUser);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().First().Message);
            }
            Console.WriteLine(mediaAfterUpdate[0].mediaId);
            mediaId = int.Parse(mediaAfterUpdate[0].mediaId);

            Assert.AreEqual("syndicate", mediaAfterUpdate[0].classNames);
        }
    }
}
