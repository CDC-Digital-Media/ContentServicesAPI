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

using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    class EnclosureObjectValidator : IValidator<EnclosureObject, EnclosureObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<EnclosureObject> items)
        {
            foreach (EnclosureObject item in items)
            {
                RegExValidator v = new RegExValidator(validationMessages, item.ValidationKey);

                v.IsValid(RegExValidator.Url, item.ResourceUrl, required: true, message: "ResourceUrl is invalid");
                v.IsValid(v.Any, item.ContentType, required: true, message: "ContentType is invalid");
                v.IsValid(v.Numeric, item.Size.ToString(), required: true, message: "Size is invalid");

                if (string.IsNullOrEmpty(item.ContentType))
                {
                    validationMessages.AddError(item.ValidationKey + ".ContentType", "ContentType is invalid");
                }

                //MAR 9/11/2015 - Change the size to check to < 0 from <= 0 
                if (item.Size < 0)
                {
                    validationMessages.AddError(item.ValidationKey + ".Size", "Size is invalid");
                }
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<EnclosureObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<EnclosureObject> items)
        {
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<EnclosureObject> items)
        {
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<EnclosureObject> items)
        {
        }

        public EnclosureObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, EnclosureObject theObject)
        {
            return theObject;
        }
    }
}
