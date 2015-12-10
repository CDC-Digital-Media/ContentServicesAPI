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

using System.Linq;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.Api
{
    public class UserOrganizationTransformation
    {
        public static UserOrganizationObject CreateUserOrganizationObject(SerialUserOrgItem obj)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj.id > 0)
            {
                return new UserOrganizationObject() { OrganizationId = obj.id, IsPrimary = true };
            }
            else
            {
                return new UserOrganizationObject()
                 {
                     Organization = OrganizationTransformation.CreateOrganizationObject(obj),
                     IsPrimary = true
                 };
            }
        }

        public static SerialUserOrgItem CreateSerialUserOrgItem(UserOrganizationObject userOganization)
        {
            OrganizationObject organization = userOganization.Organization;
            return CreateSerialUserOrgItem(organization);
        }

        public static SerialUserOrgItem CreateSerialUserOrgItem(OrganizationObject organization)
        {
            SerialUserOrgItem newUserOrg = new SerialUserOrgItem()
            {
                id = organization.Id,
                name = organization.Name,
                type = organization.OrganizationTypeCode,
                description = organization.OrganizationTypeDescription,
                @default = organization.IsDefault,
                address = organization.Address,
                addressContinued = organization.AddressContinued,
                city = organization.City,
                stateProvince = organization.StateProvinceId,
                county = organization.CountyId,
                country = organization.CountryId,
                postalCode = organization.PostalCode,
                geoNameId = organization.GeoNameId,
                website = organization.OrganizationDomains.ToList().Select(d => new SerialUserOrgDomain(d)).ToList()
            };
            return newUserOrg;
        }
    }
}
