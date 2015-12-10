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

using Gov.Hhs.Cdc.DataServices.Bo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Reflection;
using Gov.Hhs.Cdc.DataProvider;
using Gov.Hhs.Cdc.DataProvider.Model;

namespace Gov.Hhs.Cdc.DataProvider
{
    class DataObjectContext : DataServicesObjectContext
    {
        public override string ApplicationCode { get { return "Data"; } }

        public DataDbEntities DataDbEntities
        {
            get
            {
                return (DataDbEntities)GetEfObjectContext();
            }
        }

        public DataObjectContext(string connectionString)
            : base(connectionString)
        {
        }

        private string AddMetaData(string connectionString)
        {
            string trimmedString = connectionString.Trim().TrimEnd(new char[] { ';' });
            return @"metadata=res://*/Model.DataDb.csdl|res://*/Model.DataDb.ssdl|res://*/Model.DataDb.msl;provider=System.Data.SqlClient;provider connection string=""" +
                trimmedString + @";App=EntityFramework""";
        }

        public override ObjectContext GetEfObjectContext()
        {
            return (DataDbEntities)(efObjectContext ?? (efObjectContext = new DataDbEntities(AddMetaData(ConnectionString))));
        }

        //Used to parse the cached business object attributes
        protected override List<Assembly> GetAssembliesWithCachedBusinessObjects()
        {
            return new List<Assembly>() { typeof(ProxyCacheObject).Assembly };
        }
    }
}
