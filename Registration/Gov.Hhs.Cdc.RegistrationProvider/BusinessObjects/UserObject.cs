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
    [FilteredDataSet]
    public class UserObject : DataSourceBusinessObject, IValidationObject
    {
        public Guid UserGuid { get; set; }
        
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public DateTime? _usageGuidelinesAgreementDateTime { get; set; }
        public DateTime? UsageGuidelinesAgreementDateTime { get { return _usageGuidelinesAgreementDateTime; } }
        public bool _agreedToUsageGuidelines { get; set; }
        public bool AgreedToUsageGuidelines { set { _agreedToUsageGuidelines = value; } get { return _agreedToUsageGuidelines; } }

        [FilterSelection(CriterionType = FilterCriterionType.Text, TextType = FilterTextType.Equals)]
        public string EmailAddress { get; set; }

        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
        public string PasswordSalt { get; set; }
        public int? PasswordFormat { get; set; } 

        public string TempPassword { get; set; }
        public string UserToken { get; set; }
        public Int64 ExpirationSeconds { get; set; }

        public string ApiKey { get; set; }
        public Guid ApiClientGuid { get; set; }

        public bool IsActive { get; set; }
        public bool IsMigrated { get; set; }

        public IEnumerable<UserOrganizationObject> Organizations { get; set; }


        public UserObject()
        {
        }

        public UserObject(string emailAddress, string password)
        {
            EmailAddress = emailAddress;
            Password = password; 
        }

        public static UserObject CreateByGuid(string guid)
        {
            Guid theGuid;
            if (Guid.TryParse(guid, out theGuid))
            {
                return new UserObject() { UserGuid = theGuid };
            }
            else
            {
                throw new ApplicationException("UserObject.CreateByGuid() failed due to invalid user guid");
            }
        }

    }
}
