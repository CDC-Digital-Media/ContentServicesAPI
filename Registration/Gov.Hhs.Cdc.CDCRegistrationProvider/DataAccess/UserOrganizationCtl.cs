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
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.CdcRegistrationProvider.Model;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class UserOrganizationCtl : BaseCtl<UserOrganizationObject, Gov.Hhs.Cdc.CdcRegistrationProvider.Model.UserOrganization,
        UserOrganizationCtl, RegistrationObjectContext>
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

        public static IQueryable<UserOrganizationObject> Get(RegistrationObjectContext registrationDb, bool forUpdate = false)
        {
            IQueryable<OrganizationObject> orgObjects = OrganizationCtl.Get(registrationDb, forUpdate);
            IQueryable<UserOrganizationObject> items = from uo in registrationDb.RegistrationDbEntities.UserOrganizations
                                                       select new UserOrganizationObject()
                                                       {
                                                           OrganizationId = uo.OrganizationID,
                                                           UserGuid = uo.UserGuid,
                                                           IsPrimary = uo.PrimaryOrg == "Yes",
                                                           Organization = orgObjects.Where(o => o.Id == uo.OrganizationID).FirstOrDefault(),
                                                           DbObject = forUpdate ? uo : null
                                                       };
            return items;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new UserOrganization();
            PersistedDbObject.OrganizationID = NewBusinessObject.OrganizationId;
            
            //UserGuid is set by adding this to the User object
            //PersistedDbObject.UserGuid = NewBusinessObject.UserGuid;  

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGUID = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.PrimaryOrg = NewBusinessObject.IsActive ? "Yes" : "No";

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGUID = modifiedGuid;
        }

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.OrganizationId = PersistedDbObject.OrganizationID;
            PersistedBusinessObject.UserGuid = PersistedDbObject.UserGuid;

            PersistedBusinessObject.Organization = OrganizationCtl.Get(TheObjectContext, false).Where(o => o.Id == PersistedDbObject.OrganizationID).FirstOrDefault();
        }

        public override void AddToDb()
        {
            TheObjectContext.RegistrationDbEntities.UserOrganizations.AddObject(PersistedDbObject);
        }

        public override void Delete()
        {
            TheObjectContext.RegistrationDbEntities.UserOrganizations.Attach(PersistedDbObject);
            TheObjectContext.RegistrationDbEntities.UserOrganizations.DeleteObject(PersistedDbObject);
        }

        public void Delete(UserOrganizationObject persistedBusinessObject)
        {
            //Media.RegistrationDbEntities.UserOrganizations.Attach(PersistedDbObject);
            TheObjectContext.RegistrationDbEntities.UserOrganizations.DeleteObject((UserOrganization)persistedBusinessObject.DbObject);
        }
    }
}
