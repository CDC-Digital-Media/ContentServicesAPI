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
using Gov.Hhs.Cdc.CdcRegistrationProvider.Model;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class OrganizationDomainCtl : BaseCtl<OrganizationDomainObject, Model.OrganizationDomain,
        OrganizationDomainCtl, RegistrationObjectContext>, ICachedDataControl
    {

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


        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new OrganizationDomain();

            PersistedDbObject.OrganizationID = NewBusinessObject.OrganizationId;
            PersistedDbObject.DomainName = NewBusinessObject.DomainName;


            //OrganizationId is set by adding this to the User object


            PersistedDbObject.CreatedDateTime = modifiedDateTime;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.isDefault = (NewBusinessObject.IsDefault != null && ((bool)NewBusinessObject.IsDefault)) ? "Yes" : "No";
        }

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.OrganizationId = PersistedDbObject.OrganizationID;
        }

        public override void AddToDb()
        {
            TheObjectContext.RegistrationDbEntities.OrganizationDomains.AddObject(PersistedDbObject);
        }

        public override void Delete()
        {
            TheObjectContext.RegistrationDbEntities.OrganizationDomains.Attach(PersistedDbObject);
            TheObjectContext.RegistrationDbEntities.OrganizationDomains.DeleteObject(PersistedDbObject);
        }

        public void Delete(DataSourceBusinessObject persistedBusinessObject)
        {
            TheObjectContext.RegistrationDbEntities.OrganizationDomains.DeleteObject((OrganizationDomain)persistedBusinessObject.DbObject);
        }

        public IQueryable Get(IDataServicesObjectContext dataEntities)
        {
            return Get((RegistrationObjectContext)dataEntities);
        }

        public static IQueryable<OrganizationDomainObject> Get(RegistrationObjectContext registrationDb, bool forUpdate=false)
        {
            IQueryable<OrganizationDomainObject> items = from uo in registrationDb.RegistrationDbEntities.OrganizationDomains
                                                         select new OrganizationDomainObject()
                                                         {
                                                             OrganizationId = uo.OrganizationID,
                                                             DomainName = uo.DomainName,
                                                             IsDefault = uo.isDefault == "Yes",
                                                             DbObject = forUpdate ? uo : null
                                                         };
            return items;
        }

        public object GetDictionary(IDataServicesObjectContext dataEntities)
        {
            throw new NotImplementedException();
        }
    }
}
