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
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.CdcRegistrationProvider;
using Gov.Hhs.Cdc.Api;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Connection;

namespace RegistrationApiUnitTests
{
    [TestClass]
    public class RegisterNewOrgTests
    {
        IRegistrationProvider RegistrationProvider = new CsRegistrationProvider();
        RegistrationHelper helper = new RegistrationHelper();
        const string emailAddress = "senderEmail15@email";
        const string orgName = "Test Org";
        const string orgNameSymbol = "Test & Test";

        SerialUserOrgItem testOrg = new SerialUserOrgItem
        {
            type = "Application Developer",
            name = orgName,
            website = new List<SerialUserOrgDomain> { new SerialUserOrgDomain { url = "http://www.testorg.org", isDefault = true } },
            geoNameId = 4185484
        };
        SerialUserOrgItem testOrgSymbol = new SerialUserOrgItem
        {
            type = "Application Developer",
            name = orgNameSymbol,
            website = new List<SerialUserOrgDomain> { new SerialUserOrgDomain { url = "http://www.testorg.org", isDefault = true } },
            geoNameId = 4185484
        };
        SerialUserOrgItem testOrgMulti = new SerialUserOrgItem
        {
            type = "Application Developer",
            name = orgName,
            website = new List<SerialUserOrgDomain> { new SerialUserOrgDomain { url = "http://www.testorg.org", isDefault = true }, new SerialUserOrgDomain { url = "http://www.testorg2.com", isDefault = false } },
            geoNameId = 4185484
        };
        SerialUserOrgItem testOrgNoLoc = new SerialUserOrgItem
        {
            type = "Application Developer",
            name = orgName,
            website = new List<SerialUserOrgDomain> { new SerialUserOrgDomain { url = "http://www.testorg.org", isDefault = true } }
        };
        SerialUserOrgItem testOrgNoUrl = new SerialUserOrgItem
        {
            type = "Application Developer",
            name = orgName,
            geoNameId = 4185484
        };
        SerialUserOrgItem testOrgEmptyArray = new SerialUserOrgItem
        {
            type = "Application Developer",
            name = orgName,
            geoNameId = 4185484,
            website = new List<SerialUserOrgDomain>()
        };
        SerialUserOrgItem testOrgEmptyUrl = new SerialUserOrgItem
        {
            type = "Application Developer",
            name = orgName,
            geoNameId = 4185484,
            website = new List<SerialUserOrgDomain> { new SerialUserOrgDomain { url = ""} }
        };


        [TestInitialize]
        public void Init()
        {
            helper.DeleteAUserIfItExists(emailAddress);
            var cleanMe = new List<string> {orgName, orgNameSymbol};
            foreach (var org in cleanMe)
            {
                var messages = helper.DeleteOrgIfItExists(org);
                if (messages != null)
                {
                    if (messages.Errors().Any())
                    {
                        Console.WriteLine(messages.Errors().First().Message);
                        Console.WriteLine(messages.Errors().First().DeveloperMessage);
                        Assert.Fail();
                    }
                }
                Assert.IsFalse(helper.OrgExists(org));
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            helper.DeleteAUserIfItExists(emailAddress);
            helper.DeleteOrgIfItExists(orgName);
            helper.DeleteOrgIfItExists(orgNameSymbol);
        }

        static RegisterNewOrgTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void TestRegisterWithExistingOrganization()
        {
            var orgId = 5446;
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = new List<SerialUserOrgItem>() { new SerialUserOrgItem(orgId) }
            };

            var results = helper.CreateUser(newUserObject);

            if (results.ValidationMessages.Errors().Any())
            {
                Console.WriteLine(results.ValidationMessages.Errors().First().Message);
                Console.WriteLine(results.ValidationMessages.Errors().First().DeveloperMessage);
                Assert.Fail();
            }
            Assert.AreEqual(1, results.ResultObject.syndicationLists.Count);

            ActionResults loginActionResults = helper.Login(newUserObject);
            Assert.IsTrue(!loginActionResults.ValidationMessages.Errors().Any());
            Dictionary<string, object> serialLoginResults = (Dictionary<string, object>)loginActionResults.Results;
            object[] syndicationLists = (object[])serialLoginResults["syndicationLists"];
            Assert.AreEqual(1, syndicationLists.Count());
        }

        [TestMethod]
        public void TestRegisterWithNewOrganization()
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
            Assert.AreEqual(1, results.ResultObject.syndicationLists.Count);

            ActionResults loginActionResults = helper.Login(newUserObject);
            Assert.IsTrue(!loginActionResults.ValidationMessages.Errors().Any());
            Dictionary<string, object> serialLoginResults = (Dictionary<string, object>)loginActionResults.Results;
            object[] syndicationLists = (object[])serialLoginResults["syndicationLists"];
            Assert.AreEqual(1, syndicationLists.Count());
        }

        [TestMethod]
        public void TestRegisterWithNewNameWithSymbol()
        {
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = new List<SerialUserOrgItem>() { testOrgSymbol }
            };

            var results = helper.CreateUser(newUserObject);

            if (results.ValidationMessages.Errors().Any())
            {
                Console.WriteLine(results.ValidationMessages.Errors().First().Message);
                Console.WriteLine(results.ValidationMessages.Errors().First().DeveloperMessage);
                Assert.Fail();
            }
            Assert.AreEqual(1, results.ResultObject.syndicationLists.Count);

            ActionResults loginActionResults = helper.Login(newUserObject);
            Assert.IsTrue(!loginActionResults.ValidationMessages.Errors().Any());
            Dictionary<string, object> serialLoginResults = (Dictionary<string, object>)loginActionResults.Results;
            object[] syndicationLists = (object[])serialLoginResults["syndicationLists"];
            Assert.AreEqual(1, syndicationLists.Count());
        }


        [TestMethod]
        public void TestRegisterWithoutOrganizationLocationFails()
        {
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = new List<SerialUserOrgItem>() { testOrgNoLoc }
            };

            var results = helper.CreateUser(newUserObject);

            Assert.IsTrue(results.ValidationMessages.Errors().First().Message.Contains("Location ID or Country is required"));
        }

        [TestMethod]
        public void TestRegisterWithLocationCountryOnlySucceeds()
        {
            var org = testOrgNoLoc;
            org.country = "USA";
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = new List<SerialUserOrgItem>() { org }
            };

            var results = helper.CreateUser(newUserObject);

            if (results.ValidationMessages.Errors().Any())
            {
                Console.WriteLine(results.ValidationMessages.Errors().First().Message);
                Console.WriteLine(results.ValidationMessages.Errors().First().DeveloperMessage);
                Assert.Fail(results.ValidationMessages.Errors().First().Message);
            }
        }

        [TestMethod]
        public void TestRegisterWithoutOrganizationName()
        {
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = new List<SerialUserOrgItem>() { new SerialUserOrgItem() }
            };
            var results = helper.CreateUser(newUserObject);

            Assert.IsTrue(results.ValidationMessages.Errors().First().Message.Contains("Organization Name is required"));
        }

        [TestMethod]
        public void TestRegisterWithoutOrganization()
        {
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = null
            };

            var results = helper.CreateUser(newUserObject);

            Assert.IsTrue(results.ValidationMessages.Errors().First().Message.Contains("Organization is required"));
        }

        [TestMethod]
        public void TestOrganizationTypeSaves()
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

            var created = helper.CreateUser(newUserObject);
            var results = created.ResultObject;
            Assert.AreEqual("Application Developer", results.User.organizations[0].type);
        }

        [TestMethod]
        public void TestOrganizationLocationSaves()
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

            var created = helper.CreateUser(newUserObject);
            var results = created.ResultObject;
            Assert.IsNotNull(results.User);
            Assert.IsNotNull(results.User.organizations);
            Assert.IsTrue(results.User.organizations.Count > 0);
            Assert.AreEqual(4185484, results.User.organizations[0].geoNameId);
        }

        [TestMethod]
        public void TestRegisterWithoutOrganizationUrlFails()
        {
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = new List<SerialUserOrgItem>() { testOrgNoUrl }
            };

            var results = helper.CreateUser(newUserObject);
            Assert.IsTrue(results.ValidationMessages.Errors().Count() > 0);
            Assert.IsTrue(results.ValidationMessages.Errors().First().Message.Contains("Organization URL is required"));
        }

        [TestMethod]
        public void TestRegisterWithEmptyOrganizationUrlListFails()
        {
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = new List<SerialUserOrgItem>() { testOrgEmptyArray }
            };

            var results = helper.CreateUser(newUserObject);
            Assert.IsTrue(results.ValidationMessages.Errors().Count() > 0);
            Assert.IsTrue(results.ValidationMessages.Errors().First().Message.Contains("Organization URL is required"));
        }

        [TestMethod]
        public void TestRegisterWithEmptyStringOrganizationUrlFails()
        {
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = new List<SerialUserOrgItem>() { testOrgEmptyUrl }
            };

            var results = helper.CreateUser(newUserObject);
            Assert.IsTrue(results.ValidationMessages.Errors().Count() > 0);
            Assert.IsTrue(results.ValidationMessages.Errors().First().Message.Contains("Organization URL is required"));
        }

        [TestMethod]
        public void TestRegisterWithNewOrganizationMultiDomains()
        {
            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = new List<SerialUserOrgItem>() { testOrgMulti }
            };

            var results = helper.CreateUser(newUserObject);

            if (results.ValidationMessages.Errors().Any())
            {
                Console.WriteLine(results.ValidationMessages.Errors().First().Message);
                Console.WriteLine(results.ValidationMessages.Errors().First().DeveloperMessage);
                Assert.Fail();
            }
            Assert.AreEqual(2, results.ResultObject.syndicationLists.Count);

            ActionResults loginActionResults = helper.Login(newUserObject);
            Assert.IsTrue(!loginActionResults.ValidationMessages.Errors().Any());
            Dictionary<string, object> serialLoginResults = (Dictionary<string, object>)loginActionResults.Results;
            object[] syndicationLists = (object[])serialLoginResults["syndicationLists"];
            Assert.AreEqual(2, syndicationLists.Count());
        }

        // System.Data.SqlClient.SqlException: Operation failed. The index entry of length 2048 bytes for the index 'IX_MyList_ListName' exceeds the maximum length of 900 bytes.
        //Test to figure out how many chars we allow in domain field
        [TestMethod, Ignore]
        public void TestLongUrl()
        {
            string userUrl = String.Format("{0}://{1}/api/v1/resources/users", TestUrl.Protocol, TestUrl.PublicApiServer);
            var longUrl = "abcdefghijklmnopqrstuvwxy123abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxwzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmno123Abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxwzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmno123Abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxwzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmno123Abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxwzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmno123Abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxwzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmno123ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789123123456.com";
            //var obj = new JavaScriptSerializer().Deserialize<SerialUser>(data);
            for (int i = 1; i < 1024; i++)
            {
                helper.DeleteAUserIfItExists(emailAddress);
                helper.DeleteOrgIfItExists(orgName);

                var url = longUrl.Substring(i);
                var data = "{\"firstName\":\"Adventure\",\"middleName\":\"\",\"lastName\":\"Bob\",\"email\":\"" + emailAddress + "\",\"password\":\"Password1\",\"passwordRepeat\":\"Password1\",\"organizations\":[{\"type\":\"Application Developer\",\"typeOther\":\"\",\"name\":\"Test Org\",\"stateProvince\":\"California\",\"county\":\"Butte County\",\"country\":\"United States\",\"address\":\"\",\"addressContinued\":\"\",\"city\":\"\",\"postalCode\":0,\"phone\":0,\"fax\":0,\"email\":\"test@test.com\",\"website\":[{\"isDefault\":\"true\",\"url\":\"" + url + "\"}],\"geoNameId\":\"5332191\"}],\"agreedToUsageGuidelines\":true}";
          
                string callResults = TestApiUtility.CallAPI(userUrl, data, "POST");

                ActionResults results = new ActionResults(callResults);

                Dictionary<string, object> loginResults = (Dictionary<string, object>)results.Results;
                if (!results.ValidationMessages.Errors().Any())
                {
                    Console.WriteLine(i);
                    Console.WriteLine(url);
                    return;
                }
            }
        }

        [TestMethod]
        public void CanRegisterWithUrlof450Chars()
        {
            string userUrl = String.Format("{0}://{1}/api/v1/resources/users", TestUrl.Protocol, TestUrl.PublicApiServer);
            var url = "23Abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxwzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmno123Abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxwzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmno123ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789123123456.com";
            var data = "{\"firstName\":\"Adventure\",\"middleName\":\"\",\"lastName\":\"Bob\",\"email\":\"" + emailAddress + "\",\"password\":\"Password1\",\"passwordRepeat\":\"Password1\",\"organizations\":[{\"type\":\"Application Developer\",\"typeOther\":\"\",\"name\":\"Test Org\",\"stateProvince\":\"California\",\"county\":\"Butte County\",\"country\":\"United States\",\"address\":\"\",\"addressContinued\":\"\",\"city\":\"\",\"postalCode\":0,\"phone\":0,\"fax\":0,\"email\":\"test@test.com\",\"website\":[{\"isDefault\":\"true\",\"url\":\"" + url + "\"}],\"geoNameId\":\"5332191\"}],\"agreedToUsageGuidelines\":true}";
            var obj = new JavaScriptSerializer().Deserialize<SerialUser>(data);

            Assert.AreEqual(450, url.Length);
            string callResults = TestApiUtility.CallAPI(userUrl, data, "POST");

            ActionResults results = new ActionResults(callResults);

            Dictionary<string, object> loginResults = (Dictionary<string, object>)results.Results;

            if (results.ValidationMessages.Errors().Any())
            {
                Console.WriteLine(results.ValidationMessages.Errors().First().Message);
                Console.WriteLine(results.ValidationMessages.Errors().First().DeveloperMessage);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CannotRegisterWithUrlof451Chars()
        {
            string userUrl = String.Format("{0}://{1}/api/v1/resources/users", TestUrl.Protocol, TestUrl.PublicApiServer);
            var url = "123Abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxwzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmno123Abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxwzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmno123ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789123123456.com";
            var data = "{\"firstName\":\"Adventure\",\"middleName\":\"\",\"lastName\":\"Bob\",\"email\":\"" + emailAddress + "\",\"password\":\"Password1\",\"passwordRepeat\":\"Password1\",\"organizations\":[{\"type\":\"Application Developer\",\"typeOther\":\"\",\"name\":\"Test Org\",\"stateProvince\":\"California\",\"county\":\"Butte County\",\"country\":\"United States\",\"address\":\"\",\"addressContinued\":\"\",\"city\":\"\",\"postalCode\":0,\"phone\":0,\"fax\":0,\"email\":\"test@test.com\",\"website\":[{\"isDefault\":\"true\",\"url\":\"" + url + "\"}],\"geoNameId\":\"5332191\"}],\"agreedToUsageGuidelines\":true}";
            var obj = new JavaScriptSerializer().Deserialize<SerialUser>(data);

            Assert.AreEqual(451, url.Length);
            string callResults = TestApiUtility.CallAPI(userUrl, data, "POST");

            ActionResults results = new ActionResults(callResults);

            Dictionary<string, object> loginResults = (Dictionary<string, object>)results.Results;
            Assert.AreEqual("An error occurred while updating the entries. See the inner exception for details.", results.ValidationMessages.Errors().First().Message);
        }

    }
}
