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
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class GeoDataCtl : BaseCtl<MediaGeoDataObject, GeoData, GeoDataCtl, MediaObjectContext>
    {
        public GeoDataCtl()
        {
        }

        public GeoDataCtl(MediaObjectContext media, MediaGeoDataObject newBusinessObject)
        {
            DataEntities = media;
            SetBusinessObjects(newBusinessObject, newBusinessObject);
        }

        public static IQueryable<MediaGeoDataObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
            IQueryable<MediaGeoDataObject> items = from s in media.MediaDbEntities.GeoNamesCombineds
                                                   join a in media.MediaDbEntities.GeoDatas on s.GeoNameID equals a.GeoNameID
                                                   select new MediaGeoDataObject()
                                                   {
                                                       GeoNameId = s.GeoNameID,
                                                       MediaId = a.MediaId,
                                                       Description = a.Description,

                                                       Name = s.ASCIIName,
                                                       CountryCode = s.CountryCode,
                                                       ParentId = s.ParentID == null ? 0 : (int)s.ParentID,
                                                       Latitude = s.Latitude,
                                                       Longitude = s.Longitude,
                                                       Timezone = s.Timezone,
                                                       Admin1Code = s.Admin1Code,

                                                       DbObject = forUpdate ? a : null
                                                   };
            return items;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new GeoData();

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.Description = NewBusinessObject.Description;
            PersistedDbObject.MediaId = NewBusinessObject.MediaId;
            PersistedDbObject.GeoNameID = NewBusinessObject.GeoNameId;

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
            //PersistedBusinessObject.StorageId = PersistedDbObject.StorageID;
        }

        public override void Delete()
        {
            TheObjectContext.MediaDbEntities.GeoDatas.DeleteObject(PersistedDbObject);
        }

        public override void AddToDb()
        {
            TheObjectContext.MediaDbEntities.GeoDatas.AddObject(PersistedDbObject);
        }

        public override object Get(bool forUpdate)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return PersistedBusinessObject.GetType().Name + ", MediaId=" + PersistedBusinessObject.MediaId.ToString();
        }
    }
}
