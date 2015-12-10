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
using System.Text.RegularExpressions;
using Gov.Hhs.Cdc.DataProvider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataProvider
{
    public class ProxyCacheObjectValidator : IValidator<ProxyCacheObject, ProxyCacheObject>
    {

        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<ProxyCacheObject> items)
        {
            foreach (ProxyCacheObject proxyCacheObject in items)
            {
                //Validate url is not null
                if (string.IsNullOrEmpty(proxyCacheObject.Url))
                {
                    validationMessages.AddError(proxyCacheObject.ValidationKey, "Url is required");
                }

                //Validate dataset id is of the form "[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}" like abcd-1234
                //([01]?[0-9]|2[0-3])
                Regex datasetIdRegex = new Regex(@"^[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}$");
                if (!string.IsNullOrEmpty(proxyCacheObject.DatasetId) && !datasetIdRegex.IsMatch(proxyCacheObject.DatasetId))
                {
                    validationMessages.AddError(proxyCacheObject.ValidationKey, "Dataset ID is an invalid format.  It must be 8 alpha numeric characters with a dash separating 4, like ab12-43cd");
                }

                //Validate that expiration interval string is of the correct format: d.hh:mm:ss w/ hh = 0-23, mm = 0-59, ss = 0-59
                Regex timeSpanRegex = new Regex(@"^([0-9]{0,7}\.)?([01]?[0-9]|2[0-3]):00:00$");
                if (string.IsNullOrEmpty(proxyCacheObject.ExpirationInterval) || !timeSpanRegex.IsMatch(proxyCacheObject.ExpirationInterval))
                {
                    validationMessages.AddError(proxyCacheObject.ValidationKey, "Expiration Interval TimeSpan is an invalid format.  It must match d.hh:mm:ss");
                }
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<ProxyCacheObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ProxyCacheObject> items)
        {
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ProxyCacheObject> items)
        {
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ProxyCacheObject> items)
        {
        }

        public ProxyCacheObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, ProxyCacheObject theObject)
        {
            return theObject;
        }
    }
}
