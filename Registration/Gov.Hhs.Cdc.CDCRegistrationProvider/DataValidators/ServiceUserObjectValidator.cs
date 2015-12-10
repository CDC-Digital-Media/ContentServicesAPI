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
    public class ServiceUserObjectValidator : IValidator<ServiceUserObject, ServiceUserObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<ServiceUserObject> items)
        {
            foreach (ServiceUserObject user in (IList<ServiceUserObject>)items)
            {
                RegExValidator v = new RegExValidator(validationMessages, user.ValidationKey);

                if (user.OrganizationId < 0)
                    validationMessages.AddError(user.ValidationKey, "OrganizationId is invalid");
                if (user.Organization == null && user.OrganizationId <= 0)
                    validationMessages.AddError(user.ValidationKey, "An Organization has not been selected");
                //v.IsValid(v.AlphaNumericSpacesPunctuation, user.FirstName, required: true, message: "First name is invalid");
                //v.IsValid(v.AlphaNumericSpaces, user.MiddleName, required: false, message: "Middle name is invalid");
                //v.IsValid(v.AlphaNumericSpacesPunctuation, user.LastName, required: true, message: "Last Name is invalid");

                //Validate the Email
                if (user.EmailAddress != null)
                    user.EmailAddress = user.EmailAddress.Trim();
                v.IsValid(v.Email, user.EmailAddress, required: true, message: "Email address is invalid");
                v.IsValid(v.Alphanumeric, user.Password, required: true, message: "Password is invalid");
                v.MeetsPasswordComplexityRequirements(user.Password, "Password does not meet complexity requirements");
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<ServiceUserObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ServiceUserObject> items)
        {
            foreach (ServiceUserObject user in (IList<ServiceUserObject>)items)
            {
                if (ServiceUserCtl.Get((RegistrationObjectContext)objectContext)
                    .Where(u => u.EmailAddress == user.EmailAddress).Any())
                    validationMessages.AddError(user.ValidationKey, "The email address already exists in the system");
            }
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ServiceUserObject> items)
        {
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ServiceUserObject> items)
        {
        }

        public ServiceUserObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, ServiceUserObject theObject)
        {
            return theObject;
        }

    }
}
