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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.RegistrationProvider
{
    public interface IRegistrationProvider
    {
        LoginResults RegisterUser(UserObject userObject, bool sendEmail, string overriddenEmailAddress = null);

        UserObject GetUser(Guid userGuid);
        LoginResults SaveUser(UserObject theObject);
        ValidationMessages DeleteUser(UserObject theObject);

        ValidationMessages UpdateUserPassword(string userEmail, string currentPassword, string newPassword, string newPasswordRepeat, string apiKey);
        ValidationMessages ResetUserPassword(string userEmail, Uri passwordResetUrl, string apiKey);
        bool ValidateUsers(string userEmail, string appKey);

        LoginResults SetUserAgreement(Guid userGuid, bool agreedToUsageGuidelines);


        ValidationMessages SaveOrganizations(IList<OrganizationObject> objects);
        ValidationMessages SaveOrganization(OrganizationObject theObject);
        ValidationMessages DeleteOrganizations(IList<OrganizationObject> objects);
        ValidationMessages DeleteOrganization(OrganizationObject theObject);
        OrganizationObject GetOrganization(int organizationId);
        LoginResults Login(UserObject userObj, string apiKey);

        ServiceUserObject GetServiceUserByEmail(string emailAddress);
        ValidationMessages SaveUser(ServiceUserObject theObject);
        ValidationMessages DeleteServiceUserByEmail(string emailAddress);

        ValidationMessages SaveApiClient(ApiClientObject theObject, string apiKey);
        ValidationMessages DeleteApiClient(ApiClientObject obj, string apiKey);
        ValidationMessages UpdateApiClientApiKey(ApiClientObject obj);

 
        ApiClientObject GetApiClientByAppKey(string appKey, bool forUpdate = false);
        ApiClientObject GetApiClientByApiKey(string apiKey, bool forUpdate = false);
        List<ApiClientObject> GetApiClientList();
                
    }
}
