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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public static class ValueSetDataListCtl
    {
        internal enum KeyType {FullKey, Name };
        internal static IQueryable<ListItem> GetDataListByKey(MediaObjectContext media)
        {
            return GetDataListByKey(media, false);
        }

        internal static IQueryable<ListItem> GetDataListByKey(MediaObjectContext media, bool noLanguage)
        {

            IQueryable<ListItem> valueSetItems = from vs in media.MediaDbEntities.ValueSets
                                              where vs.Active == "Yes"
                                                 let key = noLanguage ? vs.ValueSetName :
                                                    vs.ValueSetName + "|" + vs.LanguageCode
                                              select new ListItem()
                                              {
                                                  EfSafeKeyType = (int) ListItem.KeyType.StringKey,
                                                  //Key = vs.ValueSetID.ToString() + "|" + vs.LanguageCode,
                                                  //Code = vs.ValueSetID.ToString() + "|" + vs.LanguageCode,
                                                  Value = key,
                                                  Code = key,
                                                  LongDisplayName = vs.Description,
                                                  DisplayName = vs.ValueSetName,
                                                  DisplayOrdinal = vs.DisplayOrdinal
                                              };
            return valueSetItems;
        }

        internal static IQueryable<ListItem> GetDataList(MediaObjectContext media)
        {

            IQueryable<ListItem> valueSetItems = from vs in media.MediaDbEntities.ValueSets
                                                 where vs.Active == "Yes"
                                                 select new ListItem()
                                                 {
                                                     EfSafeKeyType = (int)ListItem.KeyType.IntKey,
                                                     IntKey = vs.ValueSetID,
                                                     LongDisplayName = vs.Description,
                                                     DisplayName = vs.ValueSetName,
                                                     DisplayOrdinal = vs.DisplayOrdinal
                                                 };
            return valueSetItems;
        }



    }
}
