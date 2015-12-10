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
    public class SearchFlatVocabTests
    {
        static SearchFlatVocabTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        static ISearchControllerFactory _searchControllerFactory;
        static ISearchControllerFactory SearchControllerFactory
        {
            get
            {
                return _searchControllerFactory ?? (_searchControllerFactory = ContentServicesDependencyBuilder.SearchControllerFactory);
            }
        }

        [TestMethod]
        public void TestSortByValueName()
        {
            //ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            Criteria filterCriteria = new Criteria();
            //SearchParameters searchParameters = GetVocabularySearchParms2("ValueName", filterCriteria);
            SearchParameters searchParameters = new SearchParameters("Media", "FlatVocabValue", new Sorting("ValueName".Direction(SortOrderType.Asc)));

            DateTime startTime = DateTime.Now;
            DataSetResult mediaTopics = SearchControllerFactory.GetSearchController(searchParameters).Get();
            System.Diagnostics.Trace.WriteLine(GetCurrentMethod() + "Took " + DateTime.Now.Subtract(startTime).ToString());
            List<FlatVocabValueItem> vocabs = (List<FlatVocabValueItem>)mediaTopics.Records;
            List<string> names = (from v in vocabs select v.ValueObject.ValueName).ToList();
            List<int> ids = (from v in vocabs select v.ValueObject.ValueId).ToList();

            for (int i = 0; i < names.Count - 1; ++i)
            {
                Assert.IsTrue(string.Compare(names[i].TrimEnd(), names[i + 1].TrimEnd(), ignoreCase: true)<= 0);
            }
            //Assert.AreEqual(1, vocabs[0].ValueId);
            //Assert.AreEqual(2, vocabs[1].ValueId);
            string a = vocabs.ToString();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            return sf.GetMethod().Name;
        }


        [TestMethod]
        public void TestSortByValueId()
        {
            //Criteria filterCriteria = new Criteria();
            //SearchParameters searchParameters = GetVocabularySearchParms2("ValueId", filterCriteria);

            SearchParameters searchParameters = new SearchParameters("Media", "FlatVocabValue", new Sorting("ValueId".Direction(SortOrderType.Asc)));

            DataSetResult mediaTopics = SearchControllerFactory.GetSearchController(searchParameters).Get();
            List<FlatVocabValueItem> vocabs = (List<FlatVocabValueItem>)mediaTopics.Records;
            List<string> names = (from v in vocabs select v.ValueObject.ValueName).ToList();
            List<int> ids = (from v in vocabs select v.ValueObject.ValueId).ToList();

            for (int i = 0; i < names.Count - 1; ++i)
            {
                Assert.IsTrue(ids[0] < ids[1]);
            }
            //Assert.AreEqual(1, vocabs[0].ValueId);
            //Assert.AreEqual(2, vocabs[1].ValueId);
            string a = vocabs.ToString();
        }

        [TestMethod]
        public void GetFlatAudienceList()
        {
            SearchParameters searchParameters = new SearchParameters("Media", "FlatVocabValue", new Sorting("ValueId".Direction(SortOrderType.Asc)),
                "ValueSetName".Is("Audiences|English"));

            ISearchController searchController = SearchControllerFactory.GetSearchController(searchParameters);
            DataSetResult mediaTopics = searchController.Get();
            List<FlatVocabValueItem> vocabs = (List<FlatVocabValueItem>)mediaTopics.Records;
            List<string> names = (from v in vocabs select v.ValueObject.ValueName).ToList();
            List<int> ids = (from v in vocabs select v.ValueObject.ValueId).ToList();

            for (int i = 0; i < names.Count - 1; ++i)
            {
                Assert.IsTrue(ids[0] < ids[1]);
            }
            //Assert.AreEqual(1, vocabs[0].ValueId);
            //Assert.AreEqual(2, vocabs[1].ValueId);
            string a = vocabs.ToString();
        }

    }
}
