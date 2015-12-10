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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Search.Controller;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Global;

namespace SearchUnitTests
{
    [TestClass]
    public class SearchControllerUnitTest
    {
        static SearchControllerUnitTest()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
        }
        //[TestMethod]
        public void TestUnPaged()
        {

            Criteria filterCriteria = CreateFilterCriteria();
            SearchParameters searchParameters = CreateSearchParameters();

            SearchControllerFactory searchControllerFactory = new SearchControllerFactory();
            ISearchController controller = searchControllerFactory.GetSearchController(searchParameters);
            DataSetResult result = controller.Get();
            Assert.AreEqual<int>(1000, result.RecordCount,
                "Get did not return 1000 records");

            List<TestItem> items = result.Records.Cast<TestItem>().ToList();
            Assert.AreEqual<int>(10, items[990].IntKey1,
                "Invalid record found on TestUnPaged");
            Assert.AreEqual<int>(1000, items.Count,
                "Number of records returned invalid");


        }

        //[TestMethod]
        public void TestPaged()
        {

            //Figure out how to BuildAssemblies automatically
            Criteria filterCriteria = CreateFilterCriteria();
            SearchParameters searchParameters = CreateSearchParameters(isPaged:true, pageSize:10);

            SearchControllerFactory searchControllerFactory = new SearchControllerFactory();
            ISearchController controller = searchControllerFactory.GetSearchController(searchParameters);
            DataSetResult result = controller.Get();
            Assert.AreEqual<int>(10, result.RecordCount,
                "Get did not return 10 records");
            Assert.AreEqual<int>(1, result.PageNumber,
                "Did not return page 1");
            

            List<TestItem> items = result.Records.Cast<TestItem>().ToList();
            Assert.AreEqual<int>(991, items[9].IntKey1,
                "Invalid record found on TestUnPaged");
            Assert.AreEqual<int>(10, items.Count,
                "Number of records returned invalid");


        }
        private static SearchParameters CreateSearchParameters(bool isPaged = false, int pageSize = 0)
        {
            SearchParameters searchParameters = new SearchParameters()
            {
                ApplicationCode = "TestApp",
                DataSetCode = "TestDataSet",
                BasicCriteria = CreateFilterCriteria(),
                Paging = new Paging(pageSize, 1),
                Sorting = new Sorting( new SortColumn("StringKey1", SortOrderType.Asc) ),
                SecondsToLive = 300
            };
            return searchParameters;
        }

        private static Criteria CreateFilterCriteria()
        {
            Criteria filterCriteria = new Criteria();
            filterCriteria.Add("StringValue1", "Value10");
            return filterCriteria;
        }
    }
}
