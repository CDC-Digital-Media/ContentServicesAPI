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
using System.Data.Objects;
using System.Linq;
using System.Text.RegularExpressions;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.CsBusinessObjects.Media;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Search.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;
using System.Data.SqlClient;
using System.Data;

namespace SearchUnitTests
{
    [TestClass]
    public class SearchMediaTests
    {
        public static IObjectContextFactory MediaObjectContextFactory = new MediaObjectContextFactory();
        public MediaSearchMgr mediaSearchDataManager = new MediaSearchMgr();

        public SearchMediaTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        ISearchControllerFactory SearchControllerFactory = ContentServicesDependencyBuilder.SearchControllerFactory;

        [TestMethod]  //This is a slow test and does not need to be executed every time
        public void TestUnfilteredMediaSearch()
        {
            ISearchController theSearchController = SearchControllerFactory.GetSearchController(new SearchParameters("Media", "Media"));
            DataSetResult results = theSearchController.Get();
        }

        [TestMethod]
        public void SearchTitle()
        {
            string title = "CDC Features - Get the Facts on Diabetes";
            SearchParameters parms = new SearchParameters("Media", "Media", "Id".Direction(SortOrderType.Asc),
                "Title".Is(ExactMatch(title)));

            List<MediaObject> titleSearchControlGroup = GetFromTable(title);

            DataSetResult page1 = mediaSearchDataManager.GetData(parms);
            List<MediaObject> theItems = ((IEnumerable<MediaObject>)page1.Records).ToList();

            Assert.AreEqual(0, theItems.Where(i => !i.Title.Contains(title)).Count());
            Assert.IsTrue(theItems.Where(i => i.Title.Contains(title)).Count() > 0);
            Assert.AreEqual(theItems.Count, titleSearchControlGroup.Count());
            Console.WriteLine(theItems.Count);
        }

        [TestMethod]
        public void SearchExactTitle()
        {
            string title = "CDC Features - Get the Facts on Diabetes";
            var criteria = new SearchCriteria { ExactTitle = title };
            var result = CsMediaSearchProvider.Search(criteria);

            Assert.IsNotNull(result);

            Assert.AreEqual(0, result.Where(i => !i.Title.Contains(title)).Count());
            var matches = result.Where(r => r.Title.Contains(title));
            Assert.IsTrue(matches.Count() > 0, matches.Count().ToString());
        }

        [TestMethod]
        public void SearchTitleBunnyExclamation()
        {
            var title = "Bunny!";
            var criteria = new SearchCriteria { Title = title };
            var result = CsMediaSearchProvider.Search(criteria);
            Assert.IsNotNull(result);

            Assert.AreEqual(0, result.Where(i => !i.Title.Contains(title)).Count());
            var matches = result.Where(r => r.Title.Contains(title));
            Assert.IsTrue(matches.Count() > 0, matches.Count().ToString());
        }



        [TestMethod]
        public void CanSearchWithApostrophe()
        {
            var title = "Terrie's";
            var criteria = new SearchCriteria { Title = title };
            var results = CsMediaSearchProvider.Search(criteria);
            var count = results.Count();
            Assert.IsTrue(count > 0);
            Assert.IsTrue(count < 10);
            var first = results.First();
            Assert.IsTrue(first.Title.Contains(title));
        }

        [TestMethod, Ignore]
        public void CanSearchWithPercentSign() //Bug 809 was validated without allowing % sign
        {
            var title = "50%";
            var criteria = new SearchCriteria { Title = title };
            var results = CsMediaSearchProvider.Search(criteria);
            var count = results.Count();
            Assert.IsTrue(count > 0);
            var first = results.First();
            Assert.IsTrue(first.Title.Contains(title));
        }

        [TestMethod]
        public void SearchTitleFlu()
        {
            string title = "flu";
            var criteria = new SearchCriteria { Title = title };
            var result = CsMediaSearchProvider.Search(criteria);

            Assert.IsNotNull(result);

            Assert.IsTrue(result.Count() > 0);
        }

        [TestMethod]
        public void SearchSourceNameFda()
        {
            string sourceName = "Food and Drug Administration";
            var criteria = new SearchCriteria { SourceName = sourceName };
            var result = CsMediaSearchProvider.Search(criteria);
            Assert.IsTrue(result.Count() < 10000);
        }
                
        [TestMethod]
        public void SearchTitleAndTopic2()
        {
            string title = "Food Safety Features";
            var criteria = new SearchCriteria { Title = title, Topic = "Food Safety" };
            var result = CsMediaSearchProvider.Search(criteria);
            Console.WriteLine(result.Count());

            Assert.IsNotNull(result.First().Title);
            Assert.IsTrue(result.Where(i => i.Title.Contains(title)).DefaultIfEmpty().Count() > 0);
            Console.WriteLine(result.Where(i => i.Title.Contains(title)).Count());
        }

        [TestMethod]
        public void SearchByMediaId()
        {
            var mediaId = 138478;
            var criteria = new SearchCriteria { MediaId = mediaId.ToString() };
            var result = CsMediaSearchProvider.Search(criteria);
            Assert.AreEqual(1, result.Count());
            var item = result.First();
            Assert.AreEqual(mediaId, item.MediaId);
        }

        [TestMethod]
        public void SearchTopicId()
        {
            var topicId = 25651;
            SearchParameters parms = new SearchParameters("Media", "Media", "Id".Direction(SortOrderType.Asc),
               "TopicId".Is(topicId));

            DataSetResult page1 = mediaSearchDataManager.GetData(parms);
            Guid resultsSetId = page1.Id;

            List<MediaObject> theItems = ((IEnumerable<MediaObject>)page1.Records).ToList();
            List<int> selectedMediaIds = theItems.Select(i => i.Id).ToList();
            List<int> controlMediaIds = GetControlMediaIds(resultsSetId);

            List<int> selectedMediaIdsNotInControl = selectedMediaIds.Where(i => !controlMediaIds.Contains(i)).ToList();
            List<int> controlMediaIdsNotInSelection = controlMediaIds.Where(i => !selectedMediaIds.Contains(i)).ToList();

            Assert.IsTrue(selectedMediaIds.Count() > 0);
            Assert.AreEqual(0, selectedMediaIdsNotInControl.Count()); //Make sure all selected Ids are in actual
            Assert.AreEqual(0, controlMediaIdsNotInSelection.Count()); //Make sure all actual Ids are in select
        }

        [TestMethod]
        public void SearchTitleWithinTopicId()
        {
            string title = "food";
            var topicId = 25651;
            SearchParameters parms = new SearchParameters("Media", "Media", "Id".Direction(SortOrderType.Asc),
                "TopicId".Is(topicId), "Title".Is(title));

            DataSetResult page1 = mediaSearchDataManager.GetData(parms);
            Guid resultsSetId = page1.Id;

            List<MediaObject> theItems = ((IEnumerable<MediaObject>)page1.Records).ToList();
            List<int> selectedMediaIds = theItems.Select(i => i.Id).ToList();
            List<int> controlMediaIds = GetControlMediaIds(resultsSetId, title);

            List<int> selectedMediaIdsNotInControl = selectedMediaIds.Where(i => !controlMediaIds.Contains(i)).ToList();
            List<int> controlMediaIdsNotInSelection = controlMediaIds.Where(i => !selectedMediaIds.Contains(i)).ToList();

            Assert.IsTrue(selectedMediaIds.Count() > 0);
            Assert.AreEqual(0, selectedMediaIdsNotInControl.Count()); //Make sure all selected Ids are in actual
            Assert.IsTrue(controlMediaIdsNotInSelection.Count() > 0);
        }

        [TestMethod]
        public void SearchFullTextWithinTopicId()
        {
            string fullText = "Disease";
            var topicId = 25651;
            SearchParameters parms = new SearchParameters("Media", "Media", "Id".Direction(SortOrderType.Asc),
                "TopicId".Is(topicId), "FullSearch".Is("\"" + fullText + "\""));

             DataSetResult page1 = mediaSearchDataManager.GetData(parms);
            Guid resultsSetId = page1.Id;

            List<MediaObject> theItems = ((IEnumerable<MediaObject>)page1.Records).ToList();
            List<MediaObject> theItemsWithoutTopicId = theItems.Where(m => !m.GetAttributeValues("Topic").Select(t => t.ValueKey.Id == topicId).Any()).ToList();
            List<MediaObject> theItemsWithTopicId = theItems.Where(m => m.GetAttributeValues("Topic").Select(t => t.ValueKey.Id == topicId).Any()).ToList();
            List<int> selectedMediaIds = theItems.Select(i => i.Id).ToList();
            List<int> controlMediaIds = GetControlMediaIds(resultsSetId, fullText, fullText);

            List<int> selectedMediaIdsNotInControl = selectedMediaIds.Where(i => !controlMediaIds.Contains(i)).ToList();
            List<int> controlMediaIdsNotInSelection = controlMediaIds.Where(i => !selectedMediaIds.Contains(i)).ToList();
            //TODO: Need to add search of "Disease" in topic'
            Assert.IsTrue(selectedMediaIds.Count() > 0);
            Assert.IsTrue(0 < selectedMediaIdsNotInControl.Count() && selectedMediaIdsNotInControl.Count() < 800, "count: " + selectedMediaIdsNotInControl.Count());
            Assert.AreEqual(0, controlMediaIdsNotInSelection.Count()); //Make sure all actual Ids are in select
        }

        [TestMethod]
        public void SearchPersistentUrl()
        {
            var media = TestApiUtility.SinglePublishedHtml();
            var criteria = new SearchCriteria { PersistentUrl = media.persistentUrlToken };
            var result = CsMediaSearchProvider.Search(criteria);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(media.persistentUrlToken, result.First().PersistentUrlToken);
        }

        private List<int> GetControlMediaIds(Guid resultsSetId)
        {
            return GetControlMediaIds(resultsSetId, string.Empty);
        }

        private List<int> GetControlMediaIds(Guid resultsSetId, string title)
        {
            return GetControlMediaIds(resultsSetId, title, string.Empty);
        }

        private List<int> GetControlMediaIds(Guid resultsSetId, string title, string description)
        {
            using (MediaObjectContext media = (MediaObjectContext)MediaObjectContextFactory.Create())
            {
                List<int> valueIds = GetSelectionValueIds(resultsSetId, media);

                IQueryable<Medias> medias = from mv in media.MediaDbEntities.MediaValues
                                            join m in media.MediaDbEntities.Medias on mv.MediaId equals m.MediaId
                                            join a in media.MediaDbEntities.Attributes
                                                 on mv.AttributeID equals a.AttributeID
                                            where valueIds.Contains(mv.ValueId) && a.AttributeName == "Topic"
                                            select m;

                var query = medias as ObjectQuery;
                //if (query is null) 
                //var query = medias.ToString();
                Console.WriteLine(query.ToTraceString());

                var x = medias.Distinct().ToList();
                if (!string.IsNullOrEmpty(title))
                {
                    if (!string.IsNullOrEmpty(description))
                        medias = medias.Where(m => m.Title.Contains(title) || m.Description.Contains(description));
                    else
                        medias = medias.Where(m => m.Title.Contains(title));
                }

                List<Medias> results = medias.Distinct().OrderBy(m => m.MediaId).ToList();
                List<Medias> resultsFirst = results;

                if (title != "" || description != "")
                {
                    string titlePattern = (title == null) ? null : string.Format(@"\b{0}\b", Regex.Escape(title));
                    string descriptionPattern = (description == null) ? null : string.Format(@"\b{0}\b", Regex.Escape(description));
                    results = results.Where(m => Matches(m, title, titlePattern, description, descriptionPattern)).ToList();

                }

                List<int> mediaIds = results.Select(m => m.MediaId).Distinct().ToList();

                return mediaIds;
            }
        }

        private static bool Matches(Medias media, string title, string titlePattern, string description, string descriptionPattern)
        {
            if (titlePattern != null && (Regex.IsMatch(media.Title, titlePattern) || media.Title.StartsWith(title, StringComparison.OrdinalIgnoreCase) || media.Title.EndsWith(description, StringComparison.OrdinalIgnoreCase)))
                return true;
            if (descriptionPattern != null && (Regex.IsMatch(media.Description, descriptionPattern) || media.Description.StartsWith(description, StringComparison.OrdinalIgnoreCase) || media.Description.EndsWith(description, StringComparison.OrdinalIgnoreCase)))
                return true;

            return false;
        }

        private static List<int> GetSelectionValueIds(Guid resultsSetId, MediaObjectContext media)
        {
            IQueryable<SelectionValue> selectionValues = media.MediaDbEntities.SelectionValues;
            return selectionValues
                .Where(sv => sv.SelectionId == resultsSetId && sv.SelectionType == "TopicId")
                .Select(sv => sv.ValueId).ToList();
        }

        private List<MediaObject> GetFromTable(string title)
        {
            using (MediaObjectContext media = (MediaObjectContext)MediaObjectContextFactory.Create())
            {
                return MediaCtl.AddSubLists(media,
                    MediaCtl.Get(media).Where(c => c.Title.Contains(title) && c.DisplayOnSearch).ToList());
            }
        }

        private string ExactMatch(string value)
        {
            return "\"" + value + "\"";
        }

        [TestMethod]
        public void TestFullTextMediaSearch()
        {
            var topicId = 25651;
            var mediaIdWithTopic = 138717;
            string fullSearch = "disease";
            SearchParameters parms = new SearchParameters("Media", "Media", "Id".Direction(SortOrderType.Asc),
                "FullSearch".Is(fullSearch),
                "TopicId".Is(new List<int> { topicId }));

            ISearchController controller = SearchControllerFactory.GetSearchController(parms);
            DataSetResult page1 = controller.Get();
            IEnumerable<MediaObject> page1Items = (IEnumerable<MediaObject>)page1.Records;
            List<MediaObject> page1ItemsAsList = page1Items.ToList();
            Assert.AreEqual(mediaIdWithTopic, page1ItemsAsList[2].Id);
        }

        [TestMethod]
        public void TestFilteredMediaSearch()
        {
            SearchParameters parms = new SearchParameters("Media", "Media", "Id".Direction(SortOrderType.Asc),
                "FullSearch".Is("Smoking"));
            parms.Paging = new Paging(4, 1);

            parms.SecondsToLive = 300;

            ISearchController controller = SearchControllerFactory.GetSearchController(parms);
            DataSetResult page1 = controller.Get();
            IEnumerable<MediaObject> page1Items = (IEnumerable<MediaObject>)page1.Records;
            List<MediaObject> page1ItemsAsList = page1Items.ToList();
            Assert.AreEqual(138550, page1ItemsAsList[2].Id);
            Assert.AreEqual(4, page1Items.Count());

            int countWithNoTopics = page1Items.Where(i => i.GetAttributeValues("Topic").Count() == 0).Count();
            Assert.AreEqual(0, countWithNoTopics);

            ISearchController controller2 = SearchControllerFactory.GetSearchController(page1.Id);
            DataSetResult page10 = controller2.NavigatePages(10);
            IEnumerable<MediaObject> page10Items = (IEnumerable<MediaObject>)page10.Records;
            Assert.AreEqual(4, page10Items.Count());
        }


        [TestMethod]
        public void CanRetrieveMediaById()
        {
            var media = TestApiUtility.SinglePublishedHtml();
            var mediaId = media.mediaId;
            SearchParameters parms = new SearchParameters("Media", "Media", "MediaId".Is(mediaId));
            ISearchController controller = SearchControllerFactory.GetSearchController(parms);

            DataSetResult page1 = controller.Get();
            IEnumerable<MediaObject> page1Items = (IEnumerable<MediaObject>)page1.Records;
            Assert.AreEqual(1, page1Items.Count());
        }

    }
}
