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
    public class DomainCtl : BaseCtl<DomainObject, Gov.Hhs.Cdc.CdcRegistrationProvider.Model.Domain,
        DomainCtl, RegistrationObjectContext>
    {

        public override void UpdateIdsAfterInsert()
        {
        }

        public override string ToString()
        {
            return "Domain=" + PersistedBusinessObject.DomainName;
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return (PersistedDbObject.Description == NewBusinessObject.Description
                && PersistedDbObject.SourceCode == NewBusinessObject.SourceCode
                && AsBool(PersistedDbObject.Active) == NewBusinessObject.IsActive
                );
        }

        public override bool VersionMatches()
        {
            return true;
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<DomainObject> Get(RegistrationObjectContext registrationDb, bool forUpdate)
        {

            //PersistentUrl u = (from url in Media.PersistentUrls select url).ToList()[0];
            IQueryable<DomainObject> domainItems = from o in registrationDb.RegistrationDbEntities.Domains
                                                               select new DomainObject()
                                                               {
                                                                   DomainName = o.DomainName,
                                                                   Description = o.Description,
                                                                   SourceCode = o.SourceCode,
                                                                   IsActive = o.Active == "Yes",
                                                                   DbObject = forUpdate ? o : null
                                                               };
            return domainItems;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new Domain();
            PersistedDbObject.DomainName = NewBusinessObject.DomainName;
            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.Description = NewBusinessObject.Description;
            PersistedDbObject.SourceCode = NewBusinessObject.SourceCode;
            PersistedDbObject.Active = NewBusinessObject.IsActive ? "Yes" : "No";

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public override void AddToDb()
        {
            TheObjectContext.RegistrationDbEntities.Domains.AddObject(PersistedDbObject);

            //We don't have the ID yet, and will have to get it after the commit.
            //NewBusinessObject.Id = PersistedDbObject.MediaId;
            //PersistedBusinessObject.Id = PersistedDbObject.MediaId;
        }

        public override void Delete()
        {
            TheObjectContext.RegistrationDbEntities.Domains.DeleteObject(PersistedDbObject);
        }

        public void Delete(DomainObject persistedBusinessObject)
        {
            TheObjectContext.RegistrationDbEntities.Domains.DeleteObject((Domain)persistedBusinessObject.DbObject);
        }

    }
}
