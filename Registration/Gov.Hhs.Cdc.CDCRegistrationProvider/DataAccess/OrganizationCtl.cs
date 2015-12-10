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
    public class OrganizationCtl :
        BaseCtl<OrganizationObject, Model.Organization, OrganizationCtl, RegistrationObjectContext>,
        ICachedDataControl
    {

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.Id = PersistedDbObject.OrganizationId;
        }

        public override string ToString()
        {
            return PersistedBusinessObject.GetType().Name + ", OrganizationId=" + PersistedBusinessObject.Id.ToString();
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return (PersistedDbObject.OrganizationTypeCode == NewBusinessObject.OrganizationTypeCode
                && PersistedDbObject.OrganizationTypeOther == NewBusinessObject.OrganizationTypeOther
                && PersistedDbObject.Name == NewBusinessObject.Name
                && PersistedDbObject.Address == NewBusinessObject.Address
                && PersistedDbObject.AddressContinued == NewBusinessObject.AddressContinued
                && PersistedDbObject.City == NewBusinessObject.City
                && PersistedDbObject.StateProvinceId == NewBusinessObject.StateProvinceId
                && PersistedDbObject.CountyId == NewBusinessObject.CountyId
                && PersistedDbObject.CountryId == NewBusinessObject.CountryId
                && PersistedDbObject.PostalCode == NewBusinessObject.PostalCode
                && PersistedDbObject.Phone == NewBusinessObject.Phone
                && PersistedDbObject.Fax == NewBusinessObject.Fax
                && PersistedDbObject.Email == NewBusinessObject.Email
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

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new Organization();
            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.OrganizationTypeCode = NewBusinessObject.OrganizationTypeCode;
            PersistedDbObject.OrganizationTypeOther = NewBusinessObject.OrganizationTypeOther;

            PersistedDbObject.Name = NewBusinessObject.Name;
            PersistedDbObject.Address = NewBusinessObject.Address;
            PersistedDbObject.AddressContinued = NewBusinessObject.AddressContinued;
            PersistedDbObject.City = NewBusinessObject.City;
            PersistedDbObject.StateProvinceId = NewBusinessObject.StateProvinceId;
            PersistedDbObject.CountyId = NewBusinessObject.CountyId;
            PersistedDbObject.CountryId = NewBusinessObject.CountryId;

            PersistedDbObject.PostalCode = NewBusinessObject.PostalCode;
            PersistedDbObject.Phone = NewBusinessObject.Phone;
            PersistedDbObject.Fax = NewBusinessObject.Fax;
            PersistedDbObject.Email = NewBusinessObject.Email;
            PersistedDbObject.Active = NewBusinessObject.IsActive ? "Yes" : "No";

            PersistedDbObject.GeoNameID = NewBusinessObject.GeoNameId;

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public override void AddToDb()
        {
            TheObjectContext.RegistrationDbEntities.Organizations.AddObject(PersistedDbObject);
        }

        public void AddUser(IDataCtl user)
        {
            user.AddToDb();
        }

        public void Add(BaseCtl<UserOrganizationObject, Model.UserOrganization, UserOrganizationCtl, RegistrationObjectContext> userOrg)
        {
            PersistedDbObject.UserOrganizations.Add(userOrg.PersistedDbObject);
        }

        public void Add(BaseCtl<OrganizationDomainObject, Model.OrganizationDomain, OrganizationDomainCtl, RegistrationObjectContext> orgDomain)
        {
            PersistedDbObject.OrganizationDomains.Add(orgDomain.PersistedDbObject);
        }

        public void AddUser(BaseCtl<ServiceUserObject, ServiceUser, ServiceUserCtl, RegistrationObjectContext> serviceUser)
        {
            PersistedDbObject.ServiceUsers.Add(serviceUser.PersistedDbObject);
        }

        public override void Delete()
        {
            TheObjectContext.RegistrationDbEntities.Organizations.DeleteObject(PersistedDbObject);
        }

        public void Delete(DataSourceBusinessObject persistedBusinessObject)
        {
            TheObjectContext.RegistrationDbEntities.Organizations.DeleteObject((Organization)persistedBusinessObject.DbObject);
        }


        public IQueryable Get(IDataServicesObjectContext dataEntities)
        {
            return Get((RegistrationObjectContext)dataEntities);
        }

        public static IQueryable<OrganizationObject> Get(RegistrationObjectContext registrationDb, bool forUpdate = false)
        {
            IQueryable<OrganizationDomainObject> organizationDomains = OrganizationDomainCtl.Get(registrationDb, forUpdate);

            //PersistentUrl u = (from url in Media.PersistentUrls select url).ToList()[0];
            IQueryable<OrganizationObject> organizationItems = from o in registrationDb.RegistrationDbEntities.Organizations
                                                               select new OrganizationObject()
                                                               {
                                                                   Id = o.OrganizationId,

                                                                   //OrganizationTypeOther = o.OrganizationTypeOther,
                                                                   Name = o.Name,
                                                                   Address = o.Address,
                                                                   AddressContinued = o.AddressContinued,
                                                                   City = o.City,
                                                                   StateProvinceId = o.StateProvinceId,
                                                                   CountyId = o.CountyId,
                                                                   CountryId = o.CountryId,

                                                                   PostalCode = o.PostalCode,
                                                                   Phone = o.Phone,
                                                                   Fax = o.Fax,
                                                                   Email = o.Email,
                                                                   GeoNameId = o.GeoNameID,

                                                                   IsActive = o.Active == "Yes",
                                                                   OrganizationDomains = organizationDomains.Where(uo => uo.OrganizationId == o.OrganizationId),
                                                                   OrganizationTypeDescription = o.OrganizationType.Description,
                                                                   OrganizationTypeCode = o.OrganizationTypeCode.ToLower() != "other" ? o.OrganizationTypeCode : o.OrganizationTypeOther,
                                                                   DbObject = forUpdate ? o : null
                                                               };
            return organizationItems;
        }


        public object GetDictionary(IDataServicesObjectContext dataEntities)
        {
            throw new NotImplementedException();
        }

    }
}
