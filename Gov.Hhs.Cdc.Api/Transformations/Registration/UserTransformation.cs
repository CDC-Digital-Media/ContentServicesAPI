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
    public class UserTransformation
    {
        public static UserObject CreateUserObject(SerialUser obj)
        {
            var orgs = obj.organizations == null ? null :
                obj.organizations.Select(o => UserOrganizationTransformation.CreateUserOrganizationObject(o)).ToList();

            var userObj = new UserObject()
            {
                EmailAddress = obj.email,
                FirstName = obj.firstName,
                MiddleName = obj.middleName,
                LastName = obj.lastName,
                AgreedToUsageGuidelines = obj.agreedToUsageGuidelines,
                IsActive = true,
                Password = obj.password,
                PasswordRepeat = obj.passwordRepeat,
                Organizations = orgs,                
            };

            if (!string.IsNullOrEmpty(obj.id)) {
                userObj.UserGuid = new System.Guid(obj.id);
            }

            return userObj;
        }

        public static SerialUser CreateSerialUser(UserObject user, UserTokenObject userToken)
        {
            if (user == null)
            {
                return null;
            }
            SerialUser serialUser = new SerialUser();
            SetBaseUserValues(user, userToken, serialUser);

            serialUser.id = user.UserGuid.ToString();
            serialUser.agreedToUsageGuidelines = user.UsageGuidelinesAgreementDateTime != null;
            serialUser.organizations = user.Organizations.ToList()
                .Select(u => UserOrganizationTransformation.CreateSerialUserOrgItem(u)).ToList();
            return serialUser;
        }

        private static void SetBaseUserValues(UserObject user, UserTokenObject userToken, ISerialUserObject serialUser)
        {

            if (user != null)
            {
                serialUser.firstName = user.FirstName;
                serialUser.middleName = user.MiddleName;
                serialUser.lastName = user.LastName;
                serialUser.email = user.EmailAddress;
            }

            if (userToken != null)
            {
                serialUser.userToken = userToken.UserToken;
                serialUser.expiresAt = userToken.ExpirationUtcSeconds;
            }
        }
    }

}
