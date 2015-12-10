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
using System.Configuration;
using System.Xml;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.EmailProvider;

namespace Gov.Hhs.Cdc.CdcEmailProvider
{
    public class EmailConfiguration : DataServicesConfig
    {
        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public TimeSpan Timeout { get; set; }
        public List<EmailDetails> EmailDetails { get; set; }

        public EmailConfiguration()
        {

            List<string> configErrors = new List<string>();

            XmlNode emailSection = (XmlNode)ConfigurationManager.GetSection("eMail");
            if (emailSection != null)
            {
                XmlAttributeCollection providerAttributes = emailSection.SelectSingleNode("provider").Attributes;
                SmtpServer = GetTheStringValue(configErrors, providerAttributes, "<eMail><provider>", "smtpServer", required: true);
                SmtpPort = GetTheIntValue(providerAttributes, "<eMail><provider>", "smtpPort", true, 0, configErrors);
                Timeout = GetTheTimeSpanValue(configErrors, providerAttributes, "<eMail><provider>", "timeout", false, TimeSpan.Parse("00:00:30"));

                XmlNodeList nodes = emailSection.SelectNodes("profiles/profile");
                if (nodes != null && nodes.Count > 0)
                {
                    EmailDetails = new List<EmailDetails>();
                    foreach (XmlNode profileNode in nodes)
                    {
                        EmailDetails.Add(EmailProfileConfigurationHelper.GetEmailDetails(configErrors, profileNode, "<eMail><profiles><profile>"));
                    }
                }
                else
                {
                    EmailDetails = new List<EmailDetails>();
                }
            }


        }



    }

}
