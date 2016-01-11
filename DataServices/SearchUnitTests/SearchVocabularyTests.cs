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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Api.Public;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Search.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SearchUnitTests
{
    [TestClass]
    public class SearchVocabularyTests
    {
        static ISearchControllerFactory _searchControllerFactory;
        static ISearchControllerFactory SearchControllerFactory
        {
            get
            {
                return _searchControllerFactory ?? (_searchControllerFactory = ContentServicesDependencyBuilder.SearchControllerFactory);
            }
        }

        static SearchVocabularyTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void TestTagTypes()
        {
            IApiServiceFactory publicService = new PublicApiServiceFactory();

            List<SerialTagType> tagTypes;
            ValidationMessages messages = TestApiUtility.ApiGet<SerialTagType>(publicService,
                publicService.CreateTestUrl("tagtypes", "", "", "", 2), out tagTypes);
            var sorted = tagTypes.OrderBy(t => t.name).ToList();
            Assert.IsTrue(tagTypes.Count >= 2);

            var audience = tagTypes.Where(t => string.Equals(t.name, "audience", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            Assert.IsNotNull(audience);

            //make sure it is sorted correctly
            Assert.IsTrue(tagTypes.SequenceEqual(sorted));

            messages = TestApiUtility.ApiGet<SerialTagType>(publicService,
                publicService.CreateTestUrl("tagtypes", "", "", "sort=-name", 2), out tagTypes);
            sorted = tagTypes.OrderByDescending(t => t.name).ToList();

            var mismatches = tagTypes.Except(sorted);
            Assert.IsFalse(mismatches.Any());
        }

        [TestMethod]
        public void TestAToZ()
        {
            var publicService = new PublicApiServiceFactory();
            List<SerialAToZ> aToZs;
            ValidationMessages messages = TestApiUtility.ApiGet<SerialAToZ>(publicService,
                publicService.CreateTestUrl("atoz", 0, "", "language=english&max=0&valueset=topics"), out aToZs);
            var sorted = aToZs.OrderBy(t => t.letter).ToList();
            sorted.Add(sorted[0]);  //Move the # to the end
            sorted.RemoveAt(0);
            Assert.IsTrue(aToZs.Count >= 2);

            //make sure it is sorted correctly
            Assert.IsTrue(aToZs.SequenceEqual(sorted));

            messages = TestApiUtility.ApiGet<SerialAToZ>(publicService,
                publicService.CreateTestUrl("atoz", 0, "", "sort=-letter&language=english&max=0&valueset=topics"), out aToZs);
            sorted = aToZs.OrderByDescending(t => t.letter).ToList();
            Assert.IsTrue(aToZs.SequenceEqual(sorted));

            messages = TestApiUtility.ApiGet<SerialAToZ>(publicService,
                publicService.CreateTestUrl("atoz", 0, "", "valueset=topics&language=english&max=0"), out aToZs);
            Assert.AreEqual(27, aToZs.Count);
        }

        [TestMethod]
        public void TestTags()
        {
            int testMediaId = 138478;
            IApiServiceFactory publicService = new PublicApiServiceFactory();
            List<SerialTag> tags;
            ValidationMessages messages = TestApiUtility.ApiGet<SerialTag>(publicService,
                publicService.CreateTestUrl("tags", "", "", "", 2), out tags);
            var sorted = tags.OrderBy(t => t.name).ToList();
            Assert.IsTrue(tags.Count >= 2);


            List<SerialTag> filteredTags;
            messages = TestApiUtility.ApiGet<SerialTag>(publicService,
                publicService.CreateTestUrl("tags", "", "", "nameContains=smoking", 2), out filteredTags);
            Assert.IsTrue(filteredTags.Count > 0);
            Assert.IsFalse(filteredTags.Where(t => !t.name.ToLower().Contains("smoking"))
                .Any());

            messages = TestApiUtility.ApiGet<SerialTag>(publicService,
                publicService.CreateTestUrl("tags", "", "", "name=Smoking in Movies", 2), out filteredTags);
            Assert.AreEqual(1, filteredTags.Count);


            messages = TestApiUtility.ApiGet<SerialTag>(publicService,
                publicService.CreateTestUrl("tags", "", "", "typeID=1", 2), out filteredTags);
            Assert.IsFalse(
                filteredTags.Where(t => !string.Equals(t.type, "Topic", StringComparison.OrdinalIgnoreCase)).Any()
                );
            Assert.IsTrue(filteredTags.Count() > 10);

            messages = TestApiUtility.ApiGet<SerialTag>(publicService,
                publicService.CreateTestUrl("tags", "", "", "typename=topic", 2), out filteredTags);
            Assert.IsFalse(
                filteredTags.Where(t => !string.Equals(t.type, "Topic", StringComparison.OrdinalIgnoreCase)).Any()
                );
            Assert.IsTrue(filteredTags.Count() > 10);


            messages = TestApiUtility.ApiGet<SerialTag>(publicService,
                publicService.CreateTestUrl("tags", "", "", "mediaid=" + testMediaId, 2), out filteredTags);
            Assert.IsFalse(
                filteredTags.Where(t => !string.Equals(t.type, "Topic", StringComparison.OrdinalIgnoreCase)).Any()
                );
            Assert.IsTrue(filteredTags.Count() > 2);
        }

        [TestMethod]
        public void TestTopValueSetWithChildren()
        {
            Criteria filterCriteria = new Criteria();
            filterCriteria.Add("ValueSetName", "Topics|English");
            SearchParameters searchParameters = GetHierVocabValueSearchParms(filterCriteria);
            DateTime startTime = DateTime.Now;
            DataSetResult mediaTopics = SearchControllerFactory.GetSearchController(searchParameters).Get();
            System.Diagnostics.Trace.WriteLine(GetCurrentMethod() + "Took " + DateTime.Now.Subtract(startTime).ToString());
            List<HierVocabValueItem> vocabs = (List<HierVocabValueItem>)mediaTopics.Records;
            Assert.IsTrue(15 < vocabs.Count && vocabs.Count < 250, vocabs.Count.ToString());
        }

        [TestMethod]
        public void TestAudiencesWithChildren()
        {
            SearchParameters searchParameters = new SearchParameters("Media", "HierVocabValue", "DisplayOrdinal".Direction(SortOrderType.Asc),
                "ValueSetName".Is("Audiences|English"));
            DataSetResult audiences = SearchControllerFactory.GetSearchController(searchParameters).Get();
            List<HierVocabValueItem> hierAudiences = (List<HierVocabValueItem>)audiences.Records;
            var x = hierAudiences.Where(a => a.Children.Count > 0).ToList();
        }

        [TestMethod]
        public void TestTopValueSetWithChildrenAndCounts()
        {
            Criteria filterCriteria = new Criteria();
            filterCriteria.Add("ValueSetName", "Topics|English");
            filterCriteria.Add("IsInUse", true);
            SearchParameters searchParameters = GetHierVocabValueSearchParms(filterCriteria);
            DateTime startTime = DateTime.Now;
            DataSetResult mediaTopics = SearchControllerFactory.GetSearchController(searchParameters).Get();
            System.Diagnostics.Trace.WriteLine(GetCurrentMethod() + "Took " + DateTime.Now.Subtract(startTime).ToString());
            List<HierVocabValueItem> vocabs = (List<HierVocabValueItem>)mediaTopics.Records;
            Assert.IsTrue(2 < vocabs.Count && vocabs.Count < 500);
            string a = vocabs.ToString();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            return sf.GetMethod().Name;
        }

        private static SearchParameters GetHierVocabValueSearchParms(Criteria filterCriteria)
        {
            SearchParameters searchParameters = new SearchParameters()
            {
                ApplicationCode = "Media",
                DataSetCode = "HierVocabValue",
                BasicCriteria = filterCriteria,
                Sorting = new Sorting(new SortColumn("DisplayOrdinal", SortOrderType.Asc))
            };
            return searchParameters;
        }

        [TestMethod]
        public void TestVocabularyByMediaType()
        {
            Criteria filterCriteria = new Criteria();
            filterCriteria.Add("ValueSetName", "Topics|English");

            SearchParameters searchParameters = new SearchParameters()
            {
                ApplicationCode = "Media",
                DataSetCode = "HierVocabValue",
                BasicCriteria = filterCriteria
            };

            DataSetResult mediaTopics = SearchControllerFactory.GetSearchController(searchParameters).Get();
 
            List<HierVocabValueItem> vocabs = (List<HierVocabValueItem>)mediaTopics.Records;
            List<int> x = vocabs.Select(v => v.ValueKey.Id).ToList();
            Assert.IsTrue(10 < vocabs.Count && vocabs.Count < 250);
            var abortionSurveillance = vocabs[0].Children[0]; //should have 8 media as of 7/23/2014
            var acanthamoeba = vocabs[0].Children[0]; //0 media as of 7/23/2014
            Assert.IsTrue(abortionSurveillance.DescendantMediaUsageCount > 1 && acanthamoeba.DescendantMediaUsageCount < 500, "Actual count: " + abortionSurveillance.DescendantMediaUsageCount.ToString());
        }

        [TestMethod]
        public void TestVocabularyForEcards()
        {
            Criteria filterCriteria = new Criteria();
            filterCriteria.Add("ValueSetName", "Topics|English");
            filterCriteria.Add("MediaType", "eCard");
            filterCriteria.Add("IsInUse", true);

            SearchParameters searchParameters = GetHierVocabValueSearchParms(filterCriteria);
            DateTime startTime = DateTime.Now;
            DataSetResult mediaTopics = SearchControllerFactory.GetSearchController(searchParameters).Get();
            System.Diagnostics.Trace.WriteLine(GetCurrentMethod() + "Took " + DateTime.Now.Subtract(startTime).ToString());
            List<HierVocabValueItem> vocabs = (List<HierVocabValueItem>)mediaTopics.Records;
            List<int> x = vocabs.Select(v => v.ValueKey.Id).ToList();
            Assert.IsTrue(3 < vocabs.Count && vocabs.Count < 10, vocabs.Count.ToString());
            Assert.IsTrue(vocabs[0].Children[0].DescendantMediaUsageCount > 1 && vocabs[0].Children[0].DescendantMediaUsageCount < 200, "Actual count: " + vocabs[0].Children[0].DescendantMediaUsageCount.ToString());
        }

        [TestMethod]
        public void TestVocabularyWithCounts()
        {
            Criteria filterCriteria = new Criteria();
            filterCriteria.Add("ValueSetName", "Topics|English");

            SearchParameters searchParameters = GetHierVocabValueSearchParms(filterCriteria);

            DataSetResult mediaTopics = SearchControllerFactory.GetSearchController(searchParameters).Get();

            List<HierVocabValueItem> vocabs = (List<HierVocabValueItem>)mediaTopics.Records;
            List<int> x = vocabs.Select(v => v.ValueKey.Id).ToList();
            Assert.IsTrue(10 < vocabs.Count && vocabs.Count < 500);
        }

        [TestMethod]
        public void GetAllLanguages()
        {
            SearchParameters searchParameters = new SearchParameters()
            {
                ApplicationCode = "Media",
                DataSetCode = "Language",
                BasicCriteria = new Criteria(),
                Sorting = new Sorting(new SortColumn("DisplayOrdinal", SortOrderType.Asc))
            };
            ISearchController searchController = SearchControllerFactory.GetSearchController(searchParameters);
            DataSetResult languages = searchController.Get();
        }

    }
}
