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

using System.Collections.Generic;
using System.Linq;

namespace Gov.Hhs.Cdc.RegistrationProvider
{
    public class OrganizationTemplateObject
    {
        public string Id { get; set; }
        public string TypeCode { get; set; }
        public string TypeOther { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AddressContinued { get; set; }
        public string City { get; set; }
        public string StateProvinceId { get; set; }
        public string CountyId { get; set; }
        public string CountryId { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string GeoNameId { get; set; }
        public string Domains { get; set; }

        public OrganizationTemplateObject(OrganizationObject organization)
        {
            if (organization == null)
            { return; }

            Id = organization.Id.ToString();
            TypeCode = organization.OrganizationTypeCode;
            TypeOther = organization.OrganizationTypeOther;
            Name = organization.Name;
            Address = organization.Address;
            AddressContinued = organization.AddressContinued;
            City = organization.City;
            StateProvinceId = organization.StateProvinceId;
            CountyId = organization.CountyId;
            CountryId = organization.CountryId;
            PostalCode = organization.PostalCode;
            Phone = organization.Phone;
            Fax = organization.Fax;
            Email = organization.Email;
            GeoNameId = organization.GeoNameId == null ? "" : organization.GeoNameId.ToString();
            Domains = string.Join("<br/>", organization.OrganizationDomains.Select(a => a.DomainName).ToList());
        }
    }
}
