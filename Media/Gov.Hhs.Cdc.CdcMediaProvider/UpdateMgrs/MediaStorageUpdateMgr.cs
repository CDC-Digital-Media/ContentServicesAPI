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
    public class MediaStorageUpdateMgr : BaseUpdateMgr<StorageObject>
    {

        public override string ObjectName
        {
            get { return "MediaStorage"; }
        }

        private class CombinedMediaStorageObject
        {
            public StorageObject MediaStorage { get; set; }
            public List<MediaStorageCtl> InsertedStorage = new List<MediaStorageCtl>();
        }
        List<CombinedMediaStorageObject> theObjects;

        //
        private List<MediaStorageCtl> InsertedObjects = new List<MediaStorageCtl>();

        public MediaStorageUpdateMgr(IList<StorageObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new MediaStorageObjectValidator();
            CreateChildUpdateMgrs(Items);
        }

        public MediaStorageUpdateMgr(StorageObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName); ;
            Validator = new MediaStorageObjectValidator();
            CreateChildUpdateMgrs(Items);
        }

        private void CreateChildUpdateMgrs(IList<StorageObject> items)
        {
            theObjects = (from o in items
                          select new CombinedMediaStorageObject()
                          {
                              MediaStorage = o,
                          }).ToList();
        }

        internal List<MediaStorageCtl> Create(IDataServicesObjectContext media)
        {
            Save(media, new ValidationMessages());
            return InsertedObjects;
        }

        public override void Save(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (StorageObject item in Items)
            {
                StorageObject persistedObj =
                    MediaStorageCtl.Get((MediaObjectContext)objectContext, forUpdate: true)
                    .Where(v => v.StorageId == item.StorageId).FirstOrDefault();

                if (persistedObj == null)
                {
                    MediaStorageCtl mediaStorageCtl = MediaStorageCtl.Create(objectContext, item);
                    mediaStorageCtl.AddToDb();
                    InsertedObjects.Add(mediaStorageCtl);
                }
                else
                    MediaStorageCtl.Update(objectContext, persistedObj, item, enforceConcurrency: true);
            }
        }

        public override void Delete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (CombinedMediaStorageObject obj in theObjects)
            {
                StorageObject persistedValueSet =
                    MediaStorageCtl.Get((MediaObjectContext)objectContext, forUpdate: true)
                    .Where(v => v.StorageId == obj.MediaStorage.StorageId).FirstOrDefault();

                if (persistedValueSet != null)
                    MediaStorageCtl.Delete(objectContext, persistedValueSet);
            }
        }

        public override void UpdateIdsAfterInsert()
        {
            foreach (MediaStorageCtl mediaStorageObjectCtl in InsertedObjects)
            {
                mediaStorageObjectCtl.UpdateIdsAfterInsert();
            }
        }

    }
}
