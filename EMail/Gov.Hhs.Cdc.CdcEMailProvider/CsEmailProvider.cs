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
using System.Linq;
using System.Net.Mail;
using System.Text;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.EmailProvider;
using Gov.Hhs.Cdc.Logging;

namespace Gov.Hhs.Cdc.CdcEmailProvider
{

    public class CsEmailProvider : IEmailProvider
    {
        public static EmailConfiguration _configuration;
        public static EmailConfiguration Configuration { get { return _configuration ?? (_configuration = new EmailConfiguration()); } }
        protected static IObjectContextFactory EmailObjectContextFactory = new EmailObjectContextFactory();

        public void Send(EmailRouting routing, object keys, EmailDetails details = null, int retryAttempts = 0)
        {
            Send(null, keys, routing, details, retryAttempts);
        }

        private static int ToMilliseconds(TimeSpan timeSpan)
        {
            return timeSpan.TotalMilliseconds > (double)short.MaxValue ?
                int.MaxValue :
                (int)timeSpan.TotalMilliseconds;
        }

        public void Send(string emailTypeCode, object keys, EmailRouting routing)
        {
            Send(emailTypeCode, keys, routing, null, 0);
        }

        public void Send(string emailTypeCode, object keys, EmailRouting routing, int retryAttempts = 0)
        {
            Send(emailTypeCode, keys, routing, null, retryAttempts);
        }

        public void Send(string emailTypeCode, object keys, EmailRouting routing, EmailDetails details = null, int retryAttempts = 0)
        {
            if (Configuration.EmailDetails == null)
            {
                throw new ApplicationException("Email configuration section is missing");
            }
            EmailDetails detailsFromConfiguration = Configuration.EmailDetails.Where(p => p.EmailTypeCode == emailTypeCode).FirstOrDefault();
            if (detailsFromConfiguration == null)
            {
                detailsFromConfiguration = Configuration.EmailDetails.Where(p => string.IsNullOrEmpty(emailTypeCode) && p.IsDefault).FirstOrDefault();
            }

            EmailDetails detailsFromDb = null;
            using (EmailObjectContext emailDb = (EmailObjectContext)EmailObjectContextFactory.Create())
            {
                detailsFromDb = EmailDetailsCtl.GetEmailDetails(emailDb).Where(d => d.EmailTypeCode == emailTypeCode).OrderBy(d => d._stringFormatCode).ThenBy(d => d._stringPriorityCode).FirstOrDefault();
            }

            var persistedDetails = detailsFromDb == null || detailsFromConfiguration == null ? (detailsFromDb ?? detailsFromConfiguration) :
                                           detailsFromDb.Merge(detailsFromConfiguration);
            var effectiveDetails = (details == null || persistedDetails == null) ? (details ?? persistedDetails) :
                                           details.Merge(persistedDetails);

            if (effectiveDetails == null)
            {
                throw new ApplicationException("No email profile found in configuration or database for type code(" + emailTypeCode + ") and no details were passed");
            }
            if (effectiveDetails.From == null)
            {
                throw new ApplicationException("The from email address has not been specified for profile " + emailTypeCode);
            }

            SendEmail(routing, keys, effectiveDetails);

        }

        private void SendEmail(EmailRouting routing, object keys, EmailDetails details, int retryAttempts = 0)
        {
            keys = keys ?? new object();
            using (var smtp = new SmtpClient(Configuration.SmtpServer, Configuration.SmtpPort)
            {
                EnableSsl = false,
                Timeout = ToMilliseconds(Configuration.Timeout),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            })
            {
                //TODO:  Convert reflection to string placeholders
                List<string> bodyValues = new List<string>();
                if (!string.IsNullOrEmpty(details.Header))
                {
                    bodyValues.Add(ReflectionUtility.Replace(details.Header, keys));
                }
                if (!string.IsNullOrEmpty(details.Body))
                {
                    bodyValues.Add(ReflectionUtility.Replace(details.Body, keys));
                }
                if (!string.IsNullOrEmpty(details.Footer))
                {
                    bodyValues.Add(ReflectionUtility.Replace(details.Footer, keys));
                }

                string emailBody = string.Join("\n\n\n", bodyValues.Where(v => !string.IsNullOrEmpty(v)).ToArray());

                MailMessage mailMessage = new MailMessage(details.From, routing.ToAddresses[0],
                    ReflectionUtility.Replace(details.Subject, keys), emailBody)
                {
                    BodyEncoding = UTF8Encoding.UTF8,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
                    IsBodyHtml = details.FormatCode == EmailDetails.EmailFormatCode.HTML

                };

                if (routing.CcAddresses != null && routing.CcAddresses.Count > 0)
                {
                    mailMessage.CC.Add(string.Join(",", routing.CcAddresses.ToArray()));
                }
                if (routing.BccAddresses != null && routing.BccAddresses.Count > 0)
                {
                    mailMessage.Bcc.Add(string.Join(",", routing.BccAddresses.ToArray()));
                }

                SendToSmtp(smtp, mailMessage, retryAttempts);
            }
        }

        protected virtual void SendToSmtp(SmtpClient smtp, MailMessage mailMessage, int retryAttempts = 0)
        {
            AuditLogger.LogAuditEvent("SendToSmtp " + retryAttempts.ToString());
            try
            {
                smtp.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                Logger.LogError(ex);
                while (retryAttempts > 0)
                {
                    SendToSmtp(smtp, mailMessage, retryAttempts);
                    retryAttempts--;
                }
                throw;
            }

        }


    }
}
