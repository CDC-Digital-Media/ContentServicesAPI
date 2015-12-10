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
using System.Net.Mail;

namespace Gov.Hhs.Cdc.CdcEmailProvider
{
    public class EmailSendAttempt
    {
        public SmtpClient Smtp { get; set; }
        public MailMessage MailMessage { get; set; }
    }
    public class TestEmailProvider : CsEmailProvider
    {
        private List<EmailSendAttempt> _attemptedEmails = null;
        public List<EmailSendAttempt> SentEmails { get { return _attemptedEmails ?? (_attemptedEmails = new List<EmailSendAttempt>()); } }

        protected override void SendToSmtp(SmtpClient smtp, MailMessage mailMessage, int retryAttempts = 0)
        {
            SentEmails.Add(new EmailSendAttempt() { Smtp = smtp, MailMessage = mailMessage });
        }
    }
}
