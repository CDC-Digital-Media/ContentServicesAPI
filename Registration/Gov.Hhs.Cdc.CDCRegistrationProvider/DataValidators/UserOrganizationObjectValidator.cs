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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class UserOrganizationObjectValidator : IValidator<UserOrganizationObject, UserOrganizationObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<UserOrganizationObject> items)
        {
            foreach (UserOrganizationObject obj in (IList<UserOrganizationObject>)items)
            {
                RegExValidator v = new RegExValidator(validationMessages, obj.ValidationKey);
                if (obj.OrganizationId < 0)
                    validationMessages.AddError(obj.ValidationKey + "OrganizationId", "Organization Id is invalid");
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<UserOrganizationObject> items)
        {            
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<UserOrganizationObject> items)
        {
            foreach (UserOrganizationObject item in (IList<UserOrganizationObject>)items)
            {
                if (UserOrganizationCtl.Get((RegistrationObjectContext)objectContext)
                    .Where(u => u.OrganizationId == item.OrganizationId && u.UserGuid == item.UserGuid).Any())
                    validationMessages.AddError(item.ValidationKey, "The user already belongs to this Organization");
            }
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<UserOrganizationObject> items)
        {
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<UserOrganizationObject> items)
        {
        }

        public UserOrganizationObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, UserOrganizationObject theObject)
        {
            return theObject;
        }
    }
}
