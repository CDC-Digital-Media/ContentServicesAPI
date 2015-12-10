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

namespace Gov.Hhs.Cdc.TransactionLogProvider
{
    public class TransactionEntryCtl : BaseCtl<TransactionEntryObject, TransactionEntry,
        TransactionEntryCtl, TransactionLogObjectContext>
    {

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.TransactionId = PersistedDbObject.TransactionId;
        }

        public override string ToString()
        {
            return PersistedBusinessObject.ToString();
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return false;
        }

        public override bool VersionMatches()
        {
            return true;
        }

        public override object Get(bool forUpdate)
        {
            return Get(DataEntities, forUpdate);
        }

        public static IQueryable<TransactionEntryObject> Get(TransactionLogObjectContext objectContext, bool forUpdate = false)
        {
            IQueryable<TransactionEntryObject> items = from s in objectContext.LogDbEntities.TransactionEntries
                                                       select new TransactionEntryObject()
                                                       {
                                                           TransactionId = s.TransactionId,
                                                           HttpMethod = s.HttpMethod,
                                                           Resource = s.Resource,
                                                           ResourceId = s.ResourceId,
                                                           ResourceAction = s.ResourceAction,
                                                           QueryString = s.QueryString,
                                                           ServicePath = s.ServicePath,
                                                           InputData = s.InputData,
                                                           OutputData = s.OutputData,
                                                           Messages = s.Messages,
                                                           DbObject = forUpdate ? s : null
                                                       };
            return items;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new TransactionEntry();
            PersistedDbObject.TransactionId = NewBusinessObject.TransactionId;
            PersistedDbObject.HttpMethod = NewBusinessObject.HttpMethod;
            PersistedDbObject.Resource = NewBusinessObject.Resource;
            PersistedDbObject.ResourceId = NewBusinessObject.ResourceId;
            PersistedDbObject.ResourceAction = NewBusinessObject.ResourceAction;
            PersistedDbObject.QueryString = NewBusinessObject.QueryString;
            PersistedDbObject.ServicePath = NewBusinessObject.ServicePath;
            PersistedDbObject.InputData = NewBusinessObject.InputData;

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {

            PersistedDbObject.OutputData = NewBusinessObject.OutputData;
            PersistedDbObject.Messages = NewBusinessObject.Messages;
        }

        public override void AddToDb()
        {
            DataEntities.LogDbEntities.TransactionEntries.AddObject(PersistedDbObject);
        }

        public override void Delete()
        {
            DataEntities.LogDbEntities.TransactionEntries.DeleteObject(PersistedDbObject);
        }

    }
}
