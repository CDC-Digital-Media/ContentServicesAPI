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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class UserCtl : BaseCtl<UserObject, Gov.Hhs.Cdc.CdcRegistrationProvider.Model.User,
        UserCtl, RegistrationObjectContext>
    {

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.UserGuid = PersistedDbObject.UserGuid;
        }

        public override string ToString()
        {
            return PersistedBusinessObject.GetType().Name + ", UserId=" + PersistedBusinessObject.UserGuid.ToString();
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return (
                PersistedDbObject.FirstName == NewBusinessObject.FirstName
                && PersistedDbObject.MiddleName == NewBusinessObject.MiddleName
                && PersistedDbObject.LastName == NewBusinessObject.LastName
                && PersistedDbObject.EmailAddress == NewBusinessObject.EmailAddress
                && PersistedDbObject.ApiClientGuid == NewBusinessObject.ApiClientGuid
                && PersistedDbObject.UserToken == NewBusinessObject.UserToken
                && PersistedDbObject.ExpirationUtcSeconds == NewBusinessObject.ExpirationSeconds
                && PersistedDbObject.TempPassword == NewBusinessObject.TempPassword
                && AsBool(PersistedDbObject.Active) == NewBusinessObject.IsActive
                );
        }

        public override bool VersionMatches()
        {
            return true;
        }

        public override object Get(bool forUpdate)
        {
            return GetUsers(TheObjectContext, forUpdate);
        }

        public IQueryable Get(IDataServicesObjectContext dataEntities)
        {
            return GetUsers((RegistrationObjectContext)dataEntities);
        }

        public static IQueryable<UserObject> GetUsers(RegistrationObjectContext registrationDb, bool forUpdate = false)
        {
            IQueryable<UserOrganizationObject> userOrgObjects = UserOrganizationCtl.Get(registrationDb, forUpdate);

            IQueryable<UserObject> items = from u in registrationDb.RegistrationDbEntities.Users
                                           select new UserObject()
                                           {
                                               UserGuid = u.UserGuid,
                                               EmailAddress = u.EmailAddress,
                                               FirstName = u.FirstName,
                                               MiddleName = u.MiddleName,
                                               LastName = u.LastName,
                                               _usageGuidelinesAgreementDateTime = u.UsageGuidelinesAgreementDateTime,
                                               AgreedToUsageGuidelines = u.UsageGuidelinesAgreementDateTime != null,
                                               Password = u.Password,
                                               PasswordSalt = u.PasswordSalt,
                                               PasswordFormat = u.PasswordFormat == null ? 0 : u.PasswordFormat,
                                               IsActive = u.Active == "Yes",
                                               ApiClientGuid = u.ApiClientGuid,
                                               UserToken = u.UserToken,
                                               ExpirationSeconds = u.ExpirationUtcSeconds,
                                               TempPassword = u.TempPassword,

                                               IsMigrated = u.IsMigrated == "Yes",

                                               Organizations = userOrgObjects.Where(uo => uo.UserGuid == u.UserGuid),

                                               DbObject = forUpdate ? u : null
                                           };
            return items;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new Gov.Hhs.Cdc.CdcRegistrationProvider.Model.User();
            PersistedDbObject.UserGuid = Guid.NewGuid();

            PersistedDbObject.Password = Guid.NewGuid().ToString();     // NewBusinessObject.Password;

            PersistedDbObject.TempPassword = NewBusinessObject.TempPassword;

            PersistedDbObject.UserToken = NewBusinessObject.UserToken;
            PersistedDbObject.ExpirationUtcSeconds = NewBusinessObject.ExpirationSeconds;

            PersistedDbObject.IsMigrated = NewBusinessObject.IsMigrated ? "Yes" : "No";

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.FirstName = NewBusinessObject.FirstName;
            PersistedDbObject.MiddleName = NewBusinessObject.MiddleName;
            PersistedDbObject.LastName = NewBusinessObject.LastName;
            if (PersistedDbObject.UsageGuidelinesAgreementDateTime == null && NewBusinessObject._agreedToUsageGuidelines)
                PersistedDbObject.UsageGuidelinesAgreementDateTime = DateTime.UtcNow;
            if (PersistedDbObject.UsageGuidelinesAgreementDateTime != null && !NewBusinessObject._agreedToUsageGuidelines)
                PersistedDbObject.UsageGuidelinesAgreementDateTime = null;

            PersistedDbObject.EmailAddress = NewBusinessObject.EmailAddress;
            PersistedDbObject.Active = NewBusinessObject.IsActive ? "Yes" : "No";

            if (NewBusinessObject.ApiClientGuid != new Guid("00000000-0000-0000-0000-000000000000"))
                PersistedDbObject.ApiClientGuid = NewBusinessObject.ApiClientGuid;

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public override void AddToDb()
        {
            TheObjectContext.RegistrationDbEntities.Users.AddObject(PersistedDbObject);

            //We don't have the ID yet, and will have to get it after the commit.
            //NewBusinessObject.Id = PersistedDbObject.MediaId;
            //PersistedBusinessObject.Id = PersistedDbObject.MediaId;
        }

        public void Add(BaseCtl<UserOrganizationObject, Model.UserOrganization, UserOrganizationCtl, RegistrationObjectContext> userOrg)
        {
            PersistedDbObject.UserOrganizations.Add(userOrg.PersistedDbObject);
        }

        public override void Delete()
        {
            TheObjectContext.RegistrationDbEntities.Users.DeleteObject(PersistedDbObject);
        }

        public void Delete(DataSourceBusinessObject persistedBusinessObject)
        {
            TheObjectContext.RegistrationDbEntities.Users.DeleteObject((Gov.Hhs.Cdc.CdcRegistrationProvider.Model.User)persistedBusinessObject.DbObject);
        }
        
        public void SetPassword()
        {
            SetToken();
            PersistedDbObject.Password = NewBusinessObject.Password;
            PersistedDbObject.PasswordSalt = NewBusinessObject.PasswordSalt;
            PersistedDbObject.TempPassword = null;
            PersistedDbObject.PasswordFormat = null;
        }

        public void SetTempPassword()
        {
            PersistedDbObject.TempPassword = NewBusinessObject.TempPassword;
            PersistedDbObject.PasswordSalt = NewBusinessObject.PasswordSalt;
            PersistedDbObject.PasswordFormat = null;
        }

        public void SetToken()
        {
            PersistedDbObject.UserToken = NewBusinessObject.UserToken;
            PersistedDbObject.ExpirationUtcSeconds = NewBusinessObject.ExpirationSeconds;
        }

    }
}
