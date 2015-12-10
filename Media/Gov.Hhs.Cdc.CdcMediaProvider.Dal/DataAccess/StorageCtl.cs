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

using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{

    public class StorageCtl : BaseCtl<StorageObject, Storage, StorageCtl, MediaObjectContext>
    {
        public const string ThumbnailStorageType = "thumbnail";

        public StorageCtl()
        {
        }

        public StorageCtl(MediaObjectContext media, StorageObject newBusinessObject)
        {
            DataEntities = media;
            SetBusinessObjects(newBusinessObject, newBusinessObject);
        }

        public static IQueryable<StorageObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
            IQueryable<StorageObject> storage = from a in media.MediaDbEntities.Storages
                                                join ms in media.MediaDbEntities.MediaStorages on a.StorageID equals ms.StorageID
                                                select new StorageObject()
                                                {
                                                    StorageId = a.StorageID,
                                                    Name = a.Name,
                                                    ByteStream = a.ByteStream,
                                                    FileExtension = a.FileExtension,
                                                    Width = a.Width,
                                                    Height = a.Height,
                                                    Type = ms.MediaStorageTypeCode,
                                                    DbObject = forUpdate ? a : null
                                                };
            return storage;
        }

        public static IQueryable<StorageObject> GetThumbnailStorage(MediaObjectContext media, int mediaId, bool forUpdate = false)
        {
            return from a in media.MediaDbEntities.Storages
                   join ms in media.MediaDbEntities.MediaStorages on a.StorageID equals ms.StorageID
                   where ms.MediaID == mediaId
                   && ms.MediaStorageTypeCode.ToLower() == ThumbnailStorageType
                   select new StorageObject()
                   {
                       StorageId = a.StorageID,
                       MediaId = ms.MediaID,
                       Name = a.Name,
                       ByteStream = a.ByteStream,
                       FileExtension = a.FileExtension,
                       Width = a.Width,
                       Height = a.Height,
                       Type = ms.MediaStorageTypeCode,
                       DbObject = forUpdate ? a : null
                   };
        }

        public static IQueryable<StorageObject> GetNonThumbnailStorage(MediaObjectContext media, int mediaId, string name, bool forUpdate = false)
        {
            IQueryable<StorageObject> storage = from a in media.MediaDbEntities.Storages
                                                join ms in media.MediaDbEntities.MediaStorages on a.StorageID equals ms.StorageID
                                                where ms.MediaID == mediaId
                                                && a.Name.ToLower() == name.ToLower()
                                                && ms.MediaStorageTypeCode.ToLower() != ThumbnailStorageType
                                                select new StorageObject()
                                                {
                                                    StorageId = a.StorageID,
                                                    MediaId = ms.MediaID,
                                                    Name = a.Name,
                                                    ByteStream = a.ByteStream,
                                                    FileExtension = a.FileExtension,
                                                    Width = a.Width,
                                                    Height = a.Height,
                                                    Type = ms.MediaStorageTypeCode,
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
            PersistedDbObject = new Storage();

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.Name = NewBusinessObject.Name;
            PersistedDbObject.ByteStream = NewBusinessObject.ByteStream;
            PersistedDbObject.FileExtension = NewBusinessObject.FileExtension;
            PersistedDbObject.Height = NewBusinessObject.Height;
            PersistedDbObject.Width = NewBusinessObject.Width;

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
            TheObjectContext.MediaDbEntities.Storages.DeleteObject(PersistedDbObject);
        }

        public override void AddToDb()
        {
            TheObjectContext.MediaDbEntities.Storages.AddObject(PersistedDbObject);
        }

        public void AddMediaStorage(List<MediaStorageCtl> ctls)
        {
            foreach (var ctl in ctls)
            {
                if (ctl.PersistedDbObject.StorageID == 0 || ctl.PersistedDbObject.StorageID == PersistedDbObject.StorageID)
                {
                    PersistedDbObject.MediaStorages.Add(ctl.PersistedDbObject);
                }
            }
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
