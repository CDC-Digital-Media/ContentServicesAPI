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
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class ValueToValueSetObjectValidator : IValidator<ValueToValueSetObject, ValueToValueSetObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<ValueToValueSetObject> items)
        {
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<ValueToValueSetObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueToValueSetObject> items)
        {
        }
        internal static bool UpdateValue2ValueSet(ValueToValueSetObject valueToValueSet, ValueObject valueObject)
        {
            valueToValueSet.ValueId = valueObject.ValueId;
            valueToValueSet.ValueLanguageCode = valueObject.LanguageCode;
            return true;
        }

        internal static bool UpdateValue2ValueSetWithUnexpectedPersistedValue(IDataServicesObjectContext objectContext, ValueToValueSetObject valueToValueSet, ValueObject unexpectedPersistedValue, ValidationMessages validationMessages)
        {

            ValueToValueSetObject unexpectedPersistedValueSetObject =
                ValueToValueSetCtl.Get((MediaObjectContext)objectContext)
                .Where(v2vs =>
                    v2vs.ValueId == valueToValueSet.ValueId &&
                    v2vs.ValueSetId == valueToValueSet.ValueSetId)
                    .FirstOrDefault();
            if (unexpectedPersistedValueSetObject != null)
            {
                validationMessages.AddError(valueToValueSet.ValidationKey,
                      "This item already exists in the valueset and no action was taken");
            }
            else
            {
                validationMessages.AddWarning(valueToValueSet.ValidationKey,
                      "This item already exists as a value and was applied to the value set");
            }
            return true;
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueToValueSetObject> items)
        {
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueToValueSetObject> items)
        {
        }

        public ValueToValueSetObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, ValueToValueSetObject theObject)
        {
            return null;
        }
    }
}
