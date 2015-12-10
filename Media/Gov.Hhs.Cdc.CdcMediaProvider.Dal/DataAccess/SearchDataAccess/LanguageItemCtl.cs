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
using Gov.Hhs.Cdc.DataServicesCacheProvider;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class LanguageItemCtl : ICachedDataControl
    {
        public IQueryable Get(IDataServicesObjectContext dataEntities)
        {
            return GetLanguageItems((MediaObjectContext)dataEntities);
        }

        public static IQueryable<LanguageItem> GetLanguageItems(MediaObjectContext media)
        {
            IQueryable<LanguageItem> mediaItems =
                                              from v in media.MediaDbEntities.Languages
                                              select new LanguageItem()
                                              {
                                                  Code = v.LanguageCode,
                                                  ISOLanguageCode2 = v.ISOLanguageCode2,
                                                  ISOLanguageCode3 = v.ISOLanguageCode3,
                                                  Description = v.Description,
                                                  DisplayOrdinal = v.DisplayOrdinal,
                                                  IsActive = v.Active == "Yes"
                                              };
            return mediaItems;
        }

        public static IQueryable<LanguageItem> Get(MediaObjectContext media)
        {

            IQueryable<LanguageItem> languageItems = from v in media.MediaDbEntities.Languages
                                                  where v.Active == "Yes"
                                                  select new LanguageItem()
                                                  {
                                                      Code = v.LanguageCode,
                                                      ISOLanguageCode2 = v.ISOLanguageCode2,
                                                      ISOLanguageCode3 = v.ISOLanguageCode3,
                                                      Description = v.Description,
                                                      DisplayOrdinal = v.DisplayOrdinal,
                                                      IsActive = v.Active == "Yes"
                                                  };
            return languageItems;
        }
        
        public static IQueryable<LanguageItem> Get(MediaObjectContext media, bool getOnlyInUseItems)
        {
            IQueryable<LanguageItem> languageItems = LanguageItemCtl.Get(media);
            if (getOnlyInUseItems)
            {
                languageItems = from l in languageItems
                             join ml in media.MediaDbEntities.Medias.Select(m => m.LanguageCode).Distinct() on l.Code equals ml
                             select l;
            }
            return languageItems;
        }

        public object GetDictionary(IDataServicesObjectContext dataEntities)
        {
            throw new NotImplementedException();
        }
    }
}
