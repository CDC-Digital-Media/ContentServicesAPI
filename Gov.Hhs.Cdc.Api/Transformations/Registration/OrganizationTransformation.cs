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
    public class OrganizationTransformation
    {
        public static OrganizationObject CreateOrganizationObject(SerialUserOrgItem obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.id > 0)
            {
                return new OrganizationObject() { Id = obj.id };
            }
            else
            {
                var domains = obj.website == null ? null : obj.website.ToList().Select(w =>
                    new OrganizationDomainObject
                    {
                        DomainName = w.url,
                        IsDefault = w.isDefault,
                        OrganizationId = obj.id
                    }).ToList();

                return new OrganizationObject()
                {
                    Address = obj.address,
                    AddressContinued = obj.addressContinued,
                    City = obj.city,
                    StateProvinceId = obj.stateProvince,
                    CountyId = obj.county,
                    CountryId = obj.country,

                    Id = 0,
                    IsActive = true,
                    Name = obj.name,
                    OrganizationTypeCode = obj.type,
                    OrganizationTypeOther = obj.typeOther,

                    PostalCode = obj.postalCode,
                    GeoNameId = obj.geoNameId,
                    OrganizationDomains = domains
                };
            }
        }

    }
}
