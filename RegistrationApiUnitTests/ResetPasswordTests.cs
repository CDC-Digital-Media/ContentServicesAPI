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
using System.Linq;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Api.Public;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcRegistrationProvider;
using Gov.Hhs.Cdc.Global;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace RegistrationApiUnitTests
{
    [TestClass]
    public class ResetPasswordTests
    {
        static ResetPasswordTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void NullObjectRetunsError()
        {
            var messages = InitiatePasswordReset(null);
            Assert.AreEqual(1, messages.Errors().Count());
            var message = messages.Errors().First().Message;
            Assert.IsTrue(message.Contains("not valid"), message);
        }
        
        [TestMethod]
        public void EmptyObjectRetunsError()
        {
            var data = new SerialPasswordReset();
            var messages = InitiatePasswordReset(data);
            Assert.AreEqual(1, messages.Errors().Count());
            Assert.AreEqual("Invalid passwordResetUrl", messages.Errors().First().Message);
        }

        [TestMethod]
        public void BadEmailReturnsError()
        {
            var data = new SerialPasswordReset { email = "", passwordResetUrl = "https://localhost:55861/FilterMedia.htm" };
            var messages = InitiatePasswordReset(data);
            Assert.AreEqual(messages.Errors().Count(), 1);
            Assert.AreEqual("Invalid User", messages.Errors().First().Message);
        }

        [TestMethod]
        public void MissingUrlReturnsError()
        {
            var data = new SerialPasswordReset { email = "senderEmail1@email" };
            var messages = InitiatePasswordReset(data);
            Assert.AreEqual(1, messages.Errors().Count());
            Assert.AreEqual("Invalid passwordResetUrl", messages.Errors().First().Message);
        }

        [TestMethod]
        public void GoodEmailGoodUrlReturnsNoMessages()
        {
            var data = new SerialPasswordReset { email = "senderEmail1@email", passwordResetUrl = "https://localhost:55861/resetPassword.htm" };

            IApiServiceFactory publicService = new PublicApiServiceFactory();
            ValidationMessages messages = TestApiUtility.ApiPostWithoutOutput<SerialPasswordReset>(publicService,
                publicService.CreateTestUrl("reset_user_passwords", "", "", ""),
                data);

            Assert.AreEqual(0, messages.Errors().Count());
        }

        [TestMethod]
        public void UrlWithResetTokenReturnsNoMessages()
        {
            var data = new SerialPasswordReset { email = "senderEmail1@email", passwordResetUrl = "https://localhost:56643/index.aspx?prt=yL9MR78vCzkv95p9C9dSoTBBWgl7hi0pfnvsD2SaYk" };

            IApiServiceFactory adminService = new PublicApiServiceFactory();
            ValidationMessages messages = TestApiUtility.ApiPostWithoutOutput<SerialPasswordReset>(adminService,
                adminService.CreateTestUrl("reset_user_passwords", "", "", ""),
                data);

            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().Count() + " errors.");
           }
        }

        [TestMethod]
        public void PerformResetPassword()
        {
            //Must request reset password first so that ExpirationSeconds gets updated
            var reset = new SerialPasswordReset { email = "senderEmail1@email", passwordResetUrl = "https://localhost:56643/index.aspx?prt=yL9MR78vCzkv95p9C9dSoTBBWgl7hi0pfnvsD2SaYk" };

            IApiServiceFactory adminService = new PublicApiServiceFactory();
            ValidationMessages messages = TestApiUtility.ApiPostWithoutOutput<SerialPasswordReset>(adminService,
                adminService.CreateTestUrl("reset_user_passwords", "", "", ""),
                reset);

            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().Count() + " errors.");
            }

            string token = CredentialManager.GenerateApiKeyTokenSalt();
            var data = new SerialPasswordReset { email = "senderEmail1@email", passwordToken = token, newPassword = "Password2", newPasswordRepeat = "Password2" };
            var response = PerformResetPassword(data);
            Assert.IsNotNull(response);
            
            //If you get "invalid credentials" here, your token is expired or has already been used.
            if (response.meta.message.Count > 0)
            {
                Assert.AreEqual("", response.meta.message[0].userMessage);
            }
            Assert.AreEqual(200, response.meta.status);
        }

        [TestMethod]
        public void ResetNonMatchingPasswordsGivesError()
        {
            string token = CredentialManager.GenerateApiKeyTokenSalt();
            var data = new SerialPasswordReset { email = "senderEmail1@email", passwordToken = token, newPassword = "Password2", newPasswordRepeat = "Password3" };
            var messages = PerformResetPassword(data);

            Assert.AreEqual(1, messages.meta.message.Count, messages.meta.message[0].userMessage);
            Assert.IsTrue(messages.meta.message[0].userMessage.Contains("doesn't match"), messages.meta.message[0].userMessage);
        }

        //This test will only run successfully for a brief time after generating the token (they expire)
        [TestMethod]
        public void ResetNonexistentEmailGivesAppropriateError()
        {
            string token = CredentialManager.GenerateApiKeyTokenSalt();
            var data = new SerialPasswordReset { email = "junk@email", passwordToken = token, newPassword = "Password2", newPasswordRepeat = "Password2" };
            var messages = PerformResetPassword(data);

            Assert.AreEqual(messages.meta.message.Count, 1);
            Assert.AreEqual("The email address entered doesn't match the originating password reset request", messages.meta.message[0].userMessage);
        }

        //This test will only run successfully for a brief time after generating the token (they expire)
        [TestMethod]
        public void ResetExistingEmailGivesAppropriateError()
        {
            string token = CredentialManager.GenerateApiKeyTokenSalt();
            var data = new SerialPasswordReset { email = "senderEmail1@email", passwordToken = token, newPassword = "Password2", newPasswordRepeat = "Password2" };
            var messages = PerformResetPassword(data);

            Assert.AreEqual(messages.meta.message.Count, 1);
            Assert.AreEqual("The email address entered doesn't match the originating password reset request", messages.meta.message[0].userMessage);
        }

        public ValidationMessages InitiatePasswordReset(SerialPasswordReset reset)
        {
            string json = new JavaScriptSerializer().Serialize(reset);
            var url = string.Format("{0}://{1}/api/v1/resources/reset_user_passwords", TestUrl.Protocol, TestUrl.PublicApiServer);
            var results = TestApiUtility.CallAPI(url, json, "POST");
            return new ActionResults(results).ValidationMessages;
        }

        public SerialResponse PerformResetPassword(SerialPasswordReset reset)
        {
            string json = new JavaScriptSerializer().Serialize(reset);
            var url = string.Format("{0}://{1}/api/v1/resources/update_user_passwords", TestUrl.Protocol, TestUrl.PublicApiServer);
            var results = TestApiUtility.CallAPI(url, json, "POST");
            Console.WriteLine(results);
            var result2 = new JavaScriptSerializer().Deserialize<SerialResponse>(results);
            return result2;
        }
    }
}
