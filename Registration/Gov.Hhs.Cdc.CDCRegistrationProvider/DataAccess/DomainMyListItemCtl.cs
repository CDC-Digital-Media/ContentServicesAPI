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
using Gov.Hhs.Cdc.DataServicesCacheProvider;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class DomainSyndicationListItemCtl : ICachedDataControl
    {

        public IQueryable Get(IDataServicesObjectContext dataEntities)
        {
            return GetDomainSyndicationList((RegistrationObjectContext)dataEntities);
        }

        public static IQueryable<DomainSyndicationListObject> GetDomainSyndicationList(RegistrationObjectContext registrationDb)
        {
            IQueryable<DomainSyndicationListObject> orgItems = from l in registrationDb.RegistrationDbEntities.SyndicationLists
                                                          select new DomainSyndicationListObject()
                                                          {
                                                              DomainName = l.DomainName,
                                                              SyndicationListGuid = l.MyListGuid,
                                                              SyndicationListName = l.ListName,
                                                              IsActive = l.Active == "Yes"
                                                          };
            return orgItems;
        }

        public object GetDictionary(IDataServicesObjectContext dataEntities)
        {
            throw new NotImplementedException();
        }
    }
}
