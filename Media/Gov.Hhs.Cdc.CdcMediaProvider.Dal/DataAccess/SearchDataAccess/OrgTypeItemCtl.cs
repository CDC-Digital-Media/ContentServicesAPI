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

namespace Gov.Hhs.Cdc.DataSource.Media
{
    public class OrgTypeItemCtl
    {
        public static IQueryable<ListItem> GetDataList(MediaObjectContext media)
        {

            IQueryable<ListItem> orgTypes = from o in media.MediaDbEntities.OrganizationTypes
                                              where o.Active == "Yes"
                                              select new ListItem()
                                              {
                                                  EfSafeKeyType = (int)ListItem.KeyType.StringKey,
                                                  Key = o.OrganizationTypeCode,
                                                  Code = o.OrganizationTypeCode,
                                                  LongDisplayName = o.Description,
                                                  DisplayName = o.Description,
                                                  DisplayOrdinal = o.DisplayOrdinal
                                              };
            return orgTypes.AsQueryable();
        }

        public IQueryable Get(DataServicesObjectContext dataEntities)
        {
            MediaObjectContext media = (MediaObjectContext)dataEntities;
            IQueryable<OrgTypeItem> orgTypeItems = from o in media.MediaDbEntities.OrganizationTypes
                                                    select new OrgTypeItem()
                                                    {
                                                        OrganizationTypeCode = o.OrganizationTypeCode,
                                                        Description = o.Description,
                                                        DisplayOrdinal = o.DisplayOrdinal,
                                                        IsActive = o.Active == "Yes"
                                                    };
            return orgTypeItems;
        }

    }
}
