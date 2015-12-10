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
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SearchUnitTests
{
    [TestClass]
    public class BusinessUnitTests
    {
        static BusinessUnitTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }
        public static IObjectContextFactory MediaObjectContextFactory = new MediaObjectContextFactory();
        public BusinessUnitSearchDataMgr businessUnitSearchDataMgr = new BusinessUnitSearchDataMgr();
        public SourceSearchDataMgr sourceSearchDataMgr = new SourceSearchDataMgr();

        [TestMethod]
        public void TestAllBusinessUnits()
        {
            SearchParameters searchParameters = new SearchParameters("Media", "BusinessUnit", new Sorting("Name".Direction(SortOrderType.Asc)));

            List<SourceItem> sourceItems = sourceSearchDataMgr.GetRecords(
                new SearchParameters("Code".Is("Centers for Disease"))
                );
            List<BusinessUnitItem> businessUnits = businessUnitSearchDataMgr.GetRecords(
                new SearchParameters("Name".Direction(SortOrderType.Asc), "SourceCode".Is(sourceItems[0].Code)));
      
            Assert.IsTrue(businessUnits.Count > 10);
        }

    }

}
