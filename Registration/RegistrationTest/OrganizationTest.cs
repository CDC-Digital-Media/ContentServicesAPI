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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Search.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace OrganizationTests
{
    [TestClass]
    public class OrganizationTest
    {
        static OrganizationTest()
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
        public void TestTypeAhead()
        {
            SearchParameters searchParameters = new SearchParameters("Registration", "Organization", "Name".Direction(SortOrderType.Asc),
                "Name_StartsWith".Is("C"));

            DataSetResult organizations = SearchControllerFactory.GetSearchController(searchParameters).Get();
            List<OrganizationObject> orgs = (List<OrganizationObject>)organizations.Records;
            Assert.IsTrue(orgs.Count > 1);
            Assert.IsTrue(orgs[0].Name.StartsWith("C"));
        }

        [TestMethod]
        public void TestByOrganizationType()
        {
            SearchParameters searchParameters = new SearchParameters("Registration", "Organization", "Name".Direction(SortOrderType.Asc),
                "OrgType".Is("U.S. Federal Government"));

            DataSetResult organizations = SearchControllerFactory.GetSearchController(searchParameters).Get();
            List<OrganizationObject> orgs = (List<OrganizationObject>)organizations.Records;
            Assert.IsTrue(1 < orgs.Count && orgs.Count < 80, "actual count: " + orgs.Count);
        }

        private SearchParameters CreateNewSearchParameters(Criteria filterCriteria)
        {
            SearchParameters searchParameters = new SearchParameters()
            {
                ApplicationCode = "Registration",
                DataSetCode = "Organization",
                BasicCriteria = filterCriteria,
                Sorting = new Sorting(new SortColumn("Name", SortOrderType.Asc))
            };
            return searchParameters;
        }

    }
}
