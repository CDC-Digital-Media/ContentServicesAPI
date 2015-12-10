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
using System.Linq;

using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class MediaStorageCtl : BaseCtl<StorageObject, MediaStorage, MediaStorageCtl, MediaObjectContext>
    {
        public MediaStorageCtl()
        {
        }

        public MediaStorageCtl(MediaObjectContext media, StorageObject newBusinessObject)
        {
            DataEntities = media;
            SetBusinessObjects(newBusinessObject, newBusinessObject);
        }

        public static IQueryable<StorageObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
            IQueryable<StorageObject> storage = from a in media.MediaDbEntities.MediaStorages
                                                //join ms in media.MediaDbEntities.MediaStorages on a.StorageID equals ms.StorageID
                                                select new StorageObject()
                                           {
                                               StorageId = a.StorageID,
                                               MediaId = a.MediaID,
                                               DbObject = forUpdate ? a : null
                                           };
            return storage;
        }

        public static StorageObject GetById(MediaObjectContext media, int storageId, bool forUpdate = false)
        {
            return Get(media, forUpdate).Where(m => m.StorageId == storageId).FirstOrDefault();
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new MediaStorage();

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.MediaID = NewBusinessObject.MediaId;
            PersistedDbObject.StorageID = NewBusinessObject.StorageId;
            PersistedDbObject.MediaStorageTypeCode = NewBusinessObject.Type;

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return false;
        }

        public override bool VersionMatches()
        {
            return true;
        }

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.StorageId = PersistedDbObject.StorageID;
        }

        public override void Delete()
        {
            TheObjectContext.MediaDbEntities.MediaStorages.DeleteObject(PersistedDbObject);
        }

        public override void AddToDb()
        {
            TheObjectContext.MediaDbEntities.MediaStorages.AddObject(PersistedDbObject);
        }

        public override object Get(bool forUpdate)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return PersistedBusinessObject.GetType().Name + ", StorageId=" + PersistedBusinessObject.StorageId.ToString();
        }
    }
}
