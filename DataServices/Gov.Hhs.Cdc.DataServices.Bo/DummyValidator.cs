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

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    public class DummyValidator : IValidator<object, DataServicesObjectContext>
    {

        public void PreSaveValidate(ValidationMessages validationMessages, IEnumerable<object> items)
        {
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IEnumerable<object> items)
        {
        }

        public void ValidateSave(DataServicesObjectContext objectContext, ValidationMessages validationMessages, IEnumerable<IValidationObject> items)
        {         
        }

        public void PostSaveValidate(DataServicesObjectContext objectContext, ValidationMessages validationMessages, IEnumerable<IValidationObject> items)
        {         
        }

        public void ValidateDelete(DataServicesObjectContext objectContext, ValidationMessages validationMessages, IEnumerable<IValidationObject> items)
        {
        }

        public IValidationObject GetValidationObject(DataServicesObjectContext objectContext, ValidationMessages validationMessages, object theObject)
        {
            return null;
        }



        public void PreSaveValidate(ValidationMessages validationMessages, object items)
        {
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, object items)
        {
        }
    }
}
