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
using Gov.Hhs.Cdc.CdcMediaProvider.Dal.Dal;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    //To call the cache provider, you must implement CreateNewCachedDataControl in the ObjectContext
    /// <summary>
    /// The controller resopnsible for returning back a cached version of a business object.  If no
    /// cache provider is implemented, then it uses the normal data framework to return the object.
    /// 
    /// 
    /// 
    /// </summary>
    public static class MediaCacheController
    {
        public static LanguageItem GetLanguage(MediaObjectContext media, string code)
        {
            return media.DataServicesCacheProvider == null ?
                LanguageItemCtl.GetLanguageItems(media).Where(l => l.Code == code).FirstOrDefault() :
                media.DataServicesCacheProvider.Get<LanguageItem>(media, "Code".Is(code));
        }
        public static RelationshipTypeItem GetRelationshipType(MediaObjectContext media, string relationshipTypeName)
        {
            return media.DataServicesCacheProvider == null ?
                RelationshipTypeItemCtl.GetRelationshipTypeItem(media).Where(m => m.RelationshipTypeName == relationshipTypeName).FirstOrDefault() :
                media.DataServicesCacheProvider.Get<RelationshipTypeItem>(media, "RelationshipTypeName".Is(relationshipTypeName));
        }

        public static RelationshipTypeItem GetRelationshipTypeByShortType(MediaObjectContext media, string shortType)
        {
            return media.DataServicesCacheProvider == null ?
                RelationshipTypeItemCtl.GetRelationshipTypeItem(media).Where(m => m.ShortType == shortType).FirstOrDefault() :
                media.DataServicesCacheProvider.Get<RelationshipTypeItem>(media, "ByShortType", "ShortType".Is(shortType));
        }

        public static ValueSetObject GetValueSetItem(MediaObjectContext media, string name, string languageCode)
        {
            return media.DataServicesCacheProvider == null ?
                ValueSetObjectCtl.Get(media).Where(m => m.Name == name && m.LanguageCode == languageCode).FirstOrDefault() :
                media.DataServicesCacheProvider.Get<ValueSetObject>(media, "Name".Is(name), "LanguageCode".Is(languageCode));
        }

    }
}
