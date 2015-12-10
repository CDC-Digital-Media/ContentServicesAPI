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
using System.Data.Objects;
using Gov.Hhs.Cdc.DataServices.Bo;

using System.Reflection;

namespace Gov.Hhs.Cdc.Storage.Model
{
    public class StorageObjectContext : DataServicesObjectContext
    {
        public override string ApplicationCode { get { return "Storage"; } }

        public StorageDbEntities StorageDbEntities
        {
            get
            {
                return (StorageDbEntities)GetEfObjectContext();
            }
        }

        public string AddMetaData(string connectionString)
        {
            string trimmedString = connectionString.Trim().TrimEnd(new char[] { ';' });
            return @"metadata=res://*/Model.StorageDb.csdl|res://*/Model.StorageDb.ssdl|res://*/Model.StorageDb.msl;provider=System.Data.SqlClient;provider connection string=""" + 
                trimmedString + @";App=EntityFramework""";
        }

        public override ObjectContext GetEfObjectContext()
        {

            return (StorageDbEntities)(efObjectContext ?? (efObjectContext = new StorageDbEntities(AddMetaData(ConnectionString))));
        }

        public StorageObjectContext(string connectionString)
            : base(connectionString)
        {
        }

        //Used to parse the cached business object attributes
        protected override List<Assembly> GetAssembliesWithCachedBusinessObjects()
        {
            return new List<Assembly>() { typeof(Provider.StorageSearchProvider).Assembly };
        }
    }
}
