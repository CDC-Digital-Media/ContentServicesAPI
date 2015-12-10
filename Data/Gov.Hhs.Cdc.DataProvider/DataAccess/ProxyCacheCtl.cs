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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataProvider;
using Gov.Hhs.Cdc.DataProvider.Model;

namespace Gov.Hhs.Cdc.DataProvider
{
    class ProxyCacheCtl : BaseCtl<ProxyCacheObject, Gov.Hhs.Cdc.DataProvider.Model.ProxyCache,
        ProxyCacheCtl, DataObjectContext>
    {
        public override bool VersionMatches()
        {
            //No Rowversion
            return true;
        }

        public override string ToString()
        {
            return "ProxyCache=" + PersistedBusinessObject.Url;
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return (PersistedDbObject.Data == NewBusinessObject.Data
                && PersistedDbObject.ExpirationInterval == NewBusinessObject.ExpirationInterval.ToString()
                && PersistedDbObject.ExpirationDateTime.Equals(NewBusinessObject.ExpirationDateTime)
                && PersistedDbObject.NeedsRefresh == NewBusinessObject.NeedsRefresh
                && PersistedDbObject.ProxyCacheURL == NewBusinessObject.Url
                && PersistedDbObject.DataSetID == NewBusinessObject.DatasetId
                && PersistedDbObject.DataFailures == NewBusinessObject.Failures
                );
        }

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.Id = PersistedDbObject.ProxyCacheID;
        }

        public override void Delete()
        {
            TheObjectContext.DataDbEntities.ProxyCaches.DeleteObject(PersistedDbObject);
        }

        public override void AddToDb()
        {
            TheObjectContext.DataDbEntities.ProxyCaches.AddObject(PersistedDbObject);
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new ProxyCache();
            PersistedDbObject.ProxyCacheURL = NewBusinessObject.Url;
            PersistedDbObject.DataSetID = NewBusinessObject.DatasetId;
            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.DataFailures = 0;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.Data = NewBusinessObject.Data;
            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ExpirationInterval = NewBusinessObject.ExpirationInterval;
            PersistedDbObject.ExpirationDateTime = NewBusinessObject.ExpirationDateTime;
            PersistedDbObject.NeedsRefresh = NewBusinessObject.NeedsRefresh;
            PersistedDbObject.ProxyCacheURL = NewBusinessObject.Url;
            PersistedDbObject.DataSetID = NewBusinessObject.DatasetId;
            PersistedDbObject.DataFailures = NewBusinessObject.Failures;
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<ProxyCacheObject> Get(DataObjectContext context, bool forUpdate = false)
        {
            IQueryable<ProxyCacheObject> proxyCacheItems = from p in context.DataDbEntities.ProxyCaches 
                                                           select new ProxyCacheObject() 
                                                           {
                                                               Id = p.ProxyCacheID,
                                                               Url = p.ProxyCacheURL,
                                                               DatasetId = p.DataSetID,
                                                               Data = p.Data,
                                                               CreatedDateTime = p.CreatedDateTime,
                                                               ModifiedDateTime = p.ModifiedDateTime,
                                                               ExpirationDateTime = p.ExpirationDateTime,
                                                               ExpirationInterval = p.ExpirationInterval,
                                                               NeedsRefresh = p.NeedsRefresh,
                                                               Failures = p.DataFailures ?? 0,
                                                               DbObject = forUpdate ? p : null
                                                           };
            return proxyCacheItems;
        }
    }
}
