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

using System.Data.Objects;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.Schedule
{
    public class ScheduleObjectContext : DataServicesObjectContext
    {
        public override string ApplicationCode { get { return "Schedule"; } }

        public ScheduleDbEntities ScheduleDbEntities
        {
            get
            {
                return GetEfObjectContext() as ScheduleDbEntities;
            }
        }


        public ScheduleObjectContext(string connectionString)
            : base(connectionString)
        {
        }

        private static string AddMetaData(string connectionString)
        {
            string trimmedString = connectionString.Trim().TrimEnd(new char[] { ';' });
            return @"metadata=res://*/ScheduleDb.csdl|res://*/ScheduleDb.ssdl|res://*/ScheduleDb.msl;provider=System.Data.SqlClient;provider connection string=""" +
                trimmedString + @";App=EntityFramework""";
        }

        public override ObjectContext GetEfObjectContext()
        {
            return (ScheduleDbEntities)(efObjectContext ?? (efObjectContext = new ScheduleDbEntities(AddMetaData(ConnectionString))));
        }
    }
}
