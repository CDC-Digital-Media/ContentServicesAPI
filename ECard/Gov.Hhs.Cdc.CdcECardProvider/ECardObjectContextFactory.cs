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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServicesCacheProvider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataSource;
using Gov.Hhs.Cdc.CdcECardProvider;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.Connection;

namespace Gov.Hhs.Cdc.CdcECardProvider
{

    public class ECardObjectContextFactory : IObjectContextFactory
    {
        private const string ApplicationCode = "ECard";

        public IDataServicesObjectContext Create()
        {
            return new ECardObjectContext(ObjectContextFactoryUtility.GetConnectionStringFromName(this.ConnectionStringName));
        }

        public string ConnectionStringName
        {
            get
            {
                return Connection.CurrentConnection.Name;
            }
        }

    }
}
