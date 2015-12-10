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
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServicesCacheProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class MediaTypeItemCtl : ICachedDataControl
    {

        public IQueryable Get(IDataServicesObjectContext dataEntities)
        {
            return GetMediaTypeItem((MediaObjectContext)dataEntities);
        }

        public static IQueryable<MediaTypeItem> GetMediaTypeItem(MediaObjectContext media)
        {
            IQueryable<MediaTypeItem> mediaTypeItems = from m in media.MediaDbEntities.MediaTypes
                                                       where m.DisplayOrdinal > 0 // where m.Active == "Yes" && m.Display == "Yes" //&& m.DisplayOrdinal > 0
                                                       select new MediaTypeItem()
                                                       {
                                                           MediaTypeCode = m.MediaTypeCode,
                                                           Description = m.Description,
                                                           DisplayOrdinal = m.DisplayOrdinal,
                                                           Display = m.Display == "Yes",
                                                           IsActive = m.Active == "Yes",
                                                           Preferences = new PreferencesSet()
                                                           {
                                                               MediaTypeCode = m.MediaTypeCode,
                                                               SerializedPreferencesFor_D_MediaType = m.EmbedParameters
                                                           },
                                                           //TODO:  MOVE TO DB OR AT LEAST CACHE
                                                           MediaCount = (media.MediaDbEntities.Medias.Count(item => item.MediaType.MediaTypeCode == m.MediaTypeCode && item.MediaStatusCode == "Published"))
                                                       };
            return mediaTypeItems;
        }

        public IQueryable<ListItem> GetDataList(MediaObjectContext media)
        {

            IQueryable<ListItem> mediaItems = from m in media.MediaDbEntities.MediaTypes
                                              where m.Active == "Yes" && m.Display == "Yes"
                                              select new ListItem()
                                              {
                                                  EfSafeKeyType = (int)ListItem.KeyType.StringKey,
                                                  Value = m.MediaTypeCode,
                                                  Code = m.MediaTypeCode,
                                                  LongDisplayName = m.Description,
                                                  DisplayName = m.Description,
                                                  DisplayOrdinal = m.DisplayOrdinal
                                              };
            return mediaItems;
        }

        public static IQueryable<MediaTypeItem> Get(MediaObjectContext media)
        {
            IQueryable<MediaTypeItem> mediaTypeItems = from m in media.MediaDbEntities.MediaTypes
                                                       select new MediaTypeItem()
                                                       {
                                                           MediaTypeCode = m.MediaTypeCode,
                                                           Description = m.Description,
                                                           DisplayOrdinal = m.DisplayOrdinal,
                                                           Display = m.Display == "Yes",
                                                           IsActive = m.Active == "Yes"                                                           
                                                       };
            return mediaTypeItems;
        }


        //public object GetDictionary(IDataServicesObjectContext dataEntities)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
