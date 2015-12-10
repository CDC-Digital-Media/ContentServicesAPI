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
using System.Configuration;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public partial class UserUpdateMgr : BaseUpdateMgr<UserObject>
    {
        private class CombinedUserObject
        {
            public UserObject User { get; set; }
            public UserCtl InsertedUser { get; set; }

            public UserOrganizationUpdateMgr UserOrganizationUpdateMgr { get; set; }
        }

        List<CombinedUserObject> theObjects;

        List<UserOrganizationUpdateMgr> userOrganizationUpdateMgrs;

        public override string ObjectName { get { return "User"; } }

        public UserUpdateMgr(IList<UserObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new UserObjectValidator();
            CreateChildUpdateMgrs(Items);
        }

        public UserUpdateMgr(UserObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);
            Validator = new UserObjectValidator();
            CreateChildUpdateMgrs(Items);
        }

        private void CreateChildUpdateMgrs(IList<UserObject> items)
        {
            theObjects = (from u in items
                          let userOrgs = u.Organizations == null ? new List<UserOrganizationObject>() : u.Organizations.ToList()
                          select new CombinedUserObject()
                          {
                              User = u,
                              UserOrganizationUpdateMgr = u.Organizations == null ? null : new UserOrganizationUpdateMgr((IList<UserOrganizationObject>)u.Organizations)
                          }).ToList();
            userOrganizationUpdateMgrs = theObjects.Where(u => u.UserOrganizationUpdateMgr != null).Select(u => u.UserOrganizationUpdateMgr).ToList();
        }

        public override void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, Items);
            foreach (UserOrganizationUpdateMgr userOrganizationUpdateMgr in userOrganizationUpdateMgrs)
            {
                userOrganizationUpdateMgr.PreSaveValidate(ref validationMessages);
            }
        }

        public override void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, Items);
            foreach (UserOrganizationUpdateMgr userOrganizationUpdateMgr in userOrganizationUpdateMgrs)
            {
                userOrganizationUpdateMgr.PreDeleteValidate(validationMessages);
            }
        }

        public override void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            Validator.ValidateSave(objectContext, validationMessages, Items);
            foreach (UserOrganizationUpdateMgr userOrganizationUpdateMgr in userOrganizationUpdateMgrs)
            {
                userOrganizationUpdateMgr.ValidateSave(objectContext, validationMessages);
            }
        }

        public override void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(objectContext, validationMessages, Items);
            foreach (UserOrganizationUpdateMgr userOrganizationUpdateMgr in userOrganizationUpdateMgrs)
            {
                userOrganizationUpdateMgr.ValidateDelete(objectContext, validationMessages);
            }
        }

        public override void Save(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (CombinedUserObject combinedUserObject in theObjects)
            {
                UserObject persistedUser =
                    UserCtl.GetUsers((RegistrationObjectContext)objectContext, forUpdate: true)
                    .Where(u => u.UserGuid == combinedUserObject.User.UserGuid).FirstOrDefault();

                if (persistedUser == null)
                {
                    Insert(objectContext, combinedUserObject);
                }
                else
                {
                    UserCtl updatedUser = UserCtl.Update(objectContext, persistedUser, combinedUserObject.User, enforceConcurrency: true);
                    if (combinedUserObject.UserOrganizationUpdateMgr != null)
                        combinedUserObject.UserOrganizationUpdateMgr.Update((IDataServicesObjectContext)objectContext, updatedUser);
                }
            }
        }

        //Need to put this in a config somewhere
        public const int VALID_REQUEST_WINDOW = 900;                             // 900 seconds

        private static void Insert(IDataServicesObjectContext objectContext, CombinedUserObject combinedUserObject)
        {
            ApiClientObject apiClientItem = ApiClientCtl.Get((RegistrationObjectContext)objectContext)
                .Where(a => a.ApplicationKey == combinedUserObject.User.ApiKey)
                .FirstOrDefault();
            if (apiClientItem != null)
            {
                combinedUserObject.User.ApiClientGuid = apiClientItem.ApiClientGuid;
                combinedUserObject.User.UserToken = CredentialManager.GenerateApiKeyTokenSalt();
                combinedUserObject.User.ExpirationSeconds = CredentialManager.TokenExpirationInSeconds(VALID_REQUEST_WINDOW);
            }

            combinedUserObject.InsertedUser = UserCtl.Create(objectContext, combinedUserObject.User);
            combinedUserObject.InsertedUser.AddToDb();
            if (combinedUserObject.UserOrganizationUpdateMgr != null)
                combinedUserObject.UserOrganizationUpdateMgr.Insert(objectContext, combinedUserObject.InsertedUser);
        }

        public static void EncryptPassword(UserObject userObject)
        {
            if (userObject == null) throw new ArgumentNullException("userObject");
            userObject.Password = CredentialManager.CreateHash(userObject.Password, GetSalt(userObject));
        }

        public static string GetSalt(UserObject userObject)
        {
            string salt = userObject.PasswordSalt;
            if (string.IsNullOrEmpty(salt))
                salt = userObject.UserGuid + ConfigurationManager.AppSettings["UserPasswordSalt"];
            
            return salt;
        }

        public static void EncryptTempPassword(UserObject userObject)
        {
            if (userObject == null) throw new ArgumentNullException("userObject");
            userObject.TempPassword = CredentialManager.CreateHash(userObject.TempPassword, GetSalt(userObject));
        }

        public override void Delete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
                foreach (UserObject item in Items)
                {
                    UserObject persistedUser =
                        UserCtl.GetUsers((RegistrationObjectContext)objectContext, forUpdate: true)
                        .Where(u => u.UserGuid == item.UserGuid).FirstOrDefault();

                    if (persistedUser != null)
                    {
                        UserOrganizationUpdateMgr.Delete(objectContext, persistedUser.Organizations);
                        UserCtl.Delete(objectContext, persistedUser);
                    }
                }

            //Do not delete organizations
        }

        public override void UpdateIdsAfterInsert()
        {
            foreach (CombinedUserObject combinedUserObject in theObjects)
            {
                if (combinedUserObject.InsertedUser != null)
                {
                    combinedUserObject.InsertedUser.UpdateIdsAfterInsert();
                }
                if (combinedUserObject.UserOrganizationUpdateMgr != null)
                    combinedUserObject.UserOrganizationUpdateMgr.UpdateIdsAfterInsert();
            }
        }




    }
}
