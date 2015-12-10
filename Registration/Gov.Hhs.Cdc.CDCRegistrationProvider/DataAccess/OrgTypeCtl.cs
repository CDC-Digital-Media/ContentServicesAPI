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
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class OrgTypeCtl
    {
        public static IQueryable<ListItem> GetDataList(RegistrationObjectContext registrationDb)
        {

            IQueryable<ListItem> orgTypes = from o in registrationDb.RegistrationDbEntities.OrganizationTypes
                                            //where o.Active == "Yes"
                                            select new ListItem()
                                            {
                                                EfSafeKeyType = (int)ListItem.KeyType.StringKey,
                                                Value = o.OrganizationTypeCode,
                                                Code = o.OrganizationTypeCode,
                                                LongDisplayName = o.Description,
                                                DisplayName = o.Description,
                                                DisplayOrdinal = o.DisplayOrdinal
                                            };
            return orgTypes.AsQueryable();
        }

        public IQueryable Get(IDataServicesObjectContext dataEntities)
        {
            RegistrationObjectContext registrationDb = (RegistrationObjectContext)dataEntities;
            IQueryable<OrgTypeObject> orgTypeItems = from o in registrationDb.RegistrationDbEntities.OrganizationTypes
                                                   select new OrgTypeObject()
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
