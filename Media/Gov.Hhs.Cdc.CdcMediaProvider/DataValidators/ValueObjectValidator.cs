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
    public class ValueObjectValidator : IValidator<ValueObject, ValueObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<ValueObject> items)
        {
            foreach(ValueObject value in items)
            {
                value.ValueName = SafeTrim(value.ValueName);
                value.LanguageCode = SafeTrim(value.LanguageCode);
                value.Description = SafeTrim(value.Description);
            }
        }

        private static string SafeTrim(string value)
        {
            return value == null ? null : value.Trim();
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<ValueObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueObject> items)
        {
        }

        public static IList<ValueObject> ValidateSaveReturningUpdatedValues(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueObject> items)
        {
            List<ValueObject> newItemsList = new List<ValueObject>();
            foreach (ValueObject item in items)
            {

                //On insert, use the persisted value if exists
                ValueObject unexpectedPersistedValue =
                    item.ValueId > 0 ? null : // we expect the value to be null if valueId > 0
                    ValueCtl.Get((MediaObjectContext)objectContext)
                        .Where(v => v.ValueName == item.ValueName && v.LanguageCode == item.LanguageCode)
                        .FirstOrDefault();
                ValueObject newObject = item;
                if (unexpectedPersistedValue != null)
                {
                    newObject = unexpectedPersistedValue;
                    newObject.ValueToValueSetObjects = item.ValueToValueSetObjects;
                }
                if (newObject.ValueToValueSetObjects != null)
                {
                    foreach (var v2vs in newObject.ValueToValueSetObjects)
                    {
                        ValueToValueSetObjectValidator.UpdateValue2ValueSet(v2vs, unexpectedPersistedValue ?? item);
                        if (unexpectedPersistedValue != null)
                                ValueToValueSetObjectValidator.UpdateValue2ValueSetWithUnexpectedPersistedValue(objectContext, v2vs, unexpectedPersistedValue, validationMessages);
                    }
                }

                //Do we use the found value or the new value
                if (unexpectedPersistedValue != null)
                {
                    item.ValueId = unexpectedPersistedValue.ValueId;
                    newItemsList.Add(unexpectedPersistedValue);
                }
                else
                { newItemsList.Add(item); }
            }
            return newItemsList;

        }



        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueObject> items)
        {
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueObject> items)
        {
        }

        public ValueObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, ValueObject theObject)
        {
            return theObject;
        }
    }
}
