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

namespace Gov.Hhs.Cdc.Email
{
    public class EmailBase : MailMessage
    {

        public string SMTP_Server { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="fromAddress"></param>
        public EmailBase(string toAddress)
            : base()
        {
            To.Add(toAddress.ToString());
            this.Priority = MailPriority.Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="fromAddress"></param>
        public EmailBase(string fromAddress, string toAddress)
            : base(fromAddress, toAddress)
        {
            this.Priority = MailPriority.Normal;
        }

        /// <summary>
        /// This is a virtual function that can be implemented by derived classes to do any checking
        /// on whether or not the message is OK to send.
        /// </summary>
        /// <returns></returns>
        public virtual bool OkToSend()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Send()
        {
            if (OkToSend())
            {
                SmtpClient client = null;
                try
                {
                    client = new SmtpClient();
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = false;
                    //client.Timeout = 15000;
                    // upped timeout to 40 seconds by Thom Williams 8/8/2012
                    client.Timeout = 40000;
                    client.Host = this.SMTP_Server;

                    client.Send(this);
                }
                catch (Exception ex)
                {
                    //commented out the resend to support emails, now handled in the error listener.

                    //// Send failed.  Try to send message to developer support e-mail address
                    //try
                    //{
                    //    client = new SmtpClient();
                    //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //    client.UseDefaultCredentials = false;
                    //    client.EnableSsl = false;
                    //    client.Timeout = 15000;
                    //    client.Send(ConfigurationManager.AppSettings.Get("DeveloperSupportEMailAddress"), ConfigurationManager.AppSettings.Get("DeveloperSupportEMailAddress"), this.Subject, this.Body);
                    //}
                    //catch (Exception exp)
                    //{
                    //logger.Error(ex.ToString());
                    throw;
                    //}
                }
            }
        }

    }
}
