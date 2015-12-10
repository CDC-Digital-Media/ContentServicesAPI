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
    public class ApiClientObjectValidator : IValidator<ApiClientObject, ApiClientObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<ApiClientObject> items)
        {
            foreach (ApiClientObject obj in (IList<ApiClientObject>)items)
            {
                RegExValidator v = new RegExValidator(validationMessages, obj.ValidationKey);

                v.IsValid(v.Alpha, obj.ApiTypeName, required: true, message: "ApiTypeName is invalid");
                v.IsValid(v.AlphaSpaces, obj.Name, required: true, message: "Name is invalid");
                //if (string.IsNullOrEmpty(obj.ApplicationKey))
                //    validationMessages.AddError(obj.ValidationKey, "ApplicationKey is invalid");
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<ApiClientObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ApiClientObject> items)
        {
            foreach (ApiClientObject item in (IList<ApiClientObject>)items)
            {
                if (ApiClientCtl.Get((RegistrationObjectContext)objectContext)
                    .Where(u => u.ServerUserGuid == item.ServerUserGuid &&
                        u.ApplicationKey == item.ApplicationKey &&
                        u.ApiTypeName == item.ApiTypeName).Any())
                    validationMessages.AddError(item.ValidationKey, "The API Client already exists in the system");
            }
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ApiClientObject> items)
        {
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ApiClientObject> items)
        {
        }

        public ApiClientObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, ApiClientObject theObject)
        {
            return theObject;
        }

    }
}
