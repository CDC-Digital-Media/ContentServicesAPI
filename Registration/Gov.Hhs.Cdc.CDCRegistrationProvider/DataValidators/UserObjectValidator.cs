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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.RegistrationProvider;



namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class UserObjectValidator : IValidator<UserObject, UserObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<UserObject> items)
        {
            foreach (UserObject obj in (IList<UserObject>)items)
            {
                RegExValidator v = new RegExValidator(validationMessages, obj.ValidationKey);

                if (obj.Organizations == null || obj.Organizations.Count() == 0)
                    validationMessages.AddError(obj.ValidationKey, "Organization is required");

                //Validate the Email
                if (obj.EmailAddress != null)
                {
                    obj.EmailAddress = obj.EmailAddress.Trim();
                }
                v.IsValid(v.Email, obj.EmailAddress, required: true, message: "Email address is invalid");

                if (obj.Password != obj.PasswordRepeat)
                {
                    validationMessages.AddError(obj.ValidationKey, "Passwords must match");
                }
                if (!string.IsNullOrEmpty(obj.Password) && string.IsNullOrEmpty(obj.UserGuid.ToString()))
                {
                    ValidatePassword(obj, v);
                }

            }
        }

        private static void ValidatePassword(UserObject obj, RegExValidator v)
        {
            v.MeetsPasswordComplexityRequirements(obj.Password, "Password does not meet security requirements");
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<UserObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<UserObject> items)
        {
            foreach (UserObject item in (IList<UserObject>)items)
            {
                if (item.UserGuid == default(Guid)) //"00000000-0000-0000-0000-000000000000"
                {
                    CheckForDuplicateEmail(objectContext, validationMessages, item);
                }
            }
        }

        private static void CheckForDuplicateEmail(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, UserObject item)
        {
            if (UserCtl.GetUsers((RegistrationObjectContext)objectContext)
                .Where(u => u.EmailAddress == item.EmailAddress).Any())
            {
                validationMessages.AddError(item.ValidationKey, "The email address already exists in the system");
            }
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<UserObject> items)
        {
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<UserObject> items)
        {
        }

        public UserObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, UserObject theObject)
        {
            return theObject;
        }

    }
}
