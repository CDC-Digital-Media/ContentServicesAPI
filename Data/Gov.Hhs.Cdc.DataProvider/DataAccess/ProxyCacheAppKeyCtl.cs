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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataProvider;
using Gov.Hhs.Cdc.DataProvider.Model;

namespace Gov.Hhs.Cdc.DataProvider
{
    class ProxyCacheAppKeyCtl : BaseCtl<ProxyCacheAppKeyObject, Gov.Hhs.Cdc.DataProvider.Model.ProxyCacheAppKey,
        ProxyCacheAppKeyCtl, DataObjectContext>
    {
        public override bool VersionMatches()
        {
            //No Rowversion
            return true;
        }

        public override string ToString()
        {
            return "ProxyCacheAppKey=" + PersistedBusinessObject.ProxyCacheAppKeyId;
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return (PersistedDbObject.Description == NewBusinessObject.Description
                && PersistedDbObject.Active == NewBusinessObject.IsActive
                );
        }

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.ProxyCacheAppKeyId = PersistedDbObject.ProxyCacheAppKeyID;
        }

        public override void Delete()
        {
            TheObjectContext.DataDbEntities.ProxyCacheAppKeys.DeleteObject(PersistedDbObject);
        }

        public override void AddToDb()
        {
            TheObjectContext.DataDbEntities.ProxyCacheAppKeys.AddObject(PersistedDbObject);
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new ProxyCacheAppKey();
            PersistedDbObject.ProxyCacheAppKeyID = NewBusinessObject.ProxyCacheAppKeyId;
            PersistedDbObject.CreatedDateTime = modifiedDateTime;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.Description = NewBusinessObject.Description;
            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.Active = NewBusinessObject.IsActive;
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<ProxyCacheAppKeyObject> Get(DataObjectContext context, bool forUpdate = false)
        {
            IQueryable<ProxyCacheAppKeyObject> proxyCacheAppKeyItems = from k in context.DataDbEntities.ProxyCacheAppKeys 
                                                           select new ProxyCacheAppKeyObject() 
                                                           {
                                                               ProxyCacheAppKeyId = k.ProxyCacheAppKeyID,
                                                               Description = k.Description,
                                                               IsActive = k.Active,
                                                               CreatedDateTime = k.CreatedDateTime,
                                                               ModifiedDateTime = k.ModifiedDateTime,
                                                               DbObject = forUpdate ? k : null
                                                           };
            return proxyCacheAppKeyItems;
        }
    }
}
