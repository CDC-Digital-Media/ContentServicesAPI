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

using System.Collections.Generic;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class DomainUpdateMgr : BaseUpdateMgr<DomainObject>
    {

        public override string ObjectName { get { return "Domain"; } }
        List<DomainCtl> InsertedDomains = new List<DomainCtl>();

        public DomainUpdateMgr(IList<DomainObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new DomainObjectValidator();
            ValidationItems = Items;
        }

        public DomainUpdateMgr(DomainObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);
            Validator = new DomainObjectValidator();
            ValidationItems = Items;
        }

        public override void Save(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (DomainObject item in Items)
            {
                DomainObject persistedDomain = DomainCtl
                    .Get((RegistrationObjectContext)objectContext, forUpdate: true)
                    .Where(u => u.DomainName == item.DomainName).FirstOrDefault();

                if (persistedDomain == null)
                {
                    DomainCtl domainCtl = DomainCtl.Create(objectContext, item);
                    domainCtl.AddToDb();
                }
                else
                    DomainCtl.Update(objectContext, persistedDomain, item, enforceConcurrency: true);
            }
        }

        public override void Delete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (DomainObject domain in Items)
            {
                DomainObject persistedOrg = DomainCtl
                    .Get((RegistrationObjectContext)objectContext, forUpdate: true)
                    .Where(u => u.DomainName == domain.DomainName).FirstOrDefault();
                //TODO: Do we need to check to make sure there are no children before we delete?  Should this be part of validation?
                if (persistedOrg != null)
                    DomainCtl.Delete(objectContext, persistedOrg);
            }
        }

        public override void UpdateIdsAfterInsert()
        {
            foreach (DomainCtl dataCtl in InsertedDomains)
                dataCtl.UpdateIdsAfterInsert();
        }


    }
}
