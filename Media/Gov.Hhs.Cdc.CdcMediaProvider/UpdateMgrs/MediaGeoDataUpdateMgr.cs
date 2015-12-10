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

using System.Collections.Generic;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class MediaGeoDataUpdateMgr : BaseUpdateMgr<MediaGeoDataObject>
    {
        public override string ObjectName
        {
            get { return "GeoData"; }
        }

        private List<GeoDataCtl> InsertedObjects = new List<GeoDataCtl>();

        public MediaGeoDataUpdateMgr(IList<MediaGeoDataObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new GeoDataObjectValidator();
        }

        public MediaGeoDataUpdateMgr(MediaGeoDataObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName); ;
            Validator = new GeoDataObjectValidator();
        }

        internal List<GeoDataCtl> Create(IDataServicesObjectContext media)
        {
            List<GeoDataCtl> insertedGeoData = InsertedObjects.SelectMany(vi => Create(media, vi)).ToList();
            InsertedDataControls.AddRange(insertedGeoData);

            if (InsertedDataControls.Count == 0 && Items.Count > 0)
            {
                Save(media, new ValidationMessages());
            }

            if (insertedGeoData.Count == 0)
            {
                insertedGeoData.AddRange(InsertedObjects);
            }

            return insertedGeoData;
        }

        private static List<GeoDataCtl> Create(IDataServicesObjectContext media, GeoDataCtl vi)
        {
            GeoDataCtl InsertedGeoData = GeoDataCtl.Create(media, vi.NewBusinessObject);
            return new List<GeoDataCtl>() {  InsertedGeoData };
        }

        public override void Save(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (MediaGeoDataObject item in Items)
            {
                MediaGeoDataObject persistedObj =
                    GeoDataCtl.Get((MediaObjectContext)objectContext, forUpdate: true)
                    .Where(v => v.GeoNameId == item.GeoNameId && v.MediaId == item.MediaId).FirstOrDefault();

                if (persistedObj == null)
                {
                    GeoDataCtl geoDataCtl = GeoDataCtl.Create(objectContext, item);
                    geoDataCtl.AddToDb();
                    InsertedObjects.Add(geoDataCtl);
                }
                else
                {
                    GeoDataCtl.Update(objectContext, persistedObj, item, enforceConcurrency: true);
                }
            }
        }

        public override void Delete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (MediaGeoDataObject item in Items)
            {
                MediaGeoDataObject persistedobj =
                    GeoDataCtl.Get((MediaObjectContext)objectContext, forUpdate: true)
                    .Where(v => v.GeoNameId == item.GeoNameId && v.MediaId == item.MediaId).FirstOrDefault();

                if (persistedobj != null)
                {
                    GeoDataCtl.Delete(objectContext, persistedobj);
                }
            }
        }

        public override void UpdateIdsAfterInsert()
        {
            foreach (GeoDataCtl GeoDataCtl in InsertedObjects)
            {
                GeoDataCtl.UpdateIdsAfterInsert();
            }
        }

    }
}
