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
using Gov.Hhs.Cdc.Api.Public;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace MediaApiUnitTests
{
    [TestClass]
    public class MediaV2SearchTests
    {
        public static List<int> topicIds = new List<int>() { 25272, 25329 };

        public MediaV2SearchTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        private CsMediaProvider mediaMgr = new CsMediaProvider();
        private Stack<MediaObject> savedMedias = new Stack<MediaObject>();
        private List<SerialMediaAdmin> otherSavedMedias = new List<SerialMediaAdmin>();

        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (var media in savedMedias)
            {
                if (media != null && media.Id > 0)
                    mediaMgr.DeleteMedia(media.Id);
            }

            foreach (var media in otherSavedMedias)
            {
                if (media != null && Convert.ToInt32(media.mediaId) > 0)
                    mediaMgr.DeleteMedia(Convert.ToInt32(media.mediaId));
            }
        }
        [TestMethod]
        public void CleanupCreatedMedia()
        {
            ////delete all medias with targetUrl = http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm
            //otherSavedMedias = TestApiUtility.AdminApiMediaSearch("?mediaType=html").Where(m => m.targetUrl == "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm").ToList();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void NoStagedInStorefront()
        {
            var media = CreateNewStagedMedia();
            var saved = new MediaObject();

            //TODO:  Modify to not use out parameter
            var messages = mediaMgr.SaveMedia(media, out saved);
            Assert.AreEqual(0, messages.Errors().Count());

            var criteria = "mediatype=pdf";
            var results = TestApiUtility.PublicApiV2MediaSearch(criteria);

            Assert.IsFalse(results.Any(m => m.status.ToLower() == "staged"));
            savedMedias.Push(saved);
        }

        [TestMethod]
        public void NoCollectionsInStorefront()
        {
            var media = CreateNewCollection();
            var saved = new MediaObject();

            //TODO:  Modify to not use out parameter
            var messages = mediaMgr.SaveMedia(media, out saved);
            Assert.AreEqual(0, messages.Errors().Count());

            var criteria = "q=CreateNewCollection";
            var results = TestApiUtility.PublicApiV2MediaSearch(criteria);

            Assert.AreEqual(0, results.Count());
            savedMedias.Push(saved);
        }

        [TestMethod]
        public void CanSearchApostrophe()
        {
            var criteria = "q=Terrie's";
            var results = TestApiUtility.PublicApiV2MediaSearch(criteria);

            Assert.IsTrue(results.Count() > 0);
            Assert.IsTrue(results.Any(m => m.name.Contains("Terrie")));
        }

        [TestMethod]
        public void CanSearchTopicWithApostrophe()
        {
            var topicIdWithApostrophe = 25664;
            var newMediaObject = CreateNewMediaObjectWithTitle("CanSearchTopicWithApostrophe", new List<int> { topicIdWithApostrophe });
            var savedMedia = new MediaObject();
            var messages = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var criteria = "?topic=gay men's health";
            var results = TestApiUtility.AdminApiMediaSearch(criteria); //TODO:  Move to Admin Search (need ability to create media there)
            Assert.IsTrue(results.Count() > 0);
            Assert.IsTrue(results.Any(m => m.tags["topic"].Any(t => t.id == topicIdWithApostrophe)));

            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2TagsIdMediaSearch()
        {
            string name = "PublicApiV2TagsIdMediaSearch";

            var validTopicId = 25272;
            MediaObject newMediaObject = CreateNewMediaObjectWithTitle(name, new List<int> { validTopicId });
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);
            if (msg.Errors().Count() > 0)
            {
                Assert.Fail(msg.Errors().First().DeveloperMessage);
            }

            var url = string.Format("{0}://{1}/api/{2}/resources/{3}?{4}", TestUrl.Protocol, TestUrl.PublicApiServer, "v2", "tags/" + validTopicId + "/media", "");
            string result = TestApiUtility.Get(url);

            var x = new ActionResultsWithType<List<SerialMediaV2>>(result);
            var media = x.ResultObject;

            Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2TagIdsSearch()
        {
            string name = "PublicApiV2TagIdsSearch";
            var serviceFactory = new PublicApiServiceFactory();
            MediaObject newMediaObject = CreateNewMediaObjectWithTitle(name, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages createMessages = mediaMgr.SaveMedia(newMediaObject, out savedMedia);
            Assert.AreEqual(0, createMessages.Errors().Count());

            string param = "tagids=" + string.Join(",", topicIds.Select(a => a.ToString()).ToArray());

            List<SerialMediaV2> getMedia;
            TestApiUtility.ApiGet<SerialMediaV2>(serviceFactory,
                new TestUrl(ServiceType.PublicApi, "media", param, 2),
                out getMedia);

            Assert.IsTrue(getMedia.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2LanguageNameSearch()
        {
            string language = "English";

            MediaObject newMediaObject = CreateNewMediaObjectWithLanguage(language, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);
            Assert.IsNotNull(savedMedia);
            Console.WriteLine(savedMedia.Id);

            var criteria = "languagename=" + language.ToUpper();
            var media = TestApiUtility.PublicApiV2MediaSearch(criteria);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().language.name == language);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2LanguageIsoCodeSearch()
        {
            string language = "English";

            MediaObject newMediaObject = CreateNewMediaObjectWithLanguage(language, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "languageIsoCode=eng";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);
            if (media.Count() == 1)
                Assert.IsTrue(media.First().language.isoCode == "eng");
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2NameSearch()
        {
            string name = "PublicApiV2NameSearch";

            MediaObject newMediaObject = CreateNewMediaObjectWithTitle(name, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var criteria = "name=" + name.ToUpper();
            var media = TestApiUtility.PublicApiV2MediaSearch(criteria);
            if (media.Count() == 1)
                Assert.IsTrue(media.First().name == name);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2NameContainsSearch()
        {
            string name = "PublicApiV2NameContainsSearch";

            MediaObject newMediaObject = CreateNewMediaObjectWithTitle(name, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "nameContains=" + name.ToLower().Replace("public", "").Replace("search", "");
            var media = TestApiUtility.PublicApiV2MediaSearch(url);
            if (media.Count() == 1)
                Assert.IsTrue(media.First().name == name);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2DescriptionSearch()
        {
            string description = "PublicApiV2DescriptionSearch";

            MediaObject newMediaObject = CreateNewMediaObjectWithDescription(description, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "description=" + description.ToUpper();

            var media = TestApiUtility.PublicApiV2MediaSearch(url);
            if (media.Count() == 1)
                Assert.IsTrue(media.First().description == description);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2DescriptionContainsSearch()
        {
            string description = "PublicApiV2DescriptionContainsSearch";

            MediaObject newMediaObject = CreateNewMediaObjectWithDescription(description, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "descriptionContains=" + description.ToLower().Replace("public", "").Replace("search", "");
            var media = TestApiUtility.PublicApiV2MediaSearch(url);
            if (media.Count() == 1)
                Assert.IsTrue(media.First().description == description);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SourceNameSearch()
        {
            string sourceName = "Centers for Disease Control and Prevention";

            MediaObject newMediaObject = CreateNewMediaObjectWithSourceName(sourceName, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);
            Assert.IsNotNull(savedMedia);
            Console.WriteLine(savedMedia.Id);

            var url = "sourcename=" + sourceName.ToUpper();
            var media = TestApiUtility.PublicApiV2MediaSearch(url);
            if (media.Count() == 1)
                Assert.IsTrue(media.First().source.name == sourceName);
            else
            {
                Console.WriteLine(media.Count());
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            }
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void CanSortByModifiedDate()
        {
            var criteria = "sort=-dateModified";
            var media = TestApiUtility.PublicApiV2MediaSearch(criteria);
            var newest = media.First().dateModified.ParseUtcDateTime();

            var criteria2 = "sort=dateModified";
            var media2 = TestApiUtility.PublicApiV2MediaSearch(criteria2);
            var oldest = media2.First().dateModified.ParseUtcDateTime();

            Console.WriteLine(newest);
            Console.WriteLine(oldest);

            var diff = newest - oldest;
            Assert.IsTrue(diff.Value.TotalDays > 10, diff.Value.TotalDays.ToString());
        }

        [TestMethod]
        public void CanSortByLanguage()
        {
            var criteria1 = "sort=-language";
            var media1 = TestApiUtility.PublicApiV2MediaSearch(criteria1);
            var lang1 = media1.First().language.name;
            var another = media1.Skip(1).First().language.name;

            Assert.AreEqual(lang1.ToLower(), another.ToLower());

            var criteria2 = "sort=language";
            var media2 = TestApiUtility.PublicApiV2MediaSearch(criteria2);
            var lang2 = media2.First().language.name;

            Assert.AreNotEqual(lang1.ToLower(), lang2.ToLower());
        }

        [TestMethod]
        public void CanSortByDateAdded()
        {
            string sourceName = "Centers for Disease Control and Prevention";

            MediaObject newMediaObject = CreateNewMediaObjectWithSourceName(sourceName, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);
            Assert.IsNotNull(savedMedia);
            Console.WriteLine(savedMedia.Id);
            Console.WriteLine(savedMedia.CreatedDateTime);

            var criteria = "sort=-datemodified"; //it was strictly a coincidence that this worked.  the last added is always at the top
            var media = TestApiUtility.PublicApiV2MediaSearch(criteria);
            Assert.AreEqual(savedMedia.Id, media.First().id);
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void CanSortByModifiedDateAscending()
        {
            string sourceName = "Centers for Disease Control and Prevention";

            MediaObject newMediaObject = CreateNewMediaObjectWithSourceName(sourceName, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);
            Assert.IsNotNull(savedMedia);
            Console.WriteLine(savedMedia.Id);
            Console.WriteLine(savedMedia.CreatedDateTime);

            var criteria = "sort=datemodified";
            var media = TestApiUtility.PublicApiV2MediaSearch(criteria);
            Assert.AreNotEqual(savedMedia.Id, media.First().id);
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void CanSearchByUrl()
        {
            var url = "http://www......[domain]...../features/bleedingdisorder/index.html";
            var criteria = "q=" + url;
            var results = TestApiUtility.PublicApiV2MediaSearch(criteria);
            Assert.AreNotEqual(0, results.Count());
        }

        [TestMethod]
        public void StringIsNotUrl()
        {
            var test = "vaccines";
            Assert.IsFalse(test.IsUrl());
        }

        [TestMethod]
        public void UrlIsUrl()
        {
            var test = "http://www......[domain].....";
            Assert.IsTrue(test.IsUrl());
        }

        [TestMethod]
        public void RegularFullTextSearchReturnsRankFirst()
        {
            var text = "vaccines";
            var criteria = "q=" + text;
            var results = TestApiUtility.PublicApiV2MediaSearch(criteria);
            Assert.IsTrue(results.First().name.Contains("accin"), results.First().name);
        }

        [TestMethod]
        public void CanSearchEncodedUrl()
        {
            var url = "http%3A%2F%2Fwww......[domain].....%2Fchickenpox%2Fabout%2Fsymptoms.html";
            var criteria = "q=" + url;
            var results = TestApiUtility.PublicApiV2MediaSearch(criteria);
            Assert.AreNotEqual(0, results.Count());
        }

        [TestMethod]
        public void PublicApiV2SourceNameContainsSearch()
        {
            string sourceName = "Centers for Disease Control and Prevention";

            MediaObject newMediaObject = CreateNewMediaObjectWithSourceName(sourceName, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);
            Assert.IsNotNull(savedMedia);

            var url = "sourceNameContains=" + sourceName.ToLower().Replace("centers", "").Replace("prevention", "");
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().source.name == sourceName);
            else
            {
                Console.WriteLine(media.Count());
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            }
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SourceAcronymSearch()
        {
            string sourceName = "Centers for Disease Control and Prevention";

            MediaObject newMediaObject = CreateNewMediaObjectWithSourceName(sourceName, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "sourceacronym=cdc";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().source.acronym == "CDC");
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SourceAcronymContainsSearch()
        {
            string sourceName = "Centers for Disease Control and Prevention";

            var savedMedia = new MediaObject();
            MediaObject newMediaObject = CreateNewMediaObjectWithSourceName(sourceName, topicIds);
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "sourceacronymContains=cdc";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().source.acronym == "CDC");
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SourceUrlSearch()
        {
            string sourceUrl = "http://www.test.gov/234567/data_statistics/by_topic/secondhand_smoke/index.htm";

            MediaObject newMediaObject = CreateNewMediaObjectWithSourceUrl(sourceUrl, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "sourceurl=" + sourceUrl.ToUpper();
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().sourceUrl == sourceUrl);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void CanSearchExistingSourceUrl()
        {
            string sourceUrl = "http://www......[domain]...../mmwr/preview/mmwrhtml/mm6407a2.htm";

            var url = "sourceurl=" + sourceUrl;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);
            Assert.AreEqual(1, media.Count());
            Assert.IsTrue(media.First().sourceUrl == sourceUrl);
        }

        [TestMethod]
        public void PublicApiV2SourceUrlContainsSearch()
        {
            string sourceUrl = "http://www.test.gov/234567/data_statistics/by_topic/secondhand_smoke/index.htm";

            var savedMedia = new MediaObject();
            MediaObject newMediaObject = CreateNewMediaObjectWithSourceUrl(sourceUrl, topicIds);
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "sourceurlContains=" + sourceUrl.ToLower().Replace("http://www.test.gov/", "").Replace("/secondhand_smoke/index.htm", "");
            var media = TestApiUtility.PublicApiV2MediaSearch(url);
            if (media.Count() == 1)
                Assert.IsTrue(media.First().sourceUrl == sourceUrl);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2DateContentAuthoredSearch()
        {
            string strDate = "2014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentAuthored(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "dateContentAuthored=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);
            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentAuthored == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentAuthoredSinceDateSearch()
        {
            string strDate = "5014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);
            var savedMedia = new MediaObject();

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentAuthored(date, topicIds);
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "contentAuthoredSinceDate=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentAuthored == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentAuthoredBeforeDateSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentAuthored(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "contentAuthoredBeforeDate=" + "1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentAuthored == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentAuthoredBeforeDateEqualSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentAuthored(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "contentAuthoredBeforeDate=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            Assert.AreEqual(0, media.Count());
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentAuthoredInRangeSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentAuthored(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "contentAuthoredInRange=" + "1914-05-30T18:01:00Z,1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);
            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentAuthored == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2DateContentUpdatedSearch()
        {
            string strDate = "2014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentUpdated(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "&dateContentUpdated=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentUpdated == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentUpdatedSinceDateSearch()
        {
            string strDate = "5014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentUpdated(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "contentUpdatedSinceDate=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentUpdated == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentUpdatedBeforeDateSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentUpdated(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "contentUpdatedBeforeDate=" + "1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentUpdated == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentUpdatedInRangeSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentUpdated(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "contentUpdatedInRange=" + "1914-05-30T18:01:00Z,1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentUpdated == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2DateContentPublishedSearch()
        {
            string strDate = "2014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentPublished(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "dateContentPublished=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentPublished == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentPublishedSinceDateSearch()
        {
            string strDate = "5014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentPublished(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "contentPublishedSinceDate=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentPublished == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentPublishedBeforeDateSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentPublished(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "contentPublishedBeforeDate=" + "1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentPublished == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentPublishedInRangeSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentPublished(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "contentPublishedInRange=" + "1914-05-30T18:01:00Z,1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentPublished == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2DateContentReviewedSearch()
        {
            string strDate = "2014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            //TODO:  Create a separate test class for each of these methods.  Call during ClassInitialize.
            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentReviewed(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);
            Assert.IsNotNull(savedMedia);

            var url = "dateContentReviewed=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            Assert.IsNotNull(media);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentReviewed == strDate);
            else
            {
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            }
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentReviewedSinceDateSearch()
        {
            string strDate = "5014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentReviewed(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);
            if (msg.NumberOfErrors > 0)
            {
                Assert.Fail(msg.Errors().FirstOrDefault().Message);
            }
            Assert.IsNotNull(savedMedia);

            var url = "contentReviewedSinceDate=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
            {
                Assert.IsTrue(media.First().dateContentReviewed == strDate);
            }
            else
            {
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            }
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentReviewedBeforeDateSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentReviewed(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "contentReviewedBeforeDate=" + "1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentReviewed == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2ContentReviewedInRangeSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateContentReviewed(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "contentReviewedInRange=" + "1914-05-30T18:01:00Z,1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateContentReviewed == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2DateSyndicationCapturedSearch()
        {
            string strDate = "2014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateSyndicationCaptured(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "dateSyndicationCaptured=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateSyndicationCaptured == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SyndicationCapturedSinceDateSearch()
        {
            string strDate = "5014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateSyndicationCaptured(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "syndicationCapturedSinceDate=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateSyndicationCaptured == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SyndicationCapturedBeforeDateSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateSyndicationCaptured(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "syndicationCapturedBeforeDate=" + "1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateSyndicationCaptured == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SyndicationCapturedInRangeSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateSyndicationCaptured(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "syndicationCapturedInRange=" + "1914-05-30T18:01:00Z,1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateSyndicationCaptured == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2DateSyndicationUpdatedSearch()
        {
            string strDate = "2014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateSyndicationUpdated(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "dateSyndicationUpdated=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateSyndicationUpdated == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SyndicationUpdatedSinceDateSearch()
        {
            string strDate = "5014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateSyndicationUpdated(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "syndicationUpdatedSinceDate=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateSyndicationUpdated == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SyndicationUpdatedBeforeDateSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateSyndicationUpdated(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "syndicationUpdatedBeforeDate=" + "1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateSyndicationUpdated == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SyndicationUpdatedInRangeSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateSyndicationUpdated(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);
            Assert.IsNotNull(savedMedia);

            var url = "syndicationUpdatedInRange=" + "1914-05-30T18:01:00Z,1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);
            Assert.IsNotNull(media);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateSyndicationUpdated == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2DateSyndicationVisibleSearch()
        {
            string strDate = "2014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateSyndicationVisible(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "dateSyndicationVisible=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateSyndicationVisible == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SyndicationVisibleSinceDateSearch()
        {
            string strDate = "5014-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateSyndicationVisible(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "syndicationVisibleSinceDate=" + strDate;
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateSyndicationVisible == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SyndicationVisibleBeforeDateSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateSyndicationVisible(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "syndicationVisibleBeforeDate=" + "1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
                Assert.IsTrue(media.First().dateSyndicationVisible == strDate);
            else
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void PublicApiV2SyndicationVisibleInRangeSearch()
        {
            string strDate = "1914-05-30T18:01:00Z";
            DateTime? date = ParseUtcDate(strDate);

            MediaObject newMediaObject = CreateNewMediaObjectWithDateSyndicationVisible(date, topicIds);
            var savedMedia = new MediaObject();
            ValidationMessages msg = mediaMgr.SaveMedia(newMediaObject, out savedMedia);

            var url = "syndicationVisibleInRange=" + "1914-05-30T18:01:00Z,1914-05-31T18:01:00Z";
            var media = TestApiUtility.PublicApiV2MediaSearch(url);

            if (media.Count() == 1)
            {
                Assert.IsTrue(media.First().dateSyndicationVisible == strDate);
            }
            else
            {
                Assert.IsTrue(media.Any(a => a.id == savedMedia.Id));
            }
            savedMedias.Push(savedMedia);
        }

        [TestMethod]
        public void CanCreateBadge()
        {
            //TODO:  Move this later
            var media = new MediaObject
            {
                MediaTypeCode = "Badge"
            };
            var savedMedia = new MediaObject();
            var messages = mediaMgr.SaveMedia(media, out savedMedia);

            savedMedias.Push(savedMedia);

        }

        #region private methods

        private static MediaObject CreateNewMediaObjectWithLanguage(string language, List<int> topicValueIds)
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = language,
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "test",
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;

            return newMediaObject;
        }

        public static MediaObject CreateNewMediaObjectWithTitle(string title, List<int> topicValueIds)
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = title,
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null,
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;

            return newMediaObject;
        }

        public static MediaObject CreateNewStagedMedia()
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = "PDF",
                MimeTypeCode = ".pdf",
                CharacterEncodingCode = "UTF-8",
                Title = "New Staged Media",
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Staged,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null,
            };

            return newMediaObject;
        }

        public static MediaObject CreateNewCollection()
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = "Collection",
                CharacterEncodingCode = "UTF-8",
                Title = "CreateNewCollection",
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null,
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicIds);
            newMediaObject.AttributeValues = topics;


            return newMediaObject;
        }


        private static MediaObject CreateNewMediaObjectWithDescription(string description, List<int> topicValueIds)
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "test",
                Description = description,
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;

            return newMediaObject;
        }

        private static MediaObject CreateNewMediaObjectWithSourceName(string sourceName, List<int> topicValueIds)
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = sourceName,
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "test",
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;

            return newMediaObject;
        }

        private static MediaObject CreateNewMediaObjectWithSourceUrl(string sourceUrl, List<int> topicValueIds)
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "test",
                Description = "test",
                SourceUrl = sourceUrl,
                //SourceUrlForAPI = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;

            return newMediaObject;
        }

        private static MediaObject CreateNewMediaObjectWithDateContentAuthored(DateTime? date, List<int> topicValueIds)
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "test",
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = date,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;

            return newMediaObject;
        }

        private static MediaObject CreateNewMediaObjectWithDateContentUpdated(DateTime? date, List<int> topicValueIds)
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "test",
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = date,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;

            return newMediaObject;
        }

        private static MediaObject CreateNewMediaObjectWithDateContentPublished(DateTime? date, List<int> topicValueIds)
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "test",
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = date,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;

            return newMediaObject;
        }

        private static MediaObject CreateNewMediaObjectWithDateContentReviewed(DateTime? date, List<int> topicValueIds)
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "test",
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = date,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;

            return newMediaObject;
        }

        private static MediaObject CreateNewMediaObjectWithDateSyndicationCaptured(DateTime? date, List<int> topicValueIds)
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "test",
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = date,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = null
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;

            return newMediaObject;
        }

        private static MediaObject CreateNewMediaObjectWithDateSyndicationUpdated(DateTime? date, List<int> topicValueIds)
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "test",
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = date,
                DateSyndicationVisible = null
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;

            return newMediaObject;
        }

        private static MediaObject CreateNewMediaObjectWithDateSyndicationVisible(DateTime? date, List<int> topicValueIds)
        {
            MediaObject newMediaObject = new MediaObject()
            {
                SourceCode = "Centers for Disease Control and Prevention",
                LanguageCode = "English",
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                MimeTypeCode = ".html",
                CharacterEncodingCode = "UTF-8",
                Title = "test",
                Description = "test",
                SourceUrlForApi = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                TargetUrl = "http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm",
                RatingMinimum = 1,
                RatingMaximum = 5,
                MediaStatusCodeValue = MediaStatusCodeValue.Published,
                PublishedDateTime = DateTime.UtcNow,
                DateContentAuthored = null,
                DateContentReviewed = null,
                DateContentUpdated = null,
                DateContentPublished = null,
                DateSyndicationCaptured = null,
                DateSyndicationUpdated = null,
                DateSyndicationVisible = date
            };

            List<AttributeValueObject> topics = CreateAttributeValues("Topic", topicValueIds);
            newMediaObject.AttributeValues = topics;

            return newMediaObject;
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
                AttributeId = 1,
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

        #endregion

    }
}
