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

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataServices;
//using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class StorageUpdateMgr : BaseUpdateMgr<StorageObject>
    {
        private class CombinedStorageObject
        {
            public StorageObject Storage { get; set; }
            public List<StorageCtl> InsertedStorage = new List<StorageCtl>();
            public MediaStorageUpdateMgr MediaStorageUpdateMgr { get; set; }
        }
        List<CombinedStorageObject> theObjects;
        List<MediaStorageUpdateMgr> mediaStorageUpdateMgrs;

        public override string ObjectName
        {
            get { return "Storage"; }
        }

        private List<StorageCtl> InsertedObjects = new List<StorageCtl>();

        public StorageUpdateMgr(IList<StorageObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new StorageObjectValidator();
            CreateChildUpdateMgrs(Items);
        }

        public StorageUpdateMgr(StorageObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName); ;
            Validator = new StorageObjectValidator();
            CreateChildUpdateMgrs(Items);
        }

        private void CreateChildUpdateMgrs(IList<StorageObject> items)
        {
            theObjects = (from o in items
                          select new CombinedStorageObject()
                          {
                              Storage = o,
                              MediaStorageUpdateMgr = o.MediaId == 0 && o.StorageId == 0
                              ? null
                              : new MediaStorageUpdateMgr(new StorageObject() { MediaId = o.MediaId, StorageId = o.StorageId, Type = o.Type })
                          }).ToList();
            mediaStorageUpdateMgrs = theObjects.Select(u => u.MediaStorageUpdateMgr).ToList();
        }

        public override void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, Items);
            foreach (MediaStorageUpdateMgr mgr in mediaStorageUpdateMgrs)
            {
                mgr.PreSaveValidate(ref validationMessages);
            }
        }

        public override void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, Items);
            foreach (MediaStorageUpdateMgr mgr in mediaStorageUpdateMgrs)
            {
                if (mgr != null)
                    mgr.PreDeleteValidate(validationMessages);
            }
        }

        public override void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            Validator.ValidateSave(objectContext, validationMessages, Items);
            foreach (MediaStorageUpdateMgr mgr in mediaStorageUpdateMgrs)
            {
                if (mgr != null)
                    mgr.ValidateSave(objectContext, validationMessages);
            }
        }

        public override void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(objectContext, validationMessages, Items);
            foreach (MediaStorageUpdateMgr mgr in mediaStorageUpdateMgrs)
            {
                if (mgr != null)
                    mgr.ValidateDelete(objectContext, validationMessages);
            }
        }

        public override void Save(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (CombinedStorageObject obj in theObjects)
            {
                StorageObject persistedObj = null;

                if (obj.Storage.Type.ToLower() == "thumbnail")
                {
                    persistedObj = StorageCtl.GetThumbnailStorage((MediaObjectContext)objectContext, obj.Storage.MediaId, forUpdate: true)
                    .FirstOrDefault();
                }
                else
                {
                    persistedObj = StorageCtl.GetNonThumbnailStorage((MediaObjectContext)objectContext, obj.Storage.MediaId,
                    obj.Storage.Name, forUpdate: true)
                    .FirstOrDefault();
                }
                
                if (persistedObj == null)
                {
                    StorageCtl storageCtl = StorageCtl.Create(objectContext, obj.Storage);
                    storageCtl.AddToDb();
                    InsertedObjects.Add(storageCtl);
                    //InsertedDataControls

                    if (obj.MediaStorageUpdateMgr != null)
                    {
                        List<MediaStorageCtl> newMediaStorage = obj.MediaStorageUpdateMgr.Create(objectContext);
                        storageCtl.AddMediaStorage(newMediaStorage);
                    }
                }
                else
                {
                    StorageCtl.Update(objectContext, persistedObj, obj.Storage, enforceConcurrency: true);
                    obj.Storage.StorageId = persistedObj.StorageId;

                    //if (obj.MediaStorageUpdateMgr != null)
                    //{
                    //    obj.MediaStorageUpdateMgr.Save(objectContext, validationMessages);
                    //}
                }
            }
        }

        //private static void CreateNewOrgDomain(IDataServicesObjectContext objectContext, CombinedStorageObject combinedObject, StorageCtl storage)
        //{
        //    StorageCtl orgDomain = StorageCtl.Create(objectContext, combinedObject.Storage);
        //    combinedObject.InsertedStorage.Add(orgDomain);

        //    storage.PersistedDbObject.MediaStorages.Add(orgDomain.PersistedDbObject);
        //}

        public override void Delete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (CombinedStorageObject obj in theObjects)
            {
                StorageObject persistedValueSet =
                    StorageCtl.Get((MediaObjectContext)objectContext, forUpdate: true)
                    .Where(v => v.StorageId == obj.Storage.StorageId).FirstOrDefault();

                if (persistedValueSet != null)
                {
                    if (obj.MediaStorageUpdateMgr != null)
                        obj.MediaStorageUpdateMgr.Delete(objectContext, validationMessages);

                    StorageCtl.Delete(objectContext, persistedValueSet);
                }
            }
        }

        public override void UpdateIdsAfterInsert()
        {
            foreach (StorageCtl storageCtl in InsertedObjects)
            {
                storageCtl.UpdateIdsAfterInsert();
            }
        }

        //public override void UpdateIdsAfterInsert()
        //{
        //    foreach (CombinedStorageObject combinedObject in theObjects)
        //    {
        //        if (combinedObject.MediaStorageUpdateMgr != null)
        //            combinedObject.MediaStorageUpdateMgr.UpdateIdsAfterInsert();
        //        foreach (StorageCtl storage in combinedObject.InsertedStorage)
        //        {
        //            storage.UpdateIdsAfterInsert();
        //        }
        //    }
        //}

    }
}
