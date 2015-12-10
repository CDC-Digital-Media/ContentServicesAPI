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
using System.Reflection;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.DataProvider
{
    public class DataSearchProvider : BaseCsSearchProvider
    {
        public override string ApplicationCode { get { return "Data"; } }
        public override string Code { get { return "DataSearchProvider"; } }
        public override string Name { get { return "Content Services Data API Proxy Search Provider"; } }

        public List<Assembly> _businessObjectAssemblies = new List<Assembly>() { typeof(DataProviderSearchBusinessObjectPlaceHolder).Assembly };
        public override List<Assembly> BusinessObjectAssemblies
        {
            get { return _businessObjectAssemblies; }
        }

        public override ISearchDataManager GetSearchDataManager(string filterCode)
        {
            switch (SafeToLower(filterCode))
            {
                case "proxycache":
                    return new ProxyCacheSearchMgr();
                case "proxycacheappkey":
                    return new ProxyCacheAppKeySearchMgr();
            }
            throw new ApplicationException(
                string.Format("The data search manager '{0}' has not been defined in {1}.GetSearchDataManager()",
                    filterCode, this.GetType().Name));
        }


    }
}
