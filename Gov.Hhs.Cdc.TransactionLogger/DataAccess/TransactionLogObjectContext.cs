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
using System.Data.Objects;
using System.Reflection;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.TransactionLogProvider
{
    public class TransactionLogObjectContext : DataServicesObjectContext
    {

        public override string ApplicationCode { get { return "Media"; } }


        public TransactionLogDbEntities LogDbEntities
        {
            get
            {
                return (TransactionLogDbEntities)GetEfObjectContext();
            }
        }

        public TransactionLogObjectContext(string connectionString)
            : base(connectionString)
        {
        }

        public string AddMetaData(string connectionString)
        {
            string trimmedString = connectionString.Trim().TrimEnd(new char[] { ';' });

            return @"metadata=res://*/TransactionLogDb.csdl|res://*/TransactionLogDb.ssdl|res://*/TransactionLogDb.msl;provider=System.Data.SqlClient;provider connection string=""" +
                trimmedString + @";App=EntityFramework""";

        }

        public override ObjectContext GetEfObjectContext()
        {
            return efObjectContext ?? (efObjectContext = new TransactionLogDbEntities(AddMetaData(ConnectionString)));
        }

        //Used to parse the cached business object attributes
        protected override List<Assembly> GetAssembliesWithCachedBusinessObjects()
        {
            return new List<Assembly>() { typeof(TransactionEntryObject).Assembly };
        }
    }
}
