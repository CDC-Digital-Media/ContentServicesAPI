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
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Search.Controller;
using Gov.Hhs.Cdc.Connection;

namespace RegistrationTest
{
    public static class DAL
    {
        public static void Initialize()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        private static ISearchControllerFactory _searchControllerFactory;
        public static ISearchControllerFactory SearchControllerFactory
        {
            get
            {
                return _searchControllerFactory ?? (_searchControllerFactory = ContentServicesDependencyBuilder.SearchControllerFactory);
            }
        }

        public static List<OrgTypeObject> GetOrganizationTypes()
        {
            SearchParameters searchParameters = new SearchParameters("Registration", "OrgType", "DisplayOrdinal".Direction(SortOrderType.Asc));

            DataSetResult organizations = SearchControllerFactory.GetSearchController(searchParameters).Get();
            return (List<OrgTypeObject>)organizations.Records;
        }
        
        public static List<OrganizationObject> GetOrganizations()
        {
            SearchParameters searchParameters = new SearchParameters("Registration", "Organization", "Name".Direction(SortOrderType.Asc));
            DataSetResult organizations = SearchControllerFactory.GetSearchController(searchParameters).Get();
            return (List<OrganizationObject>)organizations.Records;
        }

    }
}
