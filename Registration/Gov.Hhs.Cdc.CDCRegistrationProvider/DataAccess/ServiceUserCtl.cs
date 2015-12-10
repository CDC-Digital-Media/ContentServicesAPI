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
using System.Linq;
using Gov.Hhs.Cdc.CdcRegistrationProvider.Model;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class ServiceUserCtl : BaseCtl<ServiceUserObject, ServiceUser, ServiceUserCtl, RegistrationObjectContext>
    {
        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.ServiceUserGuid = PersistedDbObject.ServiceUserGuid;
       }

        public override string ToString()
        {
            return PersistedBusinessObject.GetType().Name + ", ServiceUserId=" + PersistedBusinessObject.ServiceUserGuid.ToString();
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return (PersistedDbObject.OrganizationId == NewBusinessObject.OrganizationId
                && PersistedDbObject.FirstName == NewBusinessObject.FirstName
                && PersistedDbObject.MiddleName == NewBusinessObject.MiddleName
                && PersistedDbObject.LastName == NewBusinessObject.LastName
                && PersistedDbObject.EmailAddress == NewBusinessObject.EmailAddress
                && PersistedDbObject.Password == NewBusinessObject.Password
                && PersistedDbObject.PasswordSalt == NewBusinessObject.PasswordSalt
                && AsBool(PersistedDbObject.Active) == NewBusinessObject.IsActive
                );
        }

        public override bool VersionMatches()
        {
            return true;
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, returnPassword:true, forUpdate: forUpdate);
        }


        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new Gov.Hhs.Cdc.CdcRegistrationProvider.Model.ServiceUser();
            PersistedDbObject.ServiceUserGuid = Guid.NewGuid();

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            string salt = CredentialManager.GenerateApiKeyTokenSalt();

            PersistedDbObject.OrganizationId = NewBusinessObject.OrganizationId;
            PersistedDbObject.FirstName = NewBusinessObject.FirstName;
            PersistedDbObject.MiddleName = NewBusinessObject.MiddleName;
            PersistedDbObject.LastName = NewBusinessObject.LastName;
            PersistedDbObject.EmailAddress = NewBusinessObject.EmailAddress;
            PersistedDbObject.Password = CredentialManager.CreateHash(NewBusinessObject.Password, salt);
            PersistedDbObject.PasswordSalt = salt;
            PersistedDbObject.Active = NewBusinessObject.IsActive ? "Yes" : "No";

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public override void AddToDb()
        {
            TheObjectContext.RegistrationDbEntities.ServiceUsers.AddObject(PersistedDbObject);
        }

        public override void Delete()
        {
            TheObjectContext.RegistrationDbEntities.ServiceUsers.DeleteObject(PersistedDbObject);
        }

        public static IQueryable<ServiceUserObject> Get(RegistrationObjectContext registrationDb, bool returnPassword = false, bool forUpdate = false)
        {
            IQueryable<ServiceUserObject> items = from o in registrationDb.RegistrationDbEntities.ServiceUsers
                                                  select new ServiceUserObject()
                                                  {
                                                      ServiceUserGuid = o.ServiceUserGuid,
                                                      OrganizationId = o.OrganizationId,
                                                      EmailAddress = o.EmailAddress,
                                                      FirstName = o.FirstName,
                                                      MiddleName = o.MiddleName,
                                                      LastName = o.LastName,
                                                      PasswordRepeat = returnPassword ? o.Password : "",
                                                      PasswordSalt = returnPassword ? o.PasswordSalt : "",
                                                      IsActive = o.Active == "Yes",
                                                      DbObject = forUpdate ? o : null
                                                  };
            return items;
        }


    }
}
