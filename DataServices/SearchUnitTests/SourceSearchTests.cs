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

using System.Collections.Generic;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Search.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace SearchUnitTests
{
    [TestClass]
    public class SourceSearchTests
    {
        static SourceSearchTests()
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
        public void TestAllSources()
        {
            SearchParameters searchParameters = new SearchParameters("Media", "Source", new Sorting("Code".Direction(SortOrderType.Asc)));
            ISearchController theSearchController = SearchControllerFactory.GetSearchController(searchParameters);
            DataSetResult sources = theSearchController.Get();
            List<SourceItem> listOfSources = (List<SourceItem>)sources.Records;
            Assert.IsTrue(listOfSources.Count > 5);
        }

    }
}
