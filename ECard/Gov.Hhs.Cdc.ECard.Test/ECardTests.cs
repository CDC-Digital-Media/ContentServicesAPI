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
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Global;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace Gov.Hhs.Cdc.ECard.Test
{
    [TestClass]
    public class ECardTests
    {
        static ECardTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        //[TestMethod]
        public void TestECardSubmission()
        {
            string eCardViewUrl = @"http://.....[devServer]...../test_storefront/ECard.aspx";
            SerialCardSubmission cardSubmission = new SerialCardSubmission()
            {
                personalMessage = "This is the personal message string",
                senderName = "Test Sender Name",
                senderEmail = "senderEmail10@email",
                eCardApplicationUrl = HttpUtility.UrlEncode(eCardViewUrl, Encoding.UTF8),
                isFromMobile = false,
                recipients = new List<SerialCardRecipient>()
                {
                    new SerialCardRecipient(){name = "Tim Carroll Cs", emailAddress = "senderEmail10@email"},
                    new SerialCardRecipient(){name = "Tim Carroll 7", emailAddress = "senderEmail11@email"},

                }
            };

            ValidationMessages messages = SendECard(cardSubmission, 90);
            ValidationMessage message = messages.Messages.FirstOrDefault();
            Assert.AreEqual(0, messages.Errors().Count());
        }


        public ValidationMessages SendECard(SerialCardSubmission cardSubmission, int mediaId) //, out int id)
        {
            if (TestApiUtility.UseWebService)
            {
                string data = new JavaScriptSerializer().Serialize(cardSubmission);
                string ecardUrl = String.Format("{0}://{1}/api/v1/resources/ecards/{2}/send", TestUrl.Protocol, TestUrl.PublicApiServer, mediaId);

                string callResults = TestApiUtility.CallAPI(ecardUrl, data, "POST");
                SerialResponse mediaObj = new JavaScriptSerializer().Deserialize<SerialResponse>(callResults);
                //SerialResponseWithType<SerialSyndicationList> mediaObj = new JavaScriptSerializer().Deserialize<SerialResponseWithType<SerialSyndicationList>>(callResults);
                return mediaObj.meta.GetUnserializedMessages();
            }
            else
            {
                
                TestOutputWriter testWriter = new TestOutputWriter();
                ECardHandler.SubmitCard(cardSubmission, 90, testWriter, null);
                ValidationMessages messages = testWriter.ValidationMessages;
                return messages;
            }
        }

        [TestMethod]
        public void ViewECard()
        {
            string testEcardInstanceId = "C8B1B095-404F-4BFD-9884-A67C10205D15";
            SerialCardView cardView;
            ValidationMessages messages = ViewEcard(testEcardInstanceId, out cardView);
            if (messages.Errors().Any())
            {
                Console.WriteLine(messages.Errors().First().Message);
                Console.WriteLine(messages.Errors().First().DeveloperMessage);
                Assert.Fail();
            }
        }

        private ValidationMessages ViewEcard(string stringGuid, out SerialCardView cardView) //, out int id)
        {
            if (TestApiUtility.UseWebService)
            {
                string ecardUrl = String.Format("{0}://{1}/api/v1/resources/ecards/{2}/view", TestUrl.Protocol, TestUrl.PublicApiServer, stringGuid);

                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

                WebClient serviceRequest = new WebClient();
                string callResults = serviceRequest.DownloadString(new Uri(ecardUrl));
                
                SerialResponseWithType<SerialCardView> mediaObj = new JavaScriptSerializer().Deserialize<SerialResponseWithType<SerialCardView>>(callResults);
                cardView = mediaObj.results;
                return mediaObj.meta.GetUnserializedMessages();
            }
            else
            {
                TestOutputWriter testWriter = new TestOutputWriter();
                ECardHandler.ViewECard(new Guid(stringGuid), testWriter);
                SerialResponse response = (SerialResponse)testWriter.TheObject;
                cardView = (SerialCardView) response.results;
                return testWriter.ValidationMessages;
            }
        }

        //for testing purpose only, accept any dodgy certificate... 
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}
