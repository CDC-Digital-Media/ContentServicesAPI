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
using System.Xml;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.EmailProvider;

namespace Gov.Hhs.Cdc.CdcEmailProvider
{
    public class EmailProfileConfigurationHelper : DataServicesConfig
    {
        public static EmailDetails GetEmailDetails(List<string> configErrors, XmlNode profileNode, string sectionName)
        {
            XmlAttributeCollection profileAttributes = profileNode.Attributes;

            string emailTypeCode = GetValueFromSection(configErrors, profileAttributes, sectionName, "name", required: false);
            string qualifiedSectionName = sectionName + "(Name=" + emailTypeCode + ")";

            EmailDetails details = new EmailDetails()
            {
                EmailTypeCode = emailTypeCode,
                From = GetValueFromSection(configErrors, profileAttributes, qualifiedSectionName, "from", required: false),
                IsDefault = GetTheBoolValue(configErrors, profileAttributes, qualifiedSectionName, "default", required: false, defaultValue: false)
            };
            return details;
        }

    }

}
