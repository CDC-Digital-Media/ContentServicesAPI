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
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public static class BusinessUnitTypeItemCtl
    {
        public static IQueryable<BusinessUnitTypeItem> Get(MediaObjectContext media)
        {
            IQueryable<BusinessUnitTypeItem> items = from s in media.MediaDbEntities.BusinessUnitTypes
                                                    select new BusinessUnitTypeItem()
                                                {
                                                    Description = s.Description,
                                                    DisplayOrdinal = s.DisplayOrdinal,
                                                    TypeCode = s.BusinessUnitTypeCode,
                                                    IsActive = s.Active == "Yes"
                                                };
            return items;
        }
    }
}
