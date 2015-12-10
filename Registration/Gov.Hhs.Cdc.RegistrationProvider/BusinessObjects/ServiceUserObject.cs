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
    public class ServiceUserObject : DataSourceBusinessObject, IValidationObject
    {
        public Guid ServiceUserGuid { get; set; }
        public int OrganizationId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.Text, TextType = FilterTextType.Equals)]
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordRepeat { get; set; }

        //Optional attributes
        public OrganizationObject Organization { get; set; }

        public bool IsActive { get; set; }       
    }
}
