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
using System.Data.Objects;
using System.Reflection;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataSource.Dal.Cs;
using Gov.Hhs.Cdc.Connection;

namespace Gov.Hhs.Cdc.DataSource.Dal
{
    public class FrameworkObjectContext : DataServicesObjectContext
    {
        const string _applicationCode = "~Framework~";
        public override string ApplicationCode { get { return _applicationCode; } }
        public virtual ContentServicesEntities Entities{ get { return (ContentServicesEntities)GetEfObjectContext(); } }


        public FrameworkObjectContext()
            : base(ObjectContextFactoryUtility.GetConnectionStringFromName(Connection.CurrentConnection.Name))
        {
        }

        public FrameworkObjectContext(string connectionString)
            : base(connectionString)
        {
        }

        private static string AddMetaData(string connectionString)
        {
            string trimmedString = connectionString.Trim().TrimEnd(new char[] { ';' });
            return @"metadata=res://*/Dal.Cs.DataSourceDb.csdl|res://*/Dal.Cs.DataSourceDb.ssdl|res://*/Dal.Cs.DataSourceDb.msl;provider=System.Data.SqlClient;provider connection string=""" +
                trimmedString + @";App=EntityFramework""";
        }

        public override ObjectContext GetEfObjectContext()
        {
            return efObjectContext ?? (efObjectContext = new ContentServicesEntities(AddMetaData(ConnectionString)));
        }

        //Used to parse the cached business object attributes
        //protected override List<Assembly> GetAssembliesWithCachedBusinessObjects()
        //{
        //    return new List<Assembly>() { typeof(DsApplication).Assembly };
        //}

        public override ICachedDataControl CreateNewCachedDataControl<T>()
        {
            if (typeof(T) == typeof(FilterCriteriaDefinition))
            {
                return new FilterCriteriaDTOCtl();
            }
            throw new ApplicationException("ObjectContext.CreateNewCachedDataControl could not find a DataCtl for class :" + typeof(T).ToString());
        }
    }
}
