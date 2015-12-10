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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{

    public class UserTokenUpdateMgr : BaseUpdateMgr<UserTokenObject>
    {       
        public override string ObjectName { get { return "UserToken"; } }

        public UserTokenUpdateMgr(IList<UserTokenObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new UserTokenObjectValidator();
            ValidationItems = Items;
        }

        public UserTokenUpdateMgr(UserTokenObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);
            Validator = new UserTokenObjectValidator();
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
            foreach (UserTokenObject item in Items)
            {
                UserTokenObject persistedValueObject = UserTokenCtl.Get((RegistrationObjectContext)media, forUpdate: true)
                    .Where(u => u.EmailAddress == item.EmailAddress).FirstOrDefault();
                
                //Do not insert... update only
                if (persistedValueObject != null)
                {
                    item.UserGuid = persistedValueObject.UserGuid;      //Update the userGuid
                    UserTokenCtl.Update(media, persistedValueObject, item, enforceConcurrency: true);
                }
            }
        }

        public override void Delete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            //Do not delete 
        }

        public override void UpdateIdsAfterInsert()
        {
            //Do not insert
        }
    }
}
