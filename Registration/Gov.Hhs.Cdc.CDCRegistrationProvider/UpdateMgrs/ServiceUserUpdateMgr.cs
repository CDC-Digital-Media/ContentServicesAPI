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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class ServiceUserUpdateMgr : BaseUpdateMgr<ServiceUserObject>
    {
        private class CombinedServiceUserObject
        {
            public ServiceUserObject ServiceUserObject { get; set; }
            public ServiceUserCtl ServiceUserInsertObjectCtl { get; set; }
            public bool HasNewOrganization { get; set; }
            public OrganizationObject OrganizationObject { get; set; }
            public OrganizationUpdateMgr OrganizationUpdateMgr { get; set; }
        }

        List<CombinedServiceUserObject> theObjects;
        List<OrganizationUpdateMgr> organizationUpdateMgrs;


        public override string ObjectName { get { return "ServiceUser"; } }


        public ServiceUserUpdateMgr(IList<ServiceUserObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new ServiceUserObjectValidator();
            CreateChildUpdateMgrs(Items);
        }

        public ServiceUserUpdateMgr(ServiceUserObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);
            Validator = new ServiceUserObjectValidator();
            CreateChildUpdateMgrs(Items);
        }

        private void CreateChildUpdateMgrs(IList<ServiceUserObject> items)
        {
            theObjects =
                (from u in items
                 //where u.OrganizationId == 0 && u.Organization != null
                 let hasNewOrganization = u.OrganizationId == 0 && u.Organization != null
                 select new CombinedServiceUserObject()
                 {
                     ServiceUserObject = u,
                     HasNewOrganization = hasNewOrganization,
                     OrganizationObject = u.Organization,
                     OrganizationUpdateMgr = hasNewOrganization ? new OrganizationUpdateMgr(u.Organization) : null
                 }).ToList();

            organizationUpdateMgrs = (from u in theObjects
                                      where u.HasNewOrganization
                                      select u.OrganizationUpdateMgr).ToList();
        }

        public override void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, Items);
            foreach (OrganizationUpdateMgr organizationUpdateMgr in organizationUpdateMgrs)
            {
                organizationUpdateMgr.PreSaveValidate(ref validationMessages);
            }
        }

        public override void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, Items);
            foreach (OrganizationUpdateMgr organizationUpdateMgr in organizationUpdateMgrs)
            {
                organizationUpdateMgr.PreDeleteValidate(validationMessages);
            }
        }

        public override void ValidateSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateSave(media, validationMessages, Items);
            foreach (OrganizationUpdateMgr organizationUpdateMgr in organizationUpdateMgrs)
            {
                organizationUpdateMgr.ValidateSave(media, validationMessages);
            }

        }

        public override void ValidateDelete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(media, validationMessages, Items);
        }

        public override void Save(IDataServicesObjectContext registrationDb, ValidationMessages validationMessages)
        {
            //AccountObject
            foreach (CombinedServiceUserObject combinedServiceUserObject in theObjects)
            {
                ServiceUserObject persistedValueObject = ServiceUserCtl.Get((RegistrationObjectContext) registrationDb, returnPassword:true, forUpdate:true)
                    .Where(u => u.ServiceUserGuid == combinedServiceUserObject.ServiceUserObject.ServiceUserGuid).FirstOrDefault();

                if (persistedValueObject == null)
                {
                    combinedServiceUserObject.ServiceUserInsertObjectCtl = ServiceUserCtl.Create(registrationDb, combinedServiceUserObject.ServiceUserObject);
                    if (combinedServiceUserObject.HasNewOrganization)
                        combinedServiceUserObject.OrganizationUpdateMgr.Insert((IDataServicesObjectContext)registrationDb, combinedServiceUserObject.ServiceUserInsertObjectCtl);
                    else
                        combinedServiceUserObject.ServiceUserInsertObjectCtl.AddToDb();
                    InsertedDataControls.Add(combinedServiceUserObject.ServiceUserInsertObjectCtl);
                }
                else
                {
                    ServiceUserCtl.Update(registrationDb, persistedValueObject, combinedServiceUserObject.ServiceUserObject, enforceConcurrency: true);
                }
            }
        }

        public override void Delete(IDataServicesObjectContext registrationDb, ValidationMessages validationMessages)
        {
            foreach (ServiceUserObject item in Items)
            {
                //ServiceUserObject persistedValueObject = media.Get<ServiceUserObject>(forUpdate: true)
                ServiceUserObject persistedUserObject = ServiceUserCtl.Get((RegistrationObjectContext) registrationDb, returnPassword: true, forUpdate: true)
                    .Where(u => u.EmailAddress.ToLower() == item.EmailAddress.ToLower()).FirstOrDefault();

                if (persistedUserObject != null)
                    ServiceUserCtl.Delete(registrationDb, persistedUserObject);
            }

            //Do not delete organizations
        }

        public override void UpdateIdsAfterInsert()
        {
            foreach (CombinedServiceUserObject combinedServiceUserObject in theObjects)
            {
                combinedServiceUserObject.ServiceUserInsertObjectCtl.UpdateIdsAfterInsert();
                if (combinedServiceUserObject.OrganizationUpdateMgr != null && combinedServiceUserObject.OrganizationObject.Id != 0)
                {
                    combinedServiceUserObject.OrganizationUpdateMgr.UpdateIdsAfterInsert();
                    combinedServiceUserObject.ServiceUserObject.OrganizationId = combinedServiceUserObject.OrganizationObject.Id;
                }
            }
        }
    }
}
