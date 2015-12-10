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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class OrganizationObjectValidator : IValidator<OrganizationObject, OrganizationObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<OrganizationObject> items)
        {
            foreach (OrganizationObject organization in items)
            {
                RegExValidator v = new RegExValidator(validationMessages, organization.ValidationKey);
                if (organization.Id < 0)
                    validationMessages.AddError(organization.ValidationKey, "Organization Id is invalid");
                //if (organization.ParentId != null && organization.ParentId < 0)
                //    validationMessages.AddError(organization.ValidationKey, "Organization Parent Id is invalid");
                v.IsValid(v.AlphanumericSpaces, organization.OrganizationTypeOther, required: false, message: "Other Organization Type is invalid");
                v.IsValid(v.AlphanumericSpacesSymbolTastic, organization.Name, required: true, message: "Organization Name is required");
                //v.IsValid(v.AlphaNumericSpacesPunctuation, organization.Address, required: false, message: "Organization Address is invalid");
                //v.IsValid(v.AlphaNumericSpacesPunctuation, organization.AddressContinued, required: false, message: "Organization Address Continued is invalid");
                //v.IsValid(v.AlphaNumericSpacesPunctuation, organization.City, required: false, message: "Organization City is invalid");
                //v.IsValid(v.AlphaNumericSpaces, organization.PostalCode, required: false, message: "Organization PostalCode is invalid");
                //v.IsValid(v.AlphaNumericSpacesPunctuation, organization.Phone, required: false, message: "Organization Phone is invalid");
                //v.IsValid(v.AlphaNumericSpacesPunctuation, organization.Fax, required: false, message: "Organization Fax is invalid");
                v.IsValid(v.Email, organization.Email, required: false, message: "Organization Email is invalid");
                if (string.IsNullOrEmpty(organization.CountryId) && !organization.GeoNameId.HasValue)
                {
                    validationMessages.AddError(organization.ValidationKey, "Location ID or Country is required");
                }
                if (organization.OrganizationDomains == null || organization.OrganizationDomains.Count() == 0 || organization.OrganizationDomains.Any(od => string.IsNullOrEmpty(od.DomainName)))
                {
                    validationMessages.AddError(organization.ValidationKey, "Organization URL is required");
                }
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<OrganizationObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<OrganizationObject> items)
        {
            foreach (OrganizationObject org in (IEnumerable<OrganizationObject>)items)
            {
                string compressedName = org.Name.Replace(" ", "").Replace(".", "").Replace(",", "");
                if (OrganizationCtl.Get((RegistrationObjectContext)objectContext).Where(o => o.Name.Replace(" ", "").Replace(".", "").Replace(",", "") == compressedName && o.GeoNameId == org.GeoNameId).Any())
                    validationMessages.AddError(org.ValidationKey, "The organization already exists in the system");
            }
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<OrganizationObject> items)
        {
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<OrganizationObject> items)
        {
        }

        public OrganizationObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, OrganizationObject theObject)
        {
            return theObject;
        }

    }
}
