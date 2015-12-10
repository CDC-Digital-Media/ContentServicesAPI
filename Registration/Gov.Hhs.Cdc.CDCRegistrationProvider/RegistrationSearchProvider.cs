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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Search.Provider;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.CdcRegistrationProvider;
using System.Reflection;
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.DataServicesCacheProvider;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class RegistrationSearchProvider : BaseCsSearchProvider
    {
        public override string ApplicationCode { get { return "Registration"; } }
        public List<Assembly> _businessObjectAssemblies = new List<Assembly>() { typeof(RegistrationProviderSearchBusinessObjectPlaceHolder).Assembly };
        public override List<Assembly> BusinessObjectAssemblies
        {
            get { return _businessObjectAssemblies; }
        }

        public override string Code { get { return "RegistrationSearchProvider"; } }
        public override string Name { get { return "Content Services Registration Search Provider"; } }
        
        public static IObjectContextFactory ObjectContextFactory { get; set; }

        static RegistrationSearchProvider()
        {
            ObjectContextFactory = new RegistrationObjectContextFactory();
        }

        public override ISearchDataManager GetSearchDataManager(string filterCode)
        {
            //Get DataSets for Search Objects
            switch (SafeToLower(filterCode))
            {
                case "user":
                    return new UserSearchDataMgr();
                case "serviceuser":
                    return new ServiceUserSearchDataMgr();
                case "apiclient":
                    return new ApiClientSearchDataMgr();
                case "organization":
                    return new OrganizationSearchDataMgr();
                case "orgtype":
                    return new OrganizationTypeSearchDataMgr();
                case "userorganization":
                    return new UserOrganizationSearchDataMgr();
            }

            throw new ApplicationException(
                string.Format("The search data manager '{0}' has not been defined in {1}.GetSearchDataManager()",
                    filterCode, this.GetType().Name));
        }

    }
}
