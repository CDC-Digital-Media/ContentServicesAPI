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
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataSource;

namespace SearchUnitTests
{
    [TestClass]
    public class CacheFrameworkTests
    {
        [TestMethod]
        public void TestGetManagedCacheItems()
        {
            //List<CacheDataSetDefinition> dataSets = Cache.CacheAttributeMgr.GetCacheDataSets(typeof(Gov.Hhs.Cdc.DataServices.Bo.DsApplication));

            //ManagedCacheItems items = ManagedCacheItems.Get();
            //string results = items.ToString();
        }

        [TestMethod]
        public void TestMediaCachedItems()
        {
            //OrganizationMgr organizationMgr = CdcDataSource.GetDataManager<OrganizationMgr>("Media");
            //OrganizationItem organization = organizationMgr.GetFromCache(2);
            //Assert.AreEqual(organization.Name, "CDC");

        }
    }
}
