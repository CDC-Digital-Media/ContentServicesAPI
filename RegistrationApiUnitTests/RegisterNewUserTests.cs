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
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcRegistrationProvider;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.RegistrationProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace RegistrationApiUnitTests
{
    [TestClass]
    public class RegisterNewUserTests
    {
        IRegistrationProvider RegistrationProvider = new CsRegistrationProvider();
        RegistrationHelper helper = new RegistrationHelper();

        static RegisterNewUserTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        const string emailAddress = "senderEmail15@email";
        const string orgName = "Test Org";

        SerialUserOrgItem testOrg = new SerialUserOrgItem
        {
            type = "Application Developer",
            name = orgName,
            website = new List<SerialUserOrgDomain> { new SerialUserOrgDomain { url = "http://www.testorg.org", isDefault = true } },
            geoNameId = 4185484
        };

        [TestInitialize]
        public void Init()
        {
            helper.DeleteAUserIfItExists(emailAddress);
            helper.DeleteOrgIfItExists(orgName);
            Assert.IsFalse(helper.OrgExists(orgName));
        }

        [TestCleanup]
        public void Cleanup()
        {
            helper.DeleteAUserIfItExists(emailAddress);
            helper.DeleteOrgIfItExists(orgName);
        }

        [TestMethod]
        public void TestRegisterSameEmailTwice()
        {
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = new List<SerialUserOrgItem>() { new SerialUserOrgItem(6) }
            };

            ValidationMessages postUserResults1 = helper.CreateUser(newUserObject).ValidationMessages;
            ValidationMessages postUserResults2 = helper.CreateUser(newUserObject).ValidationMessages;
            Assert.IsTrue(postUserResults2.Errors().Any());  //should have error second time   
        }

        [TestMethod]
        public void TestRegisterWithoutPassword()
        {
            var user = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                organizations = new List<SerialUserOrgItem>() { new SerialUserOrgItem(2) }
            };
            var result = helper.CreateUser(user).ValidationMessages;
            var errors = result.Errors().FirstOrDefault();
            var message = errors == null ? "" : errors.Message;
            Assert.AreEqual(result.NumberOfErrors, 0, message);
        }

        [TestMethod]
        public void CanUpdateUserWithoutPassword()
        {
                        var user = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                organizations = new List<SerialUserOrgItem>() { new SerialUserOrgItem(2) }
            };
            var result = helper.CreateUser(user);
            var messages = result.ValidationMessages;
            var errors = messages.Errors().FirstOrDefault();
            var message = errors == null ? "" : errors.Message;
            Assert.AreEqual(messages.NumberOfErrors, 0, message);
            var id = result.ResultObject.User.id;

            var result2 = helper.UpdateUser(result.ResultObject.User);
        }

        [TestMethod]
        public void TestRegisterWithoutUsageAgreement()
        {
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = new List<SerialUserOrgItem>() { testOrg }
            };

            var results = helper.CreateUser(newUserObject);

            if (results.ValidationMessages.Errors().Any())
            {
                Console.WriteLine(results.ValidationMessages.Errors().First().Message);
                Console.WriteLine(results.ValidationMessages.Errors().First().DeveloperMessage);
                Assert.Fail();
            }

            var loginResults = helper.Login2(newUserObject);
            Assert.IsFalse(loginResults.ResultObject.User.agreedToUsageGuidelines);
        }

        [TestMethod]
        public void TestRegisterWithUsageAgreement()
        {
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                agreedToUsageGuidelines = true,
                organizations = new List<SerialUserOrgItem>() { testOrg }
            };

            var results = helper.CreateUser(newUserObject);

            if (results.ValidationMessages.Errors().Any())
            {
                Console.WriteLine(results.ValidationMessages.Errors().First().Message);
                Console.WriteLine(results.ValidationMessages.Errors().First().DeveloperMessage);
                Assert.Fail();
            }

            var loginResults = helper.Login2(newUserObject);
            Assert.IsTrue(loginResults.ResultObject.User.agreedToUsageGuidelines);
        }
    }
}
