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

using System.Linq;
using System.Collections.Generic;

using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public static class LocationItemCtl
    {
        public static IQueryable<LocationItem> Get(MediaObjectContext media, int? geoId)
        {
            IQueryable<GeoNamesCombined> parentItem = media.MediaDbEntities.GeoNamesCombineds;

            parentItem = parentItem.Where(a => a.GeoNameID == geoId);

            return Get(media, parentItem);
        }

        private static IQueryable<LocationItem> Get(MediaObjectContext media, IQueryable<GeoNamesCombined> parentItem)
        {
            GeoNamesCombined parent = parentItem.FirstOrDefault();

            int? id;
            if (parent == null)
            {
                id = null;
            }
            else
            {
                id = parent.ParentID;
            }

            IQueryable<LocationItem> items = from s in media.MediaDbEntities.GeoNamesCombineds
                                             select new LocationItem()
                                             {
                                                 GeoNameId = s.GeoNameID,
                                                 Name = s.ASCIIName,
                                                 CountryCode = s.CountryCode,
                                                 Admin1Code = s.Admin1Code,
                                                 Admin2Code = s.Admin2Code,
                                                 ParentId = s.ParentID,
                                                 previousId = id,
                                                 Latitude = s.Latitude,
                                                 Longitude = s.Longitude,
                                                 Timezone = s.Timezone
                                             };
            return items;
        }

        public static IQueryable<LocationItem> GetGeo(MediaObjectContext media, int mediaId)
        {
            IQueryable<LocationItem> items = from g in media.MediaDbEntities.GeoDatas
                                             join s in media.MediaDbEntities.GeoNamesCombineds on g.GeoNameID equals s.GeoNameID
                                             where g.MediaId == mediaId
                                             select new LocationItem()
                                             {
                                                 GeoNameId = s.GeoNameID,
                                                 Name = s.ASCIIName,
                                                 CountryCode = s.CountryCode,
                                                 Admin1Code = s.Admin1Code,
                                                 Admin2Code = s.Admin2Code,
                                                 ParentId = s.ParentID,
                                                 Latitude = s.Latitude,
                                                 Longitude = s.Longitude,
                                                 Timezone = s.Timezone
                                             };
            return items;
        }

    }
}
