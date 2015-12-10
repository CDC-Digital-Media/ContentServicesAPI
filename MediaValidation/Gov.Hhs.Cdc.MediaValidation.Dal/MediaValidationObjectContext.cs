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
using Gov.Hhs.Cdc.MediaValidatonProvider;

namespace Gov.Hhs.Cdc.MediaValidation.Dal
{
    public class MediaValidationObjectContext : DataServicesObjectContext
    {
        public override string ApplicationCode { get { return "MediaValidation"; } }

        public MediaValidationDbEntities MediaValidationDbEntities
        {
            get
            {
                return (MediaValidationDbEntities)GetEfObjectContext();
            }
        }

        private static string AddMetaData(string connectionString)
        {
            string trimmedString = connectionString.Trim().TrimEnd(new char[] { ';' });
            return @"metadata=res://*/MediaValidationEdm.csdl|res://*/MediaValidationEdm.ssdl|res://*/MediaValidationEdm.msl;provider=System.Data.SqlClient;provider connection string=""" +
                trimmedString + @";App=EntityFramework""";
        }

        public override ObjectContext GetEfObjectContext()
        {
            if (efObjectContext == null)
            {
                efObjectContext = new MediaValidationDbEntities(AddMetaData(ConnectionString));
            }
            return (MediaValidationDbEntities)efObjectContext;
        }

        public MediaValidationObjectContext(string connectionString)
            : base(connectionString)
        {
        }

        //Used to parse the cached business object attributes
        protected override List<Assembly> GetAssembliesWithCachedBusinessObjects()
        {
            return new List<Assembly>() { typeof(MediaValidation).Assembly };
        }

        public override ICachedDataControl CreateNewCachedDataControl<T>()
        {
            throw new ApplicationException("ObjectContext.CreateNewCachedDataControl could not find a DataCtl for class :" + typeof(T).ToString());

        }
    }
}
