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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class UserTokenCtl : BaseCtl<UserTokenObject, Model.User, UserTokenCtl, RegistrationObjectContext>
    {
        public override void UpdateIdsAfterInsert()
        {
        }

        public override string ToString()
        {
            return PersistedBusinessObject.GetType().Name + ", UserEmail=" + PersistedBusinessObject.EmailAddress;
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return true;
        }

        public override bool VersionMatches()
        {
            return true;
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<UserTokenObject> Get(RegistrationObjectContext registrationDb, bool forUpdate)
        {
            IQueryable<UserTokenObject> UserItems = from o in registrationDb.RegistrationDbEntities.Users
                                                    select new UserTokenObject()
                                                    {
                                                        EmailAddress = o.EmailAddress,
                                                        UserGuid = o.UserGuid,
                                                        UserToken = o.UserToken,
                                                        DbObject = forUpdate ? o : null
                                                    };
            return UserItems;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new Gov.Hhs.Cdc.CdcRegistrationProvider.Model.User();
 
            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.UserToken = NewBusinessObject.UserToken;
            PersistedDbObject.ExpirationUtcSeconds = NewBusinessObject.ExpirationUtcSeconds;

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public override void AddToDb()
        {
            TheObjectContext.RegistrationDbEntities.Users.AddObject(PersistedDbObject);
        }

        public override void Delete()
        {
            TheObjectContext.RegistrationDbEntities.Users.DeleteObject(PersistedDbObject);
        }

    }
}
