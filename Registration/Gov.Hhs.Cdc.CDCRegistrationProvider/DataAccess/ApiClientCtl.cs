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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.Configuration;
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class ApiClientCtl : BaseCtl<ApiClientObject, Gov.Hhs.Cdc.CdcRegistrationProvider.Model.ApiClient,
        ApiClientCtl, RegistrationObjectContext>
    {

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.ApiClientGuid = PersistedDbObject.ApiClientGuid;
        }

        public override string ToString()
        {
            return PersistedBusinessObject.GetType().Name + ", ApiClientGuid=" + PersistedBusinessObject.ApiClientGuid.ToString();
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

        public static IQueryable<ApiClientObject> Get(RegistrationObjectContext registrationDb, bool forUpdate = false)
        {
            IQueryable<ApiClientObject> items = from o in registrationDb.RegistrationDbEntities.ApiClients
                                                select new ApiClientObject()
                                                {
                                                    ApiClientGuid = o.ApiClientGuid,
                                                    ServerUserGuid = o.ServiceUserGuid,
                                                    ApplicationKey = o.ApplicationKey,
                                                    ApiTypeName = o.ApiTypeName,
                                                    Name = o.Name,
                                                    ConnectionStringName = o.ConnectionStringName,
                                                    ApiKey = o.ApiKey,

                                                    Secret = o.Secret,
                                                    Salt = o.Salt,
                                                    //Token = o.Token,
                                                    //TokenExpirationUtcDateTime = o.TokenExpirationUtcDateTime,

                                                    IsActive = o.Active == "Yes",
                                                    DbObject = forUpdate ? o : null
                                                };
            return items;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new Gov.Hhs.Cdc.CdcRegistrationProvider.Model.ApiClient();
            PersistedDbObject.ApiClientGuid = Guid.NewGuid();

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.ServiceUserGuid = NewBusinessObject.ServerUserGuid;
            PersistedDbObject.ApiTypeName = NewBusinessObject.ApiTypeName;
            PersistedDbObject.Name = NewBusinessObject.Name;
            PersistedDbObject.Active = NewBusinessObject.IsActive ? "Yes" : "No";

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public void UpdateCredentials()
        {
            string salt = CredentialManager.GenerateApiKeyTokenSalt();

            NewBusinessObject.ApplicationKey = CredentialManager.GenerateApiKeyTokenSalt();
            NewBusinessObject.Secret = CredentialManager.CreateHash(ConfigurationManager.AppSettings["ApiClientPassword"], salt);

            PersistedDbObject.ApplicationKey = NewBusinessObject.ApplicationKey;
            //PersistedDbObject.Secret = CredentialManager.CreateHash(NewBusinessObject.Secret, salt); 
            PersistedDbObject.Salt = salt;

            //UpdateToken();
        }

        //public void UpdateToken()
        //{
        //    NewBusinessObject.Token = CredentialManager.GenerateApiKeyTokenSalt();
        //    NewBusinessObject.TokenExpirationUtcDateTime = CredentialManager.TokenExpirationUtcDateTime();

        //    PersistedDbObject.Token = CredentialManager.CreateHash(NewBusinessObject.Token, PersistedDbObject.Salt);
        //    PersistedDbObject.TokenExpirationUtcDateTime = NewBusinessObject.TokenExpirationUtcDateTime;
        //}

        public void UpdateApiKey()
        {
            NewBusinessObject.ApiKey = CredentialManager.GenerateApiKeyTokenSalt();
            PersistedDbObject.ApiKey = NewBusinessObject.ApiKey;
        }

        public override void AddToDb()
        {
            TheObjectContext.RegistrationDbEntities.ApiClients.AddObject(PersistedDbObject);
        }

        public override void Delete()
        {
            TheObjectContext.RegistrationDbEntities.ApiClients.DeleteObject(PersistedDbObject);
        }

        public IQueryable Get(IDataServicesObjectContext dataEntities)
        {
            return Get((RegistrationObjectContext)dataEntities);
        }

    }
}
