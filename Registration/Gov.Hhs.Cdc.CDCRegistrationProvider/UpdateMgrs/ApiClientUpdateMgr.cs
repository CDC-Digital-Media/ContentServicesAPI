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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{

    public class ApiClientUpdateMgr : BaseUpdateMgr<ApiClientObject>
    {
        public override string ObjectName { get { return "ApiClient"; } }

        public ApiClientUpdateMgr(IList<ApiClientObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new ApiClientObjectValidator();
            ValidationItems = Items;
        }

        public ApiClientUpdateMgr(ApiClientObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);
            Validator = new ApiClientObjectValidator();
            ValidationItems = Items;
        }

        public override void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, Items);
        }

        public override void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, Items);
        }

        public override void ValidateSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateSave(media, validationMessages, Items);
        }

        public override void ValidateDelete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(media, validationMessages, Items);
        }

        public override void Save(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (ApiClientObject item in Items)
            {
                ApiClientObject persistedValueObject = ApiClientCtl.Get((RegistrationObjectContext)media, forUpdate: true)
                    .Where(u => u.ServerUserGuid == item.ServerUserGuid &&
                        u.ApplicationKey == item.ApplicationKey &&
                        u.ApiTypeName == item.ApiTypeName).FirstOrDefault();

                if (persistedValueObject == null)
                {
                    ApiClientCtl ctl = ApiClientCtl.Create(media, item);
                    ctl.UpdateCredentials();
                    ctl.UpdateApiKey();
                    
                    InsertedDataControls.Add(ctl);
                    ctl.AddToDb();
                }
                else
                    ApiClientCtl.Update(media, persistedValueObject, item, enforceConcurrency: true);            
            }
        }

        public override void Delete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (ApiClientObject item in Items)
            {
                ApiClientObject persistedObject = ApiClientCtl.Get((RegistrationObjectContext)media, forUpdate: true)
                    .Where(u => u.ApiClientGuid == item.ApiClientGuid).FirstOrDefault();

                if (persistedObject != null)
                {
                    ApiClientCtl.Delete(media, persistedObject);
                }
            } 
        }

        public override void UpdateIdsAfterInsert()
        {
            foreach (IDataCtl dataCtl in InsertedDataControls)
                dataCtl.UpdateIdsAfterInsert();
        }
    }
}
