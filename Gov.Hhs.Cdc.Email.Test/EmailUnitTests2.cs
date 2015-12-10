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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.CdcEmailProvider;
using Gov.Hhs.Cdc.EmailProvider;
using Gov.Hhs.Cdc.Connection;

namespace Gov.Hhs.Cdc.Email.Test
{
    [TestClass]
    public class EmailUnitTests2
    {
        static EmailUnitTests2()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void SendEmail2()
        {
            string theEmailAdress = "senderEmail12@email";
            string fromEmailAddress =  "senderEmail13@email";
            TestEmailProvider theEmailProvider = new TestEmailProvider();

            EmailRouting routing = new EmailRouting(theEmailAdress);
            EmailDetails details = new EmailDetails() { From = fromEmailAddress};
            var newKeys = new
            {
                SenderName = "TheSenderName",
                ECardType = "TheTypeOfECard"
            };

            theEmailProvider.Send("ECard Sent", newKeys, routing, details);

            Assert.AreEqual(1, theEmailProvider.SentEmails.Count());
            Assert.AreEqual(theEmailAdress, theEmailProvider.SentEmails[0].MailMessage.To[0].Address);
            Assert.AreEqual("TheSenderName sent you a TheTypeOfECard.", theEmailProvider.SentEmails[0].MailMessage.Subject);
            Assert.IsTrue(true);
        }
    }
}
