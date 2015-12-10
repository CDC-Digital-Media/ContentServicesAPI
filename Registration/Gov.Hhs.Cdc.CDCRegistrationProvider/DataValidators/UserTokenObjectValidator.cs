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


//using System.Text;
//using System.Data.Objects;
//using Gov.Hhs.Cdc.DataServices;
//using System.ComponentModel.DataAnnotations;
//using System.Text.RegularExpressions;
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Bo;



namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class UserTokenObjectValidator : IValidator<UserTokenObject, UserTokenObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<UserTokenObject> items)
        {
            foreach (UserTokenObject obj in (IList<UserTokenObject>)items)
            {
                RegExValidator v = new RegExValidator(validationMessages, obj.ValidationKey);
                               
                v.IsValid(v.Alphanumeric, obj.UserToken, required: true, message: "UserToken is invalid");
                //v.IsValid(v.AlphaNumeric, obj.TempPassword, required: false, message: "TempToken is invalid");

                //if (string.IsNullOrEmpty(obj.UserTokenSecret))
                //    validationMessages.AddError(obj.ValidationKey, "UserTokenSecret is invalid");
                //if (string.IsNullOrEmpty(obj.UserTokenSecretSalt))
                //    validationMessages.AddError(obj.ValidationKey, "UserTokenSecretSalt is invalid");

                if (!(obj.ExpirationUtcSeconds > 0))
                    validationMessages.AddError(obj.ValidationKey, "ExpirationUtcSeconds is invalid");
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<UserTokenObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<UserTokenObject> items)
        {
            //foreach (UserObject item in (IList<UserObject>)items)
            //{
            //    if (item.IsTokenUpdate)
            //        return;

            //    if (UserItemCtl.GetUsers((RegistrationObjectContext)objectContext)
            //        .Where(u => u.EmailAddress == item.EmailAddress).Any())
            //        validationMessages.AddError(item.ValidationKey, "The email address already exists in the system");
            //}
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<UserTokenObject> items)
        {
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<UserTokenObject> items)
        {
        }

        public UserTokenObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, UserTokenObject theObject)
        {
            return theObject;
        }

    }
}
