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
    public class OrganizationUpdateMgr : BaseUpdateMgr<OrganizationObject>
    {
        private class CombinedOrganizationObject
        {
            public OrganizationObject Organization { get; set; }
            public OrganizationCtl InsertedOrganization { get; set; }
            public OrganizationDomainUpdateMgr OrganizationDomainUpdateMgr { get; set; }
        }

        public override string ObjectName { get { return "Organization"; } }
        List<OrganizationCtl> InsertedOrganization = new List<OrganizationCtl>();

        List<CombinedOrganizationObject> CombinedItems;
        List<OrganizationDomainUpdateMgr> organizationDomainUpdateMgrs;

        public OrganizationUpdateMgr(IList<OrganizationObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new OrganizationObjectValidator();
            ValidationItems = Items;
            CreateChildUpdateMgrs(Items);
        }

        public OrganizationUpdateMgr(OrganizationObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);
            Validator = new OrganizationObjectValidator();
            ValidationItems = Items;
            CreateChildUpdateMgrs(Items);
        }

        private void CreateChildUpdateMgrs(IList<OrganizationObject> items)
        {
            CombinedItems = (from o in items
                             select new CombinedOrganizationObject()
                             {
                                 Organization = o,
                                 OrganizationDomainUpdateMgr = o.OrganizationDomains == null ? null : new OrganizationDomainUpdateMgr((IList<OrganizationDomainObject>)o.OrganizationDomains)
                             }).ToList();
            organizationDomainUpdateMgrs = CombinedItems.Select(u => u.OrganizationDomainUpdateMgr).ToList();

        }

        public override void Save(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (OrganizationObject item in Items)
            {
                OrganizationObject persistedOrg = OrganizationCtl
                    .Get((RegistrationObjectContext)objectContext, forUpdate: true)
                    .Where(u => u.Id == item.Id).FirstOrDefault();

                if (persistedOrg == null)
                {
                    OrganizationCtl orgCtl = OrganizationCtl.Create(objectContext, item);
                    orgCtl.AddToDb();
                }
                else
                    OrganizationCtl.Update(objectContext, persistedOrg, item, enforceConcurrency: true);
            }
        }

        public override void Delete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (OrganizationObject org in Items)
            {
                OrganizationObject persistedOrg = OrganizationCtl
                    .Get((RegistrationObjectContext)objectContext, forUpdate: true)
                    .Where(u => u.Id == org.Id).FirstOrDefault();
                if (persistedOrg != null)
                {
                    foreach (var dom in persistedOrg.OrganizationDomains)
                    {
                        OrganizationDomainCtl.Delete(objectContext, dom);
                    }
                    OrganizationCtl.Delete(objectContext, persistedOrg);
                }
            }
        }

        public override void UpdateIdsAfterInsert()
        {
            foreach (OrganizationCtl dataCtl in InsertedOrganization)
                dataCtl.UpdateIdsAfterInsert();
        }

        #region CallsUsedByUserOrganizationUpdateManager
        public override void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, Items);
            foreach (OrganizationDomainUpdateMgr organizationDomainUpdateMgr in organizationDomainUpdateMgrs)
            {
                if (organizationDomainUpdateMgr != null)
                {
                    organizationDomainUpdateMgr.PreSaveValidate(ref validationMessages);
                }
            }

        }

        public override void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, Items);
            foreach (OrganizationDomainUpdateMgr organizationDomainUpdateMgr in organizationDomainUpdateMgrs)
            {
                if (organizationDomainUpdateMgr != null)
                {
                    organizationDomainUpdateMgr.PreDeleteValidate(validationMessages);
                }
            }
        }

        public override void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            Validator.ValidateSave(objectContext, validationMessages, Items);
            foreach (OrganizationDomainUpdateMgr organizationDomainUpdateMgr in organizationDomainUpdateMgrs)
            {
                if (organizationDomainUpdateMgr != null)
                {
                    organizationDomainUpdateMgr.ValidateSave(objectContext, validationMessages);
                }
            }
        }

        public override void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(objectContext, validationMessages, CombinedItems.Select(c => c.Organization).ToList());
            foreach (OrganizationDomainUpdateMgr organizationDomainUpdateMgr in organizationDomainUpdateMgrs)
            {
                if (organizationDomainUpdateMgr != null)
                    organizationDomainUpdateMgr.ValidateDelete(objectContext, validationMessages);
            }
        }


        public List<OrganizationCtl> Insert(IDataServicesObjectContext objectContext, ServiceUserCtl user)
        {
            foreach (CombinedOrganizationObject itm in CombinedItems)
            {
                itm.InsertedOrganization = OrganizationCtl.Create(objectContext, itm.Organization);
                if (itm.OrganizationDomainUpdateMgr != null)
                    itm.OrganizationDomainUpdateMgr.Insert(objectContext, itm.InsertedOrganization);
            }
            return CombinedItems.Select(c => c.InsertedOrganization).ToList();
        }
        public List<OrganizationCtl> Insert(IDataServicesObjectContext objectContext, UserCtl user)
        {
            foreach (CombinedOrganizationObject itm in CombinedItems)
            {
                itm.InsertedOrganization = OrganizationCtl.Create(objectContext, itm.Organization);
                if (itm.OrganizationDomainUpdateMgr != null)
                    itm.OrganizationDomainUpdateMgr.Insert(objectContext, itm.InsertedOrganization);

            }
            return CombinedItems.Select(c => c.InsertedOrganization).ToList();
        }

        #endregion
    }
}
