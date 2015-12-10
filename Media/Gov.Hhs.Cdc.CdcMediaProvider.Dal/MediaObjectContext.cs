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

using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal.Dal;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Reflection;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class MediaObjectContext : DataServicesObjectContext
    {
        public override string ApplicationCode { get { return "Media"; } }

        public MediaDbEntities MediaDbEntities
        {
            get
            {
                return (MediaDbEntities)GetEfObjectContext();
            }
        }

        public MediaObjectContext(string connectionString)
            : base(connectionString)
        {
        }

        public string AddMetaData(string connectionString)
        {
            string trimmedString = connectionString.Trim().TrimEnd(new char[] { ';' });
            return @"metadata=res://*/MediaDb.csdl|res://*/MediaDb.ssdl|res://*/MediaDb.msl;provider=System.Data.SqlClient;provider connection string=""" +
                trimmedString + @";App=EntityFramework""";
        }

        public override ObjectContext GetEfObjectContext()
        {
            if (efObjectContext == null)
            {
                efObjectContext = new MediaDbEntities(AddMetaData(ConnectionString));
            }
            return (MediaDbEntities)efObjectContext;
        }

        //Used to parse the cached business object attributes
        protected override List<Assembly> GetAssembliesWithCachedBusinessObjects()
        {
            return new List<Assembly>() { typeof(ValueSetObject).Assembly };
        }

        public override ICachedDataControl CreateNewCachedDataControl<T>()
        {
            if (typeof(T) == typeof(LanguageItem))
            {
                return new LanguageItemCtl();
            }

            if (typeof(T) == typeof(RelationshipTypeItem))
            {
                return new RelationshipTypeItemCtl();
            }

            if (typeof(T) == typeof(ValueSetObject))
            {
                return new ValueSetObjectCtl();
            }

            if (typeof(T) == typeof(MediaTypeItem))
            {
                return new MediaTypeItemCtl();
            }

            throw new ApplicationException("ObjectContext.CreateNewCachedDataControl could not find a DataCtl for class :" + typeof(T).ToString());
        }

    }
}
