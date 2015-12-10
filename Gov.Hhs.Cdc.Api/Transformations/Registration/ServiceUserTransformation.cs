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

using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.Api
{
    public static class ServiceUserTransformation
    {
        public static ServiceUserObject CreateServiceUserObject(SerialServiceUser obj)
        {
            OrganizationObject userOrg = OrganizationTransformation.CreateOrganizationObject(obj.organization);

            return new ServiceUserObject()
            {
                EmailAddress = obj.email,
                FirstName = obj.firstName,
                MiddleName = obj.middleName,
                LastName = obj.lastName,
                IsActive = obj.active,
                Password = obj.password,
                PasswordRepeat = obj.passwordRepeat,
                OrganizationId = userOrg != null ? userOrg.Id : 0,
                Organization = userOrg
            };
        }


    }
}
