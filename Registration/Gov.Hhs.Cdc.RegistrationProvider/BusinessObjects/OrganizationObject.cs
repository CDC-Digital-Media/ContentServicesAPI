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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.RegistrationProvider
{
    [FilteredDataSet]
    public class OrganizationObject : DataSourceBusinessObject, IValidationObject, IDefaultableBusinessObject
    {
        [FilterSelection(CriterionType=FilterCriterionType.SingleSelect)]
        public int Id { get; set; }       

        [FilterSelection(CriterionType = FilterCriterionType.Text, TextType = FilterTextType.Equals)]
        [FilterSelection(Code = "Name_StartsWith", CriterionType = FilterCriterionType.Text, TextType = FilterTextType.StartsWith)]
        [FilterSelection(Code = "Name_Contains", CriterionType = FilterCriterionType.Text, TextType = FilterTextType.Contains)]
        [FilterSelection(Code = "Name_EndsWith", CriterionType = FilterCriterionType.Text, TextType = FilterTextType.EndsWith)]
        public string Name { get; set; }
        [FilterSelection(CriterionType = FilterCriterionType.Boolean)]
        public bool IsActive { get; set; }

        [FilterSelection(Code = "OrgType", CriterionType = FilterCriterionType.MultiSelect)]
        public string OrganizationTypeCode { get; set; }

        public string OrganizationTypeDescription { get; set; }
        
        //To Populate
        public string OrganizationTypeOther { get; set; }

        public string Address { get; set; }
        public string AddressContinued { get; set; }
        public string City { get; set; }

        public string StateProvinceId {get; set;}
        public string CountyId {get; set;}
        public string CountryId { get; set; }

        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public int? GeoNameId { get; set; }
        
        public bool IsDefault { get; set; }

        public IEnumerable<OrganizationDomainObject> OrganizationDomains { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
