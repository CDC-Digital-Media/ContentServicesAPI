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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class OrganizationDomainUpdateMgr : BaseUpdateMgr<OrganizationDomainObject>
    {
        private class CombinedUserOrgObject
        {
            public OrganizationDomainObject OrganizationDomain { get; set; }
            public List<OrganizationDomainCtl> InsertedOrgDomains = new List<OrganizationDomainCtl>();
            public DomainObject Domain { get; set; }
            public DomainUpdateMgr DomainUpdateMgr { get; set; }
        }

        public override string ObjectName { get { return "OrganizationDomain"; } }
        List<CombinedUserOrgObject> theObjects;

        public OrganizationDomainUpdateMgr(IList<OrganizationDomainObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new OrganizationDomainObjectValidator();
            CreateChildUpdateMgrs(Items);
        }

        public OrganizationDomainUpdateMgr(OrganizationDomainObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);
            Validator = new OrganizationDomainObjectValidator();
            CreateChildUpdateMgrs(Items);
        }

        private void CreateChildUpdateMgrs(IList<OrganizationDomainObject> items)
        {
            theObjects = (from o in items
                          select new CombinedUserOrgObject()
                          {
                              OrganizationDomain = o
                          }).ToList();
        }

        public override void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, Items);
        }

        public override void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, Items);
        }

        public override void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            Validator.ValidateSave(objectContext, validationMessages, Items);
        }

        public override void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(objectContext, validationMessages, Items);
        }

        public override void Save(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }

        private static void CreateNewOrgDomain(IDataServicesObjectContext objectContext, CombinedUserOrgObject combinedObject, OrganizationCtl org)
        {
            OrganizationDomainCtl orgDomain = OrganizationDomainCtl.Create(objectContext, combinedObject.OrganizationDomain);
            combinedObject.InsertedOrgDomains.Add(orgDomain);

            org.PersistedDbObject.OrganizationDomains.Add(orgDomain.PersistedDbObject);
       }

        public override void Delete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }

        public static void Delete(IDataServicesObjectContext objectContext, IEnumerable<OrganizationDomainObject> userOrgs)
        {
            foreach (OrganizationDomainObject persistedUserOrg in userOrgs)
            {
                OrganizationDomainCtl.Delete(objectContext, persistedUserOrg);
            }
        }

        public override void UpdateIdsAfterInsert()
        {
            foreach (CombinedUserOrgObject combinedObject in theObjects)
            {
                if (combinedObject.DomainUpdateMgr != null)
                    combinedObject.DomainUpdateMgr.UpdateIdsAfterInsert();
                foreach (OrganizationDomainCtl userOrg in combinedObject.InsertedOrgDomains)
                {
                    userOrg.UpdateIdsAfterInsert();
                }
            }
        }

        public void Insert(IDataServicesObjectContext objectContext, OrganizationCtl org)
        {
            foreach (CombinedUserOrgObject combinedObject in theObjects)
            {
                CreateNewOrgDomain(objectContext, combinedObject, org);
            }
        }

    }
}
