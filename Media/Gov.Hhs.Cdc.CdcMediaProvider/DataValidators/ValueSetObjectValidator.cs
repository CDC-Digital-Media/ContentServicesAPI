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
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class ValueSetObjectValidator : IValidator<ValueSetObject, ValueSetObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<ValueSetObject> items)
        {
            foreach (ValueSetObject valueSet in items)
            {
                if (string.IsNullOrEmpty(valueSet.LanguageCode))
                { validationMessages.AddError(valueSet.ValidationKey, "Language is required"); }
                if (string.IsNullOrEmpty(valueSet.Name))
                { validationMessages.AddError(valueSet.ValidationKey, "Name is required"); }
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<ValueSetObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueSetObject> items)
        {
            MediaObjectContext media = (MediaObjectContext)objectContext;
            foreach (ValueSetObject valueSet in items)
            {
                LanguageItem language = MediaCacheController.GetLanguage(media, valueSet.LanguageCode);
                if (language == null || !language.IsActive)
                { validationMessages.AddError(valueSet.ValidationKey, "Language is invalid"); }

                if (ValueSetObjectCtl.Get(media).Where(vs => vs.Name == valueSet.Name && valueSet.LanguageCode == vs.LanguageCode && (valueSet.Id == 0 || vs.Id != valueSet.Id)).Any())
                {
                    validationMessages.AddError(valueSet.ValidationKey, "Value set with this name already exists");
                }
            }
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueSetObject> items)
        {
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueSetObject> items)
        {
        }

        public ValueSetObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, ValueSetObject theObject)
        {
            return theObject;
        }
    }
}
